﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitcoinTransfer.Models
{
    public class WalletModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WalletId { get; set; }

        public string Address { get; set; }

        public string PrivateKey { get; set; }

        public decimal ConfirmedBalance { get; set; }
    }
}
