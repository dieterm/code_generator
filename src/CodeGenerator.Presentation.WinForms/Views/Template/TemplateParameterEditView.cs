using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing a single template parameter definition
/// </summary>
public partial class TemplateParameterEditView : UserControl, IView<TemplateParameterEditViewModel>
{
    private TemplateParameterEditViewModel? _viewModel;
    private bool _isBinding;

    public TemplateParameterEditView()
    {
        InitializeComponent();
        SetupEventHandlers();
        SetupTypeCombobox();
    }

    private void SetupEventHandlers()
    {
        txtParameterName.TextChanged += OnFieldChanged;
        txtDescription.TextChanged += OnFieldChanged;
        txtLabel.TextChanged += OnFieldChanged;
        txtTooltip.TextChanged += OnFieldChanged;
        txtDefaultValue.TextChanged += OnFieldChanged;
        txtAllowedValues.TextChanged += OnFieldChanged;
        chkRequired.CheckedChanged += OnFieldChanged;
        cboType.SelectedIndexChanged += OnTypeChanged;
        txtTableDataFilter.TextChanged += OnFieldChanged;
        numTableDataMaxRows.ValueChanged += OnFieldChanged;
    }

    private void SetupTypeCombobox()
    {
        cboType.Items.Clear();
        cboType.DisplayMember = nameof(ParameterTypeOption.DisplayName);
        foreach (var type in TemplateParameterEditViewModel.AvailableTypes)
        {
            cboType.Items.Add(type);
        }
        cboType.SelectedIndex = 0;
    }

    public void BindViewModel(TemplateParameterEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            BindFromViewModel();
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((TemplateParameterEditViewModel)(object)viewModel);
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
            case nameof(TemplateParameterEditViewModel.Parameter):
                BindFromViewModel();
                break;
            case nameof(TemplateParameterEditViewModel.IsDatasourceType):
                pnlDatasourceOptions.Visible = _viewModel?.IsDatasourceType ?? false;
                break;
        }
    }

    private void BindFromViewModel()
    {
        _isBinding = true;
        try
        {
            var param = _viewModel?.Parameter;
            var hasParameter = param != null;

            pnlMain.Enabled = hasParameter;

            if (param != null)
            {
                txtParameterName.Text = param.Name;
                txtDescription.Text = param.Description ?? string.Empty;
                txtLabel.Text = param.Label ?? string.Empty;
                txtTooltip.Text = param.Tooltip ?? string.Empty;
                txtDefaultValue.Text = param.DefaultValue ?? string.Empty;
                txtAllowedValues.Text = param.AllowedValuesText ?? string.Empty;
                chkRequired.Checked = param.Required;
                txtTableDataFilter.Text = param.TableDataFilter ?? string.Empty;
                numTableDataMaxRows.Value = param.TableDataMaxRows ?? 0;

                // Find the type in the combobox
                for (int i = 0; i < cboType.Items.Count; i++)
                {
                    if (cboType.Items[i] is ParameterTypeOption option && option.TypeName == param.TypeName)
                    {
                        cboType.SelectedIndex = i;
                        break;
                    }
                }

                pnlDatasourceOptions.Visible = _viewModel?.IsDatasourceType ?? false;
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
                txtTableDataFilter.Text = string.Empty;
                numTableDataMaxRows.Value = 0;
                pnlDatasourceOptions.Visible = false;
            }
        }
        finally
        {
            _isBinding = false;
        }
    }

    private void OnFieldChanged(object? sender, EventArgs e)
    {
        if (_isBinding || _viewModel?.Parameter == null) return;

        var param = _viewModel.Parameter;
        param.Name = txtParameterName.Text;
        param.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text;
        param.Label = string.IsNullOrWhiteSpace(txtLabel.Text) ? null : txtLabel.Text;
        param.Tooltip = string.IsNullOrWhiteSpace(txtTooltip.Text) ? null : txtTooltip.Text;
        param.DefaultValue = string.IsNullOrWhiteSpace(txtDefaultValue.Text) ? null : txtDefaultValue.Text;
        param.AllowedValuesText = string.IsNullOrWhiteSpace(txtAllowedValues.Text) ? null : txtAllowedValues.Text;
        param.Required = chkRequired.Checked;
        param.TableDataFilter = string.IsNullOrWhiteSpace(txtTableDataFilter.Text) ? null : txtTableDataFilter.Text;
        param.TableDataMaxRows = numTableDataMaxRows.Value > 0 ? (int)numTableDataMaxRows.Value : null;
    }

    private void OnTypeChanged(object? sender, EventArgs e)
    {
        if (_isBinding || _viewModel?.Parameter == null) return;

        if (cboType.SelectedItem is ParameterTypeOption option)
        {
            _viewModel.OnTypeChanged(option.TypeName);
        }
    }
}
