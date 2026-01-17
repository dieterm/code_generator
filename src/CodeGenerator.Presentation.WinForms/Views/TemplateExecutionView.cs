using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for executing a template - displays parameter input fields and execute button
/// </summary>
public partial class TemplateExecutionView : UserControl, IView<TemplateExecutionViewModel>
{
    private TemplateExecutionViewModel? _viewModel;

    public TemplateExecutionView()
    {
        InitializeComponent();
        btnExecute.Click += BtnExecute_Click;
        pnlParameters.Resize += PnlParameters_Resize;
    }

    public void BindViewModel(TemplateExecutionViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            RebuildParameterControls();
            UpdateExecutingState();
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((TemplateExecutionViewModel)(object)viewModel);
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
            case nameof(TemplateExecutionViewModel.ParameterFields):
                RebuildParameterControls();
                break;
            case nameof(TemplateExecutionViewModel.CanExecute):
            case nameof(TemplateExecutionViewModel.IsExecuting):
                UpdateExecutingState();
                break;
        }
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

        foreach (Control control in pnlParameters.Controls)
        {
            control.Enabled = !_viewModel.IsExecuting;
        }
    }

    private void BtnExecute_Click(object? sender, EventArgs e)
    {
        _viewModel?.RequestExecute();
    }
}
