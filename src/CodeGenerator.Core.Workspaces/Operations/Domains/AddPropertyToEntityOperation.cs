using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToEntityOperation : IOperation<AddPropertyToEntityParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        private PropertyArtifact? _createdProperty;
        private EntityStateArtifact? _parentState;

        public string OperationId => "AddPropertyToEntity";
        public string DisplayName => "Add Property to Entity";
        public string Description => "Add a property to an existing entity's default state";
        public Type ParameterType => typeof(AddPropertyToEntityParams);

        public AddPropertyToEntityOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddPropertyToEntityParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.PropertyName))
                return "Property name cannot be empty.";

            var domain = _workspaceContextProvider.CurrentWorkspace.FindDomain(parameters.ScopeName, parameters.DomainName);
            var entity = domain.Entities.GetEntities()
                .FirstOrDefault(e => e.Name.Equals(parameters.EntityName, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
                return $"Entity '{parameters.EntityName}' not found in domain '{parameters.DomainName}'.";

            return null;
        }

        public OperationResult Execute(AddPropertyToEntityParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var domain = _workspaceContextProvider.CurrentWorkspace!.FindDomain(parameters.ScopeName, parameters.DomainName);
            var entity = domain.Entities.GetEntities()
                .First(e => e.Name.Equals(parameters.EntityName, StringComparison.OrdinalIgnoreCase));

            _parentState = entity.DefaultState ?? entity.AddEntityState(parameters.EntityName);
            if (entity.DefaultStateId == null)
                entity.DefaultStateId = _parentState.Id;

            _createdProperty = new PropertyArtifact(parameters.PropertyName, parameters.DataType, parameters.IsNullable);
            _parentState.AddProperty(_createdProperty);

            return OperationResult.Ok($"Property '{parameters.PropertyName}' ({parameters.DataType}, nullable={parameters.IsNullable}) added to entity '{parameters.EntityName}'.");
        }

        public void Undo()
        {
            if (_createdProperty != null && _parentState != null)
                _parentState.RemoveProperty(_createdProperty);
        }

        public void Redo()
        {
            if (_createdProperty != null && _parentState != null)
                _parentState.AddProperty(_createdProperty);
        }
    }
}
