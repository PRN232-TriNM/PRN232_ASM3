using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCS.TriNM.Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<ChargingTransaction>> GetAllTransactionsAsync()
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }

        public async Task<ChargingTransaction?> GetTransactionByIdAsync(long id)
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }

        public async Task<IEnumerable<ChargingTransaction>> GetTransactionsByUserIdAsync(int userId)
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }

        public async Task<IEnumerable<ChargingTransaction>> GetTransactionsByChargerIdAsync(int chargerId)
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }

        public async Task<ChargingTransaction> CreateTransactionAsync(ChargingTransaction transaction)
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }

        public async Task<bool> UpdateTransactionAsync(ChargingTransaction transaction)
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }

        public async Task<bool> CompleteTransactionAsync(long transactionId, decimal energyConsumed, decimal amount)
        {
            throw new NotImplementedException("ChargingTransactionRepository not available in UnitOfWork");
        }
    }
}

