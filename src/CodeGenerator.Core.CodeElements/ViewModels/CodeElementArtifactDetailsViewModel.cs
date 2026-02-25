using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class CodeElementArtifactDetailsViewModel : ViewModelBase
{
    private IViewModel? _detailsViewModel;
    public IViewModel? DetailsViewModel
    {
        get { return _detailsViewModel; }
        set { SetProperty(ref _detailsViewModel, value); }
    }
}
