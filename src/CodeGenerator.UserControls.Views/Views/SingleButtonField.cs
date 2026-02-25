using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
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
    public partial class SingleButtonField : UserControl, IView<SingleButtonFieldModel>
    {
        private SingleButtonFieldModel? _viewModel;
        public SingleButtonField()
        {
            InitializeComponent();
            lblLabel.EnsureLabelVisible(btnCommand, null);
        }

        public void BindViewModel(SingleButtonFieldModel viewModel)
        {
            DataBindings.Clear();
            lblLabel.DataBindings.Clear();
            btnCommand.DataBindings.Clear();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataBindings.Add("Visible", viewModel, nameof(viewModel.Visible), false, DataSourceUpdateMode.OnPropertyChanged).ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            lblLabel.DataBindings.Add(nameof(lblLabel.Text), _viewModel, nameof(_viewModel.Label));
            btnCommand.DataBindings.Add(nameof(btnCommand.Text), _viewModel, nameof(_viewModel.ButtonText));
            btnCommand.Command = _viewModel.Command;
            btnCommand.CommandParameter = _viewModel.Target;
            _viewModel.Command.RaiseCanExecuteChanged();
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.Command))
                {
                    btnCommand.Command = _viewModel.Command;
                }
                else if (e.PropertyName == nameof(_viewModel.Target))
                {
                    btnCommand.CommandParameter = _viewModel.Target;
                    _viewModel.Command.RaiseCanExecuteChanged();
                }
            };
            //btnCommand.DataBindings.Add(nameof(btnCommand.Command), _viewModel, nameof(_viewModel.Command));
            //btnCommand.DataBindings.Add(nameof(btnCommand.CommandParameter), _viewModel, nameof(_viewModel.Target), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((SingleButtonFieldModel)(object)viewModel);
        }
    }
}
