using CodeGenerator.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core
{
    /// <summary>
    /// Extension methods for registering CodeGenerator.Core services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register all CodeGenerator.Core services
        /// </summary>
        public static IServiceCollection AddCodeGeneratorCore(this IServiceCollection services)
        {
            // Register MarkdownService as singleton
            services.AddSingleton<MarkdownService>();

            return services;
        }
    }
}
