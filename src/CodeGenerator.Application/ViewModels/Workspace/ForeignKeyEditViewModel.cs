using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace
{
    /// <summary>
    /// ViewModel for editing foreign key properties
    /// </summary>
    public class ForeignKeyEditViewModel : ViewModelBase
    {
        private ForeignKeyArtifact? _foreignKey;
        private bool _isLoading;
        private TableArtifact? _parentTable;
        private DatasourceArtifact? _datasource;

        public ForeignKeyEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Foreign Key Name", Name = "Name" };
            ReferencedTableField = new ComboboxFieldModel { Label = "Referenced Table", Name = "ReferencedTable" };
            ColumnMappings = new ObservableCollection<ForeignKeyColumnMappingViewModel>();
            AvailableSourceColumns = new ObservableCollection<ColumnItem>();
            AvailableReferencedColumns = new ObservableCollection<ColumnItem>();

            NameField.PropertyChanged += OnFieldChanged;
            ReferencedTableField.PropertyChanged += OnReferencedTableChanged;
        }

        /// <summary>
        /// The foreign key being edited
        /// </summary>
        public ForeignKeyArtifact? ForeignKey
        {
            get => _foreignKey;
            set
            {
                if (_foreignKey != null)
                {
                    _foreignKey.PropertyChanged -= ForeignKeyArtifact_PropertyChanged;
                }
                if (SetProperty(ref _foreignKey, value))
                {
                    if (_foreignKey != null)
                    {
                        _foreignKey.PropertyChanged += ForeignKeyArtifact_PropertyChanged;
                        _parentTable = _foreignKey.Parent as TableArtifact;
                        _datasource = _foreignKey.FindAncesterOfType<DatasourceArtifact>();
                    }
                    LoadAvailableTables();
                    LoadFromForeignKey();
                }
            }
        }

        private void ForeignKeyArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ForeignKeyArtifact.Name))
            {
                NameField.Value = (sender as ForeignKeyArtifact)?.Name;
            }
        }

        // Field ViewModels
        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel ReferencedTableField { get; }
        public ObservableCollection<ForeignKeyColumnMappingViewModel> ColumnMappings { get; }
        public ObservableCollection<ColumnItem> AvailableSourceColumns { get; }
        public ObservableCollection<ColumnItem> AvailableReferencedColumns { get; }

        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadAvailableTables()
        {
            ReferencedTableField.Items.Clear();

            if (_datasource == null) return;

            var tables = _datasource.GetAllDescendants()
                .OfType<TableArtifact>()
                .Where(t => t.Id != _parentTable?.Id) // Exclude the parent table
                .OrderBy(t => t.Name);

            foreach (var table in tables)
            {
                var item = new ComboboxItem { DisplayName = table.Name, Value = table.Id };
                ReferencedTableField.Items.Add(item);
            }
        }

        private void LoadFromForeignKey()
        {
            if (_foreignKey == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _foreignKey.Name;

                // Set selected referenced table
                var selectedTable = ReferencedTableField.Items
                    .FirstOrDefault(t => t.Value?.ToString() == _foreignKey.ReferencedTableId);
                ReferencedTableField.SelectedItem = selectedTable;

                LoadAvailableColumns();
                LoadColumnMappings();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadAvailableColumns()
        {
            AvailableSourceColumns.Clear();
            AvailableReferencedColumns.Clear();

            // Load source columns from parent table
            if (_parentTable != null)
            {
                foreach (var column in _parentTable.GetColumns().OrderBy(c => c.Name))
                {
                    AvailableSourceColumns.Add(new ColumnItem { Id = column.Id, Name = column.Name });
                }
            }

            // Load referenced columns from selected table
            var referencedTableId = _foreignKey?.ReferencedTableId;
            if (!string.IsNullOrEmpty(referencedTableId) && _datasource != null)
            {
                var referencedTable = _datasource.GetAllDescendants()
                    .OfType<TableArtifact>()
                    .FirstOrDefault(t => t.Id == referencedTableId);

                if (referencedTable != null)
                {
                    foreach (var column in referencedTable.GetColumns().OrderBy(c => c.Name))
                    {
                        AvailableReferencedColumns.Add(new ColumnItem { Id = column.Id, Name = column.Name });
                    }
                }
            }
        }

        private void LoadColumnMappings()
        {
            ColumnMappings.Clear();

            if (_foreignKey == null) return;

            foreach (var mapping in _foreignKey.ColumnMappings)
            {
                var vm = new ForeignKeyColumnMappingViewModel
                {
                    SourceColumnId = mapping.SourceColumnId,
                    ReferencedColumnId = mapping.ReferencedColumnId,
                    SourceColumnName = AvailableSourceColumns.FirstOrDefault(c => c.Id == mapping.SourceColumnId)?.Name ?? "(Unknown)",
                    ReferencedColumnName = AvailableReferencedColumns.FirstOrDefault(c => c.Id == mapping.ReferencedColumnId)?.Name ?? "(Unknown)"
                };
                vm.PropertyChanged += ColumnMapping_PropertyChanged;
                ColumnMappings.Add(vm);
            }
        }

        private void ColumnMapping_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _foreignKey == null) return;

            SaveColumnMappings();
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _foreignKey == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToForeignKey();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_foreignKey, field.Name, field.Value));
            }
        }

        private void OnReferencedTableChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _foreignKey == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                var selectedTableId = ReferencedTableField.SelectedItem?.Value?.ToString();
                _foreignKey.ReferencedTableId = selectedTableId;

                // Clear mappings when table changes - assign new empty list to trigger property setter
                _foreignKey.ColumnMappings = new List<ForeignKeyColumnMapping>();
                LoadAvailableColumns();
                LoadColumnMappings();

                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_foreignKey, "ReferencedTableId", selectedTableId));
            }
        }

        private void SaveToForeignKey()
        {
            if (_foreignKey == null) return;

            _foreignKey.Name = NameField.Value?.ToString() ?? "FK_New";
        }

        private void SaveColumnMappings()
        {
            if (_foreignKey == null) return;

            _foreignKey.ColumnMappings.Clear();
            foreach (var mapping in ColumnMappings)
            {
                if (!string.IsNullOrEmpty(mapping.SourceColumnId) && !string.IsNullOrEmpty(mapping.ReferencedColumnId))
                {
                    _foreignKey.ColumnMappings.Add(new ForeignKeyColumnMapping(mapping.SourceColumnId, mapping.ReferencedColumnId));
                }
            }
        }

        /// <summary>
        /// Add a new column mapping
        /// </summary>
        public void AddColumnMapping()
        {
            var vm = new ForeignKeyColumnMappingViewModel();
            vm.PropertyChanged += ColumnMapping_PropertyChanged;
            ColumnMappings.Add(vm);
        }

        /// <summary>
        /// Remove a column mapping
        /// </summary>
        public void RemoveColumnMapping(ForeignKeyColumnMappingViewModel mapping)
        {
            mapping.PropertyChanged -= ColumnMapping_PropertyChanged;
            ColumnMappings.Remove(mapping);
            SaveColumnMappings();

            if (_foreignKey != null)
            {
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_foreignKey, "ColumnMappings", null));
            }
        }

        /// <summary>
        /// Update a column mapping
        /// </summary>
        public void UpdateColumnMapping(ForeignKeyColumnMappingViewModel mapping, string? sourceColumnId, string? referencedColumnId)
        {
            mapping.SourceColumnId = sourceColumnId ?? string.Empty;
            mapping.ReferencedColumnId = referencedColumnId ?? string.Empty;
            mapping.SourceColumnName = AvailableSourceColumns.FirstOrDefault(c => c.Id == sourceColumnId)?.Name ?? "";
            mapping.ReferencedColumnName = AvailableReferencedColumns.FirstOrDefault(c => c.Id == referencedColumnId)?.Name ?? "";

            SaveColumnMappings();

            if (_foreignKey != null)
            {
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_foreignKey, "ColumnMappings", null));
            }
        }
    }

    /// <summary>
    /// ViewModel for a single column mapping in a foreign key
    /// </summary>
    public class ForeignKeyColumnMappingViewModel : ViewModelBase
    {
        private string _sourceColumnId = string.Empty;
        private string _referencedColumnId = string.Empty;
        private string _sourceColumnName = string.Empty;
        private string _referencedColumnName = string.Empty;

        public string SourceColumnId
        {
            get => _sourceColumnId;
            set => SetProperty(ref _sourceColumnId, value);
        }

        public string ReferencedColumnId
        {
            get => _referencedColumnId;
            set => SetProperty(ref _referencedColumnId, value);
        }

        public string SourceColumnName
        {
            get => _sourceColumnName;
            set => SetProperty(ref _sourceColumnName, value);
        }

        public string ReferencedColumnName
        {
            get => _referencedColumnName;
            set => SetProperty(ref _referencedColumnName, value);
        }
    }

    /// <summary>
    /// Simple item for column selection
    /// </summary>
    public class ColumnItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
