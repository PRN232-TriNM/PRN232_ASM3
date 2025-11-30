using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EVCS.TriNM.Services.Extensions
{
   
    public class PasswordEncryptionService 
    {
        private readonly IPasswordHasher<object> _passwordHasher;

        public PasswordEncryptionService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        /// <summary>
        /// Encrypts user password using ASP.NET Core Identity algorithm.
        /// </summary>
        /// <param name="plainPassword">Plain password.</param>
        /// <returns>Encrypted password.</returns>
        public string EncryptPassword(string plainPassword)
        {
            return _passwordHasher.HashPassword(null!, plainPassword);
        }

        /// <summary>
        /// Verifies user password.
        /// </summary>
        /// <param name="plainPassword">Plain password entered by user.</param>
        /// <param name="hashedPassword">Encrypted password stored in database.</param>
        /// <returns>True if password matches, otherwise false.</returns>
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
                var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
