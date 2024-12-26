using HSOne.Core.Domain.Royalty;
using HSOne.Core.Models;
using HSOne.Core.SeedWorks;

namespace HSOne.Core.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction, Guid>
    {
        Task<PagedResult<TransactionDto>> GetTransactionPagingAsync(string? userName, int fromMonth, int fromYear, int toMonth, int toYear, int pageIndex = 1, int pageSize = 10);
    }
}
