using CodeGenerator.Core.LLM.Ollama.Settings;
using CodeGenerator.Core.LLM.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.LLM.Ollama
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Ollama LLM provider
        /// </summary>
        public static IServiceCollection AddOllamaLlmProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new OllamaSettings();
            configuration.GetSection("Ollama").Bind(settings);
            services.AddSingleton(settings);

            services.AddHttpClient("Ollama");

            services.AddSingleton<ILlmProvider, OllamaLlmProvider>();

            return services;
        }
    }
}
