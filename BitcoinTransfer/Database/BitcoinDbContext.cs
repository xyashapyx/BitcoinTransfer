using System.IO;
using System.Linq;
using BitcoinTransfer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinTransfer.Database
{
    public class BitcoinDbContext : DbContext
    {
        private readonly string connectionString;
        public DbSet<WalletModel> Wallets { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }

        public BitcoinDbContext(DbContextOptions<BitcoinDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WalletModel>().HasData(
                new WalletModel
                {
                    WalletId = Consts.DefaultAddressIdToGetBitcoins,
                    Address = Consts.DefaultAddressToGetBitcoin,
                    PrivateKey = Consts.DefaultSecretToGetBitcoin
                }
            );
        }
    }
}
