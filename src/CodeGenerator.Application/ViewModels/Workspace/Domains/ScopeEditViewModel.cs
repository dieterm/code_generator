using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing scope properties
    /// </summary>
    public class ScopeEditViewModel : ViewModelBase
    {
        private ScopeArtifact? _scope;
        private bool _isLoading;

        public ScopeEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Scope Name", Name = nameof(ScopeArtifact.Name) };
            NamespaceField = new ParameterizedStringFieldModel
            {
                Label = "Namespace Pattern",
                Name = nameof(ScopeArtifact.NamespacePattern)
            };

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            NamespaceField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The scope being edited
        /// </summary>
        public ScopeArtifact? Scope
        {
            get => _scope;
            set
            {
                if (_scope != null && _scope != value)
                {
                    _scope.PropertyChanged -= Scope_PropertyChanged;
                }

                if (SetProperty(ref _scope, value))
                {
                    LoadFromScope();
                    if (_scope != null)
                    {
                        _scope.PropertyChanged += Scope_PropertyChanged;
                    }
                }
            }
        }

        private void Scope_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var scope = sender as ScopeArtifact;
            if (e.PropertyName == nameof(ScopeArtifact.Name))
            {
                NameField.Value = scope!.Name;
            }
            else if (e.PropertyName == nameof(ScopeArtifact.NamespacePattern))
            {
                NamespaceField.Value = scope!.NamespacePattern;
            }
            else if (e.PropertyName == nameof(ScopeArtifact.Context))
            {
                UpdateNamespaceParameters();
            }
        }

        /// <summary>
        /// Scope name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Namespace pattern field with parameter support
        /// </summary>
        public ParameterizedStringFieldModel NamespaceField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromScope()
        {
            if (_scope == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _scope.Name;

                NamespaceField.Parameters.Clear();
                UpdateNamespaceParameters();

                NamespaceField.Value = _scope.NamespacePattern;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void UpdateNamespaceParameters()
        {
            if (_scope == null) return;

            var context = _scope.Context;
            if (context?.NamespaceParameters == null) return;

            foreach (var (paramName, paramValue) in context.NamespaceParameters)
            {
                var existingParameter = NamespaceField.Parameters.FirstOrDefault(p => p.Parameter == paramName);
                if (existingParameter != null)
                {
                    existingParameter.ExampleValue = paramValue;
                }
                else
                {
                    NamespaceField.AddParameter(new ParameterizedStringParameter
                    {
                        Parameter = paramName,
                        ExampleValue = paramValue
                    });
                }
            }
            NamespaceField.RefreshParameterizedString();
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _scope == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToScope();

                var newValue = GetScopePropertyValue(field.Name);
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_scope, field.Name, newValue));
            }
        }

        private object? GetScopePropertyValue(string propertyName)
        {
            if (_scope == null) return null;
            return propertyName switch
            {
                nameof(ScopeArtifact.Name) => _scope.Name,
                nameof(ScopeArtifact.NamespacePattern) => _scope.NamespacePattern,
                _ => null
            };
        }

        private void SaveToScope()
        {
            if (_scope == null) return;

            _scope.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Scope";
            _scope.NamespacePattern = NamespaceField.Value?.ToString() ?? string.Empty;
        }

        public override void DisposeViewModel()
        {
            if (_scope != null)
            {
                _scope.PropertyChanged -= Scope_PropertyChanged;
            }
            base.DisposeViewModel();
        }
    }
}
