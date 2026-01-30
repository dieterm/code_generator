using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ExtensionMethods;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Template
{
    public class RootArtifactController : ArtifactControllerBase<TemplateTreeViewController, RootArtifact>
    {
        public RootArtifactController(TemplateTreeViewController treeViewController, ILogger<RootArtifactController> logger)
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(RootArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand
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
