using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddSubDomainOperation : IOperation<AddSubDomainParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        public string OperationId => "AddSubDomain";
        public string DisplayName => "Add Sub-Domain";
        public string Description => "Add a new sub-domain to an existing domain";
        public Type ParameterType => typeof(AddSubDomainParams);

        public AddSubDomainOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddSubDomainParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.ParentDomainId))
                return "Parent domain ID cannot be empty.";
            if (string.IsNullOrWhiteSpace(parameters.NewDomainName))
                return "Sub-domain name cannot be empty.";

            var parentDomain = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<DomainArtifact>(parameters.ParentDomainId);
            if (parentDomain == null)
                return $"Parent domain with ID '{parameters.ParentDomainId}' not found.";

            var existingSubDomain = parentDomain.SubDomains
                .FirstOrDefault(d => d.Name.Equals(parameters.NewDomainName, StringComparison.OrdinalIgnoreCase));
            if (existingSubDomain != null)
                return $"Sub-domain '{parameters.NewDomainName}' already exists in domain '{parentDomain.Name}'.";

            return null;
        }

        public OperationResult Execute(AddSubDomainParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            parameters.ParentDomain = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<DomainArtifact>(parameters.ParentDomainId)!;
            parameters.CreatedDomain = parameters.ParentDomain.SubDomains.AddChild(new DomainArtifact(parameters.NewDomainName));

            return OperationResult.Ok($"Sub-domain '{parameters.NewDomainName}' added to domain '{parameters.ParentDomain.Name}'.");
        }

        public void Undo(AddSubDomainParams parameters)
        {
            if (parameters.CreatedDomain != null && parameters.ParentDomain != null)
                parameters.ParentDomain.SubDomains.RemoveChild(parameters.CreatedDomain);
        }

        public void Redo(AddSubDomainParams parameters)
        {
            if (parameters.CreatedDomain != null && parameters.ParentDomain != null)
                parameters.ParentDomain.SubDomains.AddChild(parameters.CreatedDomain);
        }
    }
}
