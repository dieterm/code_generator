namespace CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection
{
    /// <summary>
    /// Service lifetime for dependency injection registration
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A single instance is created and shared across all requests
        /// </summary>
        Singleton,
        /// <summary>
        /// A new instance is created for each scope (e.g., per HTTP request)
        /// </summary>
        Scoped,
        /// <summary>
        /// A new instance is created every time it is requested
        /// </summary>
        Transient
    }
}
