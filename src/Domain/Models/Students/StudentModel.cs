namespace StudentApi.Domain.Models.Students;

public class StudentModel
{
    public static StudentModel Create(string name, DateOnly dateOfBirth)
    {
        return new StudentModel(name, dateOfBirth);
    }

    public static StudentModel FromEntity(StudentEntity entity) => new(entity);

    public StudentEntity Entity { get; }

    private StudentModel(string name, DateOnly dateOfBirth)
    {
        var now = DateTimeOffset.UtcNow;
        Entity = new StudentEntity
        {
            Id = Guid.NewGuid(),
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