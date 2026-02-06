using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Datasources
{
    /// <summary>
    /// ViewModel for editing index properties
    /// </summary>
    public class IndexEditViewModel : ViewModelBase
    {
        private IndexArtifact? _index;
        private bool _isLoading;

        public IndexEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Index Name", Name = "Name" };
            IsUniqueField = new BooleanFieldModel { Label = "Unique", Name = "IsUnique" };
            IsClusteredField = new BooleanFieldModel { Label = "Clustered", Name = "IsClustered" };

            AvailableColumns = new ObservableCollection<IndexColumnViewModel>();
            SelectedColumns = new ObservableCollection<IndexColumnViewModel>();

            // Subscribe to field changes
            NameField.PropertyChanged += OnFieldChanged;
            IsUniqueField.PropertyChanged += OnFieldChanged;
            IsClusteredField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The index being edited
        /// </summary>
        public IndexArtifact? Index
        {
            get => _index;
            set
            {
                if (SetProperty(ref _index, value))
                {
                    LoadFromIndex();
                }
            }
        }

        // Field ViewModels
        public SingleLineTextFieldModel NameField { get; }
        public BooleanFieldModel IsUniqueField { get; }
        public BooleanFieldModel IsClusteredField { get; }

        /// <summary>
        /// Available columns from the parent table/view
        /// </summary>
        public ObservableCollection<IndexColumnViewModel> AvailableColumns { get; }

        /// <summary>
        /// Columns included in the index
        /// </summary>
        public ObservableCollection<IndexColumnViewModel> SelectedColumns { get; }

        /// <summary>
        /// Currently selected available column
        /// </summary>
        private IndexColumnViewModel? _selectedAvailableColumn;
        public IndexColumnViewModel? SelectedAvailableColumn
        {
            get => _selectedAvailableColumn;
            set => SetProperty(ref _selectedAvailableColumn, value);
        }

        /// <summary>
        /// Currently selected index column
        /// </summary>
        private IndexColumnViewModel? _selectedIndexColumn;
        public IndexColumnViewModel? SelectedIndexColumn
        {
            get => _selectedIndexColumn;
            set => SetProperty(ref _selectedIndexColumn, value);
        }

        /// <summary>
        /// Event raised when a property value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromIndex()
        {
            if (_index == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _index.Name;
                IsUniqueField.Value = _index.IsUnique;
                IsClusteredField.Value = _index.IsClustered;

                // Load columns
                LoadColumns();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadColumns()
        {
            AvailableColumns.Clear();
            SelectedColumns.Clear();

            if (_index == null) return;

            // Get the parent table/view
            var parent = _index.Parent;
            if (parent == null) return;

            var allColumns = parent.Children.OfType<ColumnArtifact>().ToList();
            var indexColumnNames = _index.ColumnNames ?? new List<string>();

            // Populate selected columns (in order)
            foreach (var columnName in indexColumnNames)
            {
                var column = allColumns.FirstOrDefault(c => c.Name == columnName);
                if (column != null)
                {
                    SelectedColumns.Add(new IndexColumnViewModel
                    {
                        ColumnName = column.Name,
                        DataType = column.DataType
                    });
                }
            }

            // Populate available columns (not yet selected)
            foreach (var column in allColumns)
            {
                if (!indexColumnNames.Contains(column.Name))
                {
                    AvailableColumns.Add(new IndexColumnViewModel
                    {
                        ColumnName = column.Name,
                        DataType = column.DataType
                    });
                }
            }
        }

        /// <summary>
        /// Add the selected available column to the index
        /// </summary>
        public void AddColumn()
        {
            if (SelectedAvailableColumn == null || _index == null) return;

            var column = SelectedAvailableColumn;
            AvailableColumns.Remove(column);
            SelectedColumns.Add(column);

            _index.AddColumn(column.ColumnName);
            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, "ColumnNames", _index.ColumnNames));
        }

        /// <summary>
        /// Remove the selected column from the index
        /// </summary>
        public void RemoveColumn()
        {
            if (SelectedIndexColumn == null || _index == null) return;

            var column = SelectedIndexColumn;
            SelectedColumns.Remove(column);
            AvailableColumns.Add(column);

            _index.RemoveColumn(column.ColumnName);
            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, "ColumnNames", _index.ColumnNames));
        }

        /// <summary>
        /// Move the selected column up in the index
        /// </summary>
        public void MoveColumnUp()
        {
            if (SelectedIndexColumn == null || _index == null) return;

            var index = SelectedColumns.IndexOf(SelectedIndexColumn);
            if (index > 0)
            {
                SelectedColumns.Move(index, index - 1);
                UpdateColumnOrder();
            }
        }

        /// <summary>
        /// Move the selected column down in the index
        /// </summary>
        public void MoveColumnDown()
        {
            if (SelectedIndexColumn == null || _index == null) return;

            var index = SelectedColumns.IndexOf(SelectedIndexColumn);
            if (index < SelectedColumns.Count - 1)
            {
                SelectedColumns.Move(index, index + 1);
                UpdateColumnOrder();
            }
        }

        private void UpdateColumnOrder()
        {
            if (_index == null) return;

            _index.ColumnNames.Clear();
            foreach (var col in SelectedColumns)
            {
                _index.ColumnNames.Add(col.ColumnName);
            }

            ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, "ColumnNames", _index.ColumnNames));
        }

        private void OnFieldChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isLoading || _index == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToIndex();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, field.Name, field.Value));
            }
        }

        private void SaveToIndex()
        {
            if (_index == null) return;

            _index.Name = NameField.Value?.ToString() ?? "Index";
            _index.IsUnique = IsUniqueField.Value is bool unique && unique;
            _index.IsClustered = IsClusteredField.Value is bool clustered && clustered;
        }
    }

    /// <summary>
    /// ViewModel for a column in the index editor
    /// </summary>
    public class IndexColumnViewModel : ViewModelBase
    {
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;

        public string DisplayText => $"{ColumnName} ({DataType})";
    }
}
