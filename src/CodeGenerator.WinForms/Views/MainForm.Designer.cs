using CodeGenerator.WinForms.Properties;

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
    private Panel _userControlPreviewPanel;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusLabel;
    private ToolStripProgressBar _progressBar;
    private ListBox _logListBox;
    private ToolStripButton _newButton;
    private ToolStripButton _openButton;
    private ToolStripButton _saveButton;
    private ToolStripSeparator _separator1;
    private ToolStripButton _addEntityButton;
    private ToolStripButton _addPropertyButton;
    private ToolStripSeparator _separator2;
    private ToolStripButton _previewButton;
    private ToolStripButton _generateButton;
    private ToolStripSeparator _separator3;
    private ToolStripButton _settingsButton;
    private ImageList _treeViewImageList;

    // Menu items - File Menu
    private ToolStripMenuItem _fileMenu;
    private ToolStripMenuItem _newSchemaMenuItem;
    private ToolStripMenuItem _openSchemaMenuItem;
    private ToolStripSeparator _fileMenuSeparator1;
    private ToolStripMenuItem _saveMenuItem;
    private ToolStripMenuItem _saveAsMenuItem;
    private ToolStripSeparator _fileMenuSeparator2;
    private ToolStripMenuItem _settingsMenuItem;
    private ToolStripSeparator _fileMenuSeparator3;
    private ToolStripMenuItem _exitMenuItem;

    // Menu items - Edit Menu
    private ToolStripMenuItem _editMenu;
    private ToolStripMenuItem _addEntityMenuItem;
    private ToolStripMenuItem _addPropertyMenuItem;
    private ToolStripSeparator _editMenuSeparator;
    private ToolStripMenuItem _deleteMenuItem;

    // Menu items - Generate Menu
    private ToolStripMenuItem _generateMenu;
    private ToolStripMenuItem _previewAllMenuItem;
    private ToolStripSeparator _generateMenuSeparator1;
    private ToolStripMenuItem _generateAllMenuItem;
    private ToolStripMenuItem _generateDomainMenuItem;
    private ToolStripMenuItem _generateInfraMenuItem;
    private ToolStripMenuItem _generateAppMenuItem;
    private ToolStripMenuItem _generatePresentationMenuItem;
    private ToolStripSeparator _generateMenuSeparator2;
    private ToolStripMenuItem _generateDbScriptMenuItem;

    // Menu items - View Menu
    private ToolStripMenuItem _viewMenu;
    private ToolStripMenuItem _refreshPreviewMenuItem;
    private ToolStripMenuItem _clearLogMenuItem;

    // Menu items - Help Menu
    private ToolStripMenuItem _helpMenu;
    private ToolStripMenuItem _aboutMenuItem;

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        _menuStrip = new MenuStrip();
        _toolStrip = new ToolStrip();
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _progressBar = new ToolStripProgressBar();
        _mainSplitContainer = new SplitContainer();
        _leftSplitContainer = new SplitContainer();
        _schemaTreeView = new TreeView();
        _propertyGrid = new PropertyGrid();
        _previewTabControl = new TabControl();
        jsonTab = new TabPage();
        _jsonEditor = new RichTextBox();
        outputTab = new TabPage();
        _outputTreeView = new TreeView();
        previewTab = new TabPage();
        _previewTextBox = new RichTextBox();
        userControlTab = new TabPage();
        _userControlPreviewPanel = new Panel();
        logTab = new TabPage();
        _logListBox = new ListBox();
        _statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).BeginInit();
        _mainSplitContainer.Panel1.SuspendLayout();
        _mainSplitContainer.Panel2.SuspendLayout();
        _mainSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_leftSplitContainer).BeginInit();
        _leftSplitContainer.Panel1.SuspendLayout();
        _leftSplitContainer.Panel2.SuspendLayout();
        _leftSplitContainer.SuspendLayout();
        _previewTabControl.SuspendLayout();
        jsonTab.SuspendLayout();
        outputTab.SuspendLayout();
        previewTab.SuspendLayout();
        userControlTab.SuspendLayout();
        logTab.SuspendLayout();
        SuspendLayout();
        //
        // _treeViewImageList
        //
        _treeViewImageList = new ImageList();
        _treeViewImageList.ImageSize = new Size(16, 16);

        _treeViewImageList.Images.Add("Schema", Resources.SchemaIcon);
        _treeViewImageList.Images.Add("ValueTypes", Resources.ValueTypesIcon);
        _treeViewImageList.Images.Add("Entities", Resources.EntitiesIcon);
        _treeViewImageList.Images.Add("CodeGenSettings", Resources.SettingsIcon);
        _treeViewImageList.Images.Add("DatabaseSettings", Resources.SettingsIcon);
        _treeViewImageList.Images.Add("DDDSettings", Resources.SettingsIcon);
        // 
        // _menuStrip
        // 
        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Size = new Size(1384, 24);
        _menuStrip.TabIndex = 2;
        // File Menu
        _fileMenu = new ToolStripMenuItem();
        _fileMenu.Text = "&File";

        _newSchemaMenuItem = new ToolStripMenuItem();
        _newSchemaMenuItem.Text = "&New Schema";
        _newSchemaMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        _newSchemaMenuItem.Click += OnNewSchema;

        _openSchemaMenuItem = new ToolStripMenuItem();
        _openSchemaMenuItem.Text = "&Open Schema...";
        _openSchemaMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        _openSchemaMenuItem.Click += OnOpenSchema;

        _fileMenuSeparator1 = new ToolStripSeparator();

        _saveMenuItem = new ToolStripMenuItem();
        _saveMenuItem.Text = "&Save";
        _saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        _saveMenuItem.Click += OnSaveSchema;

        _saveAsMenuItem = new ToolStripMenuItem();
        _saveAsMenuItem.Text = "Save &As...";
        _saveAsMenuItem.Click += OnSaveSchemaAs;

        _fileMenuSeparator2 = new ToolStripSeparator();

        _settingsMenuItem = new ToolStripMenuItem();
        _settingsMenuItem.Text = "&Settings...";
        _settingsMenuItem.Click += OnOpenSettings;

        _fileMenuSeparator3 = new ToolStripSeparator();

        _exitMenuItem = new ToolStripMenuItem();
        _exitMenuItem.Text = "E&xit";
        _exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        _exitMenuItem.Click += OnExit;

        _fileMenu.DropDownItems.Add(_newSchemaMenuItem);
        _fileMenu.DropDownItems.Add(_openSchemaMenuItem);
        _fileMenu.DropDownItems.Add(_fileMenuSeparator1);
        _fileMenu.DropDownItems.Add(_saveMenuItem);
        _fileMenu.DropDownItems.Add(_saveAsMenuItem);
        _fileMenu.DropDownItems.Add(_fileMenuSeparator2);
        _fileMenu.DropDownItems.Add(_settingsMenuItem);
        _fileMenu.DropDownItems.Add(_fileMenuSeparator3);
        _fileMenu.DropDownItems.Add(_exitMenuItem);

        // Edit Menu
        _editMenu = new ToolStripMenuItem();
        _editMenu.Text = "&Edit";

        _addEntityMenuItem = new ToolStripMenuItem();
        _addEntityMenuItem.Text = "Add &Entity...";
        _addEntityMenuItem.Click += OnAddEntity;

        _addPropertyMenuItem = new ToolStripMenuItem();
        _addPropertyMenuItem.Text = "Add &Property...";
        _addPropertyMenuItem.Click += OnAddProperty;

        _editMenuSeparator = new ToolStripSeparator();

        _deleteMenuItem = new ToolStripMenuItem();
        _deleteMenuItem.Text = "&Delete";
        _deleteMenuItem.ShortcutKeys = Keys.Delete;
        _deleteMenuItem.Click += OnDeleteSelected;

        _editMenu.DropDownItems.Add(_addEntityMenuItem);
        _editMenu.DropDownItems.Add(_addPropertyMenuItem);
        _editMenu.DropDownItems.Add(_editMenuSeparator);
        _editMenu.DropDownItems.Add(_deleteMenuItem);

        // Generate Menu
        _generateMenu = new ToolStripMenuItem();
        _generateMenu.Text = "&Generate";

        _previewAllMenuItem = new ToolStripMenuItem();
        _previewAllMenuItem.Text = "&Preview All";
        _previewAllMenuItem.ShortcutKeys = Keys.F5;
        _previewAllMenuItem.Click += OnPreviewAll;

        _generateMenuSeparator1 = new ToolStripSeparator();

        _generateAllMenuItem = new ToolStripMenuItem();
        _generateAllMenuItem.Text = "Generate &All";
        _generateAllMenuItem.ShortcutKeys = Keys.F6;
        _generateAllMenuItem.Click += OnGenerateAll;

        _generateDomainMenuItem = new ToolStripMenuItem();
        _generateDomainMenuItem.Text = "Generate &Domain Layer";
        _generateDomainMenuItem.Click += GenerateDomainLayer_Click;

        _generateInfraMenuItem = new ToolStripMenuItem();
        _generateInfraMenuItem.Text = "Generate &Infrastructure Layer";
        _generateInfraMenuItem.Click += GenerateInfrastructureLayer_Click;

        _generateAppMenuItem = new ToolStripMenuItem();
        _generateAppMenuItem.Text = "Generate &Application Layer";
        _generateAppMenuItem.Click += GenerateApplicationLayer_Click;

        _generatePresentationMenuItem = new ToolStripMenuItem();
        _generatePresentationMenuItem.Text = "Generate &Presentation Layer";
        _generatePresentationMenuItem.Click += GeneratePresentationLayer_Click;

        _generateMenuSeparator2 = new ToolStripSeparator();

        _generateDbScriptMenuItem = new ToolStripMenuItem();
        _generateDbScriptMenuItem.Text = "Generate &Database Scripts";
        _generateDbScriptMenuItem.Click += GenerateDatabaseScripts_Click;

        _generateMenu.DropDownItems.Add(_previewAllMenuItem);
        _generateMenu.DropDownItems.Add(_generateMenuSeparator1);
        _generateMenu.DropDownItems.Add(_generateAllMenuItem);
        _generateMenu.DropDownItems.Add(_generateDomainMenuItem);
        _generateMenu.DropDownItems.Add(_generateInfraMenuItem);
        _generateMenu.DropDownItems.Add(_generateAppMenuItem);
        _generateMenu.DropDownItems.Add(_generatePresentationMenuItem);
        _generateMenu.DropDownItems.Add(_generateMenuSeparator2);
        _generateMenu.DropDownItems.Add(_generateDbScriptMenuItem);

        // View Menu
        _viewMenu = new ToolStripMenuItem();
        _viewMenu.Text = "&View";

        _refreshPreviewMenuItem = new ToolStripMenuItem();
        _refreshPreviewMenuItem.Text = "&Refresh Preview";
        _refreshPreviewMenuItem.ShortcutKeys = Keys.F5;
        _refreshPreviewMenuItem.Click += OnRefreshPreview;

        _clearLogMenuItem = new ToolStripMenuItem();
        _clearLogMenuItem.Text = "&Clear Log";
        _clearLogMenuItem.Click += OnClearLog;

        _viewMenu.DropDownItems.Add(_refreshPreviewMenuItem);
        _viewMenu.DropDownItems.Add(_clearLogMenuItem);

        // Help Menu
        _helpMenu = new ToolStripMenuItem();
        _helpMenu.Text = "&Help";

        _aboutMenuItem = new ToolStripMenuItem();
        _aboutMenuItem.Text = "&About...";
        _aboutMenuItem.Click += OnAbout;

        _helpMenu.DropDownItems.Add(_aboutMenuItem);

        // Add menus to MenuStrip
        _menuStrip.Items.Add(_fileMenu);
        _menuStrip.Items.Add(_editMenu);
        _menuStrip.Items.Add(_generateMenu);
        _menuStrip.Items.Add(_viewMenu);
        _menuStrip.Items.Add(_helpMenu);
        // 
        // _toolStrip
        // 
        _toolStrip.Location = new Point(0, 24);
        _toolStrip.Name = "_toolStrip";
        _toolStrip.Size = new Size(1384, 25);
        _toolStrip.TabIndex = 1;

        _newButton = new ToolStripButton();
        _newButton.Text = "New";
        _newButton.ToolTipText = "New Schema";
        _newButton.Click += OnNewSchema;

        _openButton = new ToolStripButton();
        _openButton.Text = "Open";
        _openButton.ToolTipText = "Open Schema";
        _openButton.Click += OnOpenSchema;

        _saveButton = new ToolStripButton();
        _saveButton.Text = "Save";
        _saveButton.ToolTipText = "Save Schema";
        _saveButton.Click += OnSaveSchema;

        _separator1 = new ToolStripSeparator();

        _addEntityButton = new ToolStripButton();
        _addEntityButton.Text = "Add Entity";
        _addEntityButton.ToolTipText = "Add New Entity";
        _addEntityButton.Click += OnAddEntity;

        _addPropertyButton = new ToolStripButton();
        _addPropertyButton.Text = "Add Property";
        _addPropertyButton.ToolTipText = "Add New Property";
        _addPropertyButton.Click += OnAddProperty;

        _separator2 = new ToolStripSeparator();

        _previewButton = new ToolStripButton();
        _previewButton.Text = "Preview";
        _previewButton.ToolTipText = "Preview Generated Code (F5)";
        _previewButton.Click += OnPreviewAll;

        _generateButton = new ToolStripButton();
        _generateButton.Text = "Generate";
        _generateButton.ToolTipText = "Generate All Code (F6)";
        _generateButton.Click += OnGenerateAll;

        _separator3 = new ToolStripSeparator();

        _settingsButton = new ToolStripButton();
        _settingsButton.Text = "Settings";
        _settingsButton.ToolTipText = "Open Settings";
        _settingsButton.Click += OnOpenSettings;

        _toolStrip.Items.Add(_newButton);
        _toolStrip.Items.Add(_openButton);
        _toolStrip.Items.Add(_saveButton);
        _toolStrip.Items.Add(_separator1);
        _toolStrip.Items.Add(_addEntityButton);
        _toolStrip.Items.Add(_addPropertyButton);
        _toolStrip.Items.Add(_separator2);
        _toolStrip.Items.Add(_previewButton);
        _toolStrip.Items.Add(_generateButton);
        _toolStrip.Items.Add(_separator3);
        _toolStrip.Items.Add(_settingsButton);
        // 
        // _statusStrip
        // 
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel, _progressBar });
        _statusStrip.Location = new Point(0, 839);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Size = new Size(1384, 22);
        _statusStrip.TabIndex = 3;
        // 
        // _statusLabel
        // 
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(1369, 17);
        _statusLabel.Spring = true;
        _statusLabel.Text = "Ready";
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _progressBar
        // 
        _progressBar.Name = "_progressBar";
        _progressBar.Size = new Size(100, 16);
        _progressBar.Visible = false;
        // 
        // _mainSplitContainer
        // 
        _mainSplitContainer.Dock = DockStyle.Fill;
        _mainSplitContainer.Location = new Point(0, 49);
        _mainSplitContainer.Name = "_mainSplitContainer";
        // 
        // _mainSplitContainer.Panel1
        // 
        _mainSplitContainer.Panel1.Controls.Add(_leftSplitContainer);
        // 
        // _mainSplitContainer.Panel2
        // 
        _mainSplitContainer.Panel2.Controls.Add(_previewTabControl);
        _mainSplitContainer.Size = new Size(1384, 790);
        _mainSplitContainer.SplitterDistance = 1116;
        _mainSplitContainer.TabIndex = 0;
        // 
        // _leftSplitContainer
        // 
        _leftSplitContainer.Dock = DockStyle.Fill;
        _leftSplitContainer.Location = new Point(0, 0);
        _leftSplitContainer.Name = "_leftSplitContainer";
        _leftSplitContainer.Orientation = Orientation.Horizontal;
        // 
        // _leftSplitContainer.Panel1
        // 
        _leftSplitContainer.Panel1.Controls.Add(_schemaTreeView);
        // 
        // _leftSplitContainer.Panel2
        // 
        _leftSplitContainer.Panel2.Controls.Add(_propertyGrid);
        _leftSplitContainer.Size = new Size(1116, 790);
        _leftSplitContainer.SplitterDistance = 560;
        _leftSplitContainer.TabIndex = 0;
        // 
        // _schemaTreeView
        // 
        _schemaTreeView.Dock = DockStyle.Fill;
        _schemaTreeView.HideSelection = false;
        _schemaTreeView.Location = new Point(0, 0);
        _schemaTreeView.Name = "_schemaTreeView";
        _schemaTreeView.Size = new Size(1116, 560);
        _schemaTreeView.TabIndex = 0;
        _schemaTreeView.ImageList = _treeViewImageList;
        // 
        // _propertyGrid
        // 
        _propertyGrid.BackColor = SystemColors.Control;
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.Location = new Point(0, 0);
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.PropertySort = PropertySort.Categorized;
        _propertyGrid.Size = new Size(1116, 226);
        _propertyGrid.TabIndex = 0;
        // 
        // _previewTabControl
        // 
        _previewTabControl.Controls.Add(jsonTab);
        _previewTabControl.Controls.Add(outputTab);
        _previewTabControl.Controls.Add(previewTab);
        _previewTabControl.Controls.Add(userControlTab);
        _previewTabControl.Controls.Add(logTab);
        _previewTabControl.Dock = DockStyle.Fill;
        _previewTabControl.Location = new Point(0, 0);
        _previewTabControl.Name = "_previewTabControl";
        _previewTabControl.SelectedIndex = 0;
        _previewTabControl.Size = new Size(264, 790);
        _previewTabControl.TabIndex = 0;
        // 
        // jsonTab
        // 
        jsonTab.Controls.Add(_jsonEditor);
        jsonTab.Location = new Point(4, 24);
        jsonTab.Name = "jsonTab";
        jsonTab.Size = new Size(256, 762);
        jsonTab.TabIndex = 0;
        jsonTab.Text = "JSON Schema";
        // 
        // _jsonEditor
        // 
        _jsonEditor.AcceptsTab = true;
        _jsonEditor.Dock = DockStyle.Fill;
        _jsonEditor.Font = new Font("Consolas", 10F);
        _jsonEditor.Location = new Point(0, 0);
        _jsonEditor.Name = "_jsonEditor";
        _jsonEditor.Size = new Size(256, 762);
        _jsonEditor.TabIndex = 0;
        _jsonEditor.Text = "";
        _jsonEditor.WordWrap = false;
        // 
        // outputTab
        // 
        outputTab.Controls.Add(_outputTreeView);
        outputTab.Location = new Point(4, 24);
        outputTab.Name = "outputTab";
        outputTab.Size = new Size(17, 72);
        outputTab.TabIndex = 1;
        outputTab.Text = "Output Structure";
        // 
        // _outputTreeView
        // 
        _outputTreeView.Dock = DockStyle.Fill;
        _outputTreeView.Location = new Point(0, 0);
        _outputTreeView.Name = "_outputTreeView";
        _outputTreeView.Size = new Size(17, 72);
        _outputTreeView.TabIndex = 0;
        // 
        // previewTab
        // 
        previewTab.Controls.Add(_previewTextBox);
        previewTab.Location = new Point(4, 24);
        previewTab.Name = "previewTab";
        previewTab.Size = new Size(17, 72);
        previewTab.TabIndex = 2;
        previewTab.Text = "Code Preview";
        // 
        // _previewTextBox
        // 
        _previewTextBox.Dock = DockStyle.Fill;
        _previewTextBox.Font = new Font("Consolas", 10F);
        _previewTextBox.Location = new Point(0, 0);
        _previewTextBox.Name = "_previewTextBox";
        _previewTextBox.ReadOnly = true;
        _previewTextBox.Size = new Size(17, 72);
        _previewTextBox.TabIndex = 0;
        _previewTextBox.Text = "";
        _previewTextBox.WordWrap = false;
        // 
        // userControlTab
        // 
        userControlTab.Controls.Add(_userControlPreviewPanel);
        userControlTab.Location = new Point(4, 24);
        userControlTab.Name = "userControlTab";
        userControlTab.Size = new Size(17, 72);
        userControlTab.TabIndex = 3;
        userControlTab.Text = "Visual Preview";
        // 
        // _userControlPreviewPanel
        // 
        _userControlPreviewPanel.AutoScroll = true;
        _userControlPreviewPanel.BackColor = SystemColors.Control;
        _userControlPreviewPanel.Dock = DockStyle.Fill;
        _userControlPreviewPanel.Location = new Point(0, 0);
        _userControlPreviewPanel.Name = "_userControlPreviewPanel";
        _userControlPreviewPanel.Size = new Size(17, 72);
        _userControlPreviewPanel.TabIndex = 0;
        // 
        // logTab
        // 
        logTab.Controls.Add(_logListBox);
        logTab.Location = new Point(4, 24);
        logTab.Name = "logTab";
        logTab.Size = new Size(17, 72);
        logTab.TabIndex = 4;
        logTab.Text = "Log";
        // 
        // _logListBox
        // 
        _logListBox.Dock = DockStyle.Fill;
        _logListBox.Font = new Font("Consolas", 9F);
        _logListBox.IntegralHeight = false;
        _logListBox.Location = new Point(0, 0);
        _logListBox.Name = "_logListBox";
        _logListBox.Size = new Size(17, 72);
        _logListBox.TabIndex = 0;
        // 
        // MainForm
        // 
        ClientSize = new Size(1384, 861);
        Controls.Add(_mainSplitContainer);
        Controls.Add(_toolStrip);
        Controls.Add(_menuStrip);
        Controls.Add(_statusStrip);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = _menuStrip;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Code Generator - Domain Driven Design";
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _mainSplitContainer.Panel1.ResumeLayout(false);
        _mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).EndInit();
        _mainSplitContainer.ResumeLayout(false);
        _leftSplitContainer.Panel1.ResumeLayout(false);
        _leftSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_leftSplitContainer).EndInit();
        _leftSplitContainer.ResumeLayout(false);
        _previewTabControl.ResumeLayout(false);
        jsonTab.ResumeLayout(false);
        outputTab.ResumeLayout(false);
        previewTab.ResumeLayout(false);
        userControlTab.ResumeLayout(false);
        logTab.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }


    private TabPage jsonTab;
    private TabPage outputTab;
    private TabPage previewTab;
    private TabPage userControlTab;
    private TabPage logTab;
}
