using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repositories
{
    public class TokenManagerRepository : ITokenManager
    {
        private readonly SymmetricSecurityKey _key;

        public TokenManagerRepository(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["WebPocHubJWT:Secret"]));
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.email)

            // Puedes agregar más claims según tus necesidades
        };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256); // Cambiar a HMAC-SHA256
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1), // Expire en 1 minutos a partir de ahora
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
