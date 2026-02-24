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

    public class ScopeGeneralTabViewModel : ArtifactEditViewTabModel
    {
        public LabelFieldModel FullNameField { get; }
        public SingleLineTextFieldModel NameField { get; }
        public ParameterizedStringFieldModel NamespaceField { get; }

        private ScopeArtifact? _scope;

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
            // Unsubscribe from old scope
            if (_scope != null)
            {
                _scope.PropertyChanged -= Scope_PropertyChanged;
            }

            base.BindArtifact(artifactBase);

            _scope = artifactBase as ScopeArtifact;

            // Subscribe to scope property changes
            if (_scope != null)
            {
                UpdateNamespaceParameters();
                _scope.PropertyChanged += Scope_PropertyChanged;
            }
        }

        private void Scope_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScopeArtifact.Context))
            {
                UpdateNamespaceParameters();
            }
            else if (e.PropertyName == nameof(ScopeArtifact.FullName))
            {
                FullNameField.Value = _scope?.FullName;
            }
            else if (e.PropertyName == nameof(ScopeArtifact.Name))
            {
                FullNameField.Value = _scope?.FullName;
            }
        }

        private void UpdateNamespaceParameters()
        {
            if (_scope == null) return;

            var context = _scope.Context;
            if (context?.NamespaceParameters == null) return;

            NamespaceField.Parameters.Clear();
            
            foreach (var (paramName, paramValue) in context.NamespaceParameters)
            {
                NamespaceField.AddParameter(new ParameterizedStringParameter
                {
                    Parameter = paramName,
                    ExampleValue = paramValue
                });
            }
            
            NamespaceField.RefreshParameterizedString();
        }
    }
}
