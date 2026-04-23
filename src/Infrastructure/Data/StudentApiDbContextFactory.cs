using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentApi.Infrastructure.Data;

public sealed class StudentApiDbContextFactory : IDesignTimeDbContextFactory<StudentApiDbContext>
{
    public StudentApiDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<StudentApiDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=StudentApiDb;User Id=sa;Password=SchoolApi@2026;TrustServerCertificate=True;")
            .Options;

        return new StudentApiDbContext(options);
    }
}