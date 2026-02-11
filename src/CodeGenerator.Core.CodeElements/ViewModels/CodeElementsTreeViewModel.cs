using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.ViewModels;
using CodeGenerator.Core.CodeElements.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.ViewModels
{
    public class CodeElementsTreeViewModel : ArtifactTreeViewModel<CodeElementsTreeViewController>
    {
        public CodeElementsTreeViewModel(CodeElementsTreeViewController treeViewController) : base(treeViewController)
        {
        }

        public override IEnumerable<ArtifactTreeNodeCommand> GetContextMenuCommands(IArtifact artifact)
        {
            var menuCommands = TreeViewController.GetContextMenuCommands(artifact);
            return menuCommands;
        }

        public override Task HandleDoubleClickAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
