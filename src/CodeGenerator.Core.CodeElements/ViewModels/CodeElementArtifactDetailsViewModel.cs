using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class CodeElementArtifactDetailsViewModel : ViewModelBase
{
    private ViewModelBase? _detailsViewModel;
    public ViewModelBase? DetailsViewModel
    {
        get { return _detailsViewModel; }
        set { SetProperty(ref _detailsViewModel, value); }
    }
}
