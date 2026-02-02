namespace CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection
{
    /// <summary>
    /// Information about a DI framework for UI display
    /// </summary>
    public class DIFrameworkInfo
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool SupportsPropertyInjection { get; init; }
        public bool SupportsInterceptors { get; init; }
        public bool SupportsAssemblyScanning { get; init; }
    }
}
