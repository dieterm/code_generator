using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using System.Text;

namespace CodeGenerator.Domain.DependancyInjectionFrameworks
{
    /// <summary>
    /// Ninject dependency injection framework implementation.
    /// Known for its readable syntax and contextual binding capabilities.
    /// </summary>
    public class NinjectDependencyInjection : DependancyInjectionFramework
    {
        public const string FRAMEWORK_ID = "Ninject";

        public NinjectDependencyInjection()
            : base(FRAMEWORK_ID,
                   "Ninject",
                   "A lightning-fast dependency injection framework with readable syntax and support for contextual binding. Well-suited for applications requiring flexible binding configurations.")
        {
        }

        public override string ContainerBuilderTypeName => "IKernel";
        public override string ContainerTypeName => "IKernel";
        public override bool SupportsScopedLifetime => true;
        public override bool SupportsPropertyInjection => true;
        public override bool SupportsInterceptors => true;
        public override bool SupportsAssemblyScanning => true;

        #region NuGet Packages

        public override IEnumerable<NuGetPackage> GetRequiredNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Ninject",
                Version = "3.3.6"
            };
        }

        public override IEnumerable<NuGetPackage> GetOptionalNuGetPackages()
        {
            yield return new NuGetPackage
            {
                PackageId = "Ninject.Extensions.Interception",
                Version = "3.3.0"
            };
            yield return new NuGetPackage
            {
                PackageId = "Ninject.Extensions.Conventions",
                Version = "3.3.0"
            };
            yield return new NuGetPackage
            {
                PackageId = "Ninject.Web.Common",
                Version = "3.3.2"
            };
        }

        #endregion

        #region Using Statements

        public override IEnumerable<string> GetRequiredUsings()
        {
            yield return "Ninject";
        }

        public override IEnumerable<string> GetExtensionMethodUsings()
        {
            yield return "Ninject.Modules";
        }

        #endregion

        #region Container Setup

        public override ClassElement GenerateContainerSetupClass(CodeFileElement codeFileElement, string className = "ApplicationModule")
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
                Documentation = "Ninject module for registering application services"
            };

            // Ninject modules inherit from NinjectModule
            classElement.BaseTypes.Add(new TypeReference("NinjectModule"));

            // Add Load method override
            var loadMethod = new MethodElement("Load", TypeReference.Common.Void)
            {
                AccessModifier = AccessModifier.Public,
                Modifiers = ElementModifiers.Override,
                Documentation = "Override to register bindings in the kernel"
            };
            loadMethod.Body = "// Add bindings here\n// Bind<IService>().To<Service>().InSingletonScope();";

            classElement.Methods.Add(loadMethod);

            return classElement;
        }

        public override string GenerateContainerBuilderCreation(string variableName = "kernel")
        {
            return $"var {variableName} = new StandardKernel();";
        }

        public override string GenerateBuildContainer(string builderVariableName = "kernel", string containerVariableName = "kernel")
        {
            // Ninject doesn't have a separate build step
            if (builderVariableName == containerVariableName)
            {
                return "// Kernel is ready to use";
            }
            return $"var {containerVariableName} = {builderVariableName};";
        }

        public override string GenerateResolveService(string containerVariableName, TypeReference serviceType)
        {
            return $"{containerVariableName}.Get<{serviceType.TypeName}>()";
        }

        #endregion

        #region Service Registration - Basic

        public override string GenerateServiceRegistration(ServiceRegistration registration)
        {
            var sb = new StringBuilder();
            sb.Append("Bind<");
            sb.Append(registration.ServiceType.TypeName);
            sb.Append(">()");

            if (registration.UsesInstance)
            {
                sb.Append($".ToConstant({registration.InstanceExpression})");
            }
            else if (registration.UsesFactory)
            {
                sb.Append($".ToMethod({registration.FactoryExpression})");
                sb.Append(GetLifetimeSuffix(registration.Lifetime));
            }
            else if (registration.ImplementationType != null &&
                     registration.ImplementationType.TypeName != registration.ServiceType.TypeName)
            {
                sb.Append($".To<{registration.ImplementationType.TypeName}>()");
                sb.Append(GetLifetimeSuffix(registration.Lifetime));
            }
            else
            {
                sb.Append(".ToSelf()");
                sb.Append(GetLifetimeSuffix(registration.Lifetime));
            }

            sb.Append(';');
            return sb.ToString();
        }

        public override string GenerateRegisterSingleton(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"Bind<{serviceType.TypeName}>().To<{implementationType.TypeName}>().InSingletonScope();";
            }
            return $"Bind<{serviceType.TypeName}>().ToSelf().InSingletonScope();";
        }

        public override string GenerateRegisterScoped(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"Bind<{serviceType.TypeName}>().To<{implementationType.TypeName}>().InRequestScope();";
            }
            return $"Bind<{serviceType.TypeName}>().ToSelf().InRequestScope();";
        }

        public override string GenerateRegisterTransient(TypeReference serviceType, TypeReference? implementationType = null)
        {
            if (implementationType != null && implementationType.TypeName != serviceType.TypeName)
            {
                return $"Bind<{serviceType.TypeName}>().To<{implementationType.TypeName}>().InTransientScope();";
            }
            return $"Bind<{serviceType.TypeName}>().ToSelf().InTransientScope();";
        }

        #endregion

        #region Service Registration - Factory

        public override string GenerateRegisterSingletonFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"Bind<{serviceType.TypeName}>().ToMethod({factoryExpression}).InSingletonScope();";
        }

        public override string GenerateRegisterScopedFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"Bind<{serviceType.TypeName}>().ToMethod({factoryExpression}).InRequestScope();";
        }

        public override string GenerateRegisterTransientFactory(TypeReference serviceType, string factoryExpression)
        {
            return $"Bind<{serviceType.TypeName}>().ToMethod({factoryExpression}).InTransientScope();";
        }

        #endregion

        #region Service Registration - Instance

        public override string GenerateRegisterInstance(TypeReference serviceType, string instanceExpression)
        {
            return $"Bind<{serviceType.TypeName}>().ToConstant({instanceExpression});";
        }

        #endregion

        #region Service Registration - Advanced

        public override string GenerateRegisterAssemblyTypes(string assemblyExpression, TypeReference interfaceType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            // Requires Ninject.Extensions.Conventions
            var lifetimeSuffix = GetLifetimeSuffix(lifetime);
            return $"kernel.Bind(x => x.From({assemblyExpression}).SelectAllClasses().BindAllInterfaces(){lifetimeSuffix});";
        }

        #endregion

        #region Helper Methods

        protected override string GetLifetimeMethodName(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => "InSingletonScope",
                ServiceLifetime.Scoped => "InRequestScope",
                ServiceLifetime.Transient => "InTransientScope",
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime))
            };
        }

        private string GetLifetimeSuffix(ServiceLifetime lifetime)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ".InSingletonScope()",
                ServiceLifetime.Scoped => ".InRequestScope()",
                ServiceLifetime.Transient => ".InTransientScope()",
                _ => ""
            };
        }

        /// <summary>
        /// Generate code to load a Ninject module
        /// </summary>
        public string GenerateLoadModule(string kernelVariableName, string moduleTypeName)
        {
            return $"{kernelVariableName}.Load<{moduleTypeName}>();";
        }

        /// <summary>
        /// Generate contextual binding (When condition)
        /// </summary>
        public string GenerateConditionalBinding(TypeReference serviceType, TypeReference implementationType, string condition)
        {
            return $"Bind<{serviceType.TypeName}>().To<{implementationType.TypeName}>().When({condition});";
        }

        public override string? GenerateModuleRegistrationMethodCall(string methodName, string builderVariableName = "services")
        {
            return $"{builderVariableName}.{methodName}(configuration);";
        }

        #endregion
    }
}
