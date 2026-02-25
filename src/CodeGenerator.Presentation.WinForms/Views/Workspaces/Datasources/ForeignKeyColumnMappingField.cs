using CodeGenerator.Core.Workspaces.ViewModels.Datasources;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// UserControl for foreign key column mapping management: DataGridView with add/remove buttons.
    /// </summary>
    public partial class ForeignKeyColumnMappingField : UserControl, IView<ForeignKeyColumnMappingFieldModel>
    {
        private ForeignKeyColumnMappingFieldModel? _viewModel;
        private static readonly Color DataTypeMismatchColor = Color.FromArgb(255, 200, 200);
        private static readonly Color DefaultRowColor = Color.White;
        private bool _isRefreshingGrid;

        public ForeignKeyColumnMappingField()
        {
            InitializeComponent();
        }

        public void BindViewModel(ForeignKeyColumnMappingFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                DataBindings.Clear();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ColumnMappings.CollectionChanged -= ColumnMappings_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            DataBindings.Add("Visible", viewModel, nameof(viewModel.Visible), false, DataSourceUpdateMode.OnPropertyChanged).ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.ColumnMappings.CollectionChanged += ColumnMappings_CollectionChanged;

            RefreshColumnMappingsGrid();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ForeignKeyColumnMappingFieldModel.ForeignKey))
            {
                RefreshColumnMappingsGrid();
            }
        }

        private void ColumnMappings_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => ColumnMappings_CollectionChanged(sender, e));
                return;
            }
            RefreshColumnMappingsGrid();
        }

        private void RefreshColumnMappingsGrid()
        {
            if (_viewModel == null) return;
            if (_isRefreshingGrid) return;
            _isRefreshingGrid = true;

            dgvColumnMappings.Rows.Clear();

            foreach (var mapping in _viewModel.ColumnMappings)
            {
                var rowIndex = dgvColumnMappings.Rows.Add();
                var row = dgvColumnMappings.Rows[rowIndex];
                row.Tag = mapping;

                // Source column combobox
                var sourceCell = (DataGridViewComboBoxCell)row.Cells[colSourceColumn.Index];
                sourceCell.DataSource = _viewModel.AvailableSourceColumns.ToList();
                sourceCell.DisplayMember = nameof(ColumnItem.Name);
                sourceCell.ValueMember = nameof(ColumnItem.Id);
                if (!string.IsNullOrEmpty(mapping.SourceColumnId))
                {
                    sourceCell.Value = mapping.SourceColumnId;
                }

                // Referenced column combobox
                var refCell = (DataGridViewComboBoxCell)row.Cells[colReferencedColumn.Index];
                refCell.DataSource = _viewModel.AvailableReferencedColumns.ToList();
                refCell.DisplayMember = nameof(ColumnItem.Name);
                refCell.ValueMember = nameof(ColumnItem.Id);
                if (!string.IsNullOrEmpty(mapping.ReferencedColumnId))
                {
                    refCell.Value = mapping.ReferencedColumnId;
                }

                // Update data type columns and row color
                UpdateDataTypeColumnsAndRowColor(row, mapping);
            }
            _isRefreshingGrid = false;
        }

        private void UpdateDataTypeColumnsAndRowColor(DataGridViewRow row, ForeignKeyColumnMappingViewModel mapping)
        {
            var sourceDataType = mapping.SourceColumnDataType ?? string.Empty;
            var referencedDataType = mapping.ReferencedColumnDataType ?? string.Empty;

            row.Cells[colSourceDataType.Index].Value = sourceDataType;
            row.Cells[colReferencedDataType.Index].Value = referencedDataType;

            bool hasDataTypeMismatch = !string.IsNullOrEmpty(sourceDataType) &&
                                       !string.IsNullOrEmpty(referencedDataType) &&
                                       !string.Equals(sourceDataType, referencedDataType, StringComparison.OrdinalIgnoreCase);

            row.DefaultCellStyle.BackColor = hasDataTypeMismatch ? DataTypeMismatchColor : DefaultRowColor;
        }

        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            _viewModel?.AddColumnMapping();
        }

        private void btnRemoveMapping_Click(object sender, EventArgs e)
        {
            if (_viewModel == null || dgvColumnMappings.SelectedRows.Count == 0) return;

            var selectedRow = dgvColumnMappings.SelectedRows[0];
            if (selectedRow.Tag is ForeignKeyColumnMappingViewModel mapping)
            {
                _viewModel.RemoveColumnMapping(mapping);
            }
        }

        private void dgvColumnMappings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_viewModel == null || e.RowIndex < 0) return;
            if (_isRefreshingGrid) return;

            var row = dgvColumnMappings.Rows[e.RowIndex];
            if (row.Tag is not ForeignKeyColumnMappingViewModel mapping) return;

            var sourceColumnId = row.Cells[colSourceColumn.Index].Value?.ToString();
            var referencedColumnId = row.Cells[colReferencedColumn.Index].Value?.ToString();

            _viewModel.UpdateColumnMapping(mapping, sourceColumnId, referencedColumnId);

            UpdateDataTypeColumnsAndRowColor(row, mapping);
        }

        private void dgvColumnMappings_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvColumnMappings.IsCurrentCellDirty)
            {
                dgvColumnMappings.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((ForeignKeyColumnMappingFieldModel)(object)viewModel);
        }
    }
}
