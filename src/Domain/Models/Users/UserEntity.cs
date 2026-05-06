namespace StudentApi.Domain.Models.Users;

public class UserEntity : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "user";
}
