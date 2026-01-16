using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts.FileSystem;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Template
{
    public class ExistingFolderArtifactController : ArtifactControllerBase<TemplateTreeViewController, ExistingFolderArtifact>
    {
        public ExistingFolderArtifactController(TemplateTreeViewController treeViewController, ILogger<ExistingFolderArtifactController> logger)
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ExistingFolderArtifact folderArtifact)
        {
            yield return new ArtifactTreeNodeCommand
            {
                Id = "open_folder",
                Text = "Open in Explorer",
                IconKey = "folder",
                Execute = async (a) =>
                {
                    if (Directory.Exists(folderArtifact.ExistingFolderPath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", folderArtifact.ExistingFolderPath);
                    }
                    await Task.CompletedTask;
                }
            };
        }
    }
}
