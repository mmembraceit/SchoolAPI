using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StudentApi.Domain.Models.Students;
using StudentApi.Domain.Models.Tenants;
using StudentApi.Domain.Models.Users;

namespace StudentApi.Infrastructure.Data;

public class StudentApiDbContext : DbContext
{
    public StudentApiDbContext(DbContextOptions<StudentApiDbContext> options) : base(options)
    {
    }

    public DbSet<StudentEntity> Students { get; set; } = null!;

    public DbSet<TenantEntity> Tenants { get; set; } = null!;

    public DbSet<UserEntity> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}