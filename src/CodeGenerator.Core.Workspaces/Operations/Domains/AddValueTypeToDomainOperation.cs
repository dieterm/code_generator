using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Core.Workspaces.Operations.Domains
{
    public class AddValueTypeToDomainOperation : IOperation<AddValueTypeToDomainParams>
    {
        private readonly IWorkspaceContextProvider _workspaceContextProvider;

        private ValueTypeArtifact? _createdValueType;
        private ValueTypesContainerArtifact? _parentContainer;

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

            var domain = _workspaceContextProvider.CurrentWorkspace.FindDomain(parameters.ScopeName, parameters.DomainName);
            if (domain.ValueTypes.GetValueTypes().Any(v => v.Name.Equals(parameters.ValueTypeName, StringComparison.OrdinalIgnoreCase)))
                return $"Value type '{parameters.ValueTypeName}' already exists in domain '{parameters.DomainName}'.";

            return null;
        }

        public OperationResult Execute(AddValueTypeToDomainParams parameters)
        {
            var validationError = Validate(parameters);
            if (validationError != null)
                return OperationResult.Fail(validationError);

            var domain = _workspaceContextProvider.CurrentWorkspace!.FindDomain(parameters.ScopeName, parameters.DomainName);
            _parentContainer = domain!.ValueTypes;
            _createdValueType = new ValueTypeArtifact(parameters.ValueTypeName);
            _parentContainer.AddValueType(_createdValueType);

            return OperationResult.Ok($"Value type '{parameters.ValueTypeName}' added to domain '{parameters.DomainName}'.");
        }

        public void Undo()
        {
            if (_createdValueType != null && _parentContainer != null)
                _parentContainer.RemoveValueType(_createdValueType);
        }

        public void Redo()
        {
            if (_createdValueType != null && _parentContainer != null)
                _parentContainer.AddValueType(_createdValueType);
        }
    }
}
