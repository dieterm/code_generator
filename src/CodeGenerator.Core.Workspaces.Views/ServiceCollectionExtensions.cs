using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.CleanArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.CleanArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.HexagonArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.NTierArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.OnionArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.HexagonArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.NTierArchitecture;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.MessageBus.EventHandlers;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Core.Workspaces.ViewModels.Datasources;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Presentation.WinForms.Views.Domains;
using CodeGenerator.Presentation.WinForms.Views.Workspace;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Views
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all workspace services in the DI container
        /// </summary>
        public static IServiceCollection AddWorkspaceViewsServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Entities
            services.AddTransient<IView<IArtifactEditViewModel>, ArtifactEditView>();
            services.AddTransient<IView<EntityRelationEditViewModel>, EntityRelationEditView>();
            services.AddTransient<IView<TableDataExtractionFieldModel>, TableDataExtractionField>();
            services.AddTransient<IView<IndexColumnSelectionFieldModel>, IndexColumnSelectionField>();
            services.AddTransient<IView<ForeignKeyColumnMappingFieldModel>, ForeignKeyColumnMappingField>();
            // Entity Views
            services.AddTransient<IView<EntityEditViewEditViewModel>, EntityEditViewEditView>();
            services.AddTransient<IView<EntityEditViewFieldEditViewModel>, EntityEditViewFieldEditView>();
            services.AddTransient<IView<EntityListViewEditViewModel>, EntityListViewEditView>();
            services.AddTransient<IView<EntityListViewColumnEditViewModel>, EntityListViewColumnEditView>();
            services.AddTransient<IView<EntitySelectViewEditViewModel>, EntitySelectViewEditView>();
            return services;
        }
    }
}
