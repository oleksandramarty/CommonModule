using System.Security.Claims;
using CommonModule.Shared.Enums;

namespace CommonModule.Core.Auth;

public interface IAuthService
{
    string GetCurrentToken();
    IEnumerable<Claim> GetCurrentClaims();
    Guid? GetCurrentUserId();
    UserRoleEnum GetCurrentUserRole();

    Task<string> GetCurrentTokenAsync();
    Task<IEnumerable<Claim>> GetCurrentClaimsAsync();
    Task<Guid?> GetCurrentUserIdAsync();
    Task<UserRoleEnum> GetCurrentUserRoleAsync();
        
    bool IsAuthenticated();

    Task<bool> HasUserInRoleAsync(UserRoleEnum role);
}