using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentApi.Application.Messaging;
using StudentApi.Application.Repositories;
using StudentApi.Application.Webhooks;
using StudentApi.Infrastructure.Cache;
using StudentApi.Infrastructure.Data;
using StudentApi.Infrastructure.Messaging;
using StudentApi.Infrastructure.Repositories;
using StudentApi.Infrastructure.Webhooks;

namespace StudentApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StudentApiDb")
            ?? throw new InvalidOperationException("Connection string 'StudentApiDb' was not found.");

        services.AddDbContext<StudentApiDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWebhookSubscriptionRepository, WebhookSubscriptionRepository>();

        services.AddHttpClient<IWebhookDispatcher, WebhookDispatcher>();

        services.AddCache(configuration);
        services.AddServiceBus(configuration);

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = redisConnectionString);
        }
        else
        {
            // Fall back to in-memory distributed cache for local dev without Redis.
            services.AddDistributedMemoryCache();
        }

        // Always register the raw repository, then wrap it with the caching decorator.
        services.AddScoped<StudentRepository>();
        services.AddScoped<IStudentRepository>(sp =>
            new CachedStudentRepository(
                sp.GetRequiredService<StudentRepository>(),
                sp.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CachedStudentRepository>>()));

        return services;
    }

    private static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceBusOptions>(options =>
            configuration.GetSection(ServiceBusOptions.SectionName).Bind(options));

        var connectionString = configuration
            .GetSection(ServiceBusOptions.SectionName)["ConnectionString"];

        // Only register the real Service Bus client when a connection string is configured.
        // This allows the app to start locally without Azure.
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            // ServiceBusClient is thread-safe and should be registered as a singleton.
            services.AddSingleton(new ServiceBusClient(connectionString));
            services.AddScoped<IMessagePublisher, ServiceBusPublisher>();
            services.AddHostedService<StudentEventProcessor>();
        }
        else
        {
            // Register a no-op publisher so DI resolves without errors when Service Bus is not configured.
            services.AddScoped<IMessagePublisher, NullMessagePublisher>();
        }

        return services;
    }
}