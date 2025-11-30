using EVCS.TriNM.Services.Interfaces;
using EVCS.TriNM.Repositories;
using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EVCS.TriNM.Services.Extensions;

namespace EVCS.TriNM.Services.Implements
{
    public interface IServiceProviders
    {
        IAuthService AuthService { get; }
        ILoginService LoginService { get; }
        IRegisterService RegisterService { get; }
        IUserService UserService { get; }
        IJwtHelperService JwtHelperService { get; }
        IGoogleAuthService GoogleAuthService { get; }
        IStationService StationService { get; }
        IChargerService ChargerService { get; }
        ITransactionService TransactionService { get; }
    }

    public class ServiceProviders : IServiceProviders
    {
        private readonly IUnitOfWork? _injectedUnitOfWork;
        private readonly IConfiguration? _injectedConfiguration;
        
        private IConfiguration? _configuration;
        private IUnitOfWork? _unitOfWork;
        private PasswordEncryptionService? _passwordEncryptionService;
        private IAuthService? _authService;
        private ILoginService? _loginService;
        private IRegisterService? _registerService;
        private IUserService? _userService;
        private IJwtHelperService? _jwtHelperService;
        private IGoogleAuthService? _googleAuthService;
        private IStationService? _stationService;
        private IChargerService? _chargerService;
        private ITransactionService? _transactionService;

        public ServiceProviders(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _injectedUnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _injectedConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public ServiceProviders()
        {
        }

        private IUnitOfWork UnitOfWork
        {
            get 
            { 
                if (_injectedUnitOfWork != null)
                {
                    return _injectedUnitOfWork;
                }
                
                if (_unitOfWork != null)
                {
                    return _unitOfWork;
                }
                
                var config = Configuration;
                var connectionString = config.GetConnectionString("DefaultConnection");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
                }
                var optionsBuilder = new DbContextOptionsBuilder<EVChargingDBContext>();
                optionsBuilder.UseSqlServer(connectionString);
                
                var context = new EVChargingDBContext(optionsBuilder.Options);
                _unitOfWork = new UnitOfWork(context);
                return _unitOfWork;
            }
        }

        private IConfiguration Configuration
        {
            get
            {
                if (_injectedConfiguration != null)
                {
                    return _injectedConfiguration;
                }
                
                if (_configuration == null)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
                    _configuration = builder.Build();
                }
                return _configuration;
            }
        }

        private PasswordEncryptionService PasswordEncryptionService
        {
            get { return _passwordEncryptionService ??= new PasswordEncryptionService(); }
        }

        public IAuthService AuthService
        {
            get { return _authService ??= new AuthService(UnitOfWork, Configuration); }
        }

        public ILoginService LoginService
        {
            get { return _loginService ??= new LoginService(UnitOfWork, PasswordEncryptionService, AuthService); }
        }

        public IRegisterService RegisterService
        {
            get { return _registerService ??= new RegisterService(UnitOfWork, PasswordEncryptionService); }
        }

        public IUserService UserService
        {
            get { return _userService ??= new UserService(UnitOfWork); }
        }

        public IJwtHelperService JwtHelperService
        {
            get { return _jwtHelperService ??= new JwtHelperService(); }
        }

        public IGoogleAuthService GoogleAuthService
        {
            get 
            { 
                if (_googleAuthService == null)
                {
                    var context = UnitOfWork.Context;
                    _googleAuthService = new GoogleAuthService(context, Configuration);
                }
                return _googleAuthService; 
            }
        }

        public IStationService StationService
        {
            get { return _stationService ??= new StationService(UnitOfWork); }
        }

        public IChargerService ChargerService
        {
            get { return _chargerService ??= new ChargerService(UnitOfWork); }
        }

        public ITransactionService TransactionService
        {
            get { return _transactionService ??= new TransactionService(UnitOfWork); }
        }
    }
}