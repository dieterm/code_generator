using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using System.Text;

namespace CodeGenerator.Domain.DependancyInjectionFrameworks
{
    /// <summary>
    /// Castle Windsor dependency injection framework implementation.
    /// A mature, feature-rich IoC container with excellent AOP support via Castle DynamicProxy.
    /// </summary>
    public class CastleWindsorDependencyInjection : DependancyInjectionFramework
    {
        public const string FRAMEWORK_ID = "CastleWindsor";

        public CastleWindsorDependencyInjection()
            : base(FRAMEWORK_ID,
                   "Castle Windsor",
                   "A mature and feature-rich inversion of control container. Provides excellent support for AOP via Castle DynamicProxy and has been battle-tested in enterprise applications.")
        {
        }

        public override string ContainerBuilderTypeName => "IWindsorContainer";
        public override string ContainerTypeName => "IWindsorContainer";
        public override bool SupportsScopedLifetime => true;
        public override bool SupportsPropertyInjection => true;
        public override bool SupportsInterceptors => true;
        public override bool SupportsAssemblyScanning => true;

        #region NuGet Packages

        public override IEnumerable<NuGetPackage> GetRequiredNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Castle.Windsor",
                Version = "6.0.0"
            };
        }

        public override IEnumerable<NuGetPackage> GetOptionalNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Castle.Core",
                Version = "5.1.1"
            };
            yield return new NuGetPackage
            {
                PackageId = "Castle.Windsor.Extensions.DependencyInjection",
                Version = "6.0.0"
            };
        }

        #endregion

        #region Using Statements

        public override IEnumerable<string> GetRequiredUsings()
        {
            yield return "Castle.Windsor";
            yield return "Castle.MicroKernel.Registration";
        }

        public override IEnumerable<string> GetExtensionMethodUsings()
        {
            yield return "Castle.MicroKernel.SubSystems.Configuration";
            yield return "Castle.Windsor.Installer";
        }

        #endregion

        #region Container Setup

        public override ClassElement GenerateContainerSetupClass(CodeFileElement codeFileElement, string className = "WindsorInstaller")
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
                Documentation = "Castle Windsor installer for registering application services"
            };

            // Windsor installers implement IWindsorInstaller
            classElement.BaseTypes.Add(new TypeReference("IWindsorInstaller"));

            // Add Install method
            var installMethod = new MethodElement("Install", TypeReference.Common.Void)
            {
                AccessModifier = AccessModifier.Public,
                Documentation = "Install components into the container"
            };
            installMethod.Parameters.Add(new ParameterElement("container", new TypeReference("IWindsorContainer")));
            installMethod.Parameters.Add(new ParameterElement("store", new TypeReference("IConfigurationStore")));
            installMethod.Body = @"// Register services here
// container.Register(
//     Component.For<IService>().ImplementedBy<Service>().LifestyleSingleton()
// );";

            classElement.Methods.Add(installMethod);

            return classElement;
        }

        public override string GenerateContainerBuilderCreation(string variableName = "container")
        {
            return $"var {variableName} = new WindsorContainer();";
        }

        public override string GenerateBuildContainer(string builderVariableName = "container", string containerVariableName = "container")
        {
            // Windsor doesn't have a separate build step
            if (builderVariableName == containerVariableName)
            {
                return "// Container is ready to use";
            }
            return $"var {containerVariableName} = {builderVariableName};";
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
            sb.Append("container.Register(Component.For<");
            sb.Append(registration.ServiceType.TypeName);
            sb.Append(">()");

            if (registration.UsesInstance)
            {
                sb.Append($".Instance({registration.InstanceExpression})");
            }
            else if (registration.UsesFactory)
            {
                sb.Append($".UsingFactoryMethod({registration.FactoryExpression})");
                sb.Append(GetLifetimeSuffix(registration.Lifetime));
            }
            else if (registration.ImplementationType != null &&
                     registration.ImplementationType.TypeName != registration.ServiceType.TypeName)
            {
                sb.Append($".ImplementedBy<{registration.ImplementationType.TypeName}>()");
                sb.Append(GetLifetimeSuffix(registration.Lifetime));
            }
            else
            {
                sb.Append(GetLifetimeSuffix(registration.Lifetime));
            }

            sb.Append(");");
            return sb.ToString();
        }

        public override string GenerateRegisterSingleton(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"container.Register(Component.For<{serviceType.TypeName}>().ImplementedBy<{implementationType.TypeName}>().LifestyleSingleton());";
            }
            return $"container.Register(Component.For<{serviceType.TypeName}>().LifestyleSingleton());";
        }

        public override string GenerateRegisterScoped(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"container.Register(Component.For<{serviceType.TypeName}>().ImplementedBy<{implementationType.TypeName}>().LifestyleScoped());";
            }
            return $"container.Register(Component.For<{serviceType.TypeName}>().LifestyleScoped());";
        }

        public override string GenerateRegisterTransient(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"container.Register(Component.For<{serviceType.TypeName}>().ImplementedBy<{implementationType.TypeName}>().LifestyleTransient());";
            }
            return $"container.Register(Component.For<{serviceType.TypeName}>().LifestyleTransient());";
        }

        #endregion

        #region Service Registration - Factory

        public override string GenerateRegisterSingletonFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"container.Register(Component.For<{serviceType.TypeName}>().UsingFactoryMethod({factoryExpression}).LifestyleSingleton());";
        }

        public override string GenerateRegisterScopedFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"container.Register(Component.For<{serviceType.TypeName}>().UsingFactoryMethod({factoryExpression}).LifestyleScoped());";
        }

        public override string GenerateRegisterTransientFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"container.Register(Component.For<{serviceType.TypeName}>().UsingFactoryMethod({factoryExpression}).LifestyleTransient());";
        }

        #endregion

        #region Service Registration - Instance

        public override string GenerateRegisterInstance(TypeReference serviceType, string instanceExpression)
        {
            return $"container.Register(Component.For<{serviceType.TypeName}>().Instance({instanceExpression}));";
        }

        #endregion

        #region Service Registration - Advanced

        public override string GenerateRegisterAssemblyTypes(string assemblyExpression, TypeReference interfaceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var lifetimeMethod = GetLifetimeSuffix(lifetime);
            return $"container.Register(Classes.FromAssembly({assemblyExpression}).BasedOn<{interfaceType.TypeName}>().WithServiceAllInterfaces(){lifetimeMethod});";
        }

        public override string GenerateRegisterDecorator(TypeReference serviceType, TypeReference decoratorType)
        {
            return $"container.Register(Component.For<{serviceType.TypeName}>().ImplementedBy<{decoratorType.TypeName}>().IsDefault());";
        }

        #endregion

        #region Helper Methods

        protected override string GetLifetimeMethodName(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => "LifestyleSingleton",
                ServiceLifetime.Scoped => "LifestyleScoped",
                ServiceLifetime.Transient => "LifestyleTransient",
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
            };
        }

        private string GetLifetimeSuffix(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ".LifestyleSingleton()",
                ServiceLifetime.Scoped => ".LifestyleScoped()",
                ServiceLifetime.Transient => ".LifestyleTransient()",
                _ => ""
            };
        }

        /// <summary>
        /// Generate code to install an installer
        /// </summary>
        public string GenerateInstall(string containerVariableName, string installerTypeName)
        {
            return $"{containerVariableName}.Install(new {installerTypeName}());";
        }

        /// <summary>
        /// Generate code to install from assembly
        /// </summary>
        public string GenerateInstallFromAssembly(string containerVariableName, string assemblyExpression)
        {
            return $"{containerVariableName}.Install(FromAssembly.Instance({assemblyExpression}));";
        }

        /// <summary>
        /// Generate code with interceptor
        /// </summary>
        public string GenerateWithInterceptor(TypeReference serviceType, TypeReference implementationType, string interceptorTypeName)
        {
            return $"container.Register(Component.For<{serviceType.TypeName}>().ImplementedBy<{implementationType.TypeName}>().Interceptors<{interceptorTypeName}>());";
        }

        public override string? GenerateModuleRegistrationMethodCall(string methodName, string builderVariableName = "services")
        {
            return $"{builderVariableName}.{methodName}(configuration);";
        }

        #endregion
    }
}
