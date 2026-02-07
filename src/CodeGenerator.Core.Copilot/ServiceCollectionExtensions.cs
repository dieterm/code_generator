using CodeGenerator.Application.Controllers.Copilot;
using CodeGenerator.Core.Copilot.Controllers;
using CodeGenerator.Core.Copilot.Settings;
using CodeGenerator.Core.Copilot.ViewModels;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DependancyInjectionFrameworks;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Copilot
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Domain services in the DI container
        /// </summary>
        public static IServiceCollection AddCopilotServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            configuration.GetSection("Copilot").Bind(CopilotSettings.Instance);
            services.AddSingleton<ICopilotController, CopilotController>();
            services.AddSingleton<CopilotChatViewModel>();

            return services;
        }
    }
}
