using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Datasources
{
    /// <summary>
    /// ViewModel for editing table properties
    /// </summary>
    public class TableEditViewModel : ViewModelBase
    {
        private TableArtifact? _table;
        private bool _isLoading;

        public TableEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Table Name", Name = "Name" };
            SchemaField = new SingleLineTextFieldModel { Label = "Schema", Name = "Schema" };

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            SchemaField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The table being edited
        /// </summary>
        public TableArtifact? Table
        {
            get => _table;
            set
            {
                if(_table != null)
                {
                    _table.PropertyChanged -= TableArtifact_PropertyChanged;
                }
                if (SetProperty(ref _table, value))
                {
                    LoadFromTable();
                    // listen for rename event, when table name changes outside this viewmodel
                    // (e.g., from the tree view editlabel action)
                    if(_table!=null)
                        _table.PropertyChanged += TableArtifact_PropertyChanged;
                }
            }
        }

        private void TableArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {   
            // listen for rename event, when table name changes outside this viewmodel
            // (e.g., from the tree view editlabel action)
            if (e.PropertyName == nameof(TableArtifact.Name))
            {
                NameField.Value = (sender as TableArtifact)?.Name;
            }
        }

        // Field ViewModels
        public SingleLineTextFieldModel NameField { get; }
        public SingleLineTextFieldModel SchemaField { get; }

        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromTable()
        {
            if (_table == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _table.Name;
                SchemaField.Value = _table.Schema;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _table == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToTable();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_table, field.Name, field.Value));
            }
        }

        private void SaveToTable()
        {
            if (_table == null) return;

            _table.Name = NameField.Value?.ToString() ?? "Table";
            _table.Schema = SchemaField.Value?.ToString() ?? "";
        }
    }
}
