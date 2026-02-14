using CodeGenerator.Application.ViewModels.Workspace.Datasources;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using CodeGenerator.UserControls.Views;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// View for editing table properties
    /// </summary>
    public partial class TableEditView : UserControl, IView<TableEditViewModel>
    {
        private TableEditViewModel? _viewModel;

        public TableEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(TableEditViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.PropertiesDistinctValues.CollectionChanged -= PropertiesDistinctValues_CollectionChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            // Bind fields
            txtName.BindViewModel(_viewModel.NameField);
            txtSchema.BindViewModel(_viewModel.SchemaField);
            btnLoadData.Command = _viewModel.LoadDataCommand;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.PropertiesDistinctValues.CollectionChanged += PropertiesDistinctValues_CollectionChanged;

            // Initial sync
            RebuildPropertiesPanel();
        }

        private void PropertiesDistinctValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => PropertiesDistinctValues_CollectionChanged(sender, e));
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (MultiSelectFieldModel model in e.NewItems)
                        {
                            AddPropertyPanel(model);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (MultiSelectFieldModel model in e.OldItems)
                        {
                            RemovePropertyPanel(model);
                        }
                    }
                    break;
                default:
                    RebuildPropertiesPanel();
                    break;
            }
        }

        private void RebuildPropertiesPanel()
        {
            pnlProperties.SuspendLayout();
            pnlProperties.Controls.Clear();

            if (_viewModel != null)
            {
                foreach (var model in _viewModel.PropertiesDistinctValues)
                {
                    AddPropertyPanel(model);
                }
            }

            pnlProperties.ResumeLayout(true);
        }

        private void AddPropertyPanel(MultiSelectFieldModel model)
        {
            pnlProperties.SuspendLayout();

            var container = new Panel
            {
                Tag = model,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 6)
            };

            var multiSelectField = new MultiSelectField
            {
                Dock = DockStyle.Top
            };
            multiSelectField.BindViewModel(model);

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 2, 0, 0)
            };

            var btnCreateEntities = new Button
            {
                Text = "Create Entities",
                AutoSize = true
            };
            btnCreateEntities.Click += (s, e) => ShowDomainContextMenu(btnCreateEntities, model, isEntity: true);

            var btnCreateValueTypes = new Button
            {
                Text = "Create Value Types",
                AutoSize = true
            };
            btnCreateValueTypes.Click += (s, e) => ShowDomainContextMenu(btnCreateValueTypes, model, isEntity: false);

            buttonsPanel.Controls.Add(btnCreateEntities);
            buttonsPanel.Controls.Add(btnCreateValueTypes);

            container.Controls.Add(buttonsPanel);
            container.Controls.Add(multiSelectField);

            pnlProperties.Controls.Add(container);
            container.BringToFront();

            pnlProperties.ResumeLayout(true);
        }

        private void RemovePropertyPanel(MultiSelectFieldModel model)
        {
            var panelToRemove = pnlProperties.Controls
                .OfType<Panel>()
                .FirstOrDefault(p => p.Tag == model);

            if (panelToRemove != null)
            {
                pnlProperties.Controls.Remove(panelToRemove);
                panelToRemove.Dispose();
            }
        }

        

        private void ShowDomainContextMenu(Control anchor, MultiSelectFieldModel model, bool isEntity)
        {
            var domains = GetAllDomains();
            if (domains.Count == 0) return;

            var contextMenu = new ContextMenuStrip();

            foreach (var domain in domains)
            {
                var menuItem = new ToolStripMenuItem($"{domain.Scope?.Name} / {domain.GetDomainHierarchicalName()}");
                menuItem.Tag = domain;
                menuItem.Click += (s, e) =>
                {
                    if (isEntity)
                        _viewModel?.OnCreateEntities(model, domain);
                    else
                        _viewModel?.OnCreateValueTypes(model, domain);
                };
                contextMenu.Items.Add(menuItem);
            }

            contextMenu.Show(anchor, new Point(0, anchor.Height));
        }

        private List<DomainArtifact> GetAllDomains()
        {
            var domains = new List<DomainArtifact>();
            var workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            var workspace = workspaceContextProvider.CurrentWorkspace;
            if (workspace == null) return domains;

            foreach (var scope in workspace.Scopes)
            {
                CollectDomains(scope, domains);
            }

            return domains;
        }

        private void CollectDomains(ScopeArtifact scope, List<DomainArtifact> domains)
        {
            if (scope.Domains != null)
            {
                foreach (var domain in scope.Domains)
                {
                    domains.Add(domain);
                    if (domain.SubDomains != null)
                    {
                        CollectSubDomains(domains, domain);
                    }
                }
            }

            if (scope.SubScopes != null)
            {
                foreach (var subScope in scope.SubScopes)
                {
                    CollectDomains(subScope, domains);
                }
            }
        }

        private static void CollectSubDomains(List<DomainArtifact> domains, DomainArtifact domain)
        {
            foreach (var subDomain in domain.SubDomains)
            {
                domains.Add(subDomain);
                if (subDomain.SubDomains != null)
                {
                    CollectSubDomains(domains, subDomain);
                }
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle property changes if needed
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((TableEditViewModel)(object)viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    _viewModel.PropertiesDistinctValues.CollectionChanged -= PropertiesDistinctValues_CollectionChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
