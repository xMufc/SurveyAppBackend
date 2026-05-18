using Backend.Entites;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services
{

    /// <summary>
    /// Provides methods for generating JWT tokens
    /// </summary>
    public static class JwtService
    {
        
        /// <summary>
        /// Generates a JSON Web Token for the specified user
        /// </summary>
        /// <param name="user">The user for whom the token is being generated</param>
        /// <param name="config">The application configuration containing JWT settings</param>
        /// <returns>A JWT token as a string</returns>
        public static string GenerateJwtToken(User user, IConfiguration config)
        {
            var jwtSettings = config.GetSection("JwtConfig");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(jwtSettings["ExpireHours"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
