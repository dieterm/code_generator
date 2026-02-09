using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddDomainToScopeOperation : IOperation<AddDomainToScopeParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

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
            if (string.IsNullOrWhiteSpace(parameters.ScopeId))
                return "Scope ID cannot be empty.";
            if (string.IsNullOrWhiteSpace(parameters.DomainName))
                return "Domain name cannot be empty.";

            var scope = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<ScopeArtifact>(parameters.ScopeId);
            var existingDomain = scope.Domains.FirstOrDefault(d => d.Name.Equals(parameters.DomainName, StringComparison.OrdinalIgnoreCase));
            if (existingDomain != null)
                return $"Domain '{parameters.DomainName}' with id '{existingDomain.Id}' already exists in scope '{scope.Name}'.";

            return null;
        }

        public OperationResult Execute(AddDomainToScopeParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var scope = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<ScopeArtifact>(parameters.ScopeId);
            parameters.ParentContainer = scope.Domains;
            parameters.CreatedDomain = parameters.ParentContainer.AddChild(new DomainArtifact(parameters.DomainName));

            return OperationResult.Ok($"Domain '{parameters.DomainName}' with id '{parameters.CreatedDomain.Id}' added to scope '{scope.Name}'.");
        }

        public void Undo(AddDomainToScopeParams parameters)
        {
            if (parameters.CreatedDomain != null && parameters.ParentContainer != null)
                parameters.ParentContainer.RemoveChild(parameters.CreatedDomain);
        }

        public void Redo(AddDomainToScopeParams parameters)
        {
            if (parameters.CreatedDomain != null && parameters.ParentContainer != null)
                parameters.ParentContainer.AddChild(parameters.CreatedDomain);
        }
    }
}
