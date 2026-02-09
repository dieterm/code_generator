using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddPropertyToValueTypeOperation : IOperation<AddPropertyToValueTypeParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        public string OperationId => "AddPropertyToValueType";
        public string DisplayName => "Add Property to Value Type";
        public string Description => "Add a property to an existing value type";
        public Type ParameterType => typeof(AddPropertyToValueTypeParams);

        public AddPropertyToValueTypeOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddPropertyToValueTypeParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.PropertyName))
                return "Property name cannot be empty.";

            var valueType = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<ValueTypeArtifact>(parameters.ValueTypeId);
            if (valueType == null)
                return $"Value type with ID '{parameters.ValueTypeId}' not found.";

            return null;
        }

        public OperationResult Execute(AddPropertyToValueTypeParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            parameters.ParentValueType = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<ValueTypeArtifact>(parameters.ValueTypeId)!;
            parameters.CreatedProperty = new PropertyArtifact(parameters.PropertyName, parameters.DataType, parameters.IsNullable);
            parameters.ParentValueType.AddProperty(parameters.CreatedProperty);

            return OperationResult.Ok($"Property '{parameters.PropertyName}' ({parameters.DataType}, nullable={parameters.IsNullable}) with id '{parameters.CreatedProperty.Id}' added to value type '{parameters.ParentValueType.Name}'.");
        }

        public void Undo(AddPropertyToValueTypeParams parameters)
        {
            if (parameters.CreatedProperty != null && parameters.ParentValueType != null)
                parameters.ParentValueType.RemoveProperty(parameters.CreatedProperty);
        }

        public void Redo(AddPropertyToValueTypeParams parameters)
        {
            if (parameters.CreatedProperty != null && parameters.ParentValueType != null)
                parameters.ParentValueType.AddProperty(parameters.CreatedProperty);
        }
    }
}
