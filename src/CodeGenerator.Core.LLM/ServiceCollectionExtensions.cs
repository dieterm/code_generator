using CodeGenerator.Application.Controllers.Copilot;
using CodeGenerator.Core.LLM.Controllers;
using CodeGenerator.Core.LLM.Settings;
using CodeGenerator.Core.LLM.ViewModels;
using CodeGenerator.Core.Settings.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.LLM
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers LLM abstraction layer services in the DI container.
        /// ILlmProvider implementations should be registered separately by each provider project.
        /// </summary>
        public static IServiceCollection AddLlmServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ILlmController, LlmController>();
            services.AddSingleton<LlmChatViewModel>();
            //services.AddSingleton<LlmSettingsManager>();
            services.AddKeyedSingleton<ISettingsManager, LlmSettingsManager>("LlmSettingsManager");//, (sp) => sp.GetService<LlmSettingsManager>()!);
            return services;
        }
    }
}
