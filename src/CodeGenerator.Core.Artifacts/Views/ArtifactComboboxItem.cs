using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.Views
{
    public class ArtifactComboboxItem : ComboboxItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public IArtifact Artifact { get; }

        public ArtifactComboboxItem(IArtifact artifact)
        {
            Artifact = artifact;
            this.DisplayName = artifact.TreeNodeText;
            this.Value = artifact.Id;
            Artifact.PropertyChanged += Artifact_PropertyChanged;
        }

        private void Artifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(IArtifact.TreeNodeText))
            {
                this.DisplayName = Artifact.TreeNodeText;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
            }
        }

        public override void Dispose()
        {
            Artifact.PropertyChanged -= Artifact_PropertyChanged;
        }
    }
}
