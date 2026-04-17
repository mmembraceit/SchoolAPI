namespace StudentApi.Domain.Models;

public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public bool IsDeleted => DeletedAt != null;

    public void MarkAsDeleted()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}