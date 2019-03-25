using BitcoinTransfer.Models;
using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BitcoinTransfer.Interfaces.Repositories;
using BitcoinTransfer.Interfaces.Services;
using BitcoinTransfer.ViewModels;
using ITransactionRepository = BitcoinTransfer.Interfaces.Repositories.ITransactionRepository;

namespace BitcoinTransfer.Services
{
    public class BitcoinService : IBitcoinService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public BitcoinService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
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
            var totalBalance = await MssGetBalance(fromAddress, true, ssIsTestNet);
            if (totalBalance.Confirmed <= amount)
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
            var change = totalBalance.Unconfirmed - amount;
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
            return transaction.GetHash().ToString();
        }

        private async Task<Balance> MssGetBalance(string ssAddress, bool ssIsUnspentOnly, bool ssIsTestNet)
        {
            var net = ssIsTestNet ? Network.TestNet : Network.Main;
            var client = new QBitNinjaClient(net);
            var balance = await client.GetBalance(new BitcoinPubKeyAddress(ssAddress), ssIsUnspentOnly);
            var totalBalance = new Balance();
            if (balance.Operations.Count <= 0)
                return totalBalance;
            var unspentCoins = new List<Coin>();
            var unspentCoinsConfirmed = new List<Coin>();
            foreach (var operation in balance.Operations)
            {
                unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
                if (operation.Confirmations > 0)
                    unspentCoinsConfirmed.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin));
            }
            totalBalance.Unconfirmed = unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            totalBalance.Unconfirmed = unspentCoinsConfirmed.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
            return totalBalance;
        }
   
        private async Task UpdateConfirmations(IEnumerable<TransactionModel> transactionModels)
        {
            var client = new QBitNinjaClient(Network.TestNet);
            foreach (var transactionModel in transactionModels)
            {
                var transaction = await client.GetTransaction(uint256.Parse(transactionModel.TransactionId));
                transactionModel.Confirmation = transaction.Block.Confirmations;
            }
        }        
    }
}
