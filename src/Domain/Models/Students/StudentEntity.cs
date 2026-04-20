namespace StudentApi.Domain.Models.Students;

public class StudentEntity : BaseEntity
{
    public Guid TenantId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }
}