using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Template
{
    public class RootArtifactController : TemplateArtifactControllerBase<RootArtifact>
    {
        public RootArtifactController(OperationExecutor operationExecutor, TemplateTreeViewController treeViewController, ILogger<RootArtifactController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(RootArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "create_folder",
                Text = "Create Folder",
                IconKey = "folder",
                Execute = async (a) =>
                {
                    var dirInfo = artifact.FolderPath.CreateDirectory("New Folder");
                    if (dirInfo == null) return;
                    var newFolderArtifact = new ExistingFolderArtifact(dirInfo.FullName, dirInfo.Name);
                    artifact.AddChild(newFolderArtifact);
                    TreeViewController.OnArtifactAdded(artifact, newFolderArtifact);
                    TreeViewController.RequestBeginRename(newFolderArtifact);
                }
            };
        }
    }
}
