using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddDomainToScopeOperation : IOperation<AddDomainToScopeParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        private DomainArtifact? _createdDomain;
        private OnionDomainLayerArtifact? _parentContainer;

        public string OperationId => "AddDomain";
        public string DisplayName => "Add Domain";
        public string Description => "Add a new domain to a scope";
        public Type ParameterType => typeof(AddDomainToScopeParams);

        public AddDomainToScopeOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddDomainToScopeParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.ScopeName))
                return "Scope name cannot be empty.";
            if (string.IsNullOrWhiteSpace(parameters.DomainName))
                return "Domain name cannot be empty.";

            var scope = _workspaceContextProvider.CurrentWorkspace.Scopes.FindScope(parameters.ScopeName);
            if (scope.Domains.Any(d => d.Name.Equals(parameters.DomainName, StringComparison.OrdinalIgnoreCase)))
                return $"Domain '{parameters.DomainName}' already exists in scope '{parameters.ScopeName}'.";

            return null;
        }

        public OperationResult Execute(AddDomainToScopeParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var scope = _workspaceContextProvider.CurrentWorkspace!.Scopes.FindScope(parameters.ScopeName);
            _parentContainer = scope.Domains;
            _createdDomain = _parentContainer.AddDomain(parameters.DomainName);

            return OperationResult.Ok($"Domain '{parameters.DomainName}' added to scope '{parameters.ScopeName}'.");
        }

        public void Undo()
        {
            if (_createdDomain != null && _parentContainer != null)
                _parentContainer.RemoveChild(_createdDomain);
        }

        public void Redo()
        {
            if (_createdDomain != null && _parentContainer != null)
                _parentContainer.AddChild(_createdDomain);
        }
    }
}
