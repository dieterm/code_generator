using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.ViewModels
{
    /// <summary>
    /// ViewModel for editing domain properties
    /// </summary>
    public class DomainEditViewModel : ViewModelBase
    {
        private DomainArtifact? _domain;
        private bool _isLoading;

        public DomainEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Domain Name", Name = nameof(DomainArtifact.Name) };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(DomainArtifact.Description) };
            NamespaceField = new ParameterizedStringFieldModel
            {
                Label = "Default Namespace",
                Name = nameof(DomainArtifact.NamespacePattern)
            };

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            NamespaceField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The domain being edited
        /// </summary>
        public DomainArtifact? Domain
        {
            get => _domain;
            set
            {
                if (_domain != null && _domain != value)
                {
                    _domain.PropertyChanged -= Domain_PropertyChanged;
                }

                if (SetProperty(ref _domain, value))
                {
                    LoadFromDomain();
                    if (_domain != null)
                    {
                        _domain.PropertyChanged += Domain_PropertyChanged;
                    }
                }
            }
        }

        private void Domain_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var domain = sender as DomainArtifact;
            if (e.PropertyName == nameof(DomainArtifact.Name))
            {
                NameField.Value = domain!.Name;
            }
            else if(e.PropertyName == nameof(DomainArtifact.Description))
            {
                DescriptionField.Value = domain!.Description;
            }
            else if (e.PropertyName == nameof(DomainArtifact.NamespacePattern))
            {
                NamespaceField.Value = domain!.NamespacePattern;
            }
            else if (e.PropertyName == nameof(DomainArtifact.Context))
            {
                UpdateNamespaceParameters();
            }
        }

        /// <summary>
        /// Domain name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Description field
        /// </summary>
        public SingleLineTextFieldModel DescriptionField { get; }

        /// <summary>
        /// Default namespace field with parameter support
        /// </summary>
        public ParameterizedStringFieldModel NamespaceField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromDomain()
        {
            if (_domain == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _domain.Name;
                DescriptionField.Value = _domain.Description;
                
                NamespaceField.Parameters.Clear();
                UpdateNamespaceParameters();
               
                NamespaceField.Value = _domain.NamespacePattern;
                
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void UpdateNamespaceParameters()
        {
            if(_domain==null) return;
            foreach (var (paramName, paramValue) in _domain.Context!.NamespaceParameters)
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
        }
        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _domain == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToDomain();
                
                var newValue = GetDomainPropertyValue(field.Name);
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_domain, field.Name, newValue));
            }
        }

        private object? GetDomainPropertyValue(string propertyName)
        {
            if (_domain == null) return null;
            return propertyName switch
            {
                nameof(DomainArtifact.Name) => _domain.Name,
                nameof(DomainArtifact.Description) => _domain.Description,
                nameof(DomainArtifact.NamespacePattern) => _domain.NamespacePattern,
                _ => null
            };
        }

        private void SaveToDomain()
        {
            if (_domain == null) return;

            _domain.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Domain";
            _domain.Description = DescriptionField.Value?.ToString() ?? string.Empty;
            _domain.NamespacePattern = NamespaceField.Value?.ToString() ?? string.Empty;
        }

        public override void DisposeViewModel()
        {
            if(_domain != null)
            {
                _domain.PropertyChanged -= Domain_PropertyChanged;
            }
            base.DisposeViewModel();
        }
    }
}
