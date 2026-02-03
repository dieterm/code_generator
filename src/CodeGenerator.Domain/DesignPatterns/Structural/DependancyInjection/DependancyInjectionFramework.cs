using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection
{
    /// <summary>
    /// Abstract base class for dependency injection framework implementations.
    /// Provides a common interface for generating DI container setup code.
    /// </summary>
    public abstract class DependancyInjectionFramework
    {
        /// <summary>
        /// Unique identifier of the dependency injection framework
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the dependency injection framework
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the dependency injection framework
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Whether this framework supports scoped lifetime
        /// </summary>
        public virtual bool SupportsScopedLifetime => true;

        /// <summary>
        /// Whether this framework supports property injection
        /// </summary>
        public virtual bool SupportsPropertyInjection => false;

        /// <summary>
        /// Whether this framework supports interceptors/decorators
        /// </summary>
        public virtual bool SupportsInterceptors => false;

        /// <summary>
        /// Whether this framework supports automatic assembly scanning
        /// </summary>
        public virtual bool SupportsAssemblyScanning => false;

        protected DependancyInjectionFramework(string id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Id = id;
            Name = name;
            Description = description ?? string.Empty;
        }
    }
}
