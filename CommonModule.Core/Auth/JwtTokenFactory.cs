using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CommonModule.Core.Extensions;
using CommonModule.Shared.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CommonModule.Core.Auth;

public class JwtTokenFactory: IJwtTokenFactory
{
    private readonly IConfiguration configuration;
    
    public JwtTokenFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    
    public string GenerateSalt()
    {
        var rng = new RNGCryptoServiceProvider();
        var saltBytes = new byte[32]; // Increased salt size to 32 bytes
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    public string HashPassword(string password, string salt)
    {
        var iterations = 50000; // Increased iterations to 50000
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations, HashAlgorithmName.SHA256))
        {
            return Convert.ToBase64String(pbkdf2.GetBytes(32)); // Increased hash size to 32 bytes
        }
    }
    
    public string GenerateJwtToken(User user, bool rememberMe = false)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        string secretKey = configuration["Authentication:Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
        {
            throw new ArgumentException("The JWT secret key must be at least 32 characters long.");
        }
        byte[] key = secretKey.StringToUtf8Bytes();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Roles.FirstOrDefault().Role.UserRole.ToString())
            }),
            Expires = rememberMe ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public string ExtractUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
        {
            throw new ArgumentException("The token is not in a valid JWT format.");
        }

        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
        return userIdClaim?.Value;
    }
}