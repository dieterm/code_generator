using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class MultiSelectField : UserControl, IView<MultiSelectFieldModel>
    {
        private MultiSelectFieldModel? _viewModel;
        private bool _isUpdatingFromViewModel;

        public MultiSelectField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(clbItems, lblErrorMessage);
            clbItems.ItemCheck += ClbItems_ItemCheck;
            Disposed += MultiSelectField_Disposed;
        }

        private void MultiSelectField_Disposed(object? sender, EventArgs e)
        {
            ClearDataBindings();
            clbItems.ItemCheck -= ClbItems_ItemCheck;
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        /// <summary>
        /// Gets or sets the label text for the field
        /// </summary>
        [Category("Appearance")]
        [Description("The label text displayed for this field")]
        [DefaultValue("Field Label:")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Label
        {
            get => lblLabel.Text;
            set => lblLabel.Text = value;
        }

        /// <summary>
        /// Gets or sets visibility of the error message label
        /// </summary>
        [Category("Appearance")]
        [Description("Show or hide the error message for this field")]
        [DefaultValue(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ErrorMessageVisible
        {
            get => lblErrorMessage.Visible;
            set => lblErrorMessage.Visible = value;
        }

        private void ClearDataBindings()
        {
            lblLabel.DataBindings.Clear();
            lblErrorMessage.DataBindings.Clear();
        }

        public void BindViewModel(MultiSelectFieldModel viewModel)
        {
            if (_viewModel != null)
            {
                ClearDataBindings();
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null)
                return;

            lblLabel.DataBindings.Add("Text", viewModel, nameof(viewModel.Label), false, DataSourceUpdateMode.OnPropertyChanged);
            lblErrorMessage.DataBindings.Add("Text", viewModel, nameof(viewModel.ErrorMessage), false, DataSourceUpdateMode.OnPropertyChanged);
            toolTip.SetToolTip(clbItems, viewModel.Tooltip);

            PopulateItems();
            UpdateCheckedFromViewModel();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (e.PropertyName == nameof(_viewModel.Items))
            {
                PopulateItems();
                UpdateCheckedFromViewModel();
            }
            else if (e.PropertyName == nameof(_viewModel.SelectedItems) || e.PropertyName == nameof(_viewModel.Value))
            {
                UpdateCheckedFromViewModel();
            }
            else if (e.PropertyName == nameof(_viewModel.Tooltip))
            {
                toolTip.SetToolTip(clbItems, _viewModel.Tooltip);
            }
        }

        private void PopulateItems()
        {
            if (_viewModel == null) return;

            _isUpdatingFromViewModel = true;
            try
            {
                clbItems.Items.Clear();
                foreach (var item in _viewModel.Items)
                {
                    clbItems.Items.Add(item);
                }
            }
            finally
            {
                _isUpdatingFromViewModel = false;
            }
        }

        private void UpdateCheckedFromViewModel()
        {
            if (_viewModel == null) return;

            _isUpdatingFromViewModel = true;
            try
            {
                for (int i = 0; i < clbItems.Items.Count; i++)
                {
                    var item = (ComboboxItem)clbItems.Items[i];
                    var isSelected = _viewModel.SelectedItems.Any(s =>
                        s == item || (s.Value != null && s.Value.Equals(item.Value)));
                    clbItems.SetItemChecked(i, isSelected);
                }
            }
            finally
            {
                _isUpdatingFromViewModel = false;
            }
        }

        private void ClbItems_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (_isUpdatingFromViewModel || _viewModel == null) return;

            // Use BeginInvoke so the CheckedItems collection is updated after the event
            BeginInvoke(() =>
            {
                var selected = new List<ComboboxItem>();
                for (int i = 0; i < clbItems.Items.Count; i++)
                {
                    if (clbItems.GetItemChecked(i))
                    {
                        selected.Add((ComboboxItem)clbItems.Items[i]);
                    }
                }
                _viewModel.SelectedItems = selected;
            });
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((MultiSelectFieldModel)(object)viewModel);
        }
    }
}
