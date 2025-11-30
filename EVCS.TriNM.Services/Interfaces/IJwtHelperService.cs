namespace EVCS.TriNM.Services.Interfaces
{
    public interface IJwtHelperService
    {
        int? GetCurrentUserIdFromToken(string? authHeader);
    }
}
