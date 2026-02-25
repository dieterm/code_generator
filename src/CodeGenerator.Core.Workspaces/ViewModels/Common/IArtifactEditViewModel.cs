using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Core.Workspaces.ViewModels.Common
{
    public interface IArtifactEditViewModel : IViewModel
    {
        WorkspaceArtifactBase? Artifact { get; set; }
        string ArtifactName { get; }
        ObservableCollection<ArtifactEditViewTabModel> Tabs { get; }
        event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;
        FieldViewModelBase? GetFieldByName(string name);
    }
}