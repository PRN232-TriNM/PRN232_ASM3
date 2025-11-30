using EVCS.TriNM.Repositories.Models;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(UserAccount user);
        bool IsUserInRole(string authHeader, string role);
        Task<string> GoogleLoginAsync(string idToken);
    }
}
