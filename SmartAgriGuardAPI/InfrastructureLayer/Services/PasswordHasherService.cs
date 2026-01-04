using ApplicationLayer.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using System.Security.Cryptography;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service responsible for securely hashing and verifying user passwords.
    /// Utilizes HMAC-SHA512 with a combined salt/pepper strategy for enhanced security.
    /// </summary>
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly string _globalsalt;

        /// <summary>
        /// Initializes the service with global security settings.
        /// </summary>
        /// <param name="options">Configuration options containing the system-wide GlobalSalt (Pepper).</param>
        public PasswordHasherService(IOptions<PasswordSettings> options)
        {
            _globalsalt = options.Value.GlobalSalt;
        }

        /// <summary>
        /// Generates a cryptographic hash for a plain-text password.
        /// </summary>
        /// <remarks>
        /// The salt is derived by combining a system-level 'GlobalSalt' with the unique 'userName'. 
        /// This ensures that even if two users have the same password, their hashes will be different.
        /// </remarks>
        /// <param name="password">The plain-text password to hash.</param>
        /// <param name="userName">The username, used as a per-user salt component.</param>
        /// <param name="passwordHash">The resulting byte array containing the computed hash.</param>
        public void CreatePasswordHash(string password, string userName, out byte[] passwordHash)
        {
            // Combine the global pepper and the username to create a unique key for the HMAC
            string saltString = _globalsalt + userName;
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordHash = hmac.ComputeHash(passwordBytes);
            }
        }

        /// <summary>
        /// Verifies a provided plain-text password against a previously stored hash.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="userName">The username used during the original hashing process.</param>
        /// <param name="storedHash">The hash currently stored in the database.</param>
        /// <returns>True if the password matches the stored hash; otherwise, false.</returns>
        public bool VerifyPasswordHash(string password, string userName, byte[] storedHash)
        {
            string saltString = _globalsalt + userName;
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] computedHash = hmac.ComputeHash(passwordBytes);

                // Compare the newly computed hash with the one from the database
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}