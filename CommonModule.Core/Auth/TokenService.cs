namespace CommonModule.Core.Auth;

public class TokenService: ITokenService
{
    public Task AddTokenAsync(string token, TimeSpan expiration)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenValidAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task RemoveTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUserTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAllTokensAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}