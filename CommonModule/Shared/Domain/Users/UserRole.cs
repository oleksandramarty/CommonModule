using CommonModule.Shared.Common;

namespace CommonModule.Shared.Domain.Users;

public abstract class UserRole : BaseIdEntity<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}