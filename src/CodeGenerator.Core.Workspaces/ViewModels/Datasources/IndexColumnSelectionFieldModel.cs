using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Shared.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Core.Workspaces.ViewModels.Datasources
{
    /// <summary>
    /// FieldModel for index column selection: available/selected columns, add/remove/move operations.
    /// </summary>
    public class IndexColumnSelectionFieldModel : FieldViewModelBase
    {
        private IndexArtifact? _index;

        public IndexColumnSelectionFieldModel()
        {
            Label = "Index Columns";
            Name = "IndexColumns";
            AvailableColumns = new ObservableCollection<IndexColumnViewModel>();
            SelectedColumns = new ObservableCollection<IndexColumnViewModel>();
        }

        public IndexArtifact? Index
        {
            get => _index;
            set
            {
                if (SetProperty(ref _index, value))
                {
                    LoadColumns();
                }
            }
        }

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
        /// Event raised when a column operation changes the index
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ColumnsChanged;

        private void LoadColumns()
        {
            AvailableColumns.Clear();
            SelectedColumns.Clear();

            if (_index == null) return;

            var parent = _index.Parent;
            if (parent == null) return;

            var allColumns = parent.Children.OfType<ColumnArtifact>().ToList();
            var indexColumnNames = _index.ColumnNames ?? new List<string>();

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
            ColumnsChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, "ColumnNames", _index.ColumnNames));
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
            ColumnsChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, "ColumnNames", _index.ColumnNames));
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

            ColumnsChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_index, "ColumnNames", _index.ColumnNames));
        }
    }
}
