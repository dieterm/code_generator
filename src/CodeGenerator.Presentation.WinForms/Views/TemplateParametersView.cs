using CodeGenerator.Application.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// View for displaying template parameters and executing templates
    /// </summary>
    public partial class TemplateParametersView : UserControl, IView<TemplateParametersViewModel>
    {
        private TemplateParametersViewModel? _viewModel;
        private bool _isBindingParameterDetails;

        public TemplateParametersView()
        {
            InitializeComponent();
            SetupEventHandlers();
            SetupTypeCombobox();
        }

        private void SetupEventHandlers()
        {
            btnExecute.Click += BtnExecute_Click;
            btnToggleEditMode.Click += BtnToggleEditMode_Click;
            btnSave.Click += BtnSave_Click;
            btnAddParameter.Click += BtnAddParameter_Click;
            btnRemoveParameter.Click += BtnRemoveParameter_Click;
            btnMoveUp.Click += BtnMoveUp_Click;
            btnMoveDown.Click += BtnMoveDown_Click;
            lstParameters.SelectedIndexChanged += LstParameters_SelectedIndexChanged;

            // Parameter detail field change handlers
            txtParameterName.TextChanged += ParameterDetailChanged;
            txtDescription.TextChanged += ParameterDetailChanged;
            txtLabel.TextChanged += ParameterDetailChanged;
            txtTooltip.TextChanged += ParameterDetailChanged;
            txtDefaultValue.TextChanged += ParameterDetailChanged;
            txtAllowedValues.TextChanged += ParameterDetailChanged;
            chkRequired.CheckedChanged += ParameterDetailChanged;
            cboType.SelectedIndexChanged += ParameterDetailChanged;
        }

        private void SetupTypeCombobox()
        {
            cboType.Items.Clear();
            foreach (var type in TemplateParameterEditModel.AvailableTypes)
            {
                cboType.Items.Add(type);
            }
            cboType.SelectedIndex = 0;
        }

        public void BindViewModel(TemplateParametersViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ParameterDefinitions.CollectionChanged -= ParameterDefinitions_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                _viewModel.ParameterDefinitions.CollectionChanged += ParameterDefinitions_CollectionChanged;
                UpdateFromViewModel();
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((TemplateParametersViewModel)(object)viewModel);
        }

        private void ParameterDefinitions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshParameterList();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ViewModel_PropertyChanged(sender, e)));
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(TemplateParametersViewModel.TemplateName):
                    lblTemplateName.Text = _viewModel?.TemplateName ?? string.Empty;
                    break;
                case nameof(TemplateParametersViewModel.TemplateDescription):
                    lblTemplateDescription.Text = _viewModel?.TemplateDescription ?? string.Empty;
                    break;
                case nameof(TemplateParametersViewModel.ParameterFields):
                    RebuildParameterControls();
                    break;
                case nameof(TemplateParametersViewModel.ParameterDefinitions):
                    RefreshParameterList();
                    break;
                case nameof(TemplateParametersViewModel.SelectedParameterDefinition):
                    BindSelectedParameterDetails();
                    break;
                case nameof(TemplateParametersViewModel.CanExecute):
                    btnExecute.Enabled = _viewModel?.CanExecute ?? false;
                    break;
                case nameof(TemplateParametersViewModel.IsExecuting):
                    UpdateExecutingState();
                    break;
                case nameof(TemplateParametersViewModel.IsEditMode):
                    UpdateEditModeDisplay();
                    break;
                case nameof(TemplateParametersViewModel.HasUnsavedChanges):
                    UpdateSaveButtonState();
                    break;
            }
        }

        private void UpdateFromViewModel()
        {
            if (_viewModel == null) return;

            lblTemplateName.Text = _viewModel.TemplateName;
            lblTemplateDescription.Text = _viewModel.TemplateDescription ?? string.Empty;
            btnExecute.Enabled = _viewModel.CanExecute;
            
            RefreshParameterList();
            RebuildParameterControls();
            UpdateEditModeDisplay();
            UpdateExecutingState();
            UpdateSaveButtonState();
        }

        private void UpdateEditModeDisplay()
        {
            if (_viewModel == null) return;

            var isEditMode = _viewModel.IsEditMode;
            pnlParameters.Visible = !isEditMode;
            pnlEditMode.Visible = isEditMode;
            btnExecute.Visible = !isEditMode;
            btnSave.Visible = isEditMode;
            btnToggleEditMode.Text = isEditMode ? "Run" : "Edit";
        }

        private void UpdateSaveButtonState()
        {
            btnSave.Enabled = _viewModel?.HasUnsavedChanges ?? false;
        }

        private void RefreshParameterList()
        {
            if (_viewModel == null) return;

            lstParameters.BeginUpdate();
            try
            {
                lstParameters.DataSource = null;
                lstParameters.DataSource = _viewModel.ParameterDefinitions.ToList();
                lstParameters.DisplayMember = nameof(TemplateParameterEditModel.Name);

                if (_viewModel.SelectedParameterDefinition != null)
                {
                    lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
                }
            }
            finally
            {
                lstParameters.EndUpdate();
            }
        }

        private void BindSelectedParameterDetails()
        {
            _isBindingParameterDetails = true;
            try
            {
                var param = _viewModel?.SelectedParameterDefinition;
                var hasSelection = param != null;

                pnlParameterDetails.Enabled = hasSelection;
                btnRemoveParameter.Enabled = hasSelection;
                btnMoveUp.Enabled = hasSelection && lstParameters.SelectedIndex > 0;
                btnMoveDown.Enabled = hasSelection && lstParameters.SelectedIndex < lstParameters.Items.Count - 1;

                if (param != null)
                {
                    txtParameterName.Text = param.Name;
                    txtDescription.Text = param.Description ?? string.Empty;
                    txtLabel.Text = param.Label ?? string.Empty;
                    txtTooltip.Text = param.Tooltip ?? string.Empty;
                    txtDefaultValue.Text = param.DefaultValue ?? string.Empty;
                    txtAllowedValues.Text = param.AllowedValuesText ?? string.Empty;
                    chkRequired.Checked = param.Required;

                    var typeIndex = cboType.Items.IndexOf(param.TypeName);
                    cboType.SelectedIndex = typeIndex >= 0 ? typeIndex : 0;
                }
                else
                {
                    txtParameterName.Text = string.Empty;
                    txtDescription.Text = string.Empty;
                    txtLabel.Text = string.Empty;
                    txtTooltip.Text = string.Empty;
                    txtDefaultValue.Text = string.Empty;
                    txtAllowedValues.Text = string.Empty;
                    chkRequired.Checked = false;
                    cboType.SelectedIndex = 0;
                }
            }
            finally
            {
                _isBindingParameterDetails = false;
            }
        }

        private void ParameterDetailChanged(object? sender, EventArgs e)
        {
            if (_isBindingParameterDetails || _viewModel?.SelectedParameterDefinition == null) return;

            var param = _viewModel.SelectedParameterDefinition;
            param.Name = txtParameterName.Text;
            param.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text;
            param.Label = string.IsNullOrWhiteSpace(txtLabel.Text) ? null : txtLabel.Text;
            param.Tooltip = string.IsNullOrWhiteSpace(txtTooltip.Text) ? null : txtTooltip.Text;
            param.DefaultValue = string.IsNullOrWhiteSpace(txtDefaultValue.Text) ? null : txtDefaultValue.Text;
            param.AllowedValuesText = string.IsNullOrWhiteSpace(txtAllowedValues.Text) ? null : txtAllowedValues.Text;
            param.Required = chkRequired.Checked;
            param.TypeName = cboType.SelectedItem?.ToString() ?? "System.String";

            // Refresh list to show updated name
            var selectedIndex = lstParameters.SelectedIndex;
            RefreshParameterList();
            if (selectedIndex >= 0 && selectedIndex < lstParameters.Items.Count)
            {
                lstParameters.SelectedIndex = selectedIndex;
            }
        }

        private void RebuildParameterControls()
        {
            pnlParameters.SuspendLayout();
            try
            {
                pnlParameters.Controls.Clear();

                if (_viewModel?.ParameterFields == null || _viewModel.ParameterFields.Count == 0) 
                {
                    // Show a message when there are no parameters
                    var noParamsLabel = new Label
                    {
                        Text = "No parameters defined for this template.\nClick 'Edit' to add parameters.",
                        AutoSize = false,
                        Width = pnlParameters.ClientSize.Width - 10,
                        Height = 50,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    pnlParameters.Controls.Add(noParamsLabel);
                    return;
                }

                foreach (var field in _viewModel.ParameterFields)
                {
                    var control = CreateControlForField(field);
                    if (control != null)
                    {
                        // Set width to fit the FlowLayoutPanel (minus padding and scrollbar)
                        control.Width = pnlParameters.ClientSize.Width - 30;
                        control.Margin = new Padding(3);
                        pnlParameters.Controls.Add(control);
                    }
                }
            }
            finally
            {
                pnlParameters.ResumeLayout(true);
            }
        }

        private Control? CreateControlForField(FieldViewModelBase field)
        {
            switch (field)
            {
                case BooleanFieldModel boolField:
                    var boolControl = new BooleanField();
                    boolControl.BindViewModel(boolField);
                    return boolControl;

                case ComboboxFieldModel comboField:
                    var comboControl = new ComboboxField();
                    comboControl.BindViewModel(comboField);
                    return comboControl;

                case IntegerFieldModel intField:
                    var intControl = new IntegerField();
                    intControl.BindViewModel(intField);
                    return intControl;

                case DateOnlyFieldModel dateField:
                    var dateControl = new DateOnlyField();
                    dateControl.BindViewModel(dateField);
                    return dateControl;

                case SingleLineTextFieldModel textField:
                    var textControl = new SingleLineTextField();
                    textControl.BindViewModel(textField);
                    return textControl;

                default:
                    // Default to single line text field
                    if (field is FieldViewModelBase genericField)
                    {
                        var genericTextModel = new SingleLineTextFieldModel
                        {
                            Name = genericField.Name,
                            Label = genericField.Label,
                            Tooltip = genericField.Tooltip,
                            IsRequired = genericField.IsRequired
                        };
                        var genericControl = new SingleLineTextField();
                        genericControl.BindViewModel(genericTextModel);
                        return genericControl;
                    }
                    return null;
            }
        }

        private void UpdateExecutingState()
        {
            if (_viewModel == null) return;

            btnExecute.Enabled = _viewModel.CanExecute && !_viewModel.IsExecuting;
            btnExecute.Text = _viewModel.IsExecuting ? "Executing..." : "Execute";
            
            // Disable parameter controls while executing
            foreach (Control control in pnlParameters.Controls)
            {
                control.Enabled = !_viewModel.IsExecuting;
            }
        }

        private void BtnExecute_Click(object? sender, EventArgs e)
        {
            _viewModel?.RequestExecute();
        }

        private void BtnToggleEditMode_Click(object? sender, EventArgs e)
        {
            _viewModel?.ToggleEditMode();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            _viewModel?.SaveParameterDefinitions();
        }

        private void BtnAddParameter_Click(object? sender, EventArgs e)
        {
            _viewModel?.AddParameter();
            RefreshParameterList();
            if (_viewModel?.SelectedParameterDefinition != null)
            {
                lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
            }
        }

        private void BtnRemoveParameter_Click(object? sender, EventArgs e)
        {
            _viewModel?.RemoveSelectedParameter();
        }

        private void BtnMoveUp_Click(object? sender, EventArgs e)
        {
            _viewModel?.MoveParameterUp();
            RefreshParameterList();
            if (_viewModel?.SelectedParameterDefinition != null)
            {
                lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
            }
        }

        private void BtnMoveDown_Click(object? sender, EventArgs e)
        {
            _viewModel?.MoveParameterDown();
            RefreshParameterList();
            if (_viewModel?.SelectedParameterDefinition != null)
            {
                lstParameters.SelectedItem = _viewModel.SelectedParameterDefinition;
            }
        }

        private void LstParameters_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_viewModel != null && lstParameters.SelectedItem is TemplateParameterEditModel param)
            {
                _viewModel.SelectedParameterDefinition = param;
            }
        }
    }
}
