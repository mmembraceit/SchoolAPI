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
            .IsRequired();

        entity.Property(e => e.UpdatedAt)
            .IsRequired();

        entity.Property(e => e.DeletedAt)
            .IsRequired(false);

        entity.HasQueryFilter(e => e.DeletedAt == null);
    }
}