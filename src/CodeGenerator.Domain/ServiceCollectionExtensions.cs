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
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register the default DI framework
            services.AddSingleton<DependancyInjectionFramework, MicrosoftExtensionsDependencyInjection>();

            // Register the framework manager with all built-in frameworks
            services.AddSingleton(sp => DependancyInjectionFrameworkManager.CreateWithBuiltInFrameworks());
            services.AddSingleton<CodeArchitectureManager>();
            return services;
        }
    }
}
