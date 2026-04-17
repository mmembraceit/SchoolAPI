namespace StudentApi.Domain.Models.Tenants;

public class TenantEntity : BaseEntity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}