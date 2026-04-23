using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Tenants;

namespace StudentApi.Infrastructure.Data.Maps;

public class TenantEntityMap : BaseMap<TenantEntity>
{
    private static readonly Guid SeedTenantId = Guid.Parse("B0000000-0000-0000-0000-000000000002");

    public override void Configure(EntityTypeBuilder<TenantEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("tenants");

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        entity.HasData(new TenantEntity
        {
            Id          = SeedTenantId,
            Name        = "Embrace IT",
            Description = "Default seed tenant",
            CreatedAt   = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt   = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
        });
    }
}