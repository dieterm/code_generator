using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all workspace services in the DI container
        /// </summary>
        public static IServiceCollection AddWorkspaceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Workspace services
            services.AddSingleton<WorkspaceFileService>();

            // Message bus event handlers
            services.AddSingleton<IWorkspaceMessageBusSubscriber, WorkspaceArtifactConstructionSubscriber>();

            return services;
        }
    }
}
