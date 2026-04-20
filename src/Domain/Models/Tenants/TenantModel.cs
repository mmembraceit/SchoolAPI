namespace  StudentApi.Domain.Models.Tenants;

public class TenantModel
{
    public static TenantModel Create( string name, string? description = null)
    {
        return new TenantModel(name, description);
    }

    public static TenantModel FromEntity(TenantEntity entity) => new(entity);

    public TenantEntity Entity { get; }

    private TenantModel(string name, string? description)
    {
        var now = DateTimeOffset.UtcNow;
        Entity = new TenantEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private TenantModel(TenantEntity entity)
    {
        Entity = entity;
    }

}