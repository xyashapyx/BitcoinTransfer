using System;

namespace BitcoinTransfer.ViewModels
{
    public class LastTransactionModel
    {
        private long Id { get; set; }

        public DateTime Date { get; set; }

        public string Address { get; set; }

        public double Amount { get; set; }

        public int Confirmation { get; set; }
    }
}
