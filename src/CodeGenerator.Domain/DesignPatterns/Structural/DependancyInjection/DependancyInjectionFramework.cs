using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DotNet;
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
        /// The type name of the service collection/container builder
        /// </summary>
        public abstract string ContainerBuilderTypeName { get; }

        /// <summary>
        /// The type name of the built container/service provider
        /// </summary>
        public abstract string ContainerTypeName { get; }

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

        #region NuGet Packages

        /// <summary>
        /// Define NuGet packages required by this dependency injection framework
        /// </summary>
        public abstract IEnumerable<NuGetPackage> GetRequiredNuGetPackages();

        /// <summary>
        /// Get additional NuGet packages for specific features
        /// </summary>
        public virtual IEnumerable<NuGetPackage> GetOptionalNuGetPackages()
        {
            return Enumerable.Empty<NuGetPackage>();
        }

        #endregion

        #region Using Statements

        /// <summary>
        /// Get required using/import statements for DI container setup
        /// </summary>
        public abstract IEnumerable<string> GetRequiredUsings();

        /// <summary>
        /// Get using statements for extension methods (e.g., AddControllers, AddDbContext)
        /// </summary>
        public virtual IEnumerable<string> GetExtensionMethodUsings()
        {
            return Enumerable.Empty<string>();
        }

        #endregion

        #region Container Setup / Bootstrap

        /// <summary>
        /// Generates the DI-container bootstrap setup class in the code file and return the created class element.
        /// This is the main entry point class that configures all services.
        /// </summary>
        public abstract ClassElement GenerateContainerSetupClass(CodeFileElement codeFileElement, string className = "ServiceCollectionExtensions");

        /// <summary>
        /// Generate the code to create/initialize the container builder
        /// </summary>
        /// <param name="variableName">Variable name for the container builder</param>
        /// <returns>Code statement to create the container builder</returns>
        public abstract string GenerateContainerBuilderCreation(string variableName = "services");

        /// <summary>
        /// Generate the code to build the container from the builder
        /// </summary>
        /// <param name="builderVariableName">Variable name of the container builder</param>
        /// <param name="containerVariableName">Variable name for the built container</param>
        /// <returns>Code statement to build the container</returns>
        public abstract string GenerateBuildContainer(string builderVariableName = "services", string containerVariableName = "serviceProvider");

        /// <summary>
        /// Generate code to resolve a service from the container
        /// </summary>
        /// <param name="containerVariableName">Variable name of the container</param>
        /// <param name="serviceType">Type of service to resolve</param>
        /// <returns>Code expression to resolve the service</returns>
        public abstract string GenerateResolveService(string containerVariableName, TypeReference serviceType);

        /// <summary>
        /// Generate code to resolve a service from the container (generic version)
        /// </summary>
        public virtual string GenerateResolveService<T>(string containerVariableName)
        {
            return GenerateResolveService(containerVariableName, new TypeReference(typeof(T).Name));
        }

        #endregion

        #region Service Registration - Basic

        /// <summary>
        /// Generate code to register a service with the specified lifetime
        /// </summary>
        public abstract string GenerateServiceRegistration(ServiceRegistration registration);

        /// <summary>
        /// Generate code to register a singleton service
        /// </summary>
        public abstract string GenerateRegisterSingleton(TypeReference serviceType, TypeReference? implementationType = null);

        /// <summary>
        /// Generate code to register a scoped service
        /// </summary>
        public abstract string GenerateRegisterScoped(TypeReference serviceType, TypeReference? implementationType = null);

        /// <summary>
        /// Generate code to register a transient service
        /// </summary>
        public abstract string GenerateRegisterTransient(TypeReference serviceType, TypeReference? implementationType = null);

        #endregion

        #region Service Registration - Factory

        /// <summary>
        /// Generate code to register a singleton with a factory method
        /// </summary>
        public abstract string GenerateRegisterSingletonFactory(TypeReference serviceType, string factoryExpression);

        /// <summary>
        /// Generate code to register a scoped service with a factory method
        /// </summary>
        public abstract string GenerateRegisterScopedFactory(TypeReference serviceType, string factoryExpression);

        /// <summary>
        /// Generate code to register a transient service with a factory method
        /// </summary>
        public abstract string GenerateRegisterTransientFactory(TypeReference serviceType, string factoryExpression);

        #endregion

        #region Service Registration - Instance

        /// <summary>
        /// Generate code to register a singleton instance
        /// </summary>
        public abstract string GenerateRegisterInstance(TypeReference serviceType, string instanceExpression);

        #endregion

        #region Service Registration - Advanced

        /// <summary>
        /// Generate code to register all types from an assembly that implement a specific interface
        /// </summary>
        public virtual string GenerateRegisterAssemblyTypes(string assemblyExpression, TypeReference interfaceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            throw new NotSupportedException($"{Name} does not support assembly scanning. Use a framework like Autofac or Scrutor.");
        }

        /// <summary>
        /// Generate code to register a decorator for a service
        /// </summary>
        public virtual string GenerateRegisterDecorator(TypeReference serviceType, TypeReference decoratorType)
        {
            throw new NotSupportedException($"{Name} does not support decorators natively. Consider using Scrutor or Autofac.");
        }

        /// <summary>
        /// Generate code to register multiple implementations of the same interface
        /// </summary>
        public virtual string GenerateRegisterMultiple(TypeReference serviceType, IEnumerable<TypeReference> implementationTypes, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var sb = new StringBuilder();
            foreach (var implType in implementationTypes)
            {
                sb.AppendLine(GenerateServiceRegistration(new ServiceRegistration
                {
                    ServiceType = serviceType,
                    ImplementationType = implType,
                    Lifetime = lifetime
                }));
            }
            return sb.ToString();
        }

        #endregion

        #region Extension Method Generation

        /// <summary>
        /// Generate an extension method for registering services of a specific module/layer
        /// </summary>
        /// <param name="methodName">Name of the extension method (e.g., "AddApplicationServices")</param>
        /// <param name="registrations">Service registrations to include</param>
        /// <returns>Complete method element</returns>
        public virtual MethodElement GenerateRegistrationExtensionMethod(string methodName, IEnumerable<ServiceRegistration> registrations)
        {
            var method = new MethodElement(methodName, new TypeReference(ContainerBuilderTypeName))
            {
                Modifiers = ElementModifiers.Static,
                AccessModifier = AccessModifier.Public,
                Documentation = $"Registers services for {methodName.Replace("Add", "").Replace("Services", "")} module"
            };

            // Add 'this' parameter for extension method
            method.Parameters.Add(new ParameterElement("services", new TypeReference(ContainerBuilderTypeName))
            {
                IsExtensionMethodThis = true
            });
            method.IsExtensionMethod = true;

            // Build method body
            var bodyBuilder = new StringBuilder();
            foreach (var registration in registrations)
            {
                bodyBuilder.AppendLine(GenerateServiceRegistration(registration));
            }
            bodyBuilder.AppendLine();
            bodyBuilder.AppendLine("return services;");

            method.Body = bodyBuilder.ToString();

            return method;
        }

        /// <summary>
        /// Generate an extension method that accepts IConfiguration
        /// </summary>
        public virtual MethodElement GenerateRegistrationExtensionMethodWithConfiguration(string methodName, IEnumerable<ServiceRegistration> registrations)
        {
            var method = GenerateRegistrationExtensionMethod(methodName, registrations);

            // Add IConfiguration parameter
            method.Parameters.Add(new ParameterElement("configuration", new TypeReference("IConfiguration")));

            return method;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generate a complete ServiceCollectionExtensions class with multiple registration methods
        /// </summary>
        public virtual ClassElement GenerateServiceCollectionExtensionsClass(
            string className,
            Dictionary<string, IEnumerable<ServiceRegistration>> moduleRegistrations)
        {
            var classElement = new ClassElement(className)
            {
                AccessModifier = AccessModifier.Public,
                Modifiers = ElementModifiers.Static,
                Documentation = "Extension methods for registering services in the DI container"
            };

            foreach (var module in moduleRegistrations)
            {
                var method = GenerateRegistrationExtensionMethod(module.Key, module.Value);
                classElement.Methods.Add(method);
            }

            return classElement;
        }

        /// <summary>
        /// Get the lifetime method suffix for this framework
        /// </summary>
        protected abstract string GetLifetimeMethodName(ServiceLifetime lifetime);

        #endregion

        #region Obsolete Methods (for backward compatibility)

        /// <summary>
        /// Generates a method name for registering module services based on the specified scope and layer.
        /// eg. "AddApplicationDomainServices"
        /// </summary>
        public virtual string GenerateModuleRegistrationMethodName(string scope, string layer)
        {
            return $"Add{scope}{layer}Services";
        }

        public abstract string? GenerateModuleRegistrationMethodCall(string methodName, string builderVariableName = "services");

        #endregion
    }
}
