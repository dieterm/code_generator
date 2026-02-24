using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Common
{
    public class ArtifactCustomPropertiesTabViewModel : ArtifactEditViewTabModel
    {
        //public SingleButtonFieldModel AddCustomPropertyButton { get; }
        public ArtifactCustomPropertiesTabViewModel() : base("Custom Properties")
        {
            //AddCustomPropertyButton = new SingleButtonFieldModel
            //{
            //    Label = "Add Custom Property",
            //    Name = "AddCustomProperty",
            //    Tooltip = "Add a new custom property to the workspace",
            //    ButtonText = "Add",
            //    Command = new RelayCommand((a) =>
            //    {
            //        if (Artifact is WorkspaceArtifact workspace)
            //        {
            //            var controller = ServiceProviderHolder.GetRequiredService<WorkspaceTreeViewController>();
            //            controller.AddCustomProperty(workspace);
            //        }
            //    }, (a) => a is WorkspaceArtifact),
            //    AutoBind = true
            //};
            //FieldCollection.FieldModels.Add(AddCustomPropertyButton);
        }
    }
}
