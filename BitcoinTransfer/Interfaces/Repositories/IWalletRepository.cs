using System.Threading.Tasks;
using BitcoinTransfer.Models;

namespace BitcoinTransfer.Interfaces.Repositories
{
    public interface IWalletRepository : IRepositoryBase<WalletModel>
    {
        Task<WalletModel> GetDefaultWalletToSendBitcoins();
    }
}
