using CodeGenerator.Application.ViewModels.Generation;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// Main container view for generation results.
    /// Contains a horizontal SplitContainer with:
    /// - Top: GenerationTreeView (artifact tree)
    /// - Bottom: GenerationDetailsView (artifact property details)
    /// </summary>
    public partial class GenerationResultTreeView : UserControl, IView<GenerationResultTreeViewModel>
    {
        private GenerationResultTreeViewModel? _viewModel;
        private Generation.GenerationTreeView? _generationTreeView;
        private Generation.GenerationDetailsView? _generationDetailsView;

        public GenerationResultTreeView()
        {
            InitializeComponent();
            Disposed += GenerationResultTreeView_Disposed;
        }

        private void GenerationResultTreeView_Disposed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }

        public GenerationResultTreeViewModel? ViewModel => _viewModel;

        public void BindViewModel(GenerationResultTreeViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
                BindSubViews();
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((GenerationResultTreeViewModel)(object)viewModel);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GenerationResultTreeViewModel.TreeViewModel) ||
                e.PropertyName == nameof(GenerationResultTreeViewModel.DetailsViewModel))
            {
                BindSubViews();
            }
        }

        private void BindSubViews()
        {
            if (_viewModel?.TreeViewModel != null)
            {
                if (_generationTreeView == null)
                {
                    _generationTreeView = new Generation.GenerationTreeView();
                    _generationTreeView.Dock = DockStyle.Fill;
                    splitContainer1.Panel1.Controls.Clear();
                    splitContainer1.Panel1.Controls.Add(_generationTreeView);
                }
                _generationTreeView.BindViewModel(_viewModel.TreeViewModel);
            }

            if (_viewModel?.DetailsViewModel != null)
            {
                if (_generationDetailsView == null)
                {
                    _generationDetailsView = new Generation.GenerationDetailsView();
                    _generationDetailsView.Dock = DockStyle.Fill;
                    splitContainer1.Panel2.Controls.Clear();
                    splitContainer1.Panel2.Controls.Add(_generationDetailsView);
                }
                _generationDetailsView.BindViewModel(_viewModel.DetailsViewModel);
            }
        }
    }
}
