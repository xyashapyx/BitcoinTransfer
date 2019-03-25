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

        public BitcoinDbContext(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("BitcoinInfoDb");

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(connectionString);

        public class DesignTimeActivitiesDbContextFactory : IDesignTimeDbContextFactory<BitcoinDbContext>
        {
            public BitcoinDbContext CreateDbContext(string[] args)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                    .AddJsonFile("appsettings.json", optional: false);

                var config = builder.Build();

                return new BitcoinDbContext(config);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WalletModel>().HasData(
                new WalletModel
                {
                    WalletId = Consts.defaultAddressIdToGetBitcoins,
                    Address = Consts.defaultAddressToGetBitcoin,
                    PrivateKey = Consts.defaultSecretToGetBitcoin
                }
            );
        }
    }
}
