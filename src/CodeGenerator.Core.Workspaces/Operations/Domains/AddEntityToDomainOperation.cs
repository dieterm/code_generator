using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddEntityToDomainOperation : IOperation<AddEntityToDomainParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

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

            var entitiesContainer = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<EntitiesContainerArtifact>(parameters.EntitiesContainerId);
            if (entitiesContainer == null)
                return $"Entities container with id '{parameters.EntitiesContainerId}' not found.";
            var domain = entitiesContainer.Parent as DomainArtifact;
            var entities = entitiesContainer.GetEntities();
            var existingEntity = entities.FirstOrDefault(e => e.Name.Equals(parameters.EntityName, StringComparison.OrdinalIgnoreCase));
            if (existingEntity != null)
                return $"Entity '{parameters.EntityName}' with id '{existingEntity.Id}' already exists in domain '{domain.Name}'.";

            return null;
        }

        public OperationResult Execute(AddEntityToDomainParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var entitiesContainer = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<EntitiesContainerArtifact>(parameters.EntitiesContainerId);
            if (entitiesContainer == null)
                return OperationResult.Fail($"Entities container with id '{parameters.EntitiesContainerId}' not found.");
            parameters.ParentDomain = entitiesContainer.Parent as DomainArtifact;
            parameters.CreatedEntity = new EntityArtifact(parameters.EntityName);
            parameters.ParentDomain!.AddEntity(parameters.CreatedEntity);

            return OperationResult.Ok($"Entity '{parameters.EntityName}' with id '{parameters.CreatedEntity.Id}' added to domain '{parameters.ParentDomain.Name}'.");
        }

        public void Undo(AddEntityToDomainParams parameters)
        {
            if (parameters.CreatedEntity != null && parameters.ParentDomain != null)
                parameters.ParentDomain.Entities.RemoveEntity(parameters.CreatedEntity);
        }

        public void Redo(AddEntityToDomainParams parameters)
        {
            if (parameters.CreatedEntity != null && parameters.ParentDomain != null)
                parameters.ParentDomain.AddEntity(parameters.CreatedEntity);
        }
    }
}
