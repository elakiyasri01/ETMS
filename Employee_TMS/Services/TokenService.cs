using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Employee_TMS.Services
{
    
    public class TokenService
    {
        private IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(string employeeId,string password)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
           

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5072",
                audience: "http://localhost:5072",
                claims: new[] { new Claim("EmployeeId","Password", employeeId,password) },
                expires: DateTime.UtcNow.AddMinutes(30), // Set token expiration time
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        public bool IsTokenValid(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "http://localhost:5072",
                    ValidAudience = "http://localhost:5072",
                    IssuerSigningKey = key
                }, out var validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
