using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Settings.ViewModels;

namespace CodeGenerator.Application.Services
{
    public interface IWindowManagerService
    {
        void ShowArtifactPreview(ArtifactPreviewViewModel viewModel);
        void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel);
        void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel);
        void ShowSettingsWindow(SettingsViewModel settingsViewModel);
        void ShowWorkspaceTreeView(WorkspaceTreeViewModel treeViewModel);
        void ShowWorkspaceDetailsView(ArtifactDetailsViewModel viewModel);
        void ShowTemplateTreeView(TemplateTreeViewModel treeViewModel);
        void ShowTemplateDetailsView(ArtifactDetailsViewModel artifactDetailsViewModel);
        void ShowScribanTemplateEditView(ScribanTemplateEditViewModel viewModel);
    }
}
