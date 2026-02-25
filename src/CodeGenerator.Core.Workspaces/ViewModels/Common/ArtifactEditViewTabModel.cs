using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Common
{
    public abstract class ArtifactEditViewTabModel : ViewModelBase 
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        public FieldCollectionModel FieldCollection { get; } = new FieldCollectionModel();
        
        public ArtifactEditViewTabModel(string text)
        {
            _text = text;
        }

        public virtual void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            foreach (var field in FieldCollection.FieldModels)
            {
                if (field.AutoBind)
                {
                    field.Target = artifactBase;
                }
            }
        }

    }

    public abstract class ArtifactEditViewTabModel<TArtifact> : ArtifactEditViewTabModel
        where TArtifact : WorkspaceArtifactBase
    {
        private TArtifact? _artifact;
        public TArtifact? Artifact
        {
            get { return _artifact; }
            set { SetProperty(ref _artifact, value); }
        }

        public ArtifactEditViewTabModel(string text)
                : base(text)
        {
           
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            if (_artifact != null)
                _artifact.PropertyChanged -= Artifact_PropertyChanged;

            Artifact = artifactBase as TArtifact;

            foreach (var field in FieldCollection.FieldModels)
            {
                if (field.AutoBind)
                {
                    field.Target = artifactBase;
                }
            }

            if (_artifact != null)
            {
                _artifact.PropertyChanged += Artifact_PropertyChanged;
            }
        }

        private void Artifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnArtifactPropertyChanged((sender as TArtifact)!, e.PropertyName!);
        }

        /// <summary>
        /// Overwrite this method in derived classes to react to property changes of the artifact. The method will be called whenever a property of the artifact changes, allowing you to update the UI or perform other actions as needed.
        /// </summary>
        /// <param name="artifact">The artifact that has changed.</param>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnArtifactPropertyChanged(TArtifact artifact, string propertyName)
        {

        }
    }
}
