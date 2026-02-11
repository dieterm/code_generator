using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public class CodeFileElementArtifactController : CodeElementArtifactControllerBase<CodeFileElementArtifact>
    {
        private CodeFileElementEditViewModel? _editViewModel;
        private readonly CodeElementsController _codeElementsController;

        public CodeFileElementArtifactController(CodeElementsController codeElementsController, OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<CodeFileElementArtifactController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
             _codeElementsController = codeElementsController ?? throw new ArgumentNullException(nameof(codeElementsController));
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(CodeFileElementArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();
            
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "show_code_output",
                Text = "Show Code Output",
                IconKey = "braces",
                Execute = async (a) => await ShowCodeOutputAsync(artifact)
            });

            // Rename command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
            {
                Id = "rename_codefile",
                Text = "Rename",
                IconKey = "edit",
                Execute = async (a) =>
                {
                    TreeViewController.RequestBeginRename(artifact);
                    await Task.CompletedTask;
                }
            });

            // Properties command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "codefile_properties",
                Text = "Properties",
                IconKey = "settings",
                Execute = async (a) => await ShowPropertiesAsync(artifact)
            });

            return commands;
        }

        private async Task ShowCodeOutputAsync(CodeFileElementArtifact artifact)
        {
            if (artifact.Language == null) return;
            var generator = ProgrammingLanguageCodeGenerators.GetGenerator(artifact.Language);
            if (generator == null) return;
            var code = generator.GenerateCodeFile(artifact.CodeElement);
            _codeElementsController.ShowCodeElementsEditor(code, artifact.Language);
        }

        protected override Task OnSelectedInternalAsync(CodeFileElementArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private void EnsureEditViewModel(CodeFileElementArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new CodeFileElementEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.CodeFile = artifact;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }

        private Task ShowPropertiesAsync(CodeFileElementArtifact codeFile)
        {
            EnsureEditViewModel(codeFile);
            TreeViewController.ShowArtifactDetailsView(_editViewModel!);
            return Task.CompletedTask;
        }
    }
}
