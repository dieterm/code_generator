using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.TemplateEngines.Scriban;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views;

/// <summary>
/// View for editing a Scriban template file using Syncfusion EditControl
/// </summary>
public partial class ScribanTemplateEditView : UserControl, IView<ScribanTemplateEditViewModel>
{
    private ScribanTemplateEditViewModel? _viewModel;

    public ScribanTemplateEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(ScribanTemplateEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            LoadTemplateFile();
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((ScribanTemplateEditViewModel)(object)viewModel);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(() => ViewModel_PropertyChanged(sender, e));
            return;
        }

        if (e.PropertyName == nameof(ScribanTemplateEditViewModel.TemplateFilePath))
        {
            LoadTemplateFile();
        }
    }

    private void LoadTemplateFile()
    {
        if (_viewModel == null || string.IsNullOrEmpty(_viewModel.TemplateFilePath))
        {
            editControl.Text = string.Empty;
            return;
        }

        if (File.Exists(_viewModel.TemplateFilePath))
        {
            try
            {
                editControl.LoadFile(_viewModel.TemplateFilePath);
                // Optionally, set the language for syntax highlighting if supported
                if (_viewModel.TemplateInstance != null)
                {
                    InitializeSyntaxHighlighting();
                }
            }
            catch (Exception ex)
            {
                editControl.Text = $"Error loading file: {ex.Message}";
            }
        }
        else
        {
            editControl.Text = $"File not found: {_viewModel.TemplateFilePath}";
        }
    }

    private void InitializeSyntaxHighlighting()
    {
        // template parameters
        var parameters = _viewModel.TemplateInstance.Parameters;
        // custom helper methods
        var methods = _viewModel.TemplateInstance.Functions;
        
        var engineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
        var scribanTemplateEngine = engineManager.TemplateEngines.OfType<ScribanTemplateEngine>().Single();
        // global helper methods
        var globalFunctions = scribanTemplateEngine.GlobalFunctions;
        // also include default buildin scriban methods

        IConfigLanguage scribanLanguage = new ScribanLanguageConfig(parameters, methods, globalFunctions);
        editControl.ApplyConfiguration(scribanLanguage);
    }

    /// <summary>
    /// Save the current content back to the file
    /// </summary>
    public void SaveFile()
    {
        if (_viewModel == null || string.IsNullOrEmpty(_viewModel.TemplateFilePath))
            return;

        try
        {
            editControl.SaveFile(_viewModel.TemplateFilePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
