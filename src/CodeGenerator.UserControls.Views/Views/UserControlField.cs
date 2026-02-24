using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.DotNet.DesignTools.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.UserControls.Views.Views
{
    public partial class UserControlField : UserControl, IView<UserControlFieldModel>
    {
        private UserControlFieldModel? _viewModel;
        public UserControlField()
        {
            InitializeComponent();
        }

        public void BindViewModel(UserControlFieldModel viewModel)
        {
            if(_viewModel == viewModel)
                return;

            if(_viewModel!=null)
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

            _viewModel = viewModel;

            LoadValueViewModel();

            if (_viewModel != null) { 
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserControlFieldModel.Value))
            {
                LoadValueViewModel();
            }
        }

        private void LoadValueViewModel()
        {
            Controls.Clear();
            var viewModel = _viewModel?.Value as IViewModel;
            if (viewModel == null) return;

            var viewFactory = ServiceProviderHolder.GetRequiredService<IViewFactory>();
            var fieldView = viewFactory.CreateView(viewModel);
            if (fieldView is UserControl userControl)
            {
                fieldView.BindViewModel(viewModel);
                
                Size = userControl.Size;
                Controls.Add(userControl);
                userControl.Dock = DockStyle.Fill;
            } 
            else
            {
                throw new InvalidOperationException($"The view for {viewModel.GetType().FullName} must be a UserControl.");
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((UserControlFieldModel)(object)viewModel);
        }
    }
}
