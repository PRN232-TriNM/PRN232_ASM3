using EVCS.TriNM.Services.Object.Requests;

namespace EVCS.TriNM.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<(bool isSuccess, string resultOrError)> RegisterUserAsync(RegisterRequest request);
    }
}
