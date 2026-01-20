using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// View for editing foreign key properties
    /// </summary>
    public partial class ForeignKeyEditView : UserControl, IView<ForeignKeyEditViewModel>
    {
        private ForeignKeyEditViewModel? _viewModel;

        public ForeignKeyEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(ForeignKeyEditViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ColumnMappings.CollectionChanged -= ColumnMappings_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Bind fields
            txtName.BindViewModel(_viewModel.NameField);
            cmbReferencedTable.BindViewModel(_viewModel.ReferencedTableField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.ColumnMappings.CollectionChanged += ColumnMappings_CollectionChanged;

            RefreshColumnMappingsGrid();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ForeignKeyEditViewModel.ColumnMappings))
            {
                RefreshColumnMappingsGrid();
            }
        }

        private void ColumnMappings_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshColumnMappingsGrid();
        }
        private bool _isRefreshingColumnMapptingsGrid = false;
        private void RefreshColumnMappingsGrid()
        {
            if (_viewModel == null) return;
            if(_isRefreshingColumnMapptingsGrid) return;
            _isRefreshingColumnMapptingsGrid = true;
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
            }
            _isRefreshingColumnMapptingsGrid = false;
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
            if (_isRefreshingColumnMapptingsGrid) return;
            var row = dgvColumnMappings.Rows[e.RowIndex];
            if (row.Tag is not ForeignKeyColumnMappingViewModel mapping) return;

            var sourceColumnId = row.Cells[colSourceColumn.Index].Value?.ToString();
            var referencedColumnId = row.Cells[colReferencedColumn.Index].Value?.ToString();

            _viewModel.UpdateColumnMapping(mapping, sourceColumnId, referencedColumnId);
        }

        private void dgvColumnMappings_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Commit the edit immediately when a combobox value is selected
            if (dgvColumnMappings.IsCurrentCellDirty)
            {
                dgvColumnMappings.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ForeignKeyEditViewModel)(object)viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    _viewModel.ColumnMappings.CollectionChanged -= ColumnMappings_CollectionChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
