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
            var existingScope = workspace.Scopes.FirstOrDefault(s => s.Name.Equals(parameters.ScopeName, StringComparison.OrdinalIgnoreCase));
            if (existingScope != null)
                return $"Scope '{parameters.ScopeName}' with id '{existingScope.Id}' already exists.";
            return null;
        }

        public OperationResult Execute(AddScopeToWorkspaceParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var workspace = _workspaceContextProvider.CurrentWorkspace!;
            parameters.CreatedScope = workspace.CodeArchitecture!.ScopeFactory.CreateScopeArtifact(parameters.ScopeName);
            parameters.ParentContainer = workspace.Scopes;
            parameters.ParentContainer.AddChild(parameters.CreatedScope);

            return OperationResult.Ok($"Scope '{parameters.ScopeName}' with id '{parameters.CreatedScope.Id}' added.");
        }

        public void Undo(AddScopeToWorkspaceParams parameters)
        {
            if (parameters.CreatedScope != null && parameters.ParentContainer != null)
                parameters.ParentContainer.RemoveChild(parameters.CreatedScope);
        }

        public void Redo(AddScopeToWorkspaceParams parameters)
        {
            if (parameters.CreatedScope != null && parameters.ParentContainer != null)
                parameters.ParentContainer.AddChild(parameters.CreatedScope);
        }
    }
}
