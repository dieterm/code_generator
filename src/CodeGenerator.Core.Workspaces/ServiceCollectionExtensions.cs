using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeArchitecture;
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
            
            // Onion architecture layer factories
            services.AddSingleton<IOnionArchitectureLayerFactory, OnionApplicationLayerFactory>();
            services.AddSingleton<IOnionArchitectureLayerFactory, OnionDomainLayerFactory>();
            services.AddSingleton<IOnionArchitectureLayerFactory, OnionInfrastructureLayerFactory>();
            services.AddSingleton<IOnionArchitectureLayerFactory, OnionPresentationLayerFactory>();

            // N-Tier architecture layer factories
            services.AddSingleton<INTierArchitectureLayerFactory, NTierPresentationLayerFactory>();
            services.AddSingleton<INTierArchitectureLayerFactory, NTierBusinessLayerFactory>();
            services.AddSingleton<INTierArchitectureLayerFactory, NTierDataAccessLayerFactory>();

            // Hexagonal architecture layer factories
            services.AddSingleton<IHexagonArchitectureLayerFactory, HexagonCoreLayerFactory>();
            services.AddSingleton<IHexagonArchitectureLayerFactory, HexagonPortsLayerFactory>();
            services.AddSingleton<IHexagonArchitectureLayerFactory, HexagonAdaptersLayerFactory>();

            // Clean architecture layer factories
            services.AddSingleton<ICleanArchitectureLayerFactory, CleanEntitiesLayerFactory>();
            services.AddSingleton<ICleanArchitectureLayerFactory, CleanUseCasesLayerFactory>();
            services.AddSingleton<ICleanArchitectureLayerFactory, CleanInterfaceAdaptersLayerFactory>();
            services.AddSingleton<ICleanArchitectureLayerFactory, CleanFrameworksLayerFactory>();

            return services;
        }
    }
}
