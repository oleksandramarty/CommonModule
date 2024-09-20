using CommonModule.Shared.Domain.Users;

namespace CommonModule.Core.Auth;

public interface IJwtTokenFactory
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
    string GenerateJwtToken(User user, bool rememberMe = false);
    string ExtractUserIdFromToken(string token);
}