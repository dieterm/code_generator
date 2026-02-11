using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Views;

public partial class CodeElementArtifactDetailsView : UserControl, IView<CodeElementArtifactDetailsViewModel>
{
    private CodeElementArtifactDetailsViewModel? _viewModel;

    public CodeElementArtifactDetailsView()
    {
        InitializeComponent();
        Disposed += CodeElementArtifactDetailsView_Disposed;
    }

    private void CodeElementArtifactDetailsView_Disposed(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }

    public void BindViewModel(CodeElementArtifactDetailsViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        ShowDetailsControl();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CodeElementArtifactDetailsViewModel.DetailsViewModel))
        {
            ShowDetailsControl();
        }
    }

    private void ShowDetailsControl()
    {
        Controls.Clear();

        if (_viewModel?.DetailsViewModel == null)
            return;

        var viewFactory = ServiceProviderHolder.GetRequiredService<IViewFactory>();
        var view = viewFactory.CreateView(_viewModel.DetailsViewModel);

        if (view is UserControl detailsControl)
        {
            Controls.Add(detailsControl);
            detailsControl.Dock = DockStyle.Fill;
        }
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
    {
        BindViewModel((CodeElementArtifactDetailsViewModel)(object)viewModel);
    }
}
