using CodeGenerator.Core.LLM.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.LLM.Copilot
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the GitHub Copilot LLM provider
        /// </summary>
        public static IServiceCollection AddCopilotLlmProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ILlmProvider, CopilotLlmProvider>();
            return services;
        }
    }
}
