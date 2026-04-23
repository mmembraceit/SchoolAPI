using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentApi.Domain.Models.Users;

namespace StudentApi.Infrastructure.Data.Maps;

public class UserEntityMap : BaseMap<UserEntity>
{
    private static class Seed
    {
        public static readonly Guid TenantId = Guid.Parse("B0000000-0000-0000-0000-000000000000");
        public static readonly Guid UserId = Guid.Parse("A0000000-0000-0000-0000-000000000001");
        public static readonly DateTimeOffset Timestamp = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public const string AdminEmail = "admin@embrace-it.com";
        public const string PasswordHash = "$2a$11$UVl3rWdEfmTmln4zbBWlvugBMUOyV8vvlJhsuWIPVZQRk9SmnUc/O";
    }

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

        entity.HasData(CreateSeedAdminUser());
    }

    private static UserEntity CreateSeedAdminUser()
    {
        return new UserEntity
        {
            Id = Seed.UserId,
            TenantId = Seed.TenantId,
            Email = Seed.AdminEmail,
            PasswordHash = Seed.PasswordHash,
            CreatedAt = Seed.Timestamp,
            UpdatedAt = Seed.Timestamp,
        };
    }
}
