using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Common
{
    public class ArtifactDocumentationTabViewModel : ArtifactEditViewTabModel
    {
        public SingleButtonFieldModel EditDocumentationField { get; }
        public ArtifactDocumentationTabViewModel() : base("Documentation")
        {
            EditDocumentationField = new SingleButtonFieldModel
            {
                Label = "Edit Documentation",
                Name = "EditDocumentation",
                Tooltip = "Edit markdown documentation",
                ButtonText = "Edit",
                Command = new RelayCommand((a) =>
                {
                    var controller = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
                    controller.ShowArtifactDocumentation(a as WorkspaceArtifactBase);
                }, (a) => a is WorkspaceArtifactBase),
                AutoBind = true
            };
            FieldCollection.FieldModels.Add(EditDocumentationField);
        }
    }
}
