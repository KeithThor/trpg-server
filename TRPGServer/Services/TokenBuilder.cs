using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRPGServer.Data;

namespace TRPGServer.Services
{
    /// <summary>
    /// Class responsible for creating Jwt security tokens.
    /// </summary>
    public class TokenBuilder
    {
        private readonly SymmetricKeyBuilder _keyBuilder;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public TokenBuilder(SymmetricKeyBuilder keyBuilder,
                            JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _keyBuilder = keyBuilder;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }

        /// <summary>
        /// Creates and returns a JWT token for a user.
        /// </summary>
        /// <param name="user">The user to create a token for.</param>
        /// <param name="claims">An IEnumerable of claims for the user.</param>
        /// <returns>A JWT token as a string.</returns>
        public string CreateToken(ApplicationUser user, IEnumerable<Claim> claims)
        {
            var symmetricKey = _keyBuilder.GetSymmetricKey();
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: "localhost",
                audience: "players",
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: signingCredentials,
                claims: claims);

            return _jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
