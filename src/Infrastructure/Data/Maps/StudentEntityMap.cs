using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Students;

namespace StudentApi.Infrastructure.Data.Maps;

public class StudentEntityMap : BaseMap<StudentEntity>
{
    // Tenant IDs must match TenantEntityMap seed values.
    private static readonly Guid TenantOms      = Guid.Parse("B0000000-0000-0000-0000-000000000000");
    private static readonly Guid TenantAcl      = Guid.Parse("B0000000-0000-0000-0000-000000000001");
    private static readonly Guid TenantEmbraceIt = Guid.Parse("B0000000-0000-0000-0000-000000000002");

    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

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

        entity.HasData(
            // OMS Ltd students
            new StudentEntity
            {
                Id          = Guid.Parse("C0000000-0000-0000-0000-000000000001"),
                TenantId    = TenantOms,
                Name        = "Alice Johnson",
                DateOfBirth = new DateOnly(2008, 3, 15),
                CreatedAt   = SeedDate,
                UpdatedAt   = SeedDate,
            },
            new StudentEntity
            {
                Id          = Guid.Parse("C0000000-0000-0000-0000-000000000002"),
                TenantId    = TenantOms,
                Name        = "Bob Smith",
                DateOfBirth = new DateOnly(2009, 7, 22),
                CreatedAt   = SeedDate,
                UpdatedAt   = SeedDate,
            },
            // ACL Inc students
            new StudentEntity
            {
                Id          = Guid.Parse("C0000000-0000-0000-0000-000000000003"),
                TenantId    = TenantAcl,
                Name        = "Carlos Rivera",
                DateOfBirth = new DateOnly(2007, 11, 5),
                CreatedAt   = SeedDate,
                UpdatedAt   = SeedDate,
            },
            new StudentEntity
            {
                Id          = Guid.Parse("C0000000-0000-0000-0000-000000000004"),
                TenantId    = TenantAcl,
                Name        = "Diana Chen",
                DateOfBirth = new DateOnly(2010, 1, 30),
                CreatedAt   = SeedDate,
                UpdatedAt   = SeedDate,
            },
            // Embrace IT students
            new StudentEntity
            {
                Id          = Guid.Parse("C0000000-0000-0000-0000-000000000005"),
                TenantId    = TenantEmbraceIt,
                Name        = "Ethan Patel",
                DateOfBirth = new DateOnly(2008, 9, 12),
                CreatedAt   = SeedDate,
                UpdatedAt   = SeedDate,
            },
            new StudentEntity
            {
                Id          = Guid.Parse("C0000000-0000-0000-0000-000000000006"),
                TenantId    = TenantEmbraceIt,
                Name        = "Fatima Al-Hassan",
                DateOfBirth = new DateOnly(2009, 4, 8),
                CreatedAt   = SeedDate,
                UpdatedAt   = SeedDate,
            });
    }
}