using JWTExample.Constants;
using JWTExample.DTOs;
using JWTExample.Helpers.Interfaces;
using JWTExample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWTExample.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private Settings _settings;
        private static IHttpContextAccessor _httpContextAccessor;

        public TokenHelper(IOptions<Settings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public string CreateUserToken(UserDTO request)
        {
            PasswordHelper.CreatePasswordHash(CustomerModelExample.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new UserModel
            {
                Email = CustomerModelExample.Email,
                Password = passwordHash,
                Salt = passwordSalt,
            };
            //Let's assume that above code comes from database :)

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] requestPasswordHash, out byte[] requestPasswordSalt);
            if (!PasswordHelper.VerifyPassword(request.Password, user.Password, user.Salt) || request.Email != user.Email)
            {
                throw new ArgumentException(ExceptionMessages.InvalidEmailOrPassword);
            }

            List<Claim> claim = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_settings.Token));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_settings.Issuer, null,

                claims: claim, expires: DateTime.Now.AddMinutes(_settings.ExpireDate), signingCredentials: cred
                ));

            if (_httpContextAccessor.HttpContext.Response.Headers.ContainsKey(Headers.Token))
            {
                _httpContextAccessor.HttpContext.Response.Headers.Add(Headers.Token, token);
            }


            return token;
        }

        public UserDTO DecodeUserToken()
        {
            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(Headers.Token, out var token);
            if (string.IsNullOrEmpty(token))
                return null;


            var validate = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_settings.Token))

            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;

            return new UserDTO { Email = email };
        }
    }



}
