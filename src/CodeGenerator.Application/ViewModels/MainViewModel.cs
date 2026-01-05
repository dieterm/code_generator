using CodeGenerator.Application.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Presentation.WinForms.ViewModels
{
    public class MainViewModel : ViewModelBase
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

        /// <summary>
        /// Indicates if there are unsaved changes
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }
        public bool IsClosing { get; internal set; }

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
    }

}
