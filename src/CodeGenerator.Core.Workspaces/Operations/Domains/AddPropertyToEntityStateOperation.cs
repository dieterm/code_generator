using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToEntityStateOperation : IOperation<AddPropertyToEntityStateParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        public string OperationId => "AddPropertyToEntityState";
        public string DisplayName => "Add Property to Entity State";
        public string Description => "Add a property to an existing entity state";
        public Type ParameterType => typeof(AddPropertyToEntityStateParams);

        public AddPropertyToEntityStateOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddPropertyToEntityStateParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.PropertyName))
                return "Property name cannot be empty.";

            var entityState = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<EntityStateArtifact>(parameters.EntityStateId);
            if (entityState == null)
                return $"Entity state with ID '{parameters.EntityStateId}' not found.";

            return null;
        }

        public OperationResult Execute(AddPropertyToEntityStateParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            parameters.ParentState = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<EntityStateArtifact>(parameters.EntityStateId)!;
            parameters.CreatedProperty = new PropertyArtifact(parameters.PropertyName, parameters.DataType, parameters.IsNullable);
            parameters.ParentState.AddProperty(parameters.CreatedProperty);

            return OperationResult.Ok($"Property '{parameters.PropertyName}' ({parameters.DataType}, nullable={parameters.IsNullable}) with id '{parameters.CreatedProperty.Id}' added to entity state '{parameters.ParentState.Name}'.");
        }

        public void Undo(AddPropertyToEntityStateParams parameters)
        {
            if (parameters.CreatedProperty != null && parameters.ParentState != null)
                parameters.ParentState.RemoveProperty(parameters.CreatedProperty);
        }

        public void Redo(AddPropertyToEntityStateParams parameters)
        {
            if (parameters.CreatedProperty != null && parameters.ParentState != null)
                parameters.ParentState.AddProperty(parameters.CreatedProperty);
        }
    }
}
