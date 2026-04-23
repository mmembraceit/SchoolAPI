using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Users;

namespace StudentApi.Infrastructure.Data.Maps;

public class UserEntityMap : BaseMap<UserEntity>
{
    // Fixed GUIDs — must be stable so HasData never creates duplicates on re-migration
    private static readonly Guid SeedTenantId = Guid.Parse("B0000000-0000-0000-0000-000000000002");
    private static readonly Guid SeedUserId   = Guid.Parse("A0000000-0000-0000-0000-000000000001");

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

        entity.HasData(new UserEntity
        {
            Id           = SeedUserId,
            TenantId     = SeedTenantId,
            Email        = "admin@embrace-it.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("embrace-it1234!"),
            CreatedAt    = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt    = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
        });
    }
}
