using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Security.Claims;
using CommonModule.Shared.Common.Auth;

namespace CommonModule.Core.Auth;

public class DynamoDbTokenService : ITokenService
{
    private readonly IDynamoDBContext context;
    private readonly IAmazonDynamoDB dynamoDbClient;
    private readonly IJwtTokenFactory jwtTokenFactory;

    public DynamoDbTokenService(
        IAmazonDynamoDB dynamoDbClient, 
        IDynamoDBContext context,
        IJwtTokenFactory jwtTokenFactory)
    {
        this.dynamoDbClient = dynamoDbClient;
        this.context = context;
        this.jwtTokenFactory = jwtTokenFactory;
    }

    public async Task AddTokenAsync(string token, TimeSpan expiration)
    {
        var userId = this.jwtTokenFactory.GetUserIdFromToken(token);
        var tokenItem = new TokenItem
        {
            UserId = userId,
            Token = token,
            Expiration = DateTime.UtcNow.Add(expiration)
        };

        await this.context.SaveAsync(tokenItem);
    }

    public async Task<bool> IsTokenValidAsync(string token)
    {
        var userId = this.jwtTokenFactory.GetUserIdFromToken(token);
        var tokenItem = await this.context.LoadAsync<TokenItem>(userId, token);
        return tokenItem != null && tokenItem.Expiration > DateTime.UtcNow;
    }

    public async Task RemoveTokenAsync(string token)
    {
        var userId = this.jwtTokenFactory.GetUserIdFromToken(token);
        await this.context.DeleteAsync<TokenItem>(userId, token);
    }

    public async Task RemoveUserTokenAsync(Guid userId)
    {
        var tokens = await this.context.QueryAsync<TokenItem>(userId.ToString()).GetRemainingAsync();
        foreach (var token in tokens)
        {
            await this.context.DeleteAsync(token);
        }
    }

    public async Task RemoveAllTokensAsync(Guid userId)
    {
        await RemoveUserTokenAsync(userId);
    }

    public bool IsTokenExpired(string token)
    {
        var userId = this.jwtTokenFactory.GetUserIdFromToken(token);
        var tokenItem = this.context.LoadAsync<TokenItem>(userId, token).Result;
        return tokenItem == null || tokenItem.Expiration <= DateTime.UtcNow;
    }
}