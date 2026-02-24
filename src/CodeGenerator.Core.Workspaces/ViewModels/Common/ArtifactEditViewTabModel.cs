using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
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
}
