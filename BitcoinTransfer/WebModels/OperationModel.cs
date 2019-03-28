using System.Collections.Generic;

namespace BitcoinTransfer.WebModels
{
    public class OperationModel
    {
        public decimal Amount { get; set; }
        public decimal Confirmations { get; set; }
        public decimal Height { get; set; }
        public string BlockId { get; set; }
        public string TransactionId { get; set; }
        public IEnumerable<CoinModel> ReceivedCoins { get; set; }
        public IEnumerable<CoinModel> SpendCoins { get; set; }
    }
}
