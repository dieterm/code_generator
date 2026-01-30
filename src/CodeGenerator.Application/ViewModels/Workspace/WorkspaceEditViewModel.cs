using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Domain.DotNet;
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
            DefaultOutputDirectoryField = new FolderFieldModel { Label = "Default Output Directory", Name = nameof(WorkspaceArtifact.DefaultOutputDirectory) };
            DefaultTargetFrameworkField = new ComboboxFieldModel { Label = "Target Framework", Name = nameof(WorkspaceArtifact.DefaultTargetFramework) };
            DefaultLanguageField = new ComboboxFieldModel { Label = "Default Language", Name = nameof(WorkspaceArtifact.DefaultLanguage) };
            // Set up target framework options
            DefaultTargetFrameworkField.Items = TargetFrameworks.AllFrameworks.Select((f) => new ComboboxItem { DisplayName = f.Name, Value = f.Id }).ToList();
            // Set up language options
            DefaultLanguageField.Items = DotNetLanguages.AllLanguages.Select((lang) => new ComboboxItem { 
                DisplayName = lang.DotNetCommandLineArgument, 
                Value = lang.DotNetCommandLineArgument 
            }).ToList();

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            RootNamespaceField.PropertyChanged += OnFieldChanged;
            DefaultOutputDirectoryField.PropertyChanged += OnFieldChanged;
            DefaultTargetFrameworkField.PropertyChanged += OnFieldChanged;
            DefaultLanguageField.PropertyChanged += OnFieldChanged;
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
        public FolderFieldModel DefaultOutputDirectoryField { get; }

        /// <summary>
        /// Default target framework field
        /// </summary>
        public ComboboxFieldModel DefaultTargetFrameworkField { get; }

        /// <summary>
        /// Default language field
        /// </summary>
        public ComboboxFieldModel DefaultLanguageField { get; }

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
                DefaultOutputDirectoryField.Value = _workspace.DefaultOutputDirectory;
                DefaultTargetFrameworkField.Value = _workspace.DefaultTargetFramework;
                DefaultLanguageField.Value = _workspace.DefaultLanguage;
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
            //return propertyName switch
            //{
            //    "Name" => _workspace.Name,
            //    "RootNamespace" => _workspace.RootNamespace,
            //    "DefaultOutputDirectory" => _workspace.DefaultOutputDirectory,
            //    "DefaultTargetFramework" => _workspace.DefaultTargetFramework,
            //    "DefaultLanguage" => _workspace.DefaultLanguage,
            //    _ => null
            //};
        }

        private void SaveToWorkspace()
        {
            if (_workspace == null) return;

            _workspace.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Workspace";
            _workspace.RootNamespace = RootNamespaceField.Value?.ToString() ?? "MyCompany.MyProduct";
            _workspace.DefaultOutputDirectory = DefaultOutputDirectoryField.Value?.ToString() ?? string.Empty;
            _workspace.DefaultTargetFramework = DefaultTargetFrameworkField.Value?.ToString() ?? "net8.0";
            _workspace.DefaultLanguage = DefaultLanguageField.Value?.ToString() ?? "C#";
        }
    }
}
