using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.Views;
using Syncfusion.Windows.Forms.Tools;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms
{
    public partial class MainView : RibbonForm, IView<MainViewModel>
    {
        private MainViewModel? _mainViewModel;

        public MainView()
        {
            InitializeComponent();

            BuildRibbon();
        }

        public void BuildRibbon()
        {
            var model = RibbonBuilder.Create()
                .AddTab("tabDomainModel", "Domain Model")
                    .AddToolStrip("btnNew", "New")
                        .AddButton("btnOpen", "Open")
                            .WithSize(RibbonButtonSize.Large)
                            .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                            .WithImage(LucideIcons__000000.folder_open)
                            .OnClick(OnOpenButtonClicked)
                            .Build();
            
            ServiceProviderHolder.GetRequiredService<IRibbonRenderer>().Render(model, ribbonControl);
        }

        private void OnOpenButtonClicked(EventArgs args)
        {
            var myForm = new Form { MdiParent = this, FormBorderStyle = FormBorderStyle.None, Text = "New Child Form", Size = new Size(800, 600) };
            myForm.Controls.Add(new Label { Text = "This is a docked form.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            
            dockingManager.DockControl(myForm, this, Syncfusion.Windows.Forms.Tools.DockingStyle.Right, 10);
            dockingManager.SetEnableDocking(myForm, true);
            dockingManager.SetControlSize(myForm, new Size(300, this.Height - 50));
            dockingManager.SetDockLabel(myForm, "Docked Form");
            //this.dockingManager.SetAsMDIChild(myForm, true);
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
