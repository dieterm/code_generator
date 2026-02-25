using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class ValueTypeArtifactEditViewModel : ArtifactEditViewModel<ValueTypeArtifact, ValueTypeGeneralTabViewModel>
    {
        public ValueTypeArtifactEditViewModel()
            : base("Value Type", new ValueTypeGeneralTabViewModel())
        {
        }
    }

    public class ValueTypeGeneralTabViewModel : ArtifactEditViewTabModel<ValueTypeArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public SingleLineTextFieldModel DescriptionField { get; }

        public ValueTypeGeneralTabViewModel() : base("General")
        {
            // Name field
            NameField = new SingleLineTextFieldModel
            {
                Label = "Value Type Name",
                Name = nameof(ValueTypeArtifact.Name),
                Tooltip = "Name of the value type",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            // Description field
            DescriptionField = new SingleLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(ValueTypeArtifact.Description),
                Tooltip = "Description of the value type",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);
        }
    }
}
