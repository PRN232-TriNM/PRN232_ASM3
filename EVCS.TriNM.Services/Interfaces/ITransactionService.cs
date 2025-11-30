using EVCS.TriNM.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<ChargingTransaction>> GetAllTransactionsAsync();
        Task<ChargingTransaction?> GetTransactionByIdAsync(long id);
        Task<IEnumerable<ChargingTransaction>> GetTransactionsByUserIdAsync(int userId);
        Task<IEnumerable<ChargingTransaction>> GetTransactionsByChargerIdAsync(int chargerId);
        Task<ChargingTransaction> CreateTransactionAsync(ChargingTransaction transaction);
        Task<bool> UpdateTransactionAsync(ChargingTransaction transaction);
        Task<bool> CompleteTransactionAsync(long transactionId, decimal energyConsumed, decimal amount);
    }
}

