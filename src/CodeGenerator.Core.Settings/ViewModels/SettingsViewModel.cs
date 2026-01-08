using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.Core.Settings.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public event EventHandler OkRequested = delegate { };
        public event EventHandler CancelRequested = delegate { };
        public SettingsViewModel()
        {
            OkCommand = new RelayCommand(() => OkRequested?.Invoke(this, EventArgs.Empty));
            CancelCommand = new RelayCommand(() => CancelRequested?.Invoke(this, EventArgs.Empty));
        }

        private readonly SettingSectionCollection _settingsSections = new SettingSectionCollection();
        public SettingSectionCollection SettingsSections {
            get { return _settingsSections; }
        }

        private SettingSection? _selectedSection;
        public SettingSection? SelectedSection
        {
            get { return _selectedSection; }
            set { SetProperty(ref _selectedSection, value); }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

    }
}
