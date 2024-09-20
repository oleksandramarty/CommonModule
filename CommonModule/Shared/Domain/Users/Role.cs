using CommonModule.Shared.Common;
using CommonModule.Shared.Enums;

namespace CommonModule.Shared.Domain.Users;

public abstract class Role: BaseIdEntity<int>
{
    public string Title { get; set; }
    public UserRoleEnum UserRole { get; set; }
    
    public ICollection<UserRole> Users { get; set; }
}