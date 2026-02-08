using CodeGenerator.Shared.ViewModels;
using System.Windows.Forms;

namespace CodeGenerator.Shared.Views
{
    /// <summary>
    /// Resolves the appropriate View (UserControl) for a given ViewModel type.
    /// Registrations are done in the DI container in the Presentation layer:
    /// <code>services.AddTransient&lt;IView&lt;MyViewModel&gt;, MyView&gt;()</code>
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// Creates and binds a View for the given ViewModel.
        /// Returns null if no View is registered for the ViewModel type.
        /// </summary>
        IView? CreateView(ViewModelBase viewModel);
    }
}
