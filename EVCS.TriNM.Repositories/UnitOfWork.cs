using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Repository;

namespace EVCS.TriNM.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        EVChargingDBContext Context { get; }
        StationTriNMRepository StationTriNMRepository { get; }
        IChargerTriNMRepository ChargerTriNMRepository { get; }
        IUserAccountRepository UserAccountRepository { get; }

            int SaveChanges();
        Task<int> SaveChangesAsync();
        int SaveChangesWithTransaction();
        Task<int> SaveChangesWithTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly EVChargingDBContext _context;
        private bool _disposed = false;

        public UnitOfWork(EVChargingDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public EVChargingDBContext Context => _context;
        
        public StationTriNMRepository StationTriNMRepository => new StationTriNMRepository(_context);
        public IChargerTriNMRepository ChargerTriNMRepository => new ChargerTriNMRepository(_context);
        public IUserAccountRepository UserAccountRepository => new UserAccountRepository(_context);

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChangesWithTransaction()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            int result = 0;
            strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    result = _context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
            return result;
        }

        public async Task<int> SaveChangesWithTransactionAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            int result = 0;
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    result = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        Console.WriteLine($"Rollback failed: {rollbackEx.Message}");
                    }
                    throw;
                }
            });
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
            }
            _disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
