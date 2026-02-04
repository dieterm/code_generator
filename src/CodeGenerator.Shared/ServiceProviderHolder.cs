using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Shared;

/// <summary>
/// Static class to hold the application's service provider
/// This allows access to DI services from anywhere in the application
/// </summary>
public static class ServiceProviderHolder
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the current service provider
    /// </summary>
    public static IServiceProvider ServiceProvider
    {
        get => _serviceProvider ?? throw new InvalidOperationException(
            "ServiceProvider has not been initialized. Call Initialize() first.");
        private set => _serviceProvider = value;
    }

    /// <summary>
    /// Initializes the service provider
    /// </summary>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Gets a required service from the service provider
    /// </summary>
    public static T GetRequiredService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service from the service provider
    /// </summary>
    public static T? GetService<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    /// <summary>
    /// Creates a new scope for scoped services
    /// </summary>
    public static IServiceScope CreateScope()
    {
        return ServiceProvider.CreateScope();
    }

    public static T? GetKeyedService<T>(object? serviceKey)
    {
        return ServiceProvider.GetKeyedService<T>(serviceKey);
    }

    public static IEnumerable<T> GetKeyedServices<T>(object? serviceKey)
    {
        return ServiceProvider.GetKeyedServices<T>(serviceKey);
    }

    public static IEnumerable<T> GetServices<T>()
    {
        return ServiceProvider.GetServices<T>();
    }
}
