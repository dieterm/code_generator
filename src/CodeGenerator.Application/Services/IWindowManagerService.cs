using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Settings.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    public interface IWindowManagerService
    {
        void ShowArtifactPreview(ArtifactPreviewViewModel viewModel);
        void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel);
        void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel);
        void ShowSettingsWindow(SettingsViewModel settingsViewModel);
        void ShowWorkspaceTreeView(WorkspaceTreeViewModel treeViewModel);
        void ShowWorkspaceDetailsView(WorkspaceDetailsViewModel viewModel);
    }
}
