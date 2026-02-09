using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddValueTypeToDomainOperation : IOperation<AddValueTypeToDomainParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        public string OperationId => "AddValueType";
        public string DisplayName => "Add Value Type";
        public string Description => "Add a value type to a domain";
        public Type ParameterType => typeof(AddValueTypeToDomainParams);

        public AddValueTypeToDomainOperation(IWorkspaceContextProvider workspaceContextProvider)
        {
            _workspaceContextProvider = workspaceContextProvider;
        }

        public string? Validate(AddValueTypeToDomainParams parameters)
        {
            if (_workspaceContextProvider.CurrentWorkspace == null)
                return "No workspace is currently open.";
            if (string.IsNullOrWhiteSpace(parameters.ValueTypeName))
                return "Value type name cannot be empty.";

            var domain = _workspaceContextProvider.CurrentWorkspace.FindDescendantById<DomainArtifact>(parameters.DomainId);
            var existingValueType = domain?.ValueTypes.GetValueTypes().FirstOrDefault(v => v.Name.Equals(parameters.ValueTypeName, StringComparison.OrdinalIgnoreCase));
            if (existingValueType != null)
                return $"Value type '{parameters.ValueTypeName}' with id '{existingValueType.Id}' already exists in domain '{domain.Name}'.";

            return null;
        }

        public OperationResult Execute(AddValueTypeToDomainParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var domain = _workspaceContextProvider.CurrentWorkspace!.FindDescendantById<DomainArtifact>(parameters.DomainId);
            parameters.ParentContainer = domain!.ValueTypes;
            parameters.CreatedValueType = new ValueTypeArtifact(parameters.ValueTypeName);
            parameters.ParentContainer.AddValueType(parameters.CreatedValueType);

            return OperationResult.Ok($"Value type '{parameters.ValueTypeName}' with id '{parameters.CreatedValueType.Id}' added to domain '{domain.Name}'.");
        }

        public void Undo(AddValueTypeToDomainParams parameters)
        {
            if (parameters.CreatedValueType != null && parameters.ParentContainer != null)
                parameters.ParentContainer.RemoveValueType(parameters.CreatedValueType);
        }

        public void Redo(AddValueTypeToDomainParams parameters)
        {
            if (parameters.CreatedValueType != null && parameters.ParentContainer != null)
                parameters.ParentContainer.AddValueType(parameters.CreatedValueType);
        }
    }
}
