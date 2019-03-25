using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitcoinTransfer.Database;
using BitcoinTransfer.Interfaces.Repositories;
using BitcoinTransfer.Models;
using Microsoft.EntityFrameworkCore;

namespace BitcoinTransfer.Repositories
{
    public class TransactionRepository : RepositoryBase<TransactionModel>, ITransactionRepository
    {
        public TransactionRepository(BitcoinDbContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<TransactionModel>> GetLastTransactions()
        {
            return await RepositoryContext.Transactions.Where(x => x.Confirmation < 3 || !x.HasViewed).ToListAsync();
        }
    }
}
