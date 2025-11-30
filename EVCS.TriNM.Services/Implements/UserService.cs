using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Interfaces;
using EVCS.TriNM.Services.Object.Requests;
using EVCS.TriNM.Services.Object.Responses;

namespace EVCS.TriNM.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            var users = await _unitOfWork.UserAccountRepository.GetAllAsync();
            return users.Select(MapToUserResponse);
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            return user != null ? MapToUserResponse(user) : null;
        }

        public async Task<UserResponse?> GetMyProfileAsync(int userId)
        {
            var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }
            return MapToUserResponse(user);
        }

        public async Task<(bool isSuccess, string resultOrError)> UpdateUserProfileAsync(int id, UpdateUserProfileRequest request)
        {
            try
            {
                var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return (false, "User not found");
                }

                user.FullName = request.FullName;
                user.Phone = request.Phone;
                user.EmployeeCode = request.EmployeeCode;
                user.ModifiedDate = DateTime.UtcNow;

                await _unitOfWork.UserAccountRepository.UpdateAsync(user);

                return (true, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public async Task<UserResponse?> SoftDeleteUserAsync(int id)
        {
            var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User is already deactivated");
            }

            user.IsActive = false;
            user.ModifiedDate = DateTime.UtcNow;

            await _unitOfWork.UserAccountRepository.UpdateAsync(user);

            return MapToUserResponse(user);
        }

        public async Task<bool> UpgradeToPremiumAsync(int userId)
        {
            // This is a placeholder implementation
            // You can implement premium upgrade logic based on your business requirements
            var user = await _unitOfWork.UserAccountRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found");
            }

            // For now, just return true as a placeholder
            return true;
        }

        private static UserResponse MapToUserResponse(UserAccount user)
        {
            return new UserResponse
            {
                UserAccountId = user.UserAccountId,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                EmployeeCode = user.EmployeeCode,
                RoleId = user.RoleId,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate,
                IsActive = user.IsActive
            };
        }
    }
}
