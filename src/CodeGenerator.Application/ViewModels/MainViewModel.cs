using CodeGenerator.Application.Services;
using CodeGenerator.Core.Generators;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Presentation.WinForms.ViewModels
{
    public class MainViewModel : ViewModelBase, IProgress<GenerationProgress>
    {
        private readonly IMessageBoxService _messageBoxService;
        private bool _isDirty;

        // Events
        public event EventHandler? NewRequested;
        public event EventHandler? OpenRequested;
        public event EventHandler? SaveRequested;
        public event EventHandler? SaveAsRequested;
        public event EventHandler? GenerateRequested;
        
        /// <summary>
        /// Event raised when closing is requested. 
        /// Subscribe to this to handle unsaved changes confirmation.
        /// </summary>
        public event EventHandler? ClosingRequested;

        // Commands
        public RelayCommand NewCommand { get; }
        public RelayCommand OpenCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand SaveAsCommand { get; }
        public RelayCommand GenerateCommand { get; }
        public RelayCommand ExitCommand { get; }
        
        private string _statusLabel;
        public string StatusLabel{
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
        }

        private string _progressLabel;
        public string ProgressLabel
        {
            get => _progressLabel;
            set => SetProperty(ref _progressLabel, value);
        }

        private int? _progressValue;
        public int? ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        /// <summary>
        /// Indicates if there are unsaved changes
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }
        public bool IsClosing { get; internal set; }

        private RibbonViewModel _ribbonViewModel;
        public RibbonViewModel RibbonViewModel { 
            get => _ribbonViewModel;
            set => SetProperty(ref _ribbonViewModel, value);
        }

        // Constructor
        public MainViewModel(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            
            NewCommand = new RelayCommand(_ => NewRequested?.Invoke(this, EventArgs.Empty));
            OpenCommand = new RelayCommand(_ => OpenRequested?.Invoke(this, EventArgs.Empty));
            SaveCommand = new RelayCommand(_ => SaveRequested?.Invoke(this, EventArgs.Empty));
            SaveAsCommand = new RelayCommand(_ => SaveAsRequested?.Invoke(this, EventArgs.Empty));
            GenerateCommand = new RelayCommand(_ => GenerateRequested?.Invoke(this, EventArgs.Empty));
            ExitCommand = new RelayCommand(_ => ClosingRequested?.Invoke(this, EventArgs.Empty));
        }

        public void Report(GenerationProgress progress)
        {
            if(progress.IsIndeterminate)
                ProgressValue = null;
            else
                ProgressValue = progress.PercentComplete;

            ProgressLabel = progress.Message;
        }
    }

}
