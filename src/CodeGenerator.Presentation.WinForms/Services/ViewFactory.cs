using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Presentation.WinForms.Services
{
    /// <summary>
    /// Resolves Views for ViewModels using the DI container.
    /// For each ViewModel, a View must be registered as:
    /// <code>services.AddTransient&lt;IView&lt;TViewModel&gt;, TView&gt;()</code>
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IView? CreateView(ViewModelBase viewModel)
        {
            // Build IView<TViewModel> for the concrete ViewModel type
            var viewModelType = viewModel.GetType();
            var viewInterfaceType = typeof(IView<>).MakeGenericType(viewModelType);

            var view = _serviceProvider.GetService(viewInterfaceType) as IView;
            if (view == null)
                return null;

            // Call the strongly-typed BindViewModel(TViewModel) via the generic interface
            view.BindViewModel(viewModel);
            return view;
        }
    }
}
