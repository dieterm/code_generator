using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DependancyInjectionFrameworks;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Domain
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Domain services in the DI container
        /// </summary>
        public static IServiceCollection AddDomainDotNetServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<DependancyInjectionFramework, MicrosoftExtensionsDependencyInjection>();
            services.AddSingleton<DependancyInjectionFramework, AutofacDependencyInjection>();
            services.AddSingleton<DependancyInjectionFramework, CastleWindsorDependencyInjection>();
            services.AddSingleton<DependancyInjectionFramework, NinjectDependencyInjection>();
            services.AddSingleton<DependancyInjectionFramework, SimpleInjectorDependencyInjection>();

            return services;
        }
    }
}
