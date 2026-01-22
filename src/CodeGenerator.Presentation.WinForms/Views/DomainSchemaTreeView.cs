using CodeGenerator.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class DomainSchemaTreeView : UserControl
    {
        public DomainSchemaTreeViewModel? ViewModel { get; set; }
        public DomainSchemaTreeView()
        {
            InitializeComponent();

            Disposed += DomainSchemaTreeView_Disposed;
        }

        private void DomainSchemaTreeView_Disposed(object? sender, EventArgs e)
        {
            if (ViewModel != null)
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        public void BindViewModel(DomainSchemaTreeViewModel? viewModel)
        {
            if(ViewModel != null)
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            ViewModel = viewModel;
            RefreshTreeView();
            if (viewModel != null)
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Parent == null) return;
            if (e.PropertyName == nameof(DomainSchemaTreeViewModel.DomainSchema))
            {
                RefreshTreeView();
            }
        }

        private void RefreshTreeView()
        {
            // Update the view with the new domain schema
            treeView.Nodes.Clear();
            if (ViewModel?.DomainSchema?.Definitions != null)
            {
                // Populate the tree view with the new domain schema
                foreach (var (entityName, entityDef) in ViewModel.DomainSchema.Definitions)
                {
                    var entityNode = new Syncfusion.Windows.Forms.Tools.TreeNodeAdv(entityName);
                    entityNode.Tag = entityDef;
                    if (entityDef.Properties != null)
                    {
                        foreach (var (propName, propDef) in entityDef.Properties)
                        {
                            var propertyNode = new Syncfusion.Windows.Forms.Tools.TreeNodeAdv(propName);
                            propertyNode.Tag = propDef;
                            entityNode.Nodes.Add(propertyNode);
                        }
                    }
                    treeView.Nodes.Add(entityNode);
                }
            }
        }


    }
}
