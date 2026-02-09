using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToEntityOperation : IOperation<AddPropertyToEntityParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

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

            //var domain = _workspaceContextProvider.CurrentWorkspace.FindDomain(parameters.ScopeName, parameters.DomainName);
            var entity = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<EntityArtifact>(parameters.EntityId);
            if (entity == null)
                return $"Entity with ID '{parameters.EntityId}' not found.";
            return null;
        }

        public OperationResult Execute(AddPropertyToEntityParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var entity = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<EntityArtifact>(parameters.EntityId);
            if (entity == null)
                return OperationResult.Fail($"Entity with ID '{parameters.EntityId}' not found.");

            parameters.ParentState = entity.DefaultState ?? entity.AddEntityState(entity.Name);
            if (entity.DefaultStateId == null)
                entity.DefaultStateId = parameters.ParentState.Id;

            parameters.CreatedProperty = new PropertyArtifact(parameters.PropertyName, parameters.DataType, parameters.IsNullable);
            parameters.ParentState.AddProperty(parameters.CreatedProperty);

            return OperationResult.Ok($"Property '{parameters.PropertyName}' ({parameters.DataType}, nullable={parameters.IsNullable}) with id '{parameters.CreatedProperty.Id}' added to entity '{entity.Name}'.");
        }

        public void Undo(AddPropertyToEntityParams parameters)
        {
            if (parameters.CreatedProperty != null && parameters.ParentState != null)
                parameters.ParentState.RemoveProperty(parameters.CreatedProperty);
        }

        public void Redo(AddPropertyToEntityParams parameters)
        {
            if (parameters.CreatedProperty != null && parameters.ParentState != null)
                parameters.ParentState.AddProperty(parameters.CreatedProperty);
        }
    }
}
