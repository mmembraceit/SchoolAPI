using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Users;

namespace StudentApi.Infrastructure.Data.Maps;

public class UserEntityMap : BaseMap<UserEntity>
{
    private static readonly Guid TenantOms       = Guid.Parse("B0000000-0000-0000-0000-000000000000");
    private static readonly Guid TenantAcl       = Guid.Parse("B0000000-0000-0000-0000-000000000001");
    private static readonly Guid TenantEmbraceIt = Guid.Parse("B0000000-0000-0000-0000-000000000002");

    private static readonly Guid SeedUserId  = Guid.Parse("A0000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedUserId2 = Guid.Parse("A0000000-0000-0000-0000-000000000002");
    private static readonly Guid SeedUserId3 = Guid.Parse("A0000000-0000-0000-0000-000000000003");

    // All seed users share the same password: Admin@2026
    private const string SeedPasswordHash = "$2a$11$CLq2MdG.BRilFcjK6xbGreGxgh7QZLHQxArAkKOLjPbL2Qt86mdbe";

    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public override void Configure(EntityTypeBuilder<UserEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("users");

        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.TenantId).IsRequired();

        entity.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        entity.Property(e => e.Role)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("user");

        entity.HasData(
            new UserEntity
            {
                Id           = SeedUserId,
                TenantId     = TenantOms,
                Email        = "admin@oms-ltd.com",
                PasswordHash = SeedPasswordHash,
                Role         = "admin",
                CreatedAt    = SeedDate,
                UpdatedAt    = SeedDate,
            },
            new UserEntity
            {
                Id           = SeedUserId2,
                TenantId     = TenantAcl,
                Email        = "admin@acl-inc.com",
                PasswordHash = SeedPasswordHash,
                Role         = "admin",
                CreatedAt    = SeedDate,
                UpdatedAt    = SeedDate,
            },
            new UserEntity
            {
                Id           = SeedUserId3,
                TenantId     = TenantEmbraceIt,
                Email        = "admin@embrace-it.com",
                PasswordHash = SeedPasswordHash,
                Role         = "admin",
                CreatedAt    = SeedDate,
                UpdatedAt    = SeedDate,
            });
    }
}
