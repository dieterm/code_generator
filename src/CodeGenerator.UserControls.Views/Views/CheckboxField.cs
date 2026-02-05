using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.UserControls.Views;

/// <summary>
/// Checkbox field control for boolean values
/// </summary>
public partial class CheckboxField : UserControl, IView<CheckboxFieldModel>
{
    private CheckboxFieldModel? _viewModel;

    public CheckboxField()
    {
        InitializeComponent();
        chkValue.CheckedChanged += ChkValue_CheckedChanged;
        Disposed += CheckboxField_Disposed;
    }

    private void CheckboxField_Disposed(object? sender, EventArgs e)
    {
        ClearDataBindings();
    }

    private void ChkValue_CheckedChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.Value = chkValue.Checked;
        }
    }

    [Category("Appearance")]
    [Description("The label text displayed for this field")]
    [DefaultValue("Checkbox:")]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string Label
    {
        get => chkValue.Text;
        set => chkValue.Text = value;
    }

    private void ClearDataBindings()
    {
        chkValue.DataBindings.Clear();
    }

    public void BindViewModel(CheckboxFieldModel viewModel)
    {
        if (_viewModel != null)
        {
            ClearDataBindings();
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null)
            return;

        chkValue.Text = _viewModel.Label;
        chkValue.Checked = _viewModel.Value is bool b && b;
        
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CheckboxFieldModel.Value))
        {
            if (InvokeRequired)
            {
                Invoke(() => ViewModel_PropertyChanged(sender, e));
                return;
            }
            chkValue.Checked = _viewModel?.Value is bool b && b;
        }
        else if (e.PropertyName == nameof(CheckboxFieldModel.Label))
        {
            if (InvokeRequired)
            {
                Invoke(() => ViewModel_PropertyChanged(sender, e));
                return;
            }
            chkValue.Text = _viewModel?.Label ?? string.Empty;
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((CheckboxFieldModel)(object)viewModel);
    }
}
