using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Core.CodeElements.ViewModels.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers.Statements
{
    public class CompositeStatementArtifactController : StatementArtifactControllerBase<CompositeStatementArtifact>
    {
        private CompositeStatementEditViewModel? _editViewModel;

        public CompositeStatementArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<CompositeStatementArtifactController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(CompositeStatementArtifact artifact)
        {
            var addStatementCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE) {
                Id = "AddStatement",
                Text = "Add Statement",
                IconKey = "Add",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddAssignmentStatement",
                Text = "Assignment Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new AssignmentStatement() { Name = "Assignment Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new AssignmentStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddCommentStatement",
                Text = "Comment Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new CommentStatement() { Name = "Comment Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new CommentStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddCompositeStatement",
                Text = "Composite Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new CompositeStatement() { Name = "Composite Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new CompositeStatementArtifact(statement, false));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddForEachStatement",
                Text = "ForEach Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new ForEachStatementElement() { Name = "ForEach Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new ForEachStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddForStatement",
                Text = "For Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new ForStatementElement() { Name = "For Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new ForStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddIfStatement",
                Text = "If Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new IfStatementElement() { Name = "If Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new IfStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddRawStatement",
                Text = "Raw Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new RawStatementElement() { Name = "Raw Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new RawStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddReturnStatement",
                Text = "Return Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new ReturnStatementElement() { Name = "Return Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new ReturnStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddSwitchStatement",
                Text = "Switch Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new SwitchStatementElement() { Name = "Switch Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new SwitchStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddThrowStatement",
                Text = "Throw Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new ThrowStatementElement() { Name = "Throw Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new ThrowStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddTryCatchStatement",
                Text = "Try/Catch Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new TryCatchStatementElement() { Name = "Try/Catch Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new TryCatchStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddUsingStatement",
                Text = "Using Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new UsingStatementElement() { Name = "Using Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new UsingStatementArtifact(statement));
                }
            });

            addStatementCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddWhileStatement",
                Text = "While Statement",
                IconKey = "Add",
                Execute = async (a) =>
                {
                    var statement = new WhileStatementElement() { Name = "While Statement" };
                    artifact.Statements.Add(statement);
                    artifact.AddChild(new WhileStatementArtifact(statement));
                }
            });

            yield return addStatementCommand;
        }

        public override bool CanDelete(CompositeStatementArtifact artifact)
        {
            return !artifact.IsReadOnly && artifact.Parent is CompositeStatementArtifact;
        }

        public override void Delete(CompositeStatementArtifact artifact)
        {
            if (artifact.Parent is CompositeStatementArtifact parent)
            {
                parent.Statements.Remove(artifact.StatementElement);
                parent.RemoveChild(artifact);
            }
        }

        protected override Task OnSelectedInternalAsync(CompositeStatementArtifact artifact, CancellationToken cancellationToken)
        {
            return ShowPropertiesAsync(artifact);
        }

        private Task ShowPropertiesAsync(CompositeStatementArtifact artifact)
        {
            if (_editViewModel == null)
            {
                _editViewModel = new CompositeStatementEditViewModel();
                _editViewModel.ValueChanged += OnEditViewModelValueChanged;
            }

            _editViewModel.Artifact = artifact;
            TreeViewController.ShowArtifactDetailsView(_editViewModel);
            return Task.CompletedTask;
        }

        private void OnEditViewModelValueChanged(object? sender, ArtifactPropertyChangedEventArgs e)
        {
            TreeViewController.OnArtifactPropertyChanged(e.Artifact, e.PropertyName, e.NewValue);
        }
    }
}
