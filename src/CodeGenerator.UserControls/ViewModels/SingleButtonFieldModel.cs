using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.UserControls.ViewModels
{
    public class SingleButtonFieldModel : FieldViewModelBase
    {
        private string buttonText = "Button";
        public string ButtonText
        {
            get { return buttonText; }
            set { SetProperty(ref buttonText, value); }
        }

        private RelayCommand _command;
        public RelayCommand Command
        {
            get { return _command; }
            set { SetProperty(ref _command, value); }
        }

        public override object? Target { 
            get => base.Target;
            set { 
                base.Target = value;
                Command.RaiseCanExecuteChanged();
            }
        }
    }
}
