using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.Generic;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    /// <summary>
    /// ViewModel for editing workspace properties
    /// </summary>
    public class WorkspaceEditViewModel : ViewModelBase
    {
        private WorkspaceArtifact? _workspace;

        public WorkspaceEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Workspace Name", Name = nameof(WorkspaceArtifact.Name) };
            RootNamespaceField = new SingleLineTextFieldModel { Label = "Root Namespace", Name = nameof(WorkspaceArtifact.RootNamespace) };
            OutputDirectoryField = new FolderFieldModel { Label = "Default Output Directory", Name = nameof(WorkspaceArtifact.OutputDirectory) };
            DefaultTargetFrameworkField = new ComboboxFieldModel { Label = "Target Framework", Name = nameof(WorkspaceArtifact.DefaultTargetFramework) };
            DefaultLanguageField = new ComboboxFieldModel { Label = "Default Language", Name = nameof(WorkspaceArtifact.DefaultLanguage) };
            CodeArchitectureField = new ComboboxFieldModel { Label = "Code Architecture", Name = nameof(WorkspaceArtifact.CodeArchitectureId) };
            DependencyInjectionFrameworkField = new ComboboxFieldModel { Label = "Dependency Injection Framework", Name = nameof(WorkspaceArtifact.DependencyInjectionFrameworkId) };

            // Set up target framework options
            DefaultTargetFrameworkField.Items = TargetFrameworks.AllFrameworks.Select((f) => new ComboboxItem { DisplayName = f.Name, Value = f.Id }).ToList();
            
            // Set up language options
            DefaultLanguageField.Items = DotNetLanguages.AllLanguages.Select((lang) => new ComboboxItem { 
                DisplayName = lang.Name, 
                Value = lang.Id 
            }).ToList();

            // Set up code architecture options
            var architectureManager = ServiceProviderHolder.GetRequiredService<CodeArchitectureManager>();
            var allArchitectures = architectureManager.GetAllArchitectures();
            CodeArchitectureField.Items = allArchitectures.Select(a => new ComboboxItem { DisplayName = a.Name, Value = a.Id }).ToList();

            // Set up dependency injection framework options
            var diFrameworkManager = ServiceProviderHolder.GetRequiredService<DependancyInjectionFrameworkManager>();
            var allFrameworks = diFrameworkManager.Frameworks;
            DependencyInjectionFrameworkField.Items = allFrameworks.Select(f => new ComboboxItem { DisplayName = f.Name, Value = f.Id }).ToList();

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            RootNamespaceField.PropertyChanged += OnFieldChanged;
            OutputDirectoryField.PropertyChanged += OnFieldChanged;
            DefaultTargetFrameworkField.PropertyChanged += OnFieldChanged;
            DefaultLanguageField.PropertyChanged += OnFieldChanged;
            CodeArchitectureField.PropertyChanged += OnFieldChanged;
            DependencyInjectionFrameworkField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The workspace being edited
        /// </summary>
        public WorkspaceArtifact? Workspace
        {
            get => _workspace;
            set
            {
                if(_workspace!=null)
                {
                    // remove old listener
                    _workspace.PropertyChanged -= Workspace_PropertyChanged;
                }
                if (SetProperty(ref _workspace, value))
                {
                    LoadFromWorkspace();
                    if(_workspace!=null)
                        // add new listener
                        _workspace.PropertyChanged += Workspace_PropertyChanged;
                }
            }
        }

        private void Workspace_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Handle treeview node edit changes if needed
            if (e.PropertyName == nameof(_workspace.Name))
            {
                // Example: react to name changes
                NameField.Value = _workspace.Name;
            }
        }

        /// <summary>
        /// Workspace name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Root namespace field
        /// </summary>
        public SingleLineTextFieldModel RootNamespaceField { get; }

        /// <summary>
        /// Default output directory field
        /// </summary>
        public FolderFieldModel OutputDirectoryField { get; }

        /// <summary>
        /// Default target framework field
        /// </summary>
        public ComboboxFieldModel DefaultTargetFrameworkField { get; }

        /// <summary>
        /// Default language field
        /// </summary>
        public ComboboxFieldModel DefaultLanguageField { get; }

        /// <summary>
        /// Code architecture field
        /// </summary>
        public ComboboxFieldModel CodeArchitectureField { get; }

        /// <summary>
        /// Dependency injection framework field
        /// </summary>
        public ComboboxFieldModel DependencyInjectionFrameworkField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private bool _isLoading;

        private void LoadFromWorkspace()
        {
            if (_workspace == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _workspace.Name;
                RootNamespaceField.Value = _workspace.RootNamespace;
                OutputDirectoryField.Value = _workspace.OutputDirectory;
                DefaultTargetFrameworkField.Value = _workspace.DefaultTargetFramework;
                DefaultLanguageField.Value = _workspace.DefaultLanguage;
                CodeArchitectureField.Value = _workspace.CodeArchitectureId;
                DependencyInjectionFrameworkField.Value = _workspace.DependencyInjectionFrameworkId;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isLoading || _workspace == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                // Update workspace from fields
                SaveToWorkspace();
                
                var newValue = GetWorkspacePropertyValue(field.Name);
                
                // Raise event with property name - use the controller's event args type
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_workspace, field.Name, newValue));
            }
        }

        private object? GetWorkspacePropertyValue(string propertyName)
        {
            if (_workspace == null) return null;
            return _workspace.GetValue<object?>(propertyName);
        }

        private void SaveToWorkspace()
        {
            if (_workspace == null) return;

            _workspace.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Workspace";
            _workspace.RootNamespace = RootNamespaceField.Value?.ToString() ?? "MyCompany.MyProduct";
            _workspace.OutputDirectory = OutputDirectoryField.Value?.ToString() ?? string.Empty;
            _workspace.DefaultTargetFramework = DefaultTargetFrameworkField.Value?.ToString() ?? "net8_0";
            _workspace.DefaultLanguage = DefaultLanguageField.Value?.ToString() ?? "csharp";
            _workspace.CodeArchitectureId = CodeArchitectureField.Value?.ToString();
            _workspace.DependencyInjectionFrameworkId = DependencyInjectionFrameworkField.Value?.ToString();
        }
    }
}
