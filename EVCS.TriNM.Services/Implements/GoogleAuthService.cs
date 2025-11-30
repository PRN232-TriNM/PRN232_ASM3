using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Google.Apis.Auth;
using EVCS.TriNM.Repositories.Models;
using EVCS.TriNM.Repositories.Context;
using EVCS.TriNM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EVCS.TriNM.Services.Implements
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly EVChargingDBContext _context;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(EVChargingDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> ValidateGoogleTokenAndGetOrCreateUser(string credential)
        {
            try
            {
                // Validate the Google ID token
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

                // Check if user exists
                var user = await _context.UserAccounts.FirstOrDefaultAsync(u => 
                    (!string.IsNullOrEmpty(payload.Email) && u.Email == payload.Email) || 
                    (!string.IsNullOrEmpty(payload.Subject) && u.GoogleId == payload.Subject));

                if (user == null)
                {
                    // Validate required fields from Google payload
                    if (string.IsNullOrEmpty(payload.Email) || string.IsNullOrEmpty(payload.Subject))
                    {
                        throw new Exception("Invalid Google token: missing email or subject");
                    }

                    // Create new user
                    user = new UserAccount
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FullName = payload.Name ?? "Google User",
                        GoogleId = payload.Subject,
                        PhotoUrl = payload.Picture,
                        RoleId = 1, // Set default role
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                    };

                    _context.UserAccounts.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update existing user's Google info if needed
                    if (string.IsNullOrEmpty(user.GoogleId))
                    {
                        user.GoogleId = payload.Subject;
                        user.PhotoUrl = payload.Picture;
                        await _context.SaveChangesAsync();
                    }
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to validate Google token", ex);
            }
        }

        private string GenerateJwtToken(UserAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var nameClaim = user.FullName ?? user.UserName ?? user.Email;
            Console.WriteLine($"Generating JWT for user: Email={user.Email}, Name={nameClaim}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserAccountId.ToString()),
                    new Claim(ClaimTypes.Name, nameClaim),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                }),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            Console.WriteLine($"JWT Token generated successfully");
            return tokenString;
        }
    }
}

