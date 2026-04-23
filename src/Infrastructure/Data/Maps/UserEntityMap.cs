using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Users;

namespace StudentApi.Infrastructure.Data.Maps;

public class UserEntityMap : BaseMap<UserEntity>
{
    private static readonly Guid SeedTenantId = Guid.Parse("B0000000-0000-0000-0000-000000000000");
    private static readonly Guid SeedUserId   = Guid.Parse("A0000000-0000-0000-0000-000000000001");
    private const string SeedPasswordHash = "$2a$11$UVl3rWdEfmTmln4zbBWlvugBMUOyV8vvlJhsuWIPVZQRk9SmnUc/O";

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
            PasswordHash = SeedPasswordHash,
            CreatedAt    = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt    = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
        });
    }
}
