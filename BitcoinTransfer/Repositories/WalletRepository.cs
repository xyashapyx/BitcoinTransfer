using System.Linq;
using System.Threading.Tasks;
using BitcoinTransfer.Database;
using BitcoinTransfer.Interfaces.Repositories;
using BitcoinTransfer.Models;
using Microsoft.EntityFrameworkCore;

namespace BitcoinTransfer.Repositories
{
    public class WalletRepository : RepositoryBase<WalletModel>, IWalletRepository
    {
        public WalletRepository(BitcoinDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<WalletModel> GetDefaultWalletToSendBitcoins()
        {
            return await RepositoryContext.Wallets.FindAsync(Consts.defaultAddressIdToGetBitcoins);
        }

        public async Task<WalletModel> GetOrCreate(string address)
        {
            var wallet = await RepositoryContext.Wallets.FirstOrDefaultAsync(x => x.Address == address);
            if (wallet != null)
                return wallet;
            var walletToCreate = new WalletModel
            {
                Address = address,
                ConfirmedBalance = 0
            };
            await RepositoryContext.Wallets.AddAsync(walletToCreate);
            await RepositoryContext.SaveChangesAsync();
            return await RepositoryContext.Wallets.FirstAsync(x => x.Address == address);
        }
    }
}
