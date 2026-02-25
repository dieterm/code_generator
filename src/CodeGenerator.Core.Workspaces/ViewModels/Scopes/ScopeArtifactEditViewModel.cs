using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Shared.Models;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Scopes
{
    public class ScopeArtifactEditViewModel : ArtifactEditViewModel<ScopeArtifact, ScopeGeneralTabViewModel>
    {
        public ScopeArtifactEditViewModel()
            : base("Scope", new ScopeGeneralTabViewModel())
        {

        }
    }

    public class ScopeGeneralTabViewModel : ArtifactEditViewTabModel<ScopeArtifact>
    {
        public LabelFieldModel FullNameField { get; }
        public SingleLineTextFieldModel NameField { get; }
        public ParameterizedStringFieldModel NamespaceField { get; }

        public ScopeGeneralTabViewModel() : base("General")
        {
            // Full Name field (readonly)
            FullNameField = new LabelFieldModel
            {
                Label = "Full Name",
                Name = nameof(ScopeArtifact.FullName),
                Tooltip = "Full name of the scope including parent scopes",
                AutoBind = true,
                AutoUpdate = false
            };
            FieldCollection.FieldModels.Add(FullNameField);

            // Name field
            NameField = new SingleLineTextFieldModel
            {
                Label = "Scope Name",
                Name = nameof(ScopeArtifact.Name),
                Tooltip = "Name of the scope",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            // Namespace Pattern field with parameter support
            NamespaceField = new ParameterizedStringFieldModel
            {
                Label = "Namespace Pattern",
                Name = nameof(ScopeArtifact.NamespacePattern),
                Tooltip = "Pattern for generating the scope namespace (supports parameters)",
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

        protected override void OnArtifactPropertyChanged(ScopeArtifact artifact, string propertyName)
        {
            if (propertyName == nameof(ScopeArtifact.Context))
            {
                NamespaceField.SetParameters(Artifact?.Context?.NamespaceParameters);
            }
            else if (propertyName == nameof(ScopeArtifact.FullName))
            {
                FullNameField.Value = artifact.FullName;
            }
            else if (propertyName == nameof(ScopeArtifact.Name))
            {
                FullNameField.Value = artifact.FullName;
            }
        }

    }
}
