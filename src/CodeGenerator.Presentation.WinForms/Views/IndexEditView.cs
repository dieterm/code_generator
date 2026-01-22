using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// View for editing index properties
    /// </summary>
    public partial class IndexEditView : UserControl, IView<IndexEditViewModel>
    {
        private IndexEditViewModel? _viewModel;

        public IndexEditView()
        {
            InitializeComponent();

            btnAddColumn.Click += BtnAddColumn_Click;
            btnRemoveColumn.Click += BtnRemoveColumn_Click;
            btnMoveUp.Click += BtnMoveUp_Click;
            btnMoveDown.Click += BtnMoveDown_Click;
            lstAvailableColumns.SelectedIndexChanged += LstAvailableColumns_SelectedIndexChanged;
            lstSelectedColumns.SelectedIndexChanged += LstSelectedColumns_SelectedIndexChanged;
        }

        public void BindViewModel(IndexEditViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.AvailableColumns.CollectionChanged -= AvailableColumns_CollectionChanged;
                _viewModel.SelectedColumns.CollectionChanged -= SelectedColumns_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Bind fields
            txtName.BindViewModel(_viewModel.NameField);
            chkUnique.BindViewModel(_viewModel.IsUniqueField);
            chkClustered.BindViewModel(_viewModel.IsClusteredField);

            // Refresh column lists
            RefreshAvailableColumns();
            RefreshSelectedColumns();

            // Subscribe to events
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.AvailableColumns.CollectionChanged += AvailableColumns_CollectionChanged;
            _viewModel.SelectedColumns.CollectionChanged += SelectedColumns_CollectionChanged;

            UpdateButtonStates();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IndexEditViewModel.SelectedAvailableColumn) ||
                e.PropertyName == nameof(IndexEditViewModel.SelectedIndexColumn))
            {
                UpdateButtonStates();
            }
        }

        private void AvailableColumns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => AvailableColumns_CollectionChanged(sender, e));
                return;
            }
            RefreshAvailableColumns();
        }

        private void SelectedColumns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => SelectedColumns_CollectionChanged(sender, e));
                return;
            }
            RefreshSelectedColumns();
        }

        private void RefreshAvailableColumns()
        {
            lstAvailableColumns.Items.Clear();
            if (_viewModel == null) return;

            foreach (var col in _viewModel.AvailableColumns)
            {
                lstAvailableColumns.Items.Add(new ListViewItem(col.ColumnName)
                {
                    Tag = col,
                    SubItems = { col.DataType }
                });
            }
        }

        private void RefreshSelectedColumns()
        {
            lstSelectedColumns.Items.Clear();
            if (_viewModel == null) return;

            foreach (var col in _viewModel.SelectedColumns)
            {
                lstSelectedColumns.Items.Add(new ListViewItem(col.ColumnName)
                {
                    Tag = col,
                    SubItems = { col.DataType }
                });
            }
        }

        private void LstAvailableColumns_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.SelectedAvailableColumn = lstAvailableColumns.SelectedItems.Count > 0
                ? lstAvailableColumns.SelectedItems[0].Tag as IndexColumnViewModel
                : null;

            UpdateButtonStates();
        }

        private void LstSelectedColumns_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.SelectedIndexColumn = lstSelectedColumns.SelectedItems.Count > 0
                ? lstSelectedColumns.SelectedItems[0].Tag as IndexColumnViewModel
                : null;

            UpdateButtonStates();
        }

        private void BtnAddColumn_Click(object? sender, EventArgs e)
        {
            _viewModel?.AddColumn();
        }

        private void BtnRemoveColumn_Click(object? sender, EventArgs e)
        {
            _viewModel?.RemoveColumn();
        }

        private void BtnMoveUp_Click(object? sender, EventArgs e)
        {
            _viewModel?.MoveColumnUp();
        }

        private void BtnMoveDown_Click(object? sender, EventArgs e)
        {
            _viewModel?.MoveColumnDown();
        }

        private void UpdateButtonStates()
        {
            btnAddColumn.Enabled = _viewModel?.SelectedAvailableColumn != null;
            btnRemoveColumn.Enabled = _viewModel?.SelectedIndexColumn != null;

            var selectedIndex = lstSelectedColumns.SelectedItems.Count > 0
                ? lstSelectedColumns.SelectedItems[0].Index
                : -1;

            btnMoveUp.Enabled = selectedIndex > 0;
            btnMoveDown.Enabled = selectedIndex >= 0 && selectedIndex < lstSelectedColumns.Items.Count - 1;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((IndexEditViewModel)(object)viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    _viewModel.AvailableColumns.CollectionChanged -= AvailableColumns_CollectionChanged;
                    _viewModel.SelectedColumns.CollectionChanged -= SelectedColumns_CollectionChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
