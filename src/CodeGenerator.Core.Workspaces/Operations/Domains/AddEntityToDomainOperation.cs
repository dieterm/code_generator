using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddEntityToDomainOperation : IOperation<AddEntityToDomainParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        private EntityArtifact? _createdEntity;
        private DomainArtifact? _parentDomain;

        public string OperationId => "AddEntity";
        public string DisplayName => "Add Entity";
        public string Description => "Add a new entity to a domain";
        public Type ParameterType => typeof(AddEntityToDomainParams);

        public AddEntityToDomainOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddEntityToDomainParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.EntityName))
                return "Entity name cannot be empty.";

            var domain = _workspaceContextProvider.CurrentWorkspace.FindDomain(parameters.ScopeName, parameters.DomainName);
            if (domain.Entities.GetEntities().Any(e => e.Name.Equals(parameters.EntityName, StringComparison.OrdinalIgnoreCase)))
                return $"Entity '{parameters.EntityName}' already exists in domain '{parameters.DomainName}'.";

            return null;
        }

        public OperationResult Execute(AddEntityToDomainParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            _parentDomain = _workspaceContextProvider.CurrentWorkspace!.FindDomain(parameters.ScopeName, parameters.DomainName);
            _createdEntity = new EntityArtifact(parameters.EntityName);
            _parentDomain!.AddEntity(_createdEntity);

            return OperationResult.Ok($"Entity '{parameters.EntityName}' added to domain '{parameters.DomainName}'.");
        }

        public void Undo()
        {
            if (_createdEntity != null && _parentDomain != null)
                _parentDomain.Entities.RemoveEntity(_createdEntity);
        }

        public void Redo()
        {
            if (_createdEntity != null && _parentDomain != null)
                _parentDomain.AddEntity(_createdEntity);
        }
    }
}
