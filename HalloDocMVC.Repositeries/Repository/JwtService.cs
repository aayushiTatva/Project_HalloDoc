using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class JwtService : IJwtService
    {
        #region Constructor
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration Configuration;
        public JwtService(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.Configuration = Configuration;
        }
        #endregion
        public string GenerateJWTAuthetication(UserInformation userInformation)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userInformation.UserName),
                new Claim(ClaimTypes.Role, userInformation.Role),
                new Claim("FirstName", userInformation.FirstName),
                new Claim("UserId", userInformation.UserId.ToString()),
                new Claim("Username", userInformation.UserName.ToString()),
                new Claim("AspNetUserID", userInformation.AspNetUserId.ToString())
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires =
                DateTime.UtcNow.AddMinutes(60);

            var token = new JwtSecurityToken(
                Configuration["Jwt:Issuer"],
                Configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler)
        {
            jwtSecurityTokenHandler = null;

            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);


                jwtSecurityTokenHandler = (JwtSecurityToken)validatedToken;

                if (jwtSecurityTokenHandler != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}