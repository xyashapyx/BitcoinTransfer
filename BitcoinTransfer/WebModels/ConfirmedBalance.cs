using System.Collections.Generic;

namespace BitcoinTransfer.WebModels
{
    public class ConfirmedBalance
    {
        public long TransactionCount { get; set; }

        public decimal Amount { get; set; }

        public decimal Received { get; set; }

        public IEnumerable<AssetModel> Assets { get; set; }
    }
}
