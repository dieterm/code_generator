using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.Properties;
using CodeGenerator.WinForms.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.WinForms.Controls;

public partial class DomainSchemaEditor : UserControl
{
    private readonly DomainSchemaEditorViewModel _viewModel;

    public DomainSchemaEditor()
    {
        _viewModel = new DomainSchemaEditorViewModel();
        InitializeComponent();
        SetupEventHandlers();
        LoadImages();
    }

    public DomainSchemaEditorViewModel ViewModel => _viewModel;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DomainSchema? Schema
    {
        get => _viewModel.Schema;
        set
        {
            _viewModel.Schema = value;
            RefreshTreeView();
        }
    }

    public object? SelectedItem => _viewModel.SelectedItem;

    public event EventHandler<TreeViewEventArgs>? AfterSelect;
    public event EventHandler<TreeNodeMouseClickEventArgs>? NodeMouseDoubleClick;

    private void SetupEventHandlers()
    {
        _schemaTreeView.AfterSelect += SchemaTreeView_AfterSelect;
        _schemaTreeView.NodeMouseDoubleClick += SchemaTreeView_NodeMouseDoubleClick;
        _viewModel.SchemaChanged += (s, e) => RefreshTreeView();
    }

    private void LoadImages()
    {
        try
        {
            _imageList.Images.Add("Schema", Resources.SchemaIcon);
            _imageList.Images.Add("ValueTypes", Resources.ValueTypesIcon);
            _imageList.Images.Add("Entities", Resources.EntitiesIcon);
            _imageList.Images.Add("CodeGenSettings", Resources.SettingsIcon);
            _imageList.Images.Add("DatabaseSettings", Resources.SettingsIcon);
            _imageList.Images.Add("DDDSettings", Resources.SettingsIcon);
        }
        catch
        {
            // If resources are not available, continue without images
        }
    }

