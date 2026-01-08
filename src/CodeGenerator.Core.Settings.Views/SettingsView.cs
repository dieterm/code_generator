using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CodeGenerator.Core.Settings.Views
{
    public partial class SettingsView : UserControl, ISettingsView
    {
        public SettingsViewModel? ViewModel { get; set; }
        public SettingsView()
        {
            InitializeComponent();
        }

        public void BindViewModel(SettingsViewModel viewModel)
        {
            this.ViewModel = viewModel;
            RefreshSettingsSections();
            RefreshSettingsItems();
            btnOk.Command = this.ViewModel.OkCommand;
            btnCancel.Command = this.ViewModel.CancelCommand;
            this.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ViewModel.SelectedSection))
                {
                    RefreshSettingsItems();
                }
            };
        }

        private void RefreshSettingsSections()
        {
            tvSettingSection.Nodes.Clear();
            if (this.ViewModel != null)
            {
                RefreshSettingsSections(tvSettingSection.Nodes, this.ViewModel.SettingsSections);

            }
        }
        private void RefreshSettingsSections(TreeNodeCollection target, SettingSectionCollection sections)
        {
            foreach (var section in sections)
            {
                var node = new TreeNode(section.Name)
                {
                    Tag = section
                };
                target.Add(node);
                RefreshSettingsSections(node.Nodes, section.Sections);
            }
        }

        private void RefreshSettingsItems()
        {
            var oldFieldViews = splitContainer1.Panel2.Controls.OfType<UserControl>().ToList();
            splitContainer1.Panel2.Controls.Clear();
            foreach(var oldFieldView in oldFieldViews)
            {
                // dispose old views and viewmodels
                oldFieldView.Dispose();
            }
            if (this.ViewModel.SelectedSection != null)
            {
                foreach (var settingsItem in this.ViewModel.SelectedSection.Items)
                {
                    var fieldView = CreateFieldView(settingsItem);
                    if (fieldView != null)
                    {
                        fieldView.Dock = DockStyle.Top;
                        splitContainer1.Panel2.Controls.Add(fieldView);
                        splitContainer1.Panel2.Controls.SetChildIndex(fieldView, 0);
                    }
                }
            }
        }

        private UserControl? CreateFieldView(ISettingsItem settingsItem)
        {
            var fieldName = settingsItem.FieldViewModel.GetType().Name.Replace("Model", "");
            var userControl = ServiceProviderHolder.ServiceProvider.GetRequiredKeyedService<UserControl>(fieldName);
            (userControl as IView).BindViewModel((ViewModelBase)settingsItem.FieldViewModel);
            return userControl;
        }

        private void tvSettingSection_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (ViewModel == null) return;

            if(e.Node?.Tag is SettingSection section)
            {
                ViewModel.SelectedSection = section;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
           BindViewModel((SettingsViewModel)(object)viewModel);
        }
    }
}
