using System.Threading.Tasks;
using BitcoinTransfer.Database;
using BitcoinTransfer.Interfaces.Repositories;
using BitcoinTransfer.Models;

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
    }
}
