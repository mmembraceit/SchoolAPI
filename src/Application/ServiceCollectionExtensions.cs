using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StudentApi.Application.Services;

namespace StudentApi.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITenantService, TenantService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
