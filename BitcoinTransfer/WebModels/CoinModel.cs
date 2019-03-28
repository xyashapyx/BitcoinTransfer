namespace BitcoinTransfer.WebModels
{
    public class CoinModel
    {
        public string TransactionId { get; set; }
        public string ScriptPubKey { get; set; }
        public long Index { get; set; }
        public decimal Value { get; set; }
    }
}
