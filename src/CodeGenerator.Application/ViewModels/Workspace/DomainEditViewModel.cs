using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    /// <summary>
    /// ViewModel for editing domain properties
    /// </summary>
    public class DomainEditViewModel : ViewModelBase
    {
        public const string DEFAULT_NAMESPACE_PARAMETER_WORKSPACE_ROOT_NAMESPACE = "WorkspaceRootNamespace";
        public const string DEFAULT_NAMESPACE_PARAMETER_DOMAIN_NAME = "DomainName";
        private DomainArtifact? _domain;
        private WorkspaceArtifact? _currentWorkspace;
        private bool _isLoading;

        public DomainEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Domain Name", Name = nameof(DomainArtifact.Name) };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(DomainArtifact.Description) };
            DefaultNamespaceField = new ParameterizedStringFieldModel
            {
                Label = "Default Namespace",
                Name = nameof(DomainArtifact.DefaultNamespace)
            };

            // Add workspace root namespace parameter
            var workspaceRootNamespace = GetWorkspaceRootNamespace();
            DefaultNamespaceField.AddParameter(new ParameterizedStringParameter
            {
                Parameter = DEFAULT_NAMESPACE_PARAMETER_WORKSPACE_ROOT_NAMESPACE,
                ExampleValue = workspaceRootNamespace
            });

            DefaultNamespaceField.AddParameter(new ParameterizedStringParameter
            {
                Parameter = DEFAULT_NAMESPACE_PARAMETER_DOMAIN_NAME,
                ExampleValue = string.Empty
            });

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            DefaultNamespaceField.PropertyChanged += OnFieldChanged;

            // Observe Workspace.RootNamespace property for changes
            var workspaceTreeViewController = ServiceProviderHolder.GetRequiredService<WorkspaceTreeViewController>();
            workspaceTreeViewController.WorkspaceChanged += WorkspaceTreeViewController_WorkspaceChanged;
            WorkspaceTreeViewController_WorkspaceChanged(null, workspaceTreeViewController.CurrentWorkspace);
        }

        private void WorkspaceTreeViewController_WorkspaceChanged(object? sender, WorkspaceArtifact? newWorkspace)
        {
            if (_currentWorkspace != null)
            {
                _currentWorkspace.PropertyChanged -= CurrentWorkspace_PropertyChanged;
            }
            _currentWorkspace = newWorkspace;
            if (_currentWorkspace != null)
            {
                _currentWorkspace.PropertyChanged += CurrentWorkspace_PropertyChanged;
            }
        }

        private void CurrentWorkspace_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName== nameof(WorkspaceArtifact.RootNamespace))
            {
                UpdateWorkspaceRootNamespaceParameter();
            }
        }

        private string GetWorkspaceRootNamespace()
        {
            try
            {
                var workspaceContext = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
                return workspaceContext.CurrentWorkspace?.RootNamespace ?? "MyCompany.MyProduct";
            }
            catch
            {
                return "MyCompany.MyProduct";
            }
        }

        /// <summary>
        /// The domain being edited
        /// </summary>
        public DomainArtifact? Domain
        {
            get => _domain;
            set
            {
                if (_domain != null)
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
            if (e.PropertyName == nameof(DomainArtifact.Name))
            {
                NameField.Value = _domain?.Name;
                UpdateDomainNameParameter();
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
        public ParameterizedStringFieldModel DefaultNamespaceField { get; }

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
                DefaultNamespaceField.Value = _domain.DefaultNamespace;

                // Refresh the domain name parameter
                UpdateDomainNameParameter();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void UpdateWorkspaceRootNamespaceParameter()
        {
            var workspaceRootNamespace = GetWorkspaceRootNamespace();
            var parameter = DefaultNamespaceField.Parameters.FirstOrDefault(p => p.Parameter == DEFAULT_NAMESPACE_PARAMETER_WORKSPACE_ROOT_NAMESPACE);
            if (parameter != null)
            {
                parameter.ExampleValue = workspaceRootNamespace;
                DefaultNamespaceField.RefreshParameterizedString();
            }
        }

        private void UpdateDomainNameParameter()
        {
            var parameter = DefaultNamespaceField.Parameters.FirstOrDefault(p => p.Parameter == DEFAULT_NAMESPACE_PARAMETER_DOMAIN_NAME);
            if (parameter != null)
            {
                parameter.ExampleValue = _domain?.Name ?? string.Empty;
                DefaultNamespaceField.RefreshParameterizedString();
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

            //if(e.PropertyName == nameof(ParameterizedStringFieldModel.Value) && sender == NameField)
            //{
            //    UpdateDomainNameParameter();
            //}
        }

        private object? GetDomainPropertyValue(string propertyName)
        {
            if (_domain == null) return null;
            return propertyName switch
            {
                nameof(DomainArtifact.Name) => _domain.Name,
                nameof(DomainArtifact.Description) => _domain.Description,
                nameof(DomainArtifact.DefaultNamespace) => _domain.DefaultNamespace,
                _ => null
            };
        }

        private void SaveToDomain()
        {
            if (_domain == null) return;

            _domain.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Domain";
            _domain.Description = DescriptionField.Value?.ToString() ?? string.Empty;
            _domain.DefaultNamespace = DefaultNamespaceField.Value?.ToString() ?? string.Empty;
        }
    }
}
