using BitcoinTransfer.Models;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BitcoinTransfer.Interfaces.Repositories;
using BitcoinTransfer.Interfaces.Services;
using BitcoinTransfer.ViewModels;
using Newtonsoft.Json;
using QBitNinja.Client.Models;
using ITransactionRepository = BitcoinTransfer.Interfaces.Repositories.ITransactionRepository;
using WalletModel = BitcoinTransfer.Models.WalletModel;

namespace BitcoinTransfer.Services
{
    public class BitcoinService : IBitcoinService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly HttpClientService _httpClientService;

        public BitcoinService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
            _httpClientService = new HttpClientService();
        }
        public async Task<IEnumerable<LastTransactionModel>> ProcessGetLast()
        {
            var transactions = await _transactionRepository.GetLastTransactions();
            await UpdateConfirmations(transactions);
            _transactionRepository.UpdateRange(transactions);
            await _transactionRepository.SaveAsync();
            var lastTransactions = _mapper.Map<IEnumerable<LastTransactionModel>>(transactions.Where(x => !x.HasViewed || x.Confirmation < 3));
            return lastTransactions;
        }

        public async Task<string> TransferBitcoin(decimal amount, string toAddress, bool ssIsTestNet)
        {
            var fromWallet = await _walletRepository.GetDefaultWalletToSendBitcoins();
            var net = ssIsTestNet ? Network.TestNet : Network.Main;
            var transaction = Transaction.Create(net);
            var bitcoinPrivateKey = new BitcoinSecret(fromWallet.PrivateKey);
            var fromAddress = bitcoinPrivateKey.GetAddress().ToString();
            var totalBalance = await GetBalance(fromAddress);
            if (totalBalance <= amount)
                throw new Exception("The address doesn't have enough funds!");
            var client = new QBitNinjaClient(net);
            var balance = client.GetBalance(new BitcoinPubKeyAddress(fromAddress), true).Result;
            //Add trx in
            //Get all transactions in for that address
            var txsIn = 0;
            if (balance.Operations.Any())
            {
                foreach (var operation in balance.Operations)
                {
                    foreach (Coin receivedCoin in operation.ReceivedCoins)
                    {
                        var outpointToSpend = receivedCoin.Outpoint;
                        transaction.Inputs.Add(new TxIn { PrevOut = outpointToSpend });
                        transaction.Inputs[txsIn].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
                        txsIn = txsIn + 1;
                    }
                }
            }
            //add address to send money
            var toPubKeyAddress = new BitcoinPubKeyAddress(toAddress);
            var toAddressTxOut = new TxOut
            {
                Value = new Money(amount, MoneyUnit.BTC),
                ScriptPubKey = toPubKeyAddress.ScriptPubKey
            };
            transaction.Outputs.Add(toAddressTxOut);
            //add address to send change
            var change = totalBalance - amount;
            if (change > 0)
            {
                var fromPubKeyAddress = new BitcoinPubKeyAddress(fromAddress);
                var changeAddressTxOut = new TxOut
                {
                    Value = new Money(change, MoneyUnit.BTC),
                    ScriptPubKey = fromPubKeyAddress.ScriptPubKey
                };
                transaction.Outputs.Add(changeAddressTxOut);
            }
            // sign transaction
            transaction.Sign(bitcoinPrivateKey, balance.Operations.SelectMany(x => x.ReceivedCoins.Select(y => y)).LastOrDefault());
            //Send transaction
            var broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
                throw new Exception("Error broadcasting transaction " + broadcastResponse.Error.ErrorCode + " : " + broadcastResponse.Error.Reason);
            var transactionId = transaction.GetHashCode().ToString();

            var receiver = await _walletRepository.GetOrCreate(toAddress);

            _transactionRepository.Create(new TransactionModel
            {
                Amount = amount,
                Confirmation = 0,
                CreatedDate = DateTime.UtcNow,
                FromWalletId = fromWallet.WalletId,
                TransactionId = transactionId,
                ToWalletId = receiver.WalletId
            });
            await _transactionRepository.SaveAsync();
            await UpdateBalance(fromWallet, receiver);
            return transaction.GetHash().ToString();
        }      
   
        private async Task UpdateConfirmations(IEnumerable<TransactionModel> transactionModels)
        {
            foreach (var transactionModel in transactionModels)
            {
                var confirmations = await GetConfirmations(transactionModel.TransactionId);
                transactionModel.Confirmation = confirmations;
            }
        }

        private async Task UpdateBalance(WalletModel fromWallet, WalletModel toWallet)
        {
            var fromWalletBalance = await GetBalance(fromWallet.Address);
            var toWalletBalance = await GetBalance(toWallet.Address);
            var balanceChanged = false;
            if (fromWallet.ConfirmedBalance != fromWalletBalance)
            {
                fromWallet.ConfirmedBalance = fromWalletBalance;
                _walletRepository.Update(fromWallet);
                balanceChanged = true;
            }
            if (toWallet.ConfirmedBalance != toWalletBalance)
            {
                toWallet.ConfirmedBalance = toWalletBalance;
                _walletRepository.Update(toWallet);
                balanceChanged = true;
            }

            if (balanceChanged)
                await _walletRepository.SaveAsync();
        }

        private async Task<decimal> GetBalance(string address)
        {
            var balance = await _httpClientService.GetItem<WebModels.BalanceModel>($"balances/{address}/summary");
            return balance.Confirmed.Amount / Consts.BitcoinScale;
        }

        private async Task<int> GetConfirmations(string transactionId)
        {
            var balance = await _httpClientService.GetItem<WebModels.TransactionModel>($"transactions/{transactionId}?format=json");
            return balance.Block.Confirmations;
        }
    }
}
