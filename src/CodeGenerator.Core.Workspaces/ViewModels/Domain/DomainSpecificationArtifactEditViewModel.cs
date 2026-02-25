using CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class DomainSpecificationArtifactEditViewModel : ArtifactEditViewModel<DomainSpecificationArtifact, DomainSpecificationGeneralTabViewModel>
    {
        public DomainSpecificationArtifactEditViewModel()
            : base("Domain Specification", new DomainSpecificationGeneralTabViewModel())
        {
        }
    }

    public class DomainSpecificationGeneralTabViewModel : ArtifactEditViewTabModel<DomainSpecificationArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public SingleLineTextFieldModel DescriptionField { get; }
        public SingleLineTextFieldModel CriteriaField { get; }
        public SingleLineTextFieldModel CategoryField { get; }
        public BooleanFieldModel IsCompositeField { get; }
        public BooleanFieldModel IsReusableField { get; }

        public DomainSpecificationGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Specification Name",
                Name = nameof(DomainSpecificationArtifact.Name),
                Tooltip = "Name of the specification",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            DescriptionField = new SingleLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(DomainSpecificationArtifact.Description),
                Tooltip = "Description of the specification",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);

            CriteriaField = new SingleLineTextFieldModel
            {
                Label = "Criteria",
                Name = nameof(DomainSpecificationArtifact.Criteria),
                Tooltip = "Expression or criteria for the specification",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(CriteriaField);

            CategoryField = new SingleLineTextFieldModel
            {
                Label = "Category",
                Name = nameof(DomainSpecificationArtifact.Category),
                Tooltip = "Category of the specification (e.g., Business Rule, Query, Validation)",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(CategoryField);

            IsCompositeField = new BooleanFieldModel
            {
                Label = "Is Composite",
                Name = nameof(DomainSpecificationArtifact.IsComposite),
                Tooltip = "Indicates if this is a composite specification",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsCompositeField);

            IsReusableField = new BooleanFieldModel
            {
                Label = "Is Reusable",
                Name = nameof(DomainSpecificationArtifact.IsReusable),
                Tooltip = "Indicates if this specification is reusable across multiple contexts",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsReusableField);
        }
    }
}
