using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.IdentityModel.Tokens;

namespace CodeSparkNET.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _conf;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration conf, SymmetricSecurityKey key)
        {
            _conf = conf;
            _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_conf["JWT:SigningKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _conf["JWT:Issuer"],
                Audience = _conf["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}