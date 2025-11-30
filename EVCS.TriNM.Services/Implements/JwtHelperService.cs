using EVCS.TriNM.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace EVCS.TriNM.Services.Implements
{
    public class JwtHelperService : IJwtHelperService
    {
        public int? GetCurrentUserIdFromToken(string? authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenString = authHeader.Substring("Bearer ".Length).Trim();
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(tokenString);
                
                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch (Exception)
            {
                // Token is invalid or expired
            }

            return null;
        }
    }
}
