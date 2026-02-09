using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddEntityWithPropertiesToDomainOperation : IOperation<AddEntityWithPropertiesToDomainParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        private EntityArtifact? _createdEntity;
        private DomainArtifact? _parentDomain;

        public string OperationId => "AddEntityWithProperties";
        public string DisplayName => "Add Entity with Properties";
        public string Description => "Add a new entity with properties to a domain";
        public Type ParameterType => typeof(AddEntityWithPropertiesToDomainParams);

        public AddEntityWithPropertiesToDomainOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddEntityWithPropertiesToDomainParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.EntityName))
                return "Entity name cannot be empty.";
            if (string.IsNullOrWhiteSpace(parameters.PropertiesDefinition))
                return "Properties definition cannot be empty.";

            var domain = _workspaceContextProvider.CurrentWorkspace.FindDomain(parameters.ScopeName, parameters.DomainName);
            if (domain.Entities.GetEntities().Any(e => e.Name.Equals(parameters.EntityName, StringComparison.OrdinalIgnoreCase)))
                return $"Entity '{parameters.EntityName}' already exists in domain '{parameters.DomainName}'.";

            return null;
        }

        public OperationResult Execute(AddEntityWithPropertiesToDomainParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            _parentDomain = _workspaceContextProvider.CurrentWorkspace!.FindDomain(parameters.ScopeName, parameters.DomainName);

            _createdEntity = new EntityArtifact(parameters.EntityName);
            var state = _createdEntity.AddEntityState(parameters.EntityName);
            _createdEntity.DefaultStateId = state.Id;

            var propertyDefs = parameters.PropertiesDefinition.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var propDef in propertyDefs)
            {
                var parts = propDef.Split(':', StringSplitOptions.TrimEntries);
                var propName = parts[0];
                var dataType = parts.Length > 1 ? parts[1] : "varchar";
                var isNullable = parts.Length > 2 && bool.TryParse(parts[2], out var n) && n;

                state.AddProperty(new PropertyArtifact(propName, dataType, isNullable));
            }

            _parentDomain.AddEntity(_createdEntity);

            return OperationResult.Ok($"Entity '{parameters.EntityName}' with {propertyDefs.Length} properties added to domain '{parameters.DomainName}'.");
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
