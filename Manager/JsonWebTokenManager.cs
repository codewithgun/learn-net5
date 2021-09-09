using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Catalog.Entities;
using Catalog.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Catalog.Manager
{
    public class JsonWebTokenManager
    {
        private string Issuer;
        private string Audience;
        private List<Claim> AdminClaims;
        private List<Claim> UserClaims;
        private SymmetricSecurityKey SecretKey;

        public JsonWebTokenManager(JwtSetting jwtSetting)
        {
            this.Issuer = jwtSetting.Issuer;
            this.Audience = jwtSetting.Audience;
            this.AdminClaims = new List<Claim>{
                    new Claim("type", "Admin")
                };
            this.UserClaims = new List<Claim>{
                new Claim("type", "User"),
                };
            this.SecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret));
        }

        public string CreateUserToken(User user)
        {
            List<Claim> claims = this.UserClaims.ToList(); // shallow copy
            claims.Add(new Claim("email", user.Email));
            var token = new JwtSecurityToken(
                this.Issuer,
                this.Audience,
                this.UserClaims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(this.SecretKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string CreateAdminToken()
        {
            var token = new JwtSecurityToken(
                this.Issuer,
                this.Audience,
                this.AdminClaims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(this.SecretKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}