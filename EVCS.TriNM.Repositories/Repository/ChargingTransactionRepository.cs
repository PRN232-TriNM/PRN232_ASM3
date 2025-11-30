using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCS.TriNM.Repositories.Repository
{
    public class ChargingTransactionRepository : GenericRepository<ChargingTransaction>, IChargingTransactionRepository
    {
        public ChargingTransactionRepository(EVChargingDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChargingTransaction>> GetByUserIdAsync(int userId)
        {
            return await _context.ChargingTransactions
                .Where(t => t.UserAccountId == userId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }

    public interface IChargingTransactionRepository : IGenericRepository<ChargingTransaction>
    {
        Task<IEnumerable<ChargingTransaction>> GetByUserIdAsync(int userId);
    }
}
