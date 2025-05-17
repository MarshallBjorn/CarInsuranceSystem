

namespace Infrastructure;

/// <summary>
/// Provides a static service locator for accessing dependency injection services.
/// </summary>
public static class ServiceLocator
{
    private static IServiceProvider? _provider;

    /// <summary>
    /// Initializes the service locator with the specified service provider.
    /// </summary>
    /// <param name="provider">The service provider to use for resolving services.</param>
    /// <exception cref="ArgumentNullException">Thrown when provider is null.</exception>
    public static void Initialize(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    /// Gets a service of type T from the service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <returns>The service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service provider is not initialized.</exception>
    public static T GetService<T>()
    {
        if (_provider == null)
        {
            throw new InvalidOperationException("ServiceLocator is not initialized.");
        }
        return _provider.GetService<T>();
    }

    /// <summary>
    /// Gets the current application state.
    /// </summary>

    /// <summary>
    /// Creates a new service scope.
    /// </summary>
    /// <returns>A new service scope.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service provider is not initialized.</exception>
    public static IServiceScope CreateScope()
    {
        if (_provider == null)
        {
            throw new InvalidOperationException("ServiceLocator is not initialized.");
        }
        return _provider.CreateScope();
    }
}