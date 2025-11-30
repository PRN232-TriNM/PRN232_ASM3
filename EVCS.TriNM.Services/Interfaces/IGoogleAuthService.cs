namespace EVCS.TriNM.Services.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<string> ValidateGoogleTokenAndGetOrCreateUser(string credential);
    }
}

