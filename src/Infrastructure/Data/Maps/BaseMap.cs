using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models;

namespace StudentApi.Infrastructure.Data.Maps;

public abstract class BaseMap<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("created_at")
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("updated_at")
            .IsRequired();

         entity.Property(e => e.DeletedAt)
            .HasDefaultValueSql("deleted_at")
            .IsRequired(false);

        entity.HasQueryFilter(e => e.DeletedAt == null);
    }
}