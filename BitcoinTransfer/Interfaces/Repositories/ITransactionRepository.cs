using System.Collections.Generic;
using System.Threading.Tasks;
using BitcoinTransfer.Models;

namespace BitcoinTransfer.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepositoryBase<TransactionModel>
    {
        Task<IEnumerable<TransactionModel>> GetLastTransactions();
    }
}
