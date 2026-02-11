using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class StringDictionaryField : UserControl, IView<StringDictionaryFieldModel>
    {
        private StringDictionaryFieldModel? _viewModel;

        public StringDictionaryField()
        {
            InitializeComponent();

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnRemove.Click += BtnRemove_Click;
            lstItems.SelectedIndexChanged += LstItems_SelectedIndexChanged;

            Disposed += StringDictionaryField_Disposed;
        }

        private void StringDictionaryField_Disposed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        [Category("Appearance")]
        [Description("The label text displayed in the group box header")]
        [DefaultValue("Items:")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Label
        {
            get => grpBox.Text;
            set => grpBox.Text = value;
        }

        public void BindViewModel(StringDictionaryFieldModel viewModel)
        {
            if (_viewModel != null)
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;

            _viewModel = viewModel;

            if (_viewModel == null) return;

            grpBox.Text = _viewModel.Label ?? grpBox.Text;
            lstItems.DataSource = _viewModel.Items;
            UpdateButtonStates();

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringDictionaryFieldModel.Label))
                grpBox.Text = _viewModel?.Label ?? grpBox.Text;
        }

        private void LstItems_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;
            var result = PromptKeyValueInput("Add Entry", string.Empty, string.Empty);
            if (result != null)
                _viewModel.AddItem(result.Value.Key, result.Value.Value);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null || lstItems.SelectedIndex < 0) return;
            var current = _viewModel.Items[lstItems.SelectedIndex];
            var result = PromptKeyValueInput("Edit Entry", current.Key, current.Value);
            if (result != null)
                _viewModel.UpdateItem(lstItems.SelectedIndex, result.Value.Key, result.Value.Value);
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null || lstItems.SelectedIndex < 0) return;
            _viewModel.RemoveItem(lstItems.SelectedIndex);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            var hasSelection = lstItems.SelectedIndex >= 0;
            btnEdit.Enabled = hasSelection;
            btnRemove.Enabled = hasSelection;
        }

        private static (string Key, string Value)? PromptKeyValueInput(string title, string defaultKey, string defaultValue)
        {
            using var form = new Form
            {
                Text = title, Width = 350, Height = 190,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false
            };
            var lblKey = new Label { Left = 10, Top = 15, Text = "Key:", AutoSize = true };
            var txtKey = new TextBox { Left = 10, Top = 35, Width = 310, Text = defaultKey };
            var lblValue = new Label { Left = 10, Top = 60, Text = "Value:", AutoSize = true };
            var txtValue = new TextBox { Left = 10, Top = 80, Width = 310, Text = defaultValue };
            var btnOk = new Button { Text = "OK", Left = 160, Top = 115, Width = 75, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 245, Top = 115, Width = 75, DialogResult = DialogResult.Cancel };
            form.Controls.AddRange([lblKey, txtKey, lblValue, txtValue, btnOk, btnCancel]);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;
            return form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtKey.Text)
                ? (txtKey.Text, txtValue.Text)
                : null;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((StringDictionaryFieldModel)(object)viewModel);
        }
    }
}
