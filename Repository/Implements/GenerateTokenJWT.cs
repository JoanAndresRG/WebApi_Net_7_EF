using MagicVillaApi.Models;
using MagicVillaApi.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVillaApi.Repository.Implements
{
    public class GenerateTokenJWT : IGenerateTokenJWT
    {
        private readonly IConfiguration _configuration;
        public GenerateTokenJWT(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<User> GenerateToken(User user)
        {
            // Header
            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:ClaveSecreta"])
                );
            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _header = new JwtHeader(_signingCredentials);

            // Claims 
            var _claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRol) // Agregar el rol del usuario al token
            };

            // Payload

            var _payload = new JwtPayload(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: _claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(30)
            );

            // Token

            var _token = new JwtSecurityToken(
                    _header,
                    _payload
                );

            user.Token = new JwtSecurityTokenHandler().WriteToken(_token);

            return user;
        }
    }
}
