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
        Entity = new StudentEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            DateOfBirth = dateOfBirth
        };
    }

    private StudentModel(StudentEntity entity)
    {
        Entity = entity;
    }
}