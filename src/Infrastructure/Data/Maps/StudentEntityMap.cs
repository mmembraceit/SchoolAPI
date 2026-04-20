using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Students;

namespace StudentApi.Infrastructure.Data.Maps;

public class StudentEntityMap : BaseMap<StudentEntity>
{
    public override void Configure(EntityTypeBuilder<StudentEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("students");

        entity.Property(e => e.TenantId)
            .IsRequired();

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.DateOfBirth)
            .IsRequired();
    }
}