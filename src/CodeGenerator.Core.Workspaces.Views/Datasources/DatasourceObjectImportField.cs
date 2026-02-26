using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// Reusable UserControl for datasource object import: GroupBox with load/add buttons, info label, ListView, status and error labels.
    /// </summary>
    public partial class DatasourceObjectImportField : UserControl, IView<DatasourceObjectImportFieldModel>
    {
        private DatasourceObjectImportFieldModel? _viewModel;

        public DatasourceObjectImportField()
        {
            InitializeComponent();
        }

        public void BindViewModel(DatasourceObjectImportFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.Items.CollectionChanged -= Items_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Apply configuration from ViewModel
            grpObjects.Text = _viewModel.GroupBoxText;
            btnLoad.Text = _viewModel.LoadButtonText;
            btnAddSelected.Text = _viewModel.AddSelectedButtonText;
            btnAddAll.Text = _viewModel.AddAllButtonText;
            btnAddAll.Visible = _viewModel.AddAllButtonVisible;

            // Info label
            lblInfo.Visible = _viewModel.InfoLabelVisible;
            lblInfo.Text = _viewModel.InfoText ?? string.Empty;

            // Configure columns
            lstObjects.Columns.Clear();
            foreach (var col in _viewModel.Columns)
            {
                lstObjects.Columns.Add(col.HeaderText, col.Width);
            }

            // Subscribe to events
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.Items.CollectionChanged += Items_CollectionChanged;

            // Initial state
            RefreshItems();
            UpdateUI();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => ViewModel_PropertyChanged(sender, e));
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(DatasourceObjectImportFieldModel.IsLoading):
                    UpdateUI();
                    break;
                case nameof(DatasourceObjectImportFieldModel.StatusText):
                    lblStatus.Text = _viewModel?.StatusText ?? string.Empty;
                    break;
                case nameof(DatasourceObjectImportFieldModel.ErrorText):
                    lblError.Text = _viewModel?.ErrorText ?? string.Empty;
                    lblError.Visible = !string.IsNullOrEmpty(_viewModel?.ErrorText);
                    break;
                case nameof(DatasourceObjectImportFieldModel.SelectedItem):
                    btnAddSelected.Enabled = !(_viewModel?.IsLoading ?? true) && _viewModel?.SelectedItem != null;
                    break;
                case nameof(DatasourceObjectImportFieldModel.GroupBoxText):
                    grpObjects.Text = _viewModel?.GroupBoxText ?? string.Empty;
                    break;
                case nameof(DatasourceObjectImportFieldModel.LoadButtonText):
                    if (!(_viewModel?.IsLoading ?? false))
                        btnLoad.Text = _viewModel?.LoadButtonText ?? "Load";
                    break;
                case nameof(DatasourceObjectImportFieldModel.AddSelectedButtonText):
                    btnAddSelected.Text = _viewModel?.AddSelectedButtonText ?? "Add Selected";
                    break;
                case nameof(DatasourceObjectImportFieldModel.AddAllButtonText):
                    btnAddAll.Text = _viewModel?.AddAllButtonText ?? "Add All";
                    break;
                case nameof(DatasourceObjectImportFieldModel.AddAllButtonVisible):
                    btnAddAll.Visible = _viewModel?.AddAllButtonVisible ?? false;
                    break;
                case nameof(DatasourceObjectImportFieldModel.InfoText):
                    lblInfo.Text = _viewModel?.InfoText ?? string.Empty;
                    break;
                case nameof(DatasourceObjectImportFieldModel.InfoLabelVisible):
                    lblInfo.Visible = _viewModel?.InfoLabelVisible ?? false;
                    UpdateListViewPosition();
                    break;
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => Items_CollectionChanged(sender, e));
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems!)
                    {
                        if (newItem is DatasourceObjectItemViewModel item)
                        {
                            AddListViewItem(item);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems!)
                    {
                        if (oldItem is DatasourceObjectItemViewModel item)
                        {
                            var lvItem = lstObjects.Items.Cast<ListViewItem>().FirstOrDefault(i => i.Tag == item);
                            if (lvItem != null)
                                lstObjects.Items.Remove(lvItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    lstObjects.Items.Clear();
                    break;
                default:
                    RefreshItems();
                    break;
            }

            UpdateAddAllButtonState();
        }

        private void RefreshItems()
        {
            lstObjects.Items.Clear();
            if (_viewModel == null) return;

            foreach (var item in _viewModel.Items)
            {
                AddListViewItem(item);
            }
            UpdateAddAllButtonState();
        }

        private void AddListViewItem(DatasourceObjectItemViewModel item)
        {
            var lvItem = new ListViewItem(item.Text)
            {
                Tag = item
            };
            if (!string.IsNullOrEmpty(item.ImageKey))
            {
                lvItem.ImageKey = item.ImageKey;
            }
            foreach (var subItem in item.SubItems)
            {
                lvItem.SubItems.Add(subItem);
            }
            lstObjects.Items.Add(lvItem);
        }

        private void UpdateUI()
        {
            var isLoading = _viewModel?.IsLoading ?? false;

            btnLoad.Enabled = !isLoading;
            btnLoad.Text = isLoading ? "Loading..." : (_viewModel?.LoadButtonText ?? "Load");
            btnAddSelected.Enabled = !isLoading && _viewModel?.SelectedItem != null;
            UpdateAddAllButtonState();
        }

        private void UpdateAddAllButtonState()
        {
            var isLoading = _viewModel?.IsLoading ?? false;
            btnAddAll.Enabled = !isLoading && (_viewModel?.Items.Count ?? 0) > 0;
        }

        private void UpdateListViewPosition()
        {
            var infoVisible = _viewModel?.InfoLabelVisible ?? false;
            lstObjects.Location = new Point(6, infoVisible ? 80 : 60);
        }

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            _viewModel?.LoadCommand?.Execute(null);
        }

        private void BtnAddSelected_Click(object? sender, EventArgs e)
        {
            _viewModel?.AddSelectedCommand?.Execute(null);
        }

        private void BtnAddAll_Click(object? sender, EventArgs e)
        {
            _viewModel?.AddAllCommand?.Execute(null);
        }

        private void LstObjects_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;

            _viewModel.SelectedItem = lstObjects.SelectedItems.Count > 0
                ? lstObjects.SelectedItems[0].Tag as DatasourceObjectItemViewModel
                : null;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((DatasourceObjectImportFieldModel)(object)viewModel);
        }
    }
}
