using EVCS.TriNM.Services.Object.Requests;
using EVCS.TriNM.Services.Object.Responses;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetUsersAsync();
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<UserResponse?> GetMyProfileAsync(int userId);
        Task<(bool isSuccess, string resultOrError)> UpdateUserProfileAsync(int id, UpdateUserProfileRequest request);
        Task<UserResponse?> SoftDeleteUserAsync(int id);
        Task<bool> UpgradeToPremiumAsync(int userId);
    }
}
