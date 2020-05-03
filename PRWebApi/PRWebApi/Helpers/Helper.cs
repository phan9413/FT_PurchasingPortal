using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using PRWebApi.Models;

namespace PRWebApi.Helpers
{
    public class Helper
    {
        private IConfiguration _config;
        public Helper(IConfiguration config)
        {
            _config = config;
        }
        public Helper()
        { }
        public string GenerateJasonWebToken(UserLogin user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Oid.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: credentials);

            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }

        public string GetJsonWebToken(string token, string tokeykey)
        {
            //SecurityToken jwttoken = new JwtSecurityTokenHandler().ReadToken(token);
            //var claims = new JwtSecurityTokenHandler().ValidateToken(jwttoken);
            //return token;
            return "";
        }
    }

}
