using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using StudentApi.Infrastructure.Data;

namespace StudentApi.IntegrationTests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly InMemoryDatabaseRoot _dbRoot = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Jwt:SecretKey",         "integration-test-secret-key-min-32-chars!!");
        builder.UseSetting("Jwt:Issuer",            "StudentApi");
        builder.UseSetting("Jwt:Audience",          "StudentApiUsers");
        builder.UseSetting("Jwt:ExpirationMinutes", "60");
        builder.UseSetting("ConnectionStrings:StudentApiDb", "not-used");
        builder.UseSetting("ConnectionStrings:Redis", "");

        builder.ConfigureServices(services =>
        {
            // EF Core 10 registers both DbContextOptions<T> and IDbContextOptionsConfiguration<T>.
            // Both must be removed so SqlServer configuration is fully gone before we add InMemory.
            var efDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<StudentApiDbContext>) ||
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<StudentApiDbContext>))
                .ToList();

            foreach (var d in efDescriptors)
                services.Remove(d);

            // Register InMemory options as a Singleton so every scope shares the same database
            // root, which allows the seed data populated by EnsureCreated() below to be visible
            // during actual request handling.
            services.AddSingleton(
                new DbContextOptionsBuilder<StudentApiDbContext>()
                    .UseInMemoryDatabase("IntegrationTestDb", _dbRoot)
                    .Options);

            // Replace distributed cache so tests don't need Redis.
            var cacheDescriptors = services
                .Where(d => d.ServiceType == typeof(Microsoft.Extensions.Caching.Distributed.IDistributedCache))
                .ToList();

            foreach (var d in cacheDescriptors)
                services.Remove(d);

            services.AddDistributedMemoryCache();

            // Apply schema and seed data.
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StudentApiDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
