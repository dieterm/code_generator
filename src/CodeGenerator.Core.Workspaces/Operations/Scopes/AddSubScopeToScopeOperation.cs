using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Scopes
{
    public class AddSubScopeToScopeOperation : IOperation<AddSubScopeToScopeParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        private ScopeArtifact? _createdScope;
        private SubScopesContainerArtifact? _parentContainer;

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
            if (string.IsNullOrWhiteSpace(parameters.ParentScopeName))
                return "Parent scope name cannot be empty.";
            if (string.IsNullOrWhiteSpace(parameters.NewScopeName))
                return "New scope name cannot be empty.";

            var existing = _workspaceContextProvider.CurrentWorkspace?.FindScope(parameters.NewScopeName, false);
            if (existing != null)
                return $"Scope '{parameters.NewScopeName}' already exists.";

            return null;
        }

        public OperationResult Execute(AddSubScopeToScopeParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var parentScope = _workspaceContextProvider.CurrentWorkspace!.Scopes.FindScope(parameters.ParentScopeName);
            _parentContainer = parentScope.SubScopes;
            _createdScope = _parentContainer.AddScope(parameters.NewScopeName);

            return OperationResult.Ok($"Sub-scope '{parameters.NewScopeName}' added to scope '{parameters.ParentScopeName}'.");
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
