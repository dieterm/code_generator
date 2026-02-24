using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Common
{
    public  class ArtifactEditViewModel<TArtifact, TGeneralTab> : ViewModelBase, IArtifactEditViewModel 
        where TArtifact : WorkspaceArtifactBase
        where TGeneralTab : ArtifactEditViewTabModel
    {
        private string _artifactName;
        public string ArtifactName
        {
            get { return _artifactName; }
            set { SetProperty(ref _artifactName, value); }
        }

        private TArtifact? _artifact;
        public TArtifact? Artifact
        {
            get { return _artifact; }
            set
            {
                SetProperty(ref _artifact, value);
                BindArtifact(value);
            }
        }

        public virtual void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            foreach (var tab in Tabs)
            {
                tab.BindArtifact(artifactBase);
            }
        }

        public ObservableCollection<ArtifactEditViewTabModel> Tabs { get; } = [];

        WorkspaceArtifactBase? IArtifactEditViewModel.Artifact => Artifact;

        public ArtifactEditViewModel(string artifactName, TGeneralTab generalTab, params ArtifactEditViewTabModel[] optionalTabs)
        {
            _artifactName = artifactName;
            Tabs.Add(generalTab);
            foreach (var tab in optionalTabs)
            {
                Tabs.Add(tab);
            }
            Tabs.Add(new ArtifactDocumentationTabViewModel());
            Tabs.Add(new ArtifactCustomPropertiesTabViewModel());
        }

        public FieldViewModelBase? GetFieldByName(string name)
        {
            foreach (var tab in Tabs)
            {
                var field = tab.FieldCollection.FieldModels.FirstOrDefault(f => f.Name == name);
                if (field != null)
                    return field;
            }
            return null;
        }

        public ArtifactDocumentationTabViewModel DocumentationTab { get { return Tabs.OfType<ArtifactDocumentationTabViewModel>().Single(); } }
        public ArtifactCustomPropertiesTabViewModel CustomPropertiesTab { get { return Tabs.OfType<ArtifactCustomPropertiesTabViewModel>().Single(); } }
        public TGeneralTab GeneralTab { get { return Tabs.OfType<TGeneralTab>().Single(); } }
    }
}
