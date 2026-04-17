namespace StudentApi.Domain.Models.Students;

public class StudentEntity : BaseEntity
{
    public string Name { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }
}