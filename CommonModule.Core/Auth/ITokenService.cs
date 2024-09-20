namespace CommonModule.Core.Auth;

public interface ITokenService
{
    Task AddTokenAsync(string token, TimeSpan expiration);
    Task<bool> IsTokenValidAsync(string token);
    Task RemoveTokenAsync(string token);
    Task RemoveUserTokenAsync(Guid userId);
    Task RemoveAllTokensAsync(Guid userId);
}
