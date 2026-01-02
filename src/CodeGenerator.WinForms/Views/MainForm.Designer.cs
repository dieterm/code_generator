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

    private void CreateMenus()
    {
        // File Menu
        var fileMenu = new ToolStripMenuItem();
        fileMenu.Text = "&File";

        var newSchemaMenuItem = new ToolStripMenuItem();
        newSchemaMenuItem.Text = "&New Schema";
        newSchemaMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        newSchemaMenuItem.Click += OnNewSchema;

        var openSchemaMenuItem = new ToolStripMenuItem();
        openSchemaMenuItem.Text = "&Open Schema...";
        openSchemaMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        openSchemaMenuItem.Click += OnOpenSchema;

        var separator1 = new ToolStripSeparator();

        var saveMenuItem = new ToolStripMenuItem();
        saveMenuItem.Text = "&Save";
        saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        saveMenuItem.Click += OnSaveSchema;

        var saveAsMenuItem = new ToolStripMenuItem();
        saveAsMenuItem.Text = "Save &As...";
        saveAsMenuItem.Click += OnSaveSchemaAs;

        var separator2 = new ToolStripSeparator();

        var settingsMenuItem = new ToolStripMenuItem();
        settingsMenuItem.Text = "&Settings...";
        settingsMenuItem.Click += OnOpenSettings;

        var separator3 = new ToolStripSeparator();

        var exitMenuItem = new ToolStripMenuItem();
        exitMenuItem.Text = "E&xit";
        exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        exitMenuItem.Click += OnExit;

        fileMenu.DropDownItems.Add(newSchemaMenuItem);
        fileMenu.DropDownItems.Add(openSchemaMenuItem);
        fileMenu.DropDownItems.Add(separator1);
        fileMenu.DropDownItems.Add(saveMenuItem);
        fileMenu.DropDownItems.Add(saveAsMenuItem);
        fileMenu.DropDownItems.Add(separator2);
        fileMenu.DropDownItems.Add(settingsMenuItem);
        fileMenu.DropDownItems.Add(separator3);
        fileMenu.DropDownItems.Add(exitMenuItem);

        // Edit Menu
        var editMenu = new ToolStripMenuItem();
        editMenu.Text = "&Edit";

        var addEntityMenuItem = new ToolStripMenuItem();
        addEntityMenuItem.Text = "Add &Entity...";
        addEntityMenuItem.Click += OnAddEntity;

        var addPropertyMenuItem = new ToolStripMenuItem();
        addPropertyMenuItem.Text = "Add &Property...";
        addPropertyMenuItem.Click += OnAddProperty;

        var separator4 = new ToolStripSeparator();

        var deleteMenuItem = new ToolStripMenuItem();
        deleteMenuItem.Text = "&Delete";
        deleteMenuItem.ShortcutKeys = Keys.Delete;
        deleteMenuItem.Click += OnDeleteSelected;

        editMenu.DropDownItems.Add(addEntityMenuItem);
        editMenu.DropDownItems.Add(addPropertyMenuItem);
        editMenu.DropDownItems.Add(separator4);
        editMenu.DropDownItems.Add(deleteMenuItem);

        // Generate Menu
        var generateMenu = new ToolStripMenuItem();
        generateMenu.Text = "&Generate";

        var previewAllMenuItem = new ToolStripMenuItem();
        previewAllMenuItem.Text = "&Preview All";
        previewAllMenuItem.ShortcutKeys = Keys.F5;
        previewAllMenuItem.Click += OnPreviewAll;

        var separator5 = new ToolStripSeparator();

        var generateAllMenuItem = new ToolStripMenuItem();
        generateAllMenuItem.Text = "Generate &All";
        generateAllMenuItem.ShortcutKeys = Keys.F6;
        generateAllMenuItem.Click += OnGenerateAll;

        var generateDomainMenuItem = new ToolStripMenuItem();
        generateDomainMenuItem.Text = "Generate &Domain Layer";
        generateDomainMenuItem.Click += GenerateDomainLayer_Click;

        var generateInfraMenuItem = new ToolStripMenuItem();
        generateInfraMenuItem.Text = "Generate &Infrastructure Layer";
        generateInfraMenuItem.Click += GenerateInfrastructureLayer_Click;

        var generateAppMenuItem = new ToolStripMenuItem();
        generateAppMenuItem.Text = "Generate &Application Layer";
        generateAppMenuItem.Click += GenerateApplicationLayer_Click;

        var generatePresentationMenuItem = new ToolStripMenuItem();
        generatePresentationMenuItem.Text = "Generate &Presentation Layer";
        generatePresentationMenuItem.Click += GeneratePresentationLayer_Click;

        var separator6 = new ToolStripSeparator();

        var generateDbScriptMenuItem = new ToolStripMenuItem();
        generateDbScriptMenuItem.Text = "Generate &Database Scripts";
        generateDbScriptMenuItem.Click += GenerateDatabaseScripts_Click;

        generateMenu.DropDownItems.Add(previewAllMenuItem);
        generateMenu.DropDownItems.Add(separator5);
        generateMenu.DropDownItems.Add(generateAllMenuItem);
        generateMenu.DropDownItems.Add(generateDomainMenuItem);
        generateMenu.DropDownItems.Add(generateInfraMenuItem);
        generateMenu.DropDownItems.Add(generateAppMenuItem);
        generateMenu.DropDownItems.Add(generatePresentationMenuItem);
        generateMenu.DropDownItems.Add(separator6);
        generateMenu.DropDownItems.Add(generateDbScriptMenuItem);

        // View Menu
        var viewMenu = new ToolStripMenuItem();
        viewMenu.Text = "&View";

        var refreshPreviewMenuItem = new ToolStripMenuItem();
        refreshPreviewMenuItem.Text = "&Refresh Preview";
        refreshPreviewMenuItem.ShortcutKeys = Keys.F5;
        refreshPreviewMenuItem.Click += OnRefreshPreview;

        var clearLogMenuItem = new ToolStripMenuItem();
        clearLogMenuItem.Text = "&Clear Log";
        clearLogMenuItem.Click += OnClearLog;

        viewMenu.DropDownItems.Add(refreshPreviewMenuItem);
        viewMenu.DropDownItems.Add(clearLogMenuItem);

        // Help Menu
        var helpMenu = new ToolStripMenuItem();
        helpMenu.Text = "&Help";

        var aboutMenuItem = new ToolStripMenuItem();
        aboutMenuItem.Text = "&About...";
        aboutMenuItem.Click += OnAbout;

        helpMenu.DropDownItems.Add(aboutMenuItem);

        // Add menus to MenuStrip
        _menuStrip.Items.Add(fileMenu);
        _menuStrip.Items.Add(editMenu);
        _menuStrip.Items.Add(generateMenu);
        _menuStrip.Items.Add(viewMenu);
        _menuStrip.Items.Add(helpMenu);
    }
    

    private TabPage jsonTab;
    private TabPage outputTab;
    private TabPage previewTab;
    private TabPage userControlTab;
    private TabPage logTab;
}
