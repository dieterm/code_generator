using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Artifacts;
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
    public class CsvValueObjectReaderImplementationController : WorkspaceArtifactControllerBase<CsvValueObjectReaderImplementationArtifact>
    {
        private CsvValueObjectReaderImplementationArtifactEditViewModel? _editViewModel;

        public CsvValueObjectReaderImplementationController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<CsvValueObjectReaderImplementationController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(CsvValueObjectReaderImplementationArtifact artifact)
        {
            return Enumerable.Empty<ArtifactTreeNodeCommand>();
        }

        protected override Task OnSelectedInternalAsync(CsvValueObjectReaderImplementationArtifact artifact, CancellationToken cancellationToken)
        {
            EnsureEditViewModel(artifact);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }

        private void EnsureEditViewModel(CsvValueObjectReaderImplementationArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new CsvValueObjectReaderImplementationArtifactEditViewModel();
            }

            _editViewModel.CsvValueObjectReaderImplementationArtifact = artifact;
        }
    }
}
