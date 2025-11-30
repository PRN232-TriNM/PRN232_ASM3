using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Object.Requests;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface ILoginService
    {
        Task<(bool isSuccess, string resultOrError)> ValidateUserAsync(LoginRequest request);
        Task<UserAccount?> GetUserAccountAsync(string username, string password);
    }
}
