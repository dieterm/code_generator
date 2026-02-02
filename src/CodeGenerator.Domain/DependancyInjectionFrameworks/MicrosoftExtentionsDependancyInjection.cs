using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using System.Text;

namespace CodeGenerator.Domain.DependancyInjectionFrameworks
{
    /// <summary>
    /// Microsoft.Extensions.DependencyInjection implementation.
    /// The default DI framework provided by Microsoft as part of the .NET ecosystem.
    /// </summary>
    public class MicrosoftExtensionsDependencyInjection : DependancyInjectionFramework
    {
        public const string FRAMEWORK_ID = "MicrosoftExtensionsDependencyInjection";

        public MicrosoftExtensionsDependencyInjection()
            : base(FRAMEWORK_ID,
                   "Microsoft.Extensions.DependencyInjection",
                   "The default dependency injection framework provided by Microsoft as part of the .NET ecosystem. Lightweight, fast, and well-integrated with ASP.NET Core and other Microsoft frameworks.")
        {
        }

        public override string ContainerBuilderTypeName => "IServiceCollection";
        public override string ContainerTypeName => "IServiceProvider";
        public override bool SupportsScopedLifetime => true;
        public override bool SupportsPropertyInjection => false;
        public override bool SupportsInterceptors => false;
        public override bool SupportsAssemblyScanning => false;

        #region NuGet Packages

        public override IEnumerable<NuGetPackage> GetRequiredNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Microsoft.Extensions.DependencyInjection",
                Version = "8.0.0"
            };
            yield return new NuGetPackage
            {
                PackageId = "Microsoft.Extensions.DependencyInjection.Abstractions",
                Version = "8.0.0"
            };
        }

        public override IEnumerable<NuGetPackage> GetOptionalNuGetPackages()
        {
            // Scrutor adds assembly scanning and decoration support
            yield return new NuGetPackage
            {
                PackageId = "Scrutor",
                Version = "4.2.2"
            };
        }

        #endregion

        #region Using Statements

        public override IEnumerable<string> GetRequiredUsings()
        {
            yield return "Microsoft.Extensions.DependencyInjection";
        }

        public override IEnumerable<string> GetExtensionMethodUsings()
        {
            yield return "Microsoft.Extensions.Configuration";
        }

        #endregion

        #region Container Setup

        public override ClassElement GenerateContainerSetupClass(CodeFileElement codeFileElement, string className = "ServiceCollectionExtensions")
        {
            // Add required usings
            foreach (var usingNs in GetRequiredUsings())
            {
                if (!codeFileElement.Usings.Any(u => u.Namespace == usingNs))
                {
                    codeFileElement.Usings.Add(new UsingElement(usingNs));
                }
            }
            foreach (var usingNs in GetExtensionMethodUsings())
            {
                if (!codeFileElement.Usings.Any(u => u.Namespace == usingNs))
                {
                    codeFileElement.Usings.Add(new UsingElement(usingNs));
                }
            }

            var classElement = new ClassElement(className)
            {
                AccessModifier = AccessModifier.Public,
                Modifiers = ElementModifiers.Static,
                Documentation = "Extension methods for configuring services in the DI container"
            };
            
            return classElement;
        }

        public override string GenerateContainerBuilderCreation(string variableName = "services")
        {
            return $"var {variableName} = new ServiceCollection();";
        }

        public override string GenerateBuildContainer(string builderVariableName = "services", string containerVariableName = "serviceProvider")
        {
            return $"var {containerVariableName} = {builderVariableName}.BuildServiceProvider();";
        }

        public override string GenerateResolveService(string containerVariableName, TypeReference serviceType)
        {
            return $"{containerVariableName}.GetRequiredService<{serviceType.TypeName}>()";
        }

        #endregion

        #region Service Registration - Basic

        public override string GenerateServiceRegistration(ServiceRegistration registration)
        {
            var methodName = GetLifetimeMethodName(registration.Lifetime);

            if (registration.UsesInstance)
            {
                return $"services.AddSingleton<{registration.ServiceType.TypeName}>({registration.InstanceExpression});";
            }

            if (registration.UsesFactory)
            {
                if (registration.ImplementationType != null)
                {
                    return $"services.{methodName}<{registration.ServiceType.TypeName}, {registration.ImplementationType.TypeName}>({registration.FactoryExpression});";
                }
                return $"services.{methodName}<{registration.ServiceType.TypeName}>({registration.FactoryExpression});";
            }

            if (registration.ImplementationType != null && 
                registration.ImplementationType.TypeName != registration.ServiceType.TypeName)
            {
                return $"services.{methodName}<{registration.ServiceType.TypeName}, {registration.ImplementationType.TypeName}>();";
            }

            return $"services.{methodName}<{registration.ServiceType.TypeName}>();";
        }

        public override string GenerateRegisterSingleton(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"services.AddSingleton<{serviceType.TypeName}, {implementationType.TypeName}>();";
            }
            return $"services.AddSingleton<{serviceType.TypeName}>();";
        }

        public override string GenerateRegisterScoped(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"services.AddScoped<{serviceType.TypeName}, {implementationType.TypeName}>();";
            }
            return $"services.AddScoped<{serviceType.TypeName}>();";
        }

        public override string GenerateRegisterTransient(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"services.AddTransient<{serviceType.TypeName}, {implementationType.TypeName}>();";
            }
            return $"services.AddTransient<{serviceType.TypeName}>();";
        }

        #endregion

        #region Service Registration - Factory

        public override string GenerateRegisterSingletonFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"services.AddSingleton<{serviceType.TypeName}>({factoryExpression});";
        }

        public override string GenerateRegisterScopedFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"services.AddScoped<{serviceType.TypeName}>({factoryExpression});";
        }

        public override string GenerateRegisterTransientFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"services.AddTransient<{serviceType.TypeName}>({factoryExpression});";
        }

        #endregion

        #region Service Registration - Instance

        public override string GenerateRegisterInstance(TypeReference serviceType, string instanceExpression)
        {
            return $"services.AddSingleton<{serviceType.TypeName}>({instanceExpression});";
        }

        #endregion

        #region Helper Methods

        protected override string GetLifetimeMethodName(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => "AddSingleton",
                ServiceLifetime.Scoped => "AddScoped",
                ServiceLifetime.Transient => "AddTransient",
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
            };
        }

        public override string? GenerateModuleRegistrationMethodCall(string methodName, string builderVariableName = "services")
        {
            return $"{builderVariableName}.{methodName}();";
        }

        #endregion
    }
}
