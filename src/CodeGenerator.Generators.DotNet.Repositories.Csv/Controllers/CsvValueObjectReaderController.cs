using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Generators.DotNet.Repositories.Csv.ViewModels;
using CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Controllers
{
    public class CsvValueObjectReaderController : WorkspaceArtifactControllerBase<CsvValueObjectReaderArtifact>
    {
        private CsvValueObjectReaderArtifactEditViewModel? _editViewModel;
        public CsvValueObjectReaderController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<CsvValueObjectReaderController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(CsvValueObjectReaderArtifact artifact)
        {
            return Enumerable.Empty<ArtifactTreeNodeCommand>();
        }

        protected override Task OnSelectedInternalAsync(CsvValueObjectReaderArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

        private void EnsureEditViewModel(CsvValueObjectReaderArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new CsvValueObjectReaderArtifactEditViewModel();
            }

            _editViewModel.CsvValueObjectReaderArtifact = artifact;
        }
    }
}
