using CodeGenerator.Core.Workspaces.Artifacts.Domains.Factories;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class DomainFactoryArtifactEditViewModel : ArtifactEditViewModel<DomainFactoryArtifact, DomainFactoryGeneralTabViewModel>
    {
        public DomainFactoryArtifactEditViewModel()
            : base("Domain Factory", new DomainFactoryGeneralTabViewModel())
        {
        }
    }

    public class DomainFactoryGeneralTabViewModel : ArtifactEditViewTabModel<DomainFactoryArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public SingleLineTextFieldModel DescriptionField { get; }
        public SingleLineTextFieldModel CategoryField { get; }
        public BooleanFieldModel CreatesAggregatesField { get; }
        public BooleanFieldModel IsStatelessField { get; }
        public BooleanFieldModel HasDependenciesField { get; }

        public DomainFactoryGeneralTabViewModel() : base("General")
        {
            NameField = new SingleLineTextFieldModel
            {
                Label = "Factory Name",
                Name = nameof(DomainFactoryArtifact.Name),
                Tooltip = "Name of the factory",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            DescriptionField = new SingleLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(DomainFactoryArtifact.Description),
                Tooltip = "Description of the factory",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);

            CategoryField = new SingleLineTextFieldModel
            {
                Label = "Category",
                Name = nameof(DomainFactoryArtifact.Category),
                Tooltip = "Category of the factory (e.g., Entity Factory, Value Object Factory)",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(CategoryField);

            CreatesAggregatesField = new BooleanFieldModel
            {
                Label = "Creates Aggregates",
                Name = nameof(DomainFactoryArtifact.CreatesAggregates),
                Tooltip = "Indicates if this factory creates aggregates",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(CreatesAggregatesField);

            IsStatelessField = new BooleanFieldModel
            {
                Label = "Is Stateless",
                Name = nameof(DomainFactoryArtifact.IsStateless),
                Tooltip = "Indicates if this factory is stateless",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsStatelessField);

            HasDependenciesField = new BooleanFieldModel
            {
                Label = "Has Dependencies",
                Name = nameof(DomainFactoryArtifact.HasDependencies),
                Tooltip = "Indicates if this factory has dependencies",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(HasDependenciesField);
        }
    }
}