    private void SchemaTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag != null)
        {
            _viewModel.SelectedItem = e.Node.Tag;
            ShowEditorForSelectedItem(e.Node.Tag);
        }

        AfterSelect?.Invoke(this, e);
    }
    private void ShowEditorForSelectedItem(object item)
    {
        _editorPanel.Controls.Clear();
        _domainSchemaEditView = null;
        _dddMetadataEditView = null;
        _entityEditView = null;
        _propertyEditView = null;
        _codeGenMetadataEditView = null;
        _databaseMetadataEditView = null;

        UserControl? editorView = null;

        switch (item)
        {
            case DomainSchema schema:
                _domainSchemaEditView = new DomainSchemaEditView();
                _domainSchemaEditView.LoadSchema(schema);
                editorView = _domainSchemaEditView;
                break;

            case CodeGenMetadata codeGenMetadata:
                _codeGenMetadataEditView = new CodeGenMetadataEditView();
                _codeGenMetadataEditView.LoadMetadata(codeGenMetadata);
                editorView = _codeGenMetadataEditView;
                break;

            case DatabaseMetadata databaseMetadata:
                _databaseMetadataEditView = new DatabaseMetadataEditView();
                _databaseMetadataEditView.LoadMetadata(databaseMetadata);
                editorView = _databaseMetadataEditView;
                break;

            case DomainDrivenDesignMetadata dddMetadata:
                _dddMetadataEditView = new DomainDrivenDesignMetadataEditView();
                _dddMetadataEditView.LoadMetadata(dddMetadata);
                editorView = _dddMetadataEditView;
                break;

            case EntityDefinition entity:
                _entityEditView = new EntityDefinitionEditView();
                _entityEditView.LoadEntity(entity);
                editorView = _entityEditView;
                break;

            case PropertyDefinition property:
                _propertyEditView = new PropertyDefinitionEditView();
                _propertyEditView.LoadProperty(property, Schema);
                editorView = _propertyEditView;
                break;

            case EntityDomainDrivenDesignMetadata:
            case PropertyDomainDrivenDesignMetadata:
                var label = new Label();
                label.Text = $"Editor for {item.GetType().Name} not yet implemented.\nPlease use the Property Editor form for detailed editing.";
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Font = new Font(Font.FontFamily, 10);
                _editorPanel.Controls.Add(label);
                return;
        }

        if (editorView != null)
        {
            editorView.Dock = DockStyle.Fill;
            _editorPanel.Controls.Add(editorView);
        }
    }
    private void SchemaTreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        NodeMouseDoubleClick?.Invoke(this, e);
    }

    public void RefreshTreeView()
    {
        _schemaTreeView.BeginUpdate();
        _schemaTreeView.Nodes.Clear();

        if (_viewModel.Schema == null)
        {
            _schemaTreeView.EndUpdate();
            return;
        }

        var rootNode = new TreeNode(_viewModel.Schema.Title ?? "Domain Schema")
        {
            ImageKey = "Schema",
            SelectedImageKey = "Schema",
            Tag = _viewModel.Schema
        };

        if (_viewModel.Schema.CodeGenMetadata == null)
            _viewModel.Schema.CodeGenMetadata = new CodeGenMetadata();

        if (_viewModel.Schema.CodeGenMetadata != null)
        {
            var codeGenNode = new TreeNode("Code Generation Settings")
            {
                Tag = _viewModel.Schema.CodeGenMetadata,
                ImageKey = "CodeGenSettings",
                SelectedImageKey = "CodeGenSettings"
            };
            rootNode.Nodes.Add(codeGenNode);
        }

        if (_viewModel.Schema.DatabaseMetadata == null)
            _viewModel.Schema.DatabaseMetadata = new DatabaseMetadata();

        if (_viewModel.Schema.DatabaseMetadata != null)
        {
            var dbNode = new TreeNode("Database Settings")
            {
                Tag = _viewModel.Schema.DatabaseMetadata,
                ImageKey = "DatabaseSettings",
                SelectedImageKey = "DatabaseSettings"
            };
            rootNode.Nodes.Add(dbNode);
        }

        if (_viewModel.Schema.DomainDrivenDesignMetadata == null)
            _viewModel.Schema.DomainDrivenDesignMetadata = new DomainDrivenDesignMetadata();

        if (_viewModel.Schema.DomainDrivenDesignMetadata != null)
        {
            var dddNode = new TreeNode("Domain Driven Design Settings")
            {
                Tag = _viewModel.Schema.DomainDrivenDesignMetadata,
                ImageKey = "DDDSettings",
                SelectedImageKey = "DDDSettings"
            };
            rootNode.Nodes.Add(dddNode);
        }

        if (_viewModel.Schema.Definitions != null)
        {
            var valueTypeNodes = new TreeNode("Value Types")
            {
                ImageKey = "ValueTypes",
                SelectedImageKey = "ValueTypes"
            };
            var getEntityName = (string name, EntityDefinition d) => !string.IsNullOrWhiteSpace(d.Type) ? $"{name}: {d.Type}" : name;
            Func<string, PropertyDefinition, string> getPropertyName = (string name, PropertyDefinition p) =>
            {
                if (p.Type == "array" && p.Items != null)
                {
                    var itemType = !string.IsNullOrWhiteSpace(p.Items.Type) ? p.Items.Type : (!string.IsNullOrWhiteSpace(p.Items.Ref) ? p.Items.Ref : "object");
                    return $"{name}: {itemType}[]";
                }
                else if (!string.IsNullOrWhiteSpace(p.Ref))
                {
                    return $"{name}: {p.Ref}";
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(p.Type) ? $"{name}: {p.Type}" : name;
                }
            };
            foreach (var (name, entity) in _viewModel.Schema.Definitions.Where(d =>
                d.Value.DomainDrivenDesignMetadata != null &&
                d.Value.DomainDrivenDesignMetadata.ValueObject == true))
            {

                var valueTypeNode = new TreeNode(getEntityName(name, entity)) { Tag = entity };

                if (entity.Properties != null)
                {
                    foreach (var (propName, prop) in entity.Properties)
                    {
                        var propNode = new TreeNode(getPropertyName(propName, prop))
                        {
                            Tag = prop
                        };
                        valueTypeNode.Nodes.Add(propNode);
                    }
                }

                valueTypeNodes.Nodes.Add(valueTypeNode);
            }
            rootNode.Nodes.Add(valueTypeNodes);

            var entitiesNode = new TreeNode("Entities")
            {
                ImageKey = "Entities",
                SelectedImageKey = "Entities"
            };

            foreach (var (name, entity) in _viewModel.Schema.Definitions.Where(d =>
                d.Value.DomainDrivenDesignMetadata == null ||
                d.Value.DomainDrivenDesignMetadata.ValueObject == false))
            {
                var entityNode = new TreeNode(getEntityName(name, entity)) { Tag = entity };

                if (entity.Properties != null)
                {
                    foreach (var (propName, prop) in entity.Properties)
                    {
                        var propNode = new TreeNode(getPropertyName(propName, prop))
                        {
                            Tag = prop
                        };
                        entityNode.Nodes.Add(propNode);
                    }
                }

                entitiesNode.Nodes.Add(entityNode);
            }
            rootNode.Nodes.Add(entitiesNode);
        }

        _schemaTreeView.Nodes.Add(rootNode);
        rootNode.ExpandAll();
        _schemaTreeView.EndUpdate();
    }

    public TreeNode? SelectedNode => _schemaTreeView.SelectedNode;

    public void ClearSelection()
    {
        _schemaTreeView.SelectedNode = null;
        _editorPanel.Controls.Clear();
        _viewModel.SelectedItem = null;
    }
}

