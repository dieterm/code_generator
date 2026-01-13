using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Application.ViewModels
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
                if (SetProperty(ref _table, value))
                {
                    LoadFromTable();
                }
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

        private void OnFieldChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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
