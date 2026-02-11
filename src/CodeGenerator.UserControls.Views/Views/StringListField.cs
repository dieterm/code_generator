using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views
{
    public partial class StringListField : UserControl, IView<StringListFieldModel>
    {
        private StringListFieldModel? _viewModel;

        public StringListField()
        {
            InitializeComponent();

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnRemove.Click += BtnRemove_Click;
            lstItems.SelectedIndexChanged += LstItems_SelectedIndexChanged;

            Disposed += StringListField_Disposed;
        }

        private void StringListField_Disposed(object? sender, EventArgs e)
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

        public void BindViewModel(StringListFieldModel viewModel)
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
            if (e.PropertyName == nameof(StringListFieldModel.Label))
                grpBox.Text = _viewModel?.Label ?? grpBox.Text;
        }

        private void LstItems_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null) return;
            var value = PromptInput("Add Item", "Value:", string.Empty);
            if (value != null)
                _viewModel.AddItem(value);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_viewModel == null || lstItems.SelectedIndex < 0) return;
            var current = _viewModel.Items[lstItems.SelectedIndex];
            var value = PromptInput("Edit Item", "Value:", current);
            if (value != null)
                _viewModel.UpdateItem(lstItems.SelectedIndex, value);
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

        private static string? PromptInput(string title, string label, string defaultValue)
        {
            using var form = new Form
            {
                Text = title, Width = 350, Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false
            };
            var lbl = new Label { Left = 10, Top = 15, Text = label, AutoSize = true };
            var txt = new TextBox { Left = 10, Top = 35, Width = 310, Text = defaultValue };
            var btnOk = new Button { Text = "OK", Left = 160, Top = 70, Width = 75, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 245, Top = 70, Width = 75, DialogResult = DialogResult.Cancel };
            form.Controls.AddRange([lbl, txt, btnOk, btnCancel]);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;
            return form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txt.Text) ? txt.Text : null;
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((StringListFieldModel)(object)viewModel);
        }
    }
}
