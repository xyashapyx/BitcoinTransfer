using System.Collections.Generic;
using System.Threading.Tasks;
using BitcoinTransfer.ViewModels;

namespace BitcoinTransfer.Interfaces.Services
{
    public interface IBitcoinService
    {
        Task<IEnumerable<LastTransactionModel>> ProcessGetLast();
    }
}
