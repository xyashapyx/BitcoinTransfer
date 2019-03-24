using BitcoinTransfer.Models;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinTransfer.Services
{
    public class BitcoinNetworkService
    {
        public async Task<Balance> MssGetBalance(string ssAddress, bool ssIsUnspentOnly, bool ssIsTestNet)
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

        public async Task<int> GetConfirmationsCount(string transactionId)
        {
            var client = new QBitNinjaClient(Network.TestNet);
            var transaction = await client.GetTransaction(uint256.Parse(transactionId));
            return transaction.Block.Confirmations;
        }

        public async Task<string> MssTransfer(string ssPrivateKey, decimal ssTransferValue, string ssToAddress, decimal ssMinerFee, bool ssIsTestNet)
        {
            var net = ssIsTestNet ? Network.TestNet : Network.Main;
            var transaction = Transaction.Create(net);
            var bitcoinPrivateKey = new BitcoinSecret(ssPrivateKey);
            var fromAddress = bitcoinPrivateKey.GetAddress().ToString();
            var totalBalance = await MssGetBalance(fromAddress, true, ssIsTestNet);
            if (totalBalance.Confirmed <= ssTransferValue)
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
            var toPubKeyAddress = new BitcoinPubKeyAddress(ssToAddress);
            TxOut toAddressTxOut = new TxOut()
            {
                Value = new Money((decimal)ssTransferValue, MoneyUnit.BTC),
                ScriptPubKey = toPubKeyAddress.ScriptPubKey
            };
            transaction.Outputs.Add(toAddressTxOut);
            //add address to send change
            decimal change = totalBalance.Unconfirmed - ssTransferValue - ssMinerFee;
            if (change > 0)
            {
                var fromPubKeyAddress = new BitcoinPubKeyAddress(fromAddress);
                TxOut changeAddressTxOut = new TxOut
                {
                    Value = new Money(change, MoneyUnit.BTC),
                    ScriptPubKey = fromPubKeyAddress.ScriptPubKey
                };
                transaction.Outputs.Add(changeAddressTxOut);
            }
            // sign transaction
            transaction.Sign(bitcoinPrivateKey, false);
            //Send transaction
            var broadcastResponse = client.Broadcast(transaction).Result;
            if (!broadcastResponse.Success)
                throw new Exception("Error broadcasting transaction " + broadcastResponse.Error.ErrorCode + " : " + broadcastResponse.Error.Reason);
            return transaction.GetHash().ToString();
        }   
    }
}
