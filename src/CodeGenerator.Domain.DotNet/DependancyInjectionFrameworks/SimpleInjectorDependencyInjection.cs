using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using System.Text;

namespace CodeGenerator.Domain.DependancyInjectionFrameworks
{
    /// <summary>
    /// Simple Injector dependency injection framework implementation.
    /// Known for its speed, strict validation, and diagnostic capabilities.
    /// </summary>
    public class SimpleInjectorDependencyInjection : DotNetDependancyInjectionFramework
    {
        public const string FRAMEWORK_ID = "SimpleInjector";

        public SimpleInjectorDependencyInjection()
            : base(FRAMEWORK_ID,
                   "Simple Injector",
                   "A fast, strict dependency injection library with powerful diagnostic capabilities. Validates configuration at startup to catch errors early.")
        {
        }

        public override string ContainerBuilderTypeName => "Container";
        public override string ContainerTypeName => "Container";
        public override bool SupportsScopedLifetime => true;
        public override bool SupportsPropertyInjection => false;
        public override bool SupportsInterceptors => true;
        public override bool SupportsAssemblyScanning => true;

        #region NuGet Packages

        public override IEnumerable<NuGetPackage> GetRequiredNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "SimpleInjector",
                Version = "5.4.4"
            };
        }

        public override IEnumerable<NuGetPackage> GetOptionalNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "SimpleInjector.Integration.AspNetCore",
                Version = "5.4.4"
            };
            yield return new NuGetPackage
            {
                PackageId = "SimpleInjector.Integration.GenericHost",
                Version = "5.4.4"
            };
        }

        #endregion

        #region Using Statements

        public override IEnumerable<string> GetRequiredUsings()
        {
            yield return "SimpleInjector";
        }

        public override IEnumerable<string> GetExtensionMethodUsings()
        {
            yield return "SimpleInjector.Lifestyles";
        }

        #endregion

        #region Container Setup

        public override ClassElement GenerateContainerSetupClass(CodeFileElement codeFileElement, string className = "ContainerConfiguration")
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
                Documentation = "Configuration class for Simple Injector container setup"
            };

            // Add Configure method
            var configureMethod = new MethodElement("Configure", new TypeReference("Container"))
            {
                AccessModifier = AccessModifier.Public,
                Modifiers = ElementModifiers.Static,
                Documentation = "Creates and configures the Simple Injector container"
            };
            configureMethod.Body = @"var container = new Container();
container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

// Register services here
// container.Register<IService, Service>(Lifestyle.Singleton);

// Verify the container configuration
container.Verify();

return container;";

            classElement.Methods.Add(configureMethod);

            return classElement;
        }

        public override string GenerateContainerBuilderCreation(string variableName = "container")
        {
            return $"var {variableName} = new Container();";
        }

        public override string GenerateBuildContainer(string builderVariableName = "container", string containerVariableName = "container")
        {
            // Simple Injector doesn't have a separate build step - just verify
            if (builderVariableName == containerVariableName)
            {
                return $"{builderVariableName}.Verify();";
            }
            return $"var {containerVariableName} = {builderVariableName};\n{containerVariableName}.Verify();";
        }

        public override string GenerateResolveService(string containerVariableName, TypeReference serviceType)
        {
            return $"{containerVariableName}.GetInstance<{serviceType.TypeName}>()";
        }

        #endregion

        #region Service Registration - Basic

        public override string GenerateServiceRegistration(ServiceRegistration registration)
        {
            if (!string.IsNullOrEmpty(registration.RawRegistrationCode))
            {
                return registration.RawRegistrationCode;
            }
            var lifestyleParam = GetLifetimeMethodName(registration.Lifetime);

            if (registration.UsesInstance)
            {
                return $"container.RegisterInstance<{registration.ServiceType.TypeName}>({registration.InstanceExpression});";
            }

            if (registration.UsesFactory)
            {
                return $"container.Register<{registration.ServiceType.TypeName}>({registration.FactoryExpression}, {lifestyleParam});";
            }

            if (registration.ImplementationType != null &&
                registration.ImplementationType.TypeName != registration.ServiceType.TypeName)
            {
                return $"container.Register<{registration.ServiceType.TypeName}, {registration.ImplementationType.TypeName}>({lifestyleParam});";
            }

            return $"container.Register<{registration.ServiceType.TypeName}>({lifestyleParam});";
        }

        public override string GenerateRegisterSingleton(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"container.Register<{serviceType.TypeName}, {implementationType.TypeName}>(Lifestyle.Singleton);";
            }
            return $"container.Register<{serviceType.TypeName}>(Lifestyle.Singleton);";
        }

        public override string GenerateRegisterScoped(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"container.Register<{serviceType.TypeName}, {implementationType.TypeName}>(Lifestyle.Scoped);";
            }
            return $"container.Register<{serviceType.TypeName}>(Lifestyle.Scoped);";
        }

        public override string GenerateRegisterTransient(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"container.Register<{serviceType.TypeName}, {implementationType.TypeName}>(Lifestyle.Transient);";
            }
            return $"container.Register<{serviceType.TypeName}>(Lifestyle.Transient);";
        }

        #endregion

        #region Service Registration - Factory

        public override string GenerateRegisterSingletonFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"container.Register<{serviceType.TypeName}>({factoryExpression}, Lifestyle.Singleton);";
        }

        public override string GenerateRegisterScopedFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"container.Register<{serviceType.TypeName}>({factoryExpression}, Lifestyle.Scoped);";
        }

        public override string GenerateRegisterTransientFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"container.Register<{serviceType.TypeName}>({factoryExpression}, Lifestyle.Transient);";
        }

        #endregion

        #region Service Registration - Instance

        public override string GenerateRegisterInstance(TypeReference serviceType, string instanceExpression)
        {
            return $"container.RegisterInstance<{serviceType.TypeName}>({instanceExpression});";
        }

        #endregion

        #region Service Registration - Advanced

        public override string GenerateRegisterAssemblyTypes(string assemblyExpression, TypeReference interfaceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var lifestyle = GetLifetimeMethodName(lifetime);
            return $"container.Register(typeof({interfaceType.TypeName}), {assemblyExpression}, {lifestyle});";
        }

        public override string GenerateRegisterDecorator(TypeReference serviceType, TypeReference decoratorType)
        {
            return $"container.RegisterDecorator<{serviceType.TypeName}, {decoratorType.TypeName}>();";
        }

        #endregion

        #region Helper Methods

        protected override string GetLifetimeMethodName(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => "Lifestyle.Singleton",
                ServiceLifetime.Scoped => "Lifestyle.Scoped",
                ServiceLifetime.Transient => "Lifestyle.Transient",
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
            };
        }

        /// <summary>
        /// Generate verification call for container
        /// </summary>
        public string GenerateVerify(string containerVariableName = "container")
        {
            return $"{containerVariableName}.Verify();";
        }

        /// <summary>
        /// Generate code to register a collection
        /// </summary>
        public string GenerateRegisterCollection(TypeReference serviceType, IEnumerable<TypeReference> implementationTypes)
        {
            var typesList = string.Join(", ", implementationTypes.Select(t => $"typeof({t.TypeName})"));
            return $"container.Collection.Register<{serviceType.TypeName}>(new[] {{ {typesList} }});";
        }

        public override string? GenerateModuleRegistrationMethodCall(string methodName, string builderVariableName = "services")
        {
            return $"{builderVariableName}.{methodName}(configuration);";
        }

        #endregion
    }
}
