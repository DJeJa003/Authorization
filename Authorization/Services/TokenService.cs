using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(string username, bool isAdmin)
        {
            var secretKeyString = _configuration["JwtSecretKey"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            var roleClaim = isAdmin ? new Claim(ClaimTypes.Role, "admin") : new Claim(ClaimTypes.Role, "testuser");
            claims.Add(roleClaim);

            Console.WriteLine($"Generated token for user {username} with {(isAdmin ? "admin" : "testuser")} role.");
            var tokenOptions = new JwtSecurityToken(
                issuer: "MyTestAuthServer",
                audience: "MyTestApiUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}
