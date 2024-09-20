using System.Security.Claims;
using CommonModule.Shared.Domain.Users;

namespace CommonModule.Core.Auth;

public interface IJwtTokenFactory
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
    string GenerateJwtToken(
        Guid userId,
        string login,
        string email,
        string roles,
        bool rememberMe = false, 
        ClaimsIdentity additionalClaims = null);

    string GenerateNewJwtToken(ClaimsPrincipal user);

    Guid GetUserIdFromToken(string token);
    bool IsTokenRefreshable(string token);
}