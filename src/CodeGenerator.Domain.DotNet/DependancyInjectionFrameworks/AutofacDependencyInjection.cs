using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using System.Text;

namespace CodeGenerator.Domain.DependancyInjectionFrameworks
{
    /// <summary>
    /// Autofac dependency injection framework implementation.
    /// A feature-rich DI container with support for modules, property injection, interceptors, and assembly scanning.
    /// </summary>
    public class AutofacDependencyInjection : DotNetDependancyInjectionFramework
    {
        public const string FRAMEWORK_ID = "Autofac";

        public AutofacDependencyInjection()
            : base(FRAMEWORK_ID,
                   "Autofac",
                   "A feature-rich inversion of control container for .NET. Supports modules, property injection, interceptors, decorators, and automatic assembly scanning.")
        {
        }

        public override string ContainerBuilderTypeName => "ContainerBuilder";
        public override string ContainerTypeName => "IContainer";
        public override bool SupportsScopedLifetime => true;
        public override bool SupportsPropertyInjection => true;
        public override bool SupportsInterceptors => true;
        public override bool SupportsAssemblyScanning => true;

        #region NuGet Packages

        public override IEnumerable<NuGetPackage> GetRequiredNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Autofac",
                Version = "8.0.0"
            };
        }

        public override IEnumerable<NuGetPackage> GetOptionalNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Autofac.Extensions.DependencyInjection",
                Version = "9.0.0"
            };
            yield return new NuGetPackage
            {
                PackageId = "Autofac.Extras.DynamicProxy",
                Version = "7.1.0"
            };
        }

        #endregion

        #region Using Statements

        public override IEnumerable<string> GetRequiredUsings()
        {
            yield return "Autofac";
        }

        public override IEnumerable<string> GetExtensionMethodUsings()
        {
            yield return "System.Reflection";
            yield return "Autofac.Extensions.DependencyInjection";
        }

        #endregion

        #region Container Setup

        public override ClassElement GenerateContainerSetupClass(CodeFileElement codeFileElement, string className = "AutofacModule")
        {
            // Add required usings
            foreach (var usingNs in GetRequiredUsings())
            {
                if (!codeFileElement.Usings.Any(u => u.Namespace == usingNs))
                {
                    codeFileElement.Usings.Add(new UsingElement(usingNs));
                }
            }

            var classElement = new ClassElement(className)
            {
                AccessModifier = AccessModifier.Public,
                Documentation = "Autofac module for registering services"
            };

            // Autofac modules typically inherit from Module
            classElement.BaseTypes.Add(new TypeReference("Module"));

            // Add Load method override
            var loadMethod = new MethodElement("Load", TypeReference.Common.Void)
            {
                AccessModifier = AccessModifier.Protected,
                Modifiers = ElementModifiers.Override,
                Documentation = "Override to add registrations to the container"
            };
            loadMethod.Parameters.Add(new ParameterElement("builder", new TypeReference("ContainerBuilder")));
            loadMethod.Body = "// Add registrations here\n// builder.RegisterType<MyService>().As<IMyService>();";

            classElement.Methods.Add(loadMethod);

            return classElement;
        }

        public override string GenerateContainerBuilderCreation(string variableName = "builder")
        {
            return $"var {variableName} = new ContainerBuilder();";
        }

        public override string GenerateBuildContainer(string builderVariableName = "builder", string containerVariableName = "container")
        {
            return $"var {containerVariableName} = {builderVariableName}.Build();";
        }

        public override string GenerateResolveService(string containerVariableName, TypeReference serviceType)
        {
            return $"{containerVariableName}.Resolve<{serviceType.TypeName}>()";
        }

        #endregion

        #region Service Registration - Basic

        public override string GenerateServiceRegistration(ServiceRegistration registration)
        {
            var sb = new StringBuilder();
            sb.Append("builder.RegisterType<");

            var implType = registration.ImplementationType ?? registration.ServiceType;
            sb.Append(implType.TypeName);
            sb.Append(">()");

            if (registration.ImplementationType != null &&
                registration.ImplementationType.TypeName != registration.ServiceType.TypeName)
            {
                sb.Append($".As<{registration.ServiceType.TypeName}>()");
            }

            sb.Append(GetLifetimeSuffix(registration.Lifetime));
            sb.Append(';');

            return sb.ToString();
        }

        public override string GenerateRegisterSingleton(TypeReference serviceType, TypeReference? implementationType = null)
        {
            var implType = implementationType ?? serviceType;
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"builder.RegisterType<{implType.TypeName}>().As<{serviceType.TypeName}>().SingleInstance();";
            }
            return $"builder.RegisterType<{serviceType.TypeName}>().SingleInstance();";
        }

        public override string GenerateRegisterScoped(TypeReference serviceType, TypeReference? implementationType = null)
        {
            var implType = implementationType ?? serviceType;
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"builder.RegisterType<{implType.TypeName}>().As<{serviceType.TypeName}>().InstancePerLifetimeScope();";
            }
            return $"builder.RegisterType<{serviceType.TypeName}>().InstancePerLifetimeScope();";
        }

        public override string GenerateRegisterTransient(TypeReference serviceType, TypeReference? implementationType = null)
        {
            var implType = implementationType ?? serviceType;
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"builder.RegisterType<{implType.TypeName}>().As<{serviceType.TypeName}>().InstancePerDependency();";
            }
            return $"builder.RegisterType<{serviceType.TypeName}>().InstancePerDependency();";
        }

        #endregion

        #region Service Registration - Factory

        public override string GenerateRegisterSingletonFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"builder.Register({factoryExpression}).As<{serviceType.TypeName}>().SingleInstance();";
        }

        public override string GenerateRegisterScopedFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"builder.Register({factoryExpression}).As<{serviceType.TypeName}>().InstancePerLifetimeScope();";
        }

        public override string GenerateRegisterTransientFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"builder.Register({factoryExpression}).As<{serviceType.TypeName}>().InstancePerDependency();";
        }

        #endregion

        #region Service Registration - Instance

        public override string GenerateRegisterInstance(TypeReference serviceType, string instanceExpression)
        {
            return $"builder.RegisterInstance({instanceExpression}).As<{serviceType.TypeName}>();";
        }

        #endregion

        #region Service Registration - Advanced

        public override string GenerateRegisterAssemblyTypes(string assemblyExpression, TypeReference interfaceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var lifetimeSuffix = GetLifetimeSuffix(lifetime);
            return $"builder.RegisterAssemblyTypes({assemblyExpression}).AsImplementedInterfaces(){lifetimeSuffix};";
        }

        public override string GenerateRegisterDecorator(TypeReference serviceType, TypeReference decoratorType)
        {
            return $"builder.RegisterDecorator<{decoratorType.TypeName}, {serviceType.TypeName}>();";
        }

        #endregion

        #region Helper Methods

        protected override string GetLifetimeMethodName(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => "SingleInstance",
                ServiceLifetime.Scoped => "InstancePerLifetimeScope",
                ServiceLifetime.Transient => "InstancePerDependency",
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
            };
        }

        private string GetLifetimeSuffix(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ".SingleInstance()",
                ServiceLifetime.Scoped => ".InstancePerLifetimeScope()",
                ServiceLifetime.Transient => ".InstancePerDependency()",
                _ => ""
            };
        }

        /// <summary>
        /// Generate code to register a module
        /// </summary>
        public string GenerateRegisterModule(string moduleTypeName)
        {
            return $"builder.RegisterModule<{moduleTypeName}>();";
        }

        /// <summary>
        /// Generate code for property injection
        /// </summary>
        public string GenerateWithPropertyInjection(TypeReference serviceType, TypeReference? implementationType = null)
        {
            var implType = implementationType ?? serviceType;
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"builder.RegisterType<{implType.TypeName}>().As<{serviceType.TypeName}>().PropertiesAutowired();";
            }
            return $"builder.RegisterType<{serviceType.TypeName}>().PropertiesAutowired();";
        }

        public override string? GenerateModuleRegistrationMethodCall(string methodName, string builderVariableName = "services")
        {
            return $"{builderVariableName}.{methodName}(configuration);";
        }

        #endregion
    }
}
