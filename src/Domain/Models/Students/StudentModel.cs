namespace StudentApi.Domain.Models.Students;

public class StudentModel
{
    public static StudentModel Create(Guid tenantId, string name, DateOnly dateOfBirth)
    {
        return new StudentModel(tenantId, name, dateOfBirth);
    }

    public static StudentModel FromEntity(StudentEntity entity) => new(entity);

    public StudentEntity Entity { get; }

    private StudentModel(Guid tenantId, string name, DateOnly dateOfBirth)
    {
        var now = DateTimeOffset.UtcNow;
        Entity = new StudentEntity
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            DateOfBirth = dateOfBirth,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private StudentModel(StudentEntity entity)
    {
        Entity = entity;
    }
}