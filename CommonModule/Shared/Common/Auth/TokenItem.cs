namespace CommonModule.Shared.Common.Auth;

public class TokenItem
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}