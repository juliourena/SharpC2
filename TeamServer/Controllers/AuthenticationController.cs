using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace TeamServer.Controllers
{
    public class AuthenticationController
    {
        private static byte[] _serverPassword { get; set; } = HashPassword("a");
        public static byte[] JWTSecret { get; private set; } = Helpers.GeneratePseudoRandomBytes(128);

        public static void SetPassword(string plaintext)
        {
            _serverPassword = HashPassword(plaintext);
        }

        public static bool ValidatePassword(string plaintext)
        {
            return _serverPassword.SequenceEqual(HashPassword(plaintext));
        }

        public static string GenerateAuthenticationToken(string nick)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, nick) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(JWTSecret), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static byte[] HashPassword(string plaintext)
        {
            using (var crypto = SHA512.Create())
            {
                return crypto.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            }
        }
    }
}
