using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Common
{
    public abstract class ArtifactEditViewModel<TArtifact, TGeneralTab> : ViewModelBase, IArtifactEditViewModel 
        where TArtifact : WorkspaceArtifactBase
        where TGeneralTab : ArtifactEditViewTabModel<TArtifact>
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
                if (_artifact != null)
                {
                    UnsubscribeFromFieldChanges();
                }
                
                SetProperty(ref _artifact, value);
                BindArtifact(value);
                
                if (_artifact != null)
                {
                    SubscribeToFieldChanges();
                }
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

        WorkspaceArtifactBase? IArtifactEditViewModel.Artifact { 
            get { return Artifact; } 
            set { Artifact = value as TArtifact; }
        }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

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

        private void SubscribeToFieldChanges()
        {
            foreach (var tab in Tabs)
            {
                foreach (var field in tab.FieldCollection.FieldModels)
                {
                    field.PropertyChanged += OnFieldPropertyChanged;
                }
            }
        }

        private void UnsubscribeFromFieldChanges()
        {
            foreach (var tab in Tabs)
            {
                foreach (var field in tab.FieldCollection.FieldModels)
                {
                    field.PropertyChanged -= OnFieldPropertyChanged;
                }
            }
        }

        private void OnFieldPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_artifact == null) return;
            
            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                if (field.AutoUpdate)
                {
                    OnValueChanged(_artifact, field.Name, field.Value);
                }
            }
        }

        protected void OnValueChanged(TArtifact artifact, string propertyName, object? newValue)
        {
            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(artifact, propertyName, newValue));
        }
        protected void OnValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
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

        public override void DisposeViewModel()
        {
            UnsubscribeFromFieldChanges();
            base.DisposeViewModel();
        }
    }
}
