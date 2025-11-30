using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Services.Extensions;
using EVCS.TriNM.Services.Interfaces;
using EVCS.TriNM.Services.Object.Requests;

namespace EVCS.TriNM.Services.Implements
{
    public class RegisterService : IRegisterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PasswordEncryptionService _passwordEncryptionService;

        public RegisterService(IUnitOfWork unitOfWork, PasswordEncryptionService passwordEncryptionService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordEncryptionService = passwordEncryptionService ?? throw new ArgumentNullException(nameof(passwordEncryptionService));
        }

        public async Task<(bool isSuccess, string resultOrError)> RegisterUserAsync(RegisterRequest request)
        {
            try
            {
                var existingUserByEmail = await _unitOfWork.UserAccountRepository.GetByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return (false, "Email already exists");
                }

                var existingUserByUsername = await _unitOfWork.UserAccountRepository.GetByUserNameAsync(request.UserName);
                if (existingUserByUsername != null)
                {
                    return (false, "Username already exists");
                }

                var newUser = new UserAccount
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PasswordHash = _passwordEncryptionService.EncryptPassword(request.Password),
                    FullName = request.FullName,
                    Phone = request.Phone ?? string.Empty,
                    EmployeeCode = request.EmployeeCode,
                    RoleId = request.RoleId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.UserAccountRepository.AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                return (true, "User registered successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}");
            }
        }
    }
}
