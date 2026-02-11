using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Application.Services
{
    public interface IWindowManagerService
    {
        void ShowViewModel<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;
        void ShowArtifactPreview(ArtifactPreviewViewModel viewModel);
        void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel);
        void ShowSettingsWindow(SettingsViewModel settingsViewModel);
     
    }
}
