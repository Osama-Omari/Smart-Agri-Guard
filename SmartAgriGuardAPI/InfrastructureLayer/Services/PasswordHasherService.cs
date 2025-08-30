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
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly string _globalsalt;

        public PasswordHasherService(IOptions<PasswordSettings> options)
        {
            _globalsalt = options.Value.GlobalSalt;
        }

        
        public void CreatePasswordHash(string password, string userName, out byte[] passwordHash)
        {
            string saltString = _globalsalt + userName;
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordHash = hmac.ComputeHash(passwordBytes);
            }

            
        }

        public bool VerifyPasswordHash(string password, string userName, byte[] storedHash)
        {
            string saltString = _globalsalt + userName;
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] computedHash = hmac.ComputeHash(passwordBytes);
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
