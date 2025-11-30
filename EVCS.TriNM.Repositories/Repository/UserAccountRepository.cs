using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCS.TriNM.Repositories.Repository
{
    public class UserAccountRepository : GenericRepository<UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(EVChargingDBContext context) : base(context)
        {
        }

        public async Task<UserAccount?> GetByEmailAsync(string email)
        {
            return await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserAccount?> GetByUserNameAsync(string userName)
        {
            return await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<UserAccount?> GetByGoogleIdAsync(string googleId)
        {
            return await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }
    }

    public interface IUserAccountRepository : IGenericRepository<UserAccount>
    {
        Task<UserAccount?> GetByEmailAsync(string email);
        Task<UserAccount?> GetByUserNameAsync(string userName);
        Task<UserAccount?> GetByGoogleIdAsync(string googleId);
    }
}

