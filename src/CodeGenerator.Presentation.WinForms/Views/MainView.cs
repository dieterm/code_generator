using CodeGenerator.Application.Events.DomainSchema;
using CodeGenerator.Application.MessageBus;
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.Views;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using System.ComponentModel;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms
{
    public partial class MainView : RibbonForm, IView<MainViewModel>
    {
        private MainViewModel? _mainViewModel;

        public MainView()
        {
            InitializeComponent();
            
            if (DesignMode) return;

            BuildRibbon();

            SubscribeToMessageBus();
        }

        private void SubscribeToMessageBus()
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();
            messageBus.Subscribe<DomainSchemaLoadedEvent>(OnDomainSchemaLoaded);
        }

        private void OnDomainSchemaLoaded(DomainSchemaLoadedEvent e)
        {
            var schemaTreeView = new DomainSchemaTreeView();
            schemaTreeView.BindViewModel(e.TreeViewModel);
            dockingManager.DockControl(schemaTreeView, this, DockingStyle.Left, 4);
            dockingManager.SetEnableDocking(schemaTreeView, true);
            dockingManager.SetControlSize(schemaTreeView, new Size(300, this.Height - 50));
            dockingManager.SetDockLabel(schemaTreeView, "Domain Schema");
            lblStatus.Text = e.FilePath;
        }

        public void BuildRibbon()
        {
            // Build Ribbon Model
            var model = RibbonBuilder.Create()
                .AddTab("tabDomainModel", "Domain Model")
                    .AddToolStrip("toolstripNew", "New")
                        .AddButton("btnOpen", "Open")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage(nameof(LucideIcons__000000.folder_open))
                            .OnClick((e) => _mainViewModel?.OpenCommand.Execute(null))
                            
                        .AddButton("btnGenerate", "Generate")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage(nameof(LucideIcons__000000.scroll_text))
                            .OnClick((e) => _mainViewModel?.GenerateCommand.Execute(null))
                .Build();
            // Render Ribbon
            ServiceProviderHolder.GetRequiredService<IRibbonRenderer>()
                .SetResourceManager(LucideIcons__000000.ResourceManager) // Image source
                .Render(model, ribbonControl);
        }

        public void BindViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            
            // Bind New Command
            btnNew.Click += (s, e) => _mainViewModel.NewCommand.Execute(null);
            
            // Bind Open Command
            btnOpen.Click += (s, e) => _mainViewModel.OpenCommand.Execute(null);
            
            // Bind Save Command
            btnSave.Click += (s, e) => _mainViewModel.SaveCommand.Execute(null);
            
            // Bind Generate Command
            //btnGenerate.Click += (s, e) => _mainViewModel.GenerateCommand.Execute(null);
            
            // Bind Exit Command
            btnExit.Click += (s, e) => _mainViewModel.ExitCommand.Execute(null);

            // Handle FormClosing event
            FormClosing += OnFormClosing;

            // Bind Status Label
            _mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
            
        }

        private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(_mainViewModel == null) throw new InvalidOperationException("MainViewModel is not bound.");
            
            if (e.PropertyName == nameof(MainViewModel.StatusLabel))
            {
                lblStatus.Text = _mainViewModel.StatusLabel;
            }
            if(e.PropertyName == nameof(MainViewModel.ProgressLabel))
            {
                lblProgress.Text = _mainViewModel.ProgressLabel;
            }
            if(e.PropertyName == nameof(MainViewModel.ProgressValue))
            {
                if (_mainViewModel.ProgressValue == null)
                {
                    pbProgress.Style = ProgressBarStyle.Marquee;
                    return;
                } else { 
                    pbProgress.Style = ProgressBarStyle.Continuous;
                    pbProgress.Value = _mainViewModel.ProgressValue.Value;
                }
            }
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_mainViewModel.IsClosing)
            {
                _mainViewModel.ExitCommand.Execute(null);
                e.Cancel = true;
            } else { 
                e.Cancel = !_mainViewModel.IsClosing;
            }
        }
    }
}
