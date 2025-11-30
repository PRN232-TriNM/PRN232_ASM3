using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Extensions;
using EVCS.TriNM.Services.Interfaces;
using EVCS.TriNM.Services.Object.Requests;

namespace EVCS.TriNM.Services.Implements
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncryptionService _passwordEncryptionService;
        private readonly IAuthService _authService;

        public LoginService(IUnitOfWork unitOfWork, PasswordEncryptionService passwordEncryptionService, IAuthService authService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordEncryptionService = passwordEncryptionService ?? throw new ArgumentNullException(nameof(passwordEncryptionService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<(bool isSuccess, string resultOrError)> ValidateUserAsync(LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork.UserAccountRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return (false, "Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return (false, "Account is deactivated");
                }

                if (!_passwordEncryptionService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return (false, "Invalid email or password");
                }

                var token = _authService.GenerateJwtToken(user);
                return (true, token);
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}");
            }
        }

        public async Task<UserAccount?> GetUserAccountAsync(string username, string password)
        {
            try
            {
                var user = await _unitOfWork.UserAccountRepository.GetByUserNameAsync(username);
                if (user == null)
                {
                    user = await _unitOfWork.UserAccountRepository.GetByEmailAsync(username);
                }

                if (user == null || !user.IsActive)
                {
                    return null;
                }

                if (!_passwordEncryptionService.VerifyPassword(password, user.PasswordHash))
                {
                    return null;
                }

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
