using CodeGenerator.Core.Workspaces.ViewModels.Datasources;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// UserControl for index column selection: available/selected columns with add/remove/move operations.
    /// </summary>
    public partial class IndexColumnSelectionField : UserControl, IView<IndexColumnSelectionFieldModel>
    {
        private IndexColumnSelectionFieldModel? _viewModel;

        public IndexColumnSelectionField()
        {
            InitializeComponent();

            btnAddColumn.Click += BtnAddColumn_Click;
            btnRemoveColumn.Click += BtnRemoveColumn_Click;
            btnMoveUp.Click += BtnMoveUp_Click;
            btnMoveDown.Click += BtnMoveDown_Click;
            lstAvailableColumns.SelectedIndexChanged += LstAvailableColumns_SelectedIndexChanged;
            lstSelectedColumns.SelectedIndexChanged += LstSelectedColumns_SelectedIndexChanged;
        }

        public void BindViewModel(IndexColumnSelectionFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                DataBindings.Clear();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.AvailableColumns.CollectionChanged -= AvailableColumns_CollectionChanged;
                _viewModel.SelectedColumns.CollectionChanged -= SelectedColumns_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            DataBindings.Add("Visible", viewModel, nameof(viewModel.Visible), false, DataSourceUpdateMode.OnPropertyChanged).ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;

            RefreshAvailableColumns();
            RefreshSelectedColumns();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.AvailableColumns.CollectionChanged += AvailableColumns_CollectionChanged;
            _viewModel.SelectedColumns.CollectionChanged += SelectedColumns_CollectionChanged;

            UpdateButtonStates();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IndexColumnSelectionFieldModel.SelectedAvailableColumn) ||
                e.PropertyName == nameof(IndexColumnSelectionFieldModel.SelectedIndexColumn))
            {
                UpdateButtonStates();
            }
        }

        private void AvailableColumns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => AvailableColumns_CollectionChanged(sender, e));
                return;
            }
            RefreshAvailableColumns();
        }

        private void SelectedColumns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((IndexColumnSelectionFieldModel)(object)viewModel);
        }
    }
}
