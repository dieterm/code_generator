namespace CodeGenerator.WinForms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private MenuStrip _menuStrip;
    private ToolStrip _toolStrip;
    private SplitContainer _mainSplitContainer;
    private SplitContainer _leftSplitContainer;
    private TreeView _schemaTreeView;
    private PropertyGrid _propertyGrid;
    private TabControl _previewTabControl;
    private RichTextBox _jsonEditor;
    private TreeView _outputTreeView;
    private RichTextBox _previewTextBox;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusLabel;
    private ToolStripProgressBar _progressBar;
    private ListBox _logListBox;

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
        
        SuspendLayout();

        Text = "Code Generator - Domain Driven Design";
        Size = new Size(1400, 900);
        StartPosition = FormStartPosition.CenterScreen;
        Icon = SystemIcons.Application;

        _menuStrip = new MenuStrip();
        CreateMenus();

        _toolStrip = new ToolStrip();
        CreateToolbar();

        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel("Ready") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
        _progressBar = new ToolStripProgressBar { Visible = false, Width = 200 };
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel, _progressBar });

        _mainSplitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 450
        };

        _leftSplitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 400
        };

        _schemaTreeView = new TreeView
        {
            Dock = DockStyle.Fill,
            ImageList = CreateImageList(),
            ShowLines = true,
            ShowPlusMinus = true,
            ShowRootLines = true,
            HideSelection = false
        };

        _propertyGrid = new PropertyGrid
        {
            Dock = DockStyle.Fill,
            PropertySort = PropertySort.Categorized
        };

        _previewTabControl = new TabControl { Dock = DockStyle.Fill };

        var jsonTab = new TabPage("JSON Schema");
        _jsonEditor = new RichTextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10),
            WordWrap = false,
            AcceptsTab = true
        };
        jsonTab.Controls.Add(_jsonEditor);

        var outputTab = new TabPage("Output Structure");
        _outputTreeView = new TreeView
        {
            Dock = DockStyle.Fill,
            ShowLines = true,
            ShowPlusMinus = true
        };
        outputTab.Controls.Add(_outputTreeView);

        var previewTab = new TabPage("File Preview");
        _previewTextBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10),
            ReadOnly = true,
            WordWrap = false
        };
        previewTab.Controls.Add(_previewTextBox);

        var logTab = new TabPage("Log");
        _logListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 9),
            IntegralHeight = false
        };
        logTab.Controls.Add(_logListBox);

        _previewTabControl.TabPages.AddRange(new[] { jsonTab, outputTab, previewTab, logTab });

        _leftSplitContainer.Panel1.Controls.Add(_schemaTreeView);
        _leftSplitContainer.Panel2.Controls.Add(_propertyGrid);

        _mainSplitContainer.Panel1.Controls.Add(_leftSplitContainer);
        _mainSplitContainer.Panel2.Controls.Add(_previewTabControl);

        Controls.Add(_mainSplitContainer);
        Controls.Add(_toolStrip);
        Controls.Add(_menuStrip);
        Controls.Add(_statusStrip);

        MainMenuStrip = _menuStrip;

        ResumeLayout(false);
        PerformLayout();
    }

    private void CreateMenus()
    {
        var fileMenu = new ToolStripMenuItem("&File");
        fileMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&New Schema", null, OnNewSchema, Keys.Control | Keys.N),
            new ToolStripMenuItem("&Open Schema...", null, OnOpenSchema, Keys.Control | Keys.O),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Save", null, OnSaveSchema, Keys.Control | Keys.S),
            new ToolStripMenuItem("Save &As...", null, OnSaveSchemaAs),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Settings...", null, OnOpenSettings),
            new ToolStripSeparator(),
            new ToolStripMenuItem("E&xit", null, OnExit, Keys.Alt | Keys.F4)
        });

        var editMenu = new ToolStripMenuItem("&Edit");
        editMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("Add &Entity...", null, OnAddEntity),
            new ToolStripMenuItem("Add &Property...", null, OnAddProperty),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Delete", null, OnDeleteSelected, Keys.Delete)
        });

        var generateMenu = new ToolStripMenuItem("&Generate");
        generateMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Preview All", null, OnPreviewAll, Keys.F5),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Generate &All", null, OnGenerateAll, Keys.F6),
            new ToolStripMenuItem("Generate &Domain Layer", null, (s, e) => OnGenerateLayer("Entity")),
            new ToolStripMenuItem("Generate &Infrastructure Layer", null, (s, e) => OnGenerateLayer("DbContext")),
            new ToolStripMenuItem("Generate &Application Layer", null, (s, e) => OnGenerateLayer("Controller")),
            new ToolStripMenuItem("Generate &Presentation Layer", null, (s, e) => OnGenerateLayer("View_WinForms")),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Generate &Database Scripts", null, (s, e) => OnGenerateLayer("DbScript"))
        });

        var viewMenu = new ToolStripMenuItem("&View");
        viewMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Refresh Preview", null, OnRefreshPreview, Keys.F5),
            new ToolStripMenuItem("&Clear Log", null, OnClearLog)
        });

        var helpMenu = new ToolStripMenuItem("&Help");
        helpMenu.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&About...", null, OnAbout)
        });

        _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, generateMenu, viewMenu, helpMenu });
    }

    private void CreateToolbar()
    {
        _toolStrip.Items.AddRange(new ToolStripItem[]
        {
            new ToolStripButton("New", null, OnNewSchema) { ToolTipText = "New Schema" },
            new ToolStripButton("Open", null, OnOpenSchema) { ToolTipText = "Open Schema" },
            new ToolStripButton("Save", null, OnSaveSchema) { ToolTipText = "Save Schema" },
            new ToolStripSeparator(),
            new ToolStripButton("Add Entity", null, OnAddEntity) { ToolTipText = "Add New Entity" },
            new ToolStripButton("Add Property", null, OnAddProperty) { ToolTipText = "Add New Property" },
            new ToolStripSeparator(),
            new ToolStripButton("Preview", null, OnPreviewAll) { ToolTipText = "Preview Generated Code (F5)" },
            new ToolStripButton("Generate", null, OnGenerateAll) { ToolTipText = "Generate All Code (F6)" },
            new ToolStripSeparator(),
            new ToolStripButton("Settings", null, OnOpenSettings) { ToolTipText = "Open Settings" }
        });
    }

    private ImageList CreateImageList()
    {
        var imageList = new ImageList { ImageSize = new Size(16, 16) };
        return imageList;
    }
}
