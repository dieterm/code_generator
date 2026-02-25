using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Core.Workspaces.ViewModels.Scopes;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class DomainArtifactEditViewModel : ArtifactEditViewModel<DomainArtifact, DomainGeneralTabViewModel>
    {
        public DomainArtifactEditViewModel()
            : base("Domain", new DomainGeneralTabViewModel())
        {

        }
    }

    public class DomainGeneralTabViewModel : ArtifactEditViewTabModel<DomainArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public MultiLineTextFieldModel DescriptionField { get; }
        public ParameterizedStringFieldModel NamespaceField { get; }
        public DomainGeneralTabViewModel() : base("General")
        {
            // Name field
            NameField = new SingleLineTextFieldModel
            {
                Label = "Domain Name",
                Name = nameof(DomainArtifact.Name),
                Tooltip = "Name of the domain",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);
            // Description field
            DescriptionField = new MultiLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(DomainArtifact.Description),
                Tooltip = "Description of the domain",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);
            // Namespace Pattern field with parameter support
            NamespaceField = new ParameterizedStringFieldModel
            {
                Label = "Default Namespace Pattern",
                Name = nameof(DomainArtifact.NamespacePattern),
                Tooltip = "Pattern for generating the default namespace for artifacts in this domain (supports parameters)",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NamespaceField);
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            base.BindArtifact(artifactBase);

            NamespaceField.SetParameters(Artifact?.Context?.NamespaceParameters);
        }

        protected override void OnArtifactPropertyChanged(DomainArtifact artifact, string propertyName)
        {
            if (propertyName == nameof(DomainArtifact.Context))
            {
                NamespaceField.SetParameters(Artifact?.Context?.NamespaceParameters);
            }
        }
    }
}
