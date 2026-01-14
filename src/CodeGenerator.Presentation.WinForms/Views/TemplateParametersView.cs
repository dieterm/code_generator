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
        private bool _isBindingTemplateMetadata;

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

            // Handle resize to update control widths
            pnlParameters.Resize += PnlParameters_Resize;

            // Template metadata change handlers
            txtEditTemplateId.TextChanged += TemplateMetadataChanged;
            txtEditDisplayName.TextChanged += TemplateMetadataChanged;
            txtEditDescription.TextChanged += TemplateMetadataChanged;

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

        private void PnlParameters_Resize(object? sender, EventArgs e)
        {
            UpdateParameterControlWidths();
        }

        private void UpdateParameterControlWidths()
        {
            var availableWidth = pnlParameters.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 10;
            foreach (Control control in pnlParameters.Controls)
            {
                control.Width = availableWidth;
            }
        }

        private void SetupTypeCombobox()
        {
            cboType.Items.Clear();
            foreach (var type in TemplateParameterEditModel.AvailableTypes)
            {
                // Show friendly names for special types
                var displayName = type == "CodeGenerator.TableArtifactData" 
                    ? "Table Data (from Workspace)" 
                    : type;
                cboType.Items.Add(new TypeComboItem { DisplayName = displayName, TypeName = type });
            }
            cboType.DisplayMember = nameof(TypeComboItem.DisplayName);
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
                case nameof(TemplateParametersViewModel.EditableTemplateId):
                case nameof(TemplateParametersViewModel.EditableDisplayName):
                case nameof(TemplateParametersViewModel.EditableDescription):
                    BindTemplateMetadata();
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
            
            BindTemplateMetadata();
            RefreshParameterList();
            RebuildParameterControls();
            UpdateEditModeDisplay();
            UpdateExecutingState();
            UpdateSaveButtonState();
        }

        private void BindTemplateMetadata()
        {
            _isBindingTemplateMetadata = true;
            try
            {
                txtEditTemplateId.Text = _viewModel?.EditableTemplateId ?? string.Empty;
                txtEditDisplayName.Text = _viewModel?.EditableDisplayName ?? string.Empty;
                txtEditDescription.Text = _viewModel?.EditableDescription ?? string.Empty;
            }
            finally
            {
                _isBindingTemplateMetadata = false;
            }
        }

        private void TemplateMetadataChanged(object? sender, EventArgs e)
        {
            if (_isBindingTemplateMetadata || _viewModel == null) return;

            _viewModel.EditableTemplateId = txtEditTemplateId.Text;
            _viewModel.EditableDisplayName = txtEditDisplayName.Text;
            _viewModel.EditableDescription = txtEditDescription.Text;
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

                    // Find the type in the combobox
                    for (int i = 0; i < cboType.Items.Count; i++)
                    {
                        if (cboType.Items[i] is TypeComboItem item && item.TypeName == param.TypeName)
                        {
                            cboType.SelectedIndex = i;
                            break;
                        }
                    }
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
            
            // Get type name from combo item
            if (cboType.SelectedItem is TypeComboItem typeItem)
            {
                param.TypeName = typeItem.TypeName;
            }

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

                var availableWidth = pnlParameters.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 10;
                var currentTop = 5;
                var controlSpacing = 5;

                if (_viewModel?.ParameterFields == null || _viewModel.ParameterFields.Count == 0) 
                {
                    // Show a message when there are no parameters
                    var noParamsLabel = new Label
                    {
                        Text = "No parameters defined for this template.\nClick 'Edit' to add parameters.",
                        AutoSize = false,
                        Width = availableWidth,
                        Height = 50,
                        Location = new Point(5, currentTop),
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
                        control.Location = new Point(5, currentTop);
                        control.Width = availableWidth;
                        pnlParameters.Controls.Add(control);
                        currentTop += control.Height + controlSpacing;
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
                case TableArtifactFieldModel tableField:
                    // Create a ComboBox for selecting TableArtifact
                    return CreateTableArtifactControl(tableField);

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

        private Control CreateTableArtifactControl(TableArtifactFieldModel tableField)
        {
            var availableWidth = pnlParameters.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 10;
            
            var panel = new Panel
            {
                Height = 55,
                Width = availableWidth
            };

            var label = new Label
            {
                Text = tableField.Label ?? tableField.Name,
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(3, 4)
            };

            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(115, 0),
                Width = panel.Width - 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add items
            comboBox.Items.Clear();
            foreach (var table in tableField.AvailableTables)
            {
                comboBox.Items.Add(table);
            }
            comboBox.DisplayMember = nameof(TableArtifactItem.DisplayName);

            // Set selected item
            if (tableField.SelectedTable != null)
            {
                comboBox.SelectedItem = tableField.SelectedTable;
            }

            // Handle selection changed
            comboBox.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox.SelectedItem is TableArtifactItem selected)
                {
                    tableField.SelectedTable = selected;
                }
            };

            // Tooltip
            if (!string.IsNullOrEmpty(tableField.Tooltip))
            {
                var toolTip = new ToolTip();
                toolTip.SetToolTip(comboBox, tableField.Tooltip);
            }

            // Required indicator
            var requiredLabel = new Label
            {
                Text = tableField.IsRequired ? "*" : "",
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(comboBox.Right + 2, 4)
            };

            panel.Controls.Add(label);
            panel.Controls.Add(comboBox);
            panel.Controls.Add(requiredLabel);

            return panel;
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

        /// <summary>
        /// Helper class for type combo box items
        /// </summary>
        private class TypeComboItem
        {
            public string DisplayName { get; set; } = string.Empty;
            public string TypeName { get; set; } = string.Empty;
            public override string ToString() => DisplayName;
        }
    }
}
