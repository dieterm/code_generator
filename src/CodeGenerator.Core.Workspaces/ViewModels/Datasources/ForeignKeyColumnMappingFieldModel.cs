using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    /// <summary>
    /// FieldModel for foreign key column mapping management: add/remove mappings, source/referenced column selection.
    /// </summary>
    public class ForeignKeyColumnMappingFieldModel : FieldViewModelBase
    {
        private ForeignKeyArtifact? _foreignKey;
        private TableArtifact? _parentTable;
        private DatasourceArtifact? _datasource;
        private bool _isLoading;

        public ForeignKeyColumnMappingFieldModel()
        {
            Label = "Column Mappings";
            Name = "ColumnMappings";
            ColumnMappings = new ObservableCollection<ForeignKeyColumnMappingViewModel>();
            AvailableSourceColumns = new ObservableCollection<ColumnItem>();
            AvailableReferencedColumns = new ObservableCollection<ColumnItem>();
        }

        public ForeignKeyArtifact? ForeignKey
        {
            get => _foreignKey;
            set
            {
                if (SetProperty(ref _foreignKey, value))
                {
                    if (_foreignKey != null)
                    {
                        _parentTable = _foreignKey.Parent as TableArtifact;
                        _datasource = _foreignKey.FindAncesterOfType<DatasourceArtifact>();
                    }
                    else
                    {
                        _parentTable = null;
                        _datasource = null;
                    }
                }
            }
        }

        public ObservableCollection<ForeignKeyColumnMappingViewModel> ColumnMappings { get; }
        public ObservableCollection<ColumnItem> AvailableSourceColumns { get; }
        public ObservableCollection<ColumnItem> AvailableReferencedColumns { get; }

        /// <summary>
        /// Event raised when column mappings change
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? MappingsChanged;

        /// <summary>
        /// Load available columns and current mappings for the referenced table.
        /// Called when the foreign key or the referenced table changes.
        /// </summary>
        public void LoadColumnsAndMappings()
        {
            LoadAvailableColumns();
            LoadColumnMappings();
        }

        private void LoadAvailableColumns()
        {
            AvailableSourceColumns.Clear();
            AvailableReferencedColumns.Clear();

            if (_parentTable != null)
            {
                foreach (var column in _parentTable.GetColumns().OrderBy(c => c.Name))
                {
                    AvailableSourceColumns.Add(new ColumnItem
                    {
                        Id = column.Id,
                        Name = column.Name,
                        DataType = column.DataType
                    });
                }
            }

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
                        AvailableReferencedColumns.Add(new ColumnItem
                        {
                            Id = column.Id,
                            Name = column.Name,
                            DataType = column.DataType
                        });
                    }
                }
            }
        }

        private void LoadColumnMappings()
        {
            _isLoading = true;
            try
            {
                foreach (var mapping in ColumnMappings)
                {
                    mapping.PropertyChanged -= ColumnMapping_PropertyChanged;
                }
                ColumnMappings.Clear();

                if (_foreignKey == null) return;

                foreach (var mapping in _foreignKey.ColumnMappings)
                {
                    var sourceColumn = AvailableSourceColumns.FirstOrDefault(c => c.Id == mapping.SourceColumnId);
                    var referencedColumn = AvailableReferencedColumns.FirstOrDefault(c => c.Id == mapping.ReferencedColumnId);

                    var vm = new ForeignKeyColumnMappingViewModel
                    {
                        SourceColumnId = mapping.SourceColumnId,
                        ReferencedColumnId = mapping.ReferencedColumnId,
                        SourceColumnName = sourceColumn?.Name ?? "(Unknown)",
                        ReferencedColumnName = referencedColumn?.Name ?? "(Unknown)",
                        SourceColumnDataType = sourceColumn?.DataType,
                        ReferencedColumnDataType = referencedColumn?.DataType
                    };
                    vm.PropertyChanged += ColumnMapping_PropertyChanged;
                    ColumnMappings.Add(vm);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void ColumnMapping_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _foreignKey == null) return;
            SaveColumnMappings();
        }

        private void SaveColumnMappings()
        {
            if (_foreignKey == null) return;

            var newMappings = new List<ForeignKeyColumnMapping>();
            foreach (var mapping in ColumnMappings)
            {
                if (!string.IsNullOrEmpty(mapping.SourceColumnId) && !string.IsNullOrEmpty(mapping.ReferencedColumnId))
                {
                    newMappings.Add(new ForeignKeyColumnMapping(mapping.SourceColumnId, mapping.ReferencedColumnId));
                }
            }
            _foreignKey.ColumnMappings = newMappings;
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
                MappingsChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_foreignKey, "ColumnMappings", null));
            }
        }

        /// <summary>
        /// Update a column mapping
        /// </summary>
        public void UpdateColumnMapping(ForeignKeyColumnMappingViewModel mapping, string? sourceColumnId, string? referencedColumnId)
        {
            var sourceColumn = AvailableSourceColumns.FirstOrDefault(c => c.Id == sourceColumnId);
            var referencedColumn = AvailableReferencedColumns.FirstOrDefault(c => c.Id == referencedColumnId);

            mapping.SourceColumnId = sourceColumnId ?? string.Empty;
            mapping.ReferencedColumnId = referencedColumnId ?? string.Empty;
            mapping.SourceColumnName = sourceColumn?.Name ?? "";
            mapping.ReferencedColumnName = referencedColumn?.Name ?? "";
            mapping.SourceColumnDataType = sourceColumn?.DataType;
            mapping.ReferencedColumnDataType = referencedColumn?.DataType;

            SaveColumnMappings();

            if (_foreignKey != null)
            {
                MappingsChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_foreignKey, "ColumnMappings", null));
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
        private string? _sourceColumnDataType;
        private string? _referencedColumnDataType;

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

        public string? SourceColumnDataType
        {
            get => _sourceColumnDataType;
            set => SetProperty(ref _sourceColumnDataType, value);
        }

        public string? ReferencedColumnDataType
        {
            get => _referencedColumnDataType;
            set => SetProperty(ref _referencedColumnDataType, value);
        }
    }

    /// <summary>
    /// Simple item for column selection
    /// </summary>
    public class ColumnItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DataType { get; set; }

        public override string ToString() => Name;
    }
}
