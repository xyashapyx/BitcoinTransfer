using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitcoinTransfer.Models
{
    public class TransactionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Transaction { get; set; }

        public string TransactionId { get; set; }

        public bool HasViewed { get; set; }

        public long FromWalletId { get; set; }

        public long ToWalletId { get; set; }

        public decimal Amount { get; set; }

        public int Confirmation { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
