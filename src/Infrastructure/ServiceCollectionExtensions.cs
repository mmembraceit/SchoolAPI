using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentApi.Application.Repositories;
using StudentApi.Infrastructure.Data;
using StudentApi.Infrastructure.Repositories;

namespace StudentApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StudentApiDb")
            ?? throw new InvalidOperationException("Connection string 'StudentApiDb' was not found.");

        services.AddDbContext<StudentApiDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();

        return services;
    }
}