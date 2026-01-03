namespace CodeGenerator.WinForms.Controls;

partial class DomainSchemaEditor
{
    private System.ComponentModel.IContainer components = null;

    private SplitContainer _splitContainer;
    private TreeView _schemaTreeView;
    private Panel _editorPanel;
    private ImageList _imageList;

    private DomainSchemaEditView? _domainSchemaEditView;
    private DomainDrivenDesignMetadataEditView? _dddMetadataEditView;
    private EntityDefinitionEditView? _entityEditView;
    private PropertyDefinitionEditView? _propertyEditView;
    private CodeGenMetadataEditView? _codeGenMetadataEditView;
    private DatabaseMetadataEditView? _databaseMetadataEditView;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // SplitContainer
        _splitContainer = new SplitContainer();
        _splitContainer.Dock = DockStyle.Fill;
        _splitContainer.Orientation = Orientation.Horizontal;
        _splitContainer.SplitterDistance = 560;

        // TreeView
        _schemaTreeView = new TreeView();
        _schemaTreeView.Dock = DockStyle.Fill;
        _schemaTreeView.HideSelection = false;
        _schemaTreeView.ImageIndex = 0;
        _schemaTreeView.SelectedImageIndex = 0;

        // ImageList
        _imageList = new ImageList(components);
        _imageList.ColorDepth = ColorDepth.Depth32Bit;
        _imageList.ImageSize = new Size(16, 16);
        _imageList.TransparentColor = Color.Transparent;

        _schemaTreeView.ImageList = _imageList;

        // Editor Panel (replaces PropertyGrid)
        _editorPanel = new Panel();
        _editorPanel.Dock = DockStyle.Fill;
        _editorPanel.BackColor = SystemColors.Control;
        _editorPanel.AutoScroll = true;

        // Add controls to split container
        _splitContainer.Panel1.Controls.Add(_schemaTreeView);
        _splitContainer.Panel2.Controls.Add(_editorPanel);

        // Add split container to UserControl
        Controls.Add(_splitContainer);
    }
}
