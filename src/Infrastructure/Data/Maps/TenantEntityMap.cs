using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Tenants;

namespace StudentApi.Infrastructure.Data.Maps;

public class TenantEntityMap : BaseMap<TenantEntity>
{
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
    }
}