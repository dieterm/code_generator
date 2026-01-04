using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.Controllers;

/// <summary>
/// Base class for all Controllers
/// </summary>
public abstract class ControllerBase<TViewModel> where TViewModel : ValidationViewModelBase
{
    protected TViewModel ViewModel { get; }

    protected ControllerBase(TViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    /// <summary>
    /// Initialize the controller
    /// </summary>
    public virtual void Initialize()
    {
    }

    /// <summary>
    /// Validate the ViewModel
    /// </summary>
    public virtual bool Validate()
    {
        return ViewModel.Validate();
    }
}
