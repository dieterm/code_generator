using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Scopes
{
    public class AddSubScopeToScopeOperation : IOperation<AddSubScopeToScopeParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        public string OperationId => "AddSubScope";
        public string DisplayName => "Add Sub-Scope";
        public string Description => "Add a new sub-scope to an existing scope";
        public Type ParameterType => typeof(AddSubScopeToScopeParams);

        public AddSubScopeToScopeOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddSubScopeToScopeParams parameters)
        {
            var workspace = _workspaceContextProvider.CurrentWorkspace;
            if (workspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.ParentScopeId))
                return "Parent scope ID cannot be empty.";
            if (string.IsNullOrWhiteSpace(parameters.NewScopeName))
                return "New scope name cannot be empty.";

            var existing = _workspaceContextProvider.CurrentWorkspace?.FindScope(parameters.NewScopeName, false);
            if (existing != null)
                return $"Scope '{parameters.NewScopeName}' with id '{existing.Id}' already exists.";

            return null;
        }

        public OperationResult Execute(AddSubScopeToScopeParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var parentScope = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<ScopeArtifact>(parameters.ParentScopeId);//.Scopes.FindScope(parameters.ParentScopeName);
            parameters.ParentContainer = parentScope.SubScopes;
            parameters.CreatedScope = parameters.ParentContainer.AddScope(parameters.NewScopeName);

            return OperationResult.Ok($"Sub-scope '{parameters.NewScopeName}' with id '{parameters.CreatedScope.Id}' added to scope '{parentScope.Name}'.");
        }

        public void Undo(AddSubScopeToScopeParams parameters)
        {
            if (parameters.CreatedScope != null && parameters.ParentContainer != null)
                parameters.ParentContainer.RemoveChild(parameters.CreatedScope);
        }

        public void Redo(AddSubScopeToScopeParams parameters)
        {
            if (parameters.CreatedScope != null && parameters.ParentContainer != null)
                parameters.ParentContainer.AddChild(parameters.CreatedScope);
        }
    }
}
