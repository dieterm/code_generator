using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Scopes
{
    /// <summary>
    /// Operation that adds a new scope to the workspace.
    /// Used by both application code (controllers, context menus) and Copilot.
    /// </summary>
    public class AddScopeToWorkspaceOperation : IOperation<AddScopeToWorkspaceParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        // Undo state — captured during Execute
        private IArtifact? _createdScope;
        private IArtifact? _parentContainer;

        public string OperationId => "AddScope";
        public string DisplayName => "Add Scope";
        public string Description => "Add a new scope to the workspace";
        public Type ParameterType => typeof(AddScopeToWorkspaceParams);

        public AddScopeToWorkspaceOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddScopeToWorkspaceParams parameters)
        {
            var workspace = _workspaceContextProvider.CurrentWorkspace;
            if (workspace == null)
                return "No workspace is currently open.";
            if (workspace.CodeArchitecture == null)
                return "Workspace does not have a code architecture defined. Cannot add scope.";
            if (string.IsNullOrWhiteSpace(parameters.ScopeName))
                return "Scope name cannot be empty.";
            if (workspace.Scopes.Any(s => s.Name.Equals(parameters.ScopeName, StringComparison.OrdinalIgnoreCase)))
                return $"Scope '{parameters.ScopeName}' already exists.";
            return null;
        }

        public OperationResult Execute(AddScopeToWorkspaceParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var workspace = _workspaceContextProvider.CurrentWorkspace!;
            _createdScope = workspace.CodeArchitecture!.ScopeFactory.CreateScopeArtifact(parameters.ScopeName);
            _parentContainer = workspace.Scopes;
            _parentContainer.AddChild(_createdScope);

            return OperationResult.Ok($"Scope '{parameters.ScopeName}' added.");
        }

        public void Undo()
        {
            if (_createdScope != null && _parentContainer != null)
                _parentContainer.RemoveChild(_createdScope);
        }

        public void Redo()
        {
            if (_createdScope != null && _parentContainer != null)
                _parentContainer.AddChild(_createdScope);
        }
    }
}
