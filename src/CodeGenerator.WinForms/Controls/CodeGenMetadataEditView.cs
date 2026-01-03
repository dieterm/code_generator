using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controls;

public partial class CodeGenMetadataEditView : UserControl
{
    private readonly CodeGenMetadataEditViewModel _viewModel;
    private bool _isLoading;

    public CodeGenMetadataEditView()
    {
        _viewModel = new CodeGenMetadataEditViewModel();
        InitializeComponent();
        SetupBindings();
    }

    public void LoadMetadata(CodeGenMetadata metadata)
    {
        _isLoading = true;
        _viewModel.LoadFromMetadata(metadata);
        UpdateControlsFromViewModel();
        _isLoading = false;
    }

    private void SetupBindings()
    {
        _namespaceTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Namespace = _namespaceTextBox.Text;
        };
        _outputPathTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.OutputPath = _outputPathTextBox.Text;
        };
        _targetLanguageComboBox.SelectedIndexChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.TargetLanguage = _targetLanguageComboBox.Text;
                _viewModel.UpdateMetadata();
            }
        };
        _presentationTechnologyComboBox.SelectedIndexChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.PresentationTechnology = _presentationTechnologyComboBox.Text == "(None)" ? string.Empty : _presentationTechnologyComboBox.Text;
                _viewModel.UpdateMetadata();
            }
        };
        _dataLayerTechnologyComboBox.SelectedIndexChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.DataLayerTechnology = _dataLayerTechnologyComboBox.Text;
                _viewModel.UpdateMetadata();
            }
        };
        _useDependencyInjectionCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.UseDependencyInjection = _useDependencyInjectionCheckBox.Checked;
                _viewModel.UpdateMetadata();
            }
        };
        _useLoggingCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.UseLogging = _useLoggingCheckBox.Checked;
                _viewModel.UpdateMetadata();
            }
        };
        _useConfigurationCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.UseConfiguration = _useConfigurationCheckBox.Checked;
                _viewModel.UpdateMetadata();
            }
        };

        _namespaceTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateMetadata();
        };
        _outputPathTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateMetadata();
        };
    }

    private void UpdateControlsFromViewModel()
    {
        _namespaceTextBox.Text = _viewModel.Namespace;
        _outputPathTextBox.Text = _viewModel.OutputPath;
        _targetLanguageComboBox.Text = _viewModel.TargetLanguage;
        _presentationTechnologyComboBox.Text = string.IsNullOrWhiteSpace(_viewModel.PresentationTechnology) ? "(None)" : _viewModel.PresentationTechnology;
        _dataLayerTechnologyComboBox.Text = _viewModel.DataLayerTechnology;
        _useDependencyInjectionCheckBox.Checked = _viewModel.UseDependencyInjection;
        _useLoggingCheckBox.Checked = _viewModel.UseLogging;
        _useConfigurationCheckBox.Checked = _viewModel.UseConfiguration;
    }
}
