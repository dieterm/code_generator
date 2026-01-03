using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controls;

public partial class DomainDrivenDesignMetadataEditView : UserControl
{
    private readonly DomainDrivenDesignMetadataEditViewModel _viewModel;
    private bool _isLoading;

    public DomainDrivenDesignMetadataEditView()
    {
        _viewModel = new DomainDrivenDesignMetadataEditViewModel();
        InitializeComponent();
        SetupBindings();
    }

    public void LoadMetadata(DomainDrivenDesignMetadata metadata)
    {
        _isLoading = true;
        _viewModel.LoadFromMetadata(metadata);
        UpdateControlsFromViewModel();
        _isLoading = false;
    }

    private void SetupBindings()
    {
        _boundedContextTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.BoundedContext = _boundedContextTextBox.Text;
        };
        _boundedContextTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateMetadata();
        };
    }

    private void UpdateControlsFromViewModel()
    {
        _boundedContextTextBox.Text = _viewModel.BoundedContext;
    }
}
