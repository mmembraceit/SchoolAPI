using Microsoft.Extensions.DependencyInjection;
using StudentApi.Application.Services;

namespace StudentApi.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITenantService, TenantService>();

        return services;
    }
}
