using CodeGenerator.Domain.DependancyInjectionFrameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection
{
    /// <summary>
    /// Registry and manager for dependency injection framework implementations.
    /// Provides access to all registered DI frameworks and helper methods for selection.
    /// </summary>
    public class DependancyInjectionFrameworkManager
    {
        private readonly List<DependancyInjectionFramework> _frameworks = new();

        /// <summary>
        /// All registered dependency injection frameworks
        /// </summary>
        public IReadOnlyList<DependancyInjectionFramework> Frameworks => _frameworks.AsReadOnly();

        /// <summary>
        /// The default DI framework (Microsoft.Extensions.DependencyInjection)
        /// </summary>
        public DependancyInjectionFramework DefaultFramework => 
            _frameworks.FirstOrDefault(f => f.Id == MicrosoftExtensionsDependencyInjection.FRAMEWORK_ID) 
            ?? _frameworks.First();

        /// <summary>
        /// DI Framework manager constructor with optional initial frameworks
        /// </summary>
        public DependancyInjectionFrameworkManager(IEnumerable<DependancyInjectionFramework>? frameworks = null)
        {
            if (frameworks != null)
            {
                _frameworks.AddRange(frameworks);
            }
        }

        /// <summary>
        /// Register a dependency injection framework
        /// </summary>
        public void RegisterFramework(DependancyInjectionFramework framework)
        {
            if (framework == null)
                throw new ArgumentNullException(nameof(framework));

            // Prevent duplicate registrations
            if (_frameworks.Any(f => f.Id == framework.Id))
                throw new InvalidOperationException($"Framework with ID '{framework.Id}' is already registered.");

            _frameworks.Add(framework);
        }

        /// <summary>
        /// Get a framework by its unique ID
        /// </summary>
        public DependancyInjectionFramework? GetFrameworkById(string id)
        {
            return _frameworks.FirstOrDefault(f => f.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get a framework by its display name
        /// </summary>
        public DependancyInjectionFramework? GetFrameworkByName(string name)
        {
            return _frameworks.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get all frameworks that support a specific feature
        /// </summary>
        public IEnumerable<DependancyInjectionFramework> GetFrameworksWithFeature(DIFeature feature)
        {
            return feature switch
            {
                DIFeature.PropertyInjection => _frameworks.Where(f => f.SupportsPropertyInjection),
                DIFeature.Interceptors => _frameworks.Where(f => f.SupportsInterceptors),
                DIFeature.AssemblyScanning => _frameworks.Where(f => f.SupportsAssemblyScanning),
                DIFeature.ScopedLifetime => _frameworks.Where(f => f.SupportsScopedLifetime),
                _ => _frameworks
            };
        }

        /// <summary>
        /// Check if a framework is registered
        /// </summary>
        public bool IsRegistered(string frameworkId)
        {
            return _frameworks.Any(f => f.Id.Equals(frameworkId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Unregister a framework
        /// </summary>
        public bool UnregisterFramework(string frameworkId)
        {
            var framework = GetFrameworkById(frameworkId);
            if (framework != null)
            {
                return _frameworks.Remove(framework);
            }
            return false;
        }

        /// <summary>
        /// Create a new manager with all built-in frameworks registered
        /// </summary>
        public static DependancyInjectionFrameworkManager CreateWithBuiltInFrameworks()
        {
            var manager = new DependancyInjectionFrameworkManager();
            
            // Register all built-in frameworks
            manager.RegisterFramework(new MicrosoftExtensionsDependencyInjection());
            manager.RegisterFramework(new AutofacDependencyInjection());
            manager.RegisterFramework(new SimpleInjectorDependencyInjection());
            manager.RegisterFramework(new NinjectDependencyInjection());
            manager.RegisterFramework(new CastleWindsorDependencyInjection());

            return manager;
        }

        /// <summary>
        /// Get framework display information for UI selection
        /// </summary>
        public IEnumerable<DIFrameworkInfo> GetFrameworkInfos()
        {
            return _frameworks.Select(f => new DIFrameworkInfo
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                SupportsPropertyInjection = f.SupportsPropertyInjection,
                SupportsInterceptors = f.SupportsInterceptors,
                SupportsAssemblyScanning = f.SupportsAssemblyScanning
            });
        }
    }
}
