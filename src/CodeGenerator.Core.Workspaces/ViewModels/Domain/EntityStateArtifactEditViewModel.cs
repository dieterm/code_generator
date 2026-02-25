using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class EntityStateArtifactEditViewModel : ArtifactEditViewModel<EntityStateArtifact, EntityStateGeneralTabViewModel>
    {
        public EntityStateArtifactEditViewModel()
            : base("Entity State", new EntityStateGeneralTabViewModel())
        {
        }
    }

    public class EntityStateGeneralTabViewModel : ArtifactEditViewTabModel<EntityStateArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }

        public EntityStateGeneralTabViewModel() : base("General")
        {
            // Name field
            NameField = new SingleLineTextFieldModel
            {
                Label = "State Name",
                Name = nameof(EntityStateArtifact.Name),
                Tooltip = "Name of the entity state",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);
        }
    }
}
