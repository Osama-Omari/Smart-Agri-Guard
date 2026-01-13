using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service responsible for generating JSON Web Tokens (JWT) to facilitate stateless authentication.
    /// It encodes user identity, roles, and regional settings into a signed secure string.
    /// </summary>
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a signed JWT for a validated user.
        /// </summary>
        /// <remarks>
        /// This token includes the following claims:
        /// <list type="bullet">
        /// <item><description><c>NameIdentifier</c>: The User's GUID.</description></item>
        /// <item><description><c>Name</c>: The unique username.</description></item>
        /// <item><description><c>FullName</c>: The display name of the user.</description></item>
        /// <item><description><c>Role</c>: The user's authorization level (Admin, Manager, Farmer).</description></item>
        /// <item><description><c>timezone</c>: The user's local timezone ID for data localization.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="userDTO">The user data to be encoded in the token.</param>
        /// <returns>A string representing the generated JWT.</returns>
        public string GenerateToken(UserDTO userDTO)
        {
            // Define the list of claims to be included in the token payload
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDTO.Id.ToString()),
                new Claim(ClaimTypes.Name, userDTO.Username),
                new Claim("FullName", userDTO.FullName),
                new Claim(ClaimTypes.Role, userDTO.RoleName),
                new Claim("timezone", userDTO.TimezoneId)
            };

            // Retrieve the secret key from configuration and prepare signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token descriptor
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: Claims,
                expires: DateTime.UtcNow.AddYears(100), // Note: Very long-lived token policy
                signingCredentials: creds
                );

            // Serialize the token to a string format
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}