using CodeGenerator.Domain.CodeElements;
using Microsoft.Extensions.Primitives;

namespace CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection
{
    /// <summary>
    /// Represents a service registration in the DI container
    /// </summary>
    public class ServiceRegistration
    {
        public string? RawRegistrationCode { get; set; }
        /// <summary>
        /// The contract/interface type
        /// </summary>
        public TypeReference ServiceType { get; set; } = new();

        /// <summary>
        /// The implementation type (null if same as ServiceType)
        /// </summary>
        public TypeReference? ImplementationType { get; set; }

        /// <summary>
        /// The lifetime of the service
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// Factory method expression (as code string) for factory-based registration
        /// </summary>
        public string? FactoryExpression { get; set; }

        /// <summary>
        /// Whether this registration uses a factory
        /// </summary>
        public bool UsesFactory => !string.IsNullOrEmpty(FactoryExpression);

        /// <summary>
        /// Instance expression (as code string) for instance-based registration
        /// </summary>
        public string? InstanceExpression { get; set; }

        /// <summary>
        /// Whether this registration uses an instance
        /// </summary>
        public bool UsesInstance => !string.IsNullOrEmpty(InstanceExpression);
    }
}
