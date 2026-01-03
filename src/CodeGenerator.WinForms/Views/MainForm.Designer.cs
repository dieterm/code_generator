using CodeGenerator.WinForms.Properties;
using CodeGenerator.WinForms.Controls;

namespace CodeGenerator.WinForms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private MenuStrip _menuStrip;
    private ToolStrip _toolStrip;
    private SplitContainer _mainSplitContainer;
    private DomainSchemaEditor _domainSchemaEditor;
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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        _menuStrip = new MenuStrip();
        _fileMenu = new ToolStripMenuItem();
        _newSchemaMenuItem = new ToolStripMenuItem();
        _openSchemaMenuItem = new ToolStripMenuItem();
        _fileMenuSeparator1 = new ToolStripSeparator();
        _saveMenuItem = new ToolStripMenuItem();
        _saveAsMenuItem = new ToolStripMenuItem();
        _fileMenuSeparator2 = new ToolStripSeparator();
        _settingsMenuItem = new ToolStripMenuItem();
        _fileMenuSeparator3 = new ToolStripSeparator();
        _exitMenuItem = new ToolStripMenuItem();
        _editMenu = new ToolStripMenuItem();
        _addEntityMenuItem = new ToolStripMenuItem();
        _addPropertyMenuItem = new ToolStripMenuItem();
        _editMenuSeparator = new ToolStripSeparator();
        _deleteMenuItem = new ToolStripMenuItem();
        _generateMenu = new ToolStripMenuItem();
        _previewAllMenuItem = new ToolStripMenuItem();
        _generateMenuSeparator1 = new ToolStripSeparator();
        _generateAllMenuItem = new ToolStripMenuItem();
        _generateDomainMenuItem = new ToolStripMenuItem();
        _generateInfraMenuItem = new ToolStripMenuItem();
        _generateAppMenuItem = new ToolStripMenuItem();
        _generatePresentationMenuItem = new ToolStripMenuItem();
        _generateMenuSeparator2 = new ToolStripSeparator();
        _generateDbScriptMenuItem = new ToolStripMenuItem();
        _viewMenu = new ToolStripMenuItem();
        _refreshPreviewMenuItem = new ToolStripMenuItem();
        _clearLogMenuItem = new ToolStripMenuItem();
        _helpMenu = new ToolStripMenuItem();
        _aboutMenuItem = new ToolStripMenuItem();
        _toolStrip = new ToolStrip();
        _newButton = new ToolStripButton();
        _openButton = new ToolStripButton();
        _saveButton = new ToolStripButton();
        _separator1 = new ToolStripSeparator();
        _addEntityButton = new ToolStripButton();
        _addPropertyButton = new ToolStripButton();
        _separator2 = new ToolStripSeparator();
        _previewButton = new ToolStripButton();
        _generateButton = new ToolStripButton();
        _separator3 = new ToolStripSeparator();
        _settingsButton = new ToolStripButton();
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _progressBar = new ToolStripProgressBar();
        _mainSplitContainer = new SplitContainer();
        _domainSchemaEditor = new DomainSchemaEditor();
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
        _menuStrip.SuspendLayout();
        _toolStrip.SuspendLayout();
        _statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).BeginInit();
        _mainSplitContainer.Panel1.SuspendLayout();
        _mainSplitContainer.Panel2.SuspendLayout();
        _mainSplitContainer.SuspendLayout();
        _previewTabControl.SuspendLayout();
        jsonTab.SuspendLayout();
        outputTab.SuspendLayout();
        previewTab.SuspendLayout();
        userControlTab.SuspendLayout();
        logTab.SuspendLayout();
        SuspendLayout();
        // 
        // _menuStrip
        // 
        _menuStrip.Items.AddRange(new ToolStripItem[] { _fileMenu, _editMenu, _generateMenu, _viewMenu, _helpMenu });
        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Size = new Size(1384, 24);
        _menuStrip.TabIndex = 2;
        // 
        // _fileMenu
        // 
        _fileMenu.DropDownItems.AddRange(new ToolStripItem[] { _newSchemaMenuItem, _openSchemaMenuItem, _fileMenuSeparator1, _saveMenuItem, _saveAsMenuItem, _fileMenuSeparator2, _settingsMenuItem, _fileMenuSeparator3, _exitMenuItem });
        _fileMenu.Name = "_fileMenu";
        _fileMenu.Size = new Size(37, 20);
        _fileMenu.Text = "&File";
        // 
        // _newSchemaMenuItem
        // 
        _newSchemaMenuItem.Name = "_newSchemaMenuItem";
        _newSchemaMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        _newSchemaMenuItem.Size = new Size(32, 19);
        _newSchemaMenuItem.Text = "&New Schema";
        _newSchemaMenuItem.Click += OnNewSchema;
        // 
        // _openSchemaMenuItem
        // 
        _openSchemaMenuItem.Name = "_openSchemaMenuItem";
        _openSchemaMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        _openSchemaMenuItem.Size = new Size(32, 19);
        _openSchemaMenuItem.Text = "&Open Schema...";
        _openSchemaMenuItem.Click += OnOpenSchema;
        // 
        // _fileMenuSeparator1
        // 
        _fileMenuSeparator1.Name = "_fileMenuSeparator1";
        _fileMenuSeparator1.Size = new Size(6, 6);
        // 
        // _saveMenuItem
        // 
        _saveMenuItem.Name = "_saveMenuItem";
        _saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        _saveMenuItem.Size = new Size(32, 19);
        _saveMenuItem.Text = "&Save";
        _saveMenuItem.Click += OnSaveSchema;
        // 
        // _saveAsMenuItem
        // 
        _saveAsMenuItem.Name = "_saveAsMenuItem";
        _saveAsMenuItem.Size = new Size(32, 19);
        _saveAsMenuItem.Text = "Save &As...";
        _saveAsMenuItem.Click += OnSaveSchemaAs;
        // 
        // _fileMenuSeparator2
        // 
        _fileMenuSeparator2.Name = "_fileMenuSeparator2";
        _fileMenuSeparator2.Size = new Size(6, 6);
        // 
        // _settingsMenuItem
        // 
        _settingsMenuItem.Name = "_settingsMenuItem";
        _settingsMenuItem.Size = new Size(32, 19);
        _settingsMenuItem.Text = "&Settings...";
        _settingsMenuItem.Click += OnOpenSettings;
        // 
        // _fileMenuSeparator3
        // 
        _fileMenuSeparator3.Name = "_fileMenuSeparator3";
        _fileMenuSeparator3.Size = new Size(6, 6);
        // 
        // _exitMenuItem
        // 
        _exitMenuItem.Name = "_exitMenuItem";
        _exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        _exitMenuItem.Size = new Size(32, 19);
        _exitMenuItem.Text = "E&xit";
        _exitMenuItem.Click += OnExit;
        // 
        // _editMenu
        // 
        _editMenu.DropDownItems.AddRange(new ToolStripItem[] { _addEntityMenuItem, _addPropertyMenuItem, _editMenuSeparator, _deleteMenuItem });
        _editMenu.Name = "_editMenu";
        _editMenu.Size = new Size(39, 20);
        _editMenu.Text = "&Edit";
        // 
        // _addEntityMenuItem
        // 
        _addEntityMenuItem.Name = "_addEntityMenuItem";
        _addEntityMenuItem.Size = new Size(32, 19);
        _addEntityMenuItem.Text = "Add &Entity...";
        _addEntityMenuItem.Click += OnAddEntity;
        // 
        // _addPropertyMenuItem
        // 
        _addPropertyMenuItem.Name = "_addPropertyMenuItem";
        _addPropertyMenuItem.Size = new Size(32, 19);
        _addPropertyMenuItem.Text = "Add &Property...";
        _addPropertyMenuItem.Click += OnAddProperty;
        // 
        // _editMenuSeparator
        // 
        _editMenuSeparator.Name = "_editMenuSeparator";
        _editMenuSeparator.Size = new Size(6, 6);
        // 
        // _deleteMenuItem
        // 
        _deleteMenuItem.Name = "_deleteMenuItem";
        _deleteMenuItem.ShortcutKeys = Keys.Delete;
        _deleteMenuItem.Size = new Size(32, 19);
        _deleteMenuItem.Text = "&Delete";
        _deleteMenuItem.Click += OnDeleteSelected;
        // 
        // _generateMenu
        // 
        _generateMenu.DropDownItems.AddRange(new ToolStripItem[] { _previewAllMenuItem, _generateMenuSeparator1, _generateAllMenuItem, _generateDomainMenuItem, _generateInfraMenuItem, _generateAppMenuItem, _generatePresentationMenuItem, _generateMenuSeparator2, _generateDbScriptMenuItem });
        _generateMenu.Name = "_generateMenu";
        _generateMenu.Size = new Size(66, 20);
        _generateMenu.Text = "&Generate";
        // 
        // _previewAllMenuItem
        // 
        _previewAllMenuItem.Name = "_previewAllMenuItem";
        _previewAllMenuItem.ShortcutKeys = Keys.F5;
        _previewAllMenuItem.Size = new Size(226, 22);
        _previewAllMenuItem.Text = "&Preview All";
        _previewAllMenuItem.Click += OnPreviewAll;
        // 
        // _generateMenuSeparator1
        // 
        _generateMenuSeparator1.Name = "_generateMenuSeparator1";
        _generateMenuSeparator1.Size = new Size(223, 6);
        // 
        // _generateAllMenuItem
        // 
        _generateAllMenuItem.Name = "_generateAllMenuItem";
        _generateAllMenuItem.ShortcutKeys = Keys.F6;
        _generateAllMenuItem.Size = new Size(226, 22);
        _generateAllMenuItem.Text = "Generate &All";
        _generateAllMenuItem.Click += OnGenerateAll;
        // 
        // _generateDomainMenuItem
        // 
        _generateDomainMenuItem.Name = "_generateDomainMenuItem";
        _generateDomainMenuItem.Size = new Size(226, 22);
        _generateDomainMenuItem.Text = "Generate &Domain Layer";
        _generateDomainMenuItem.Click += GenerateDomainLayer_Click;
        // 
        // _generateInfraMenuItem
        // 
        _generateInfraMenuItem.Name = "_generateInfraMenuItem";
        _generateInfraMenuItem.Size = new Size(226, 22);
        _generateInfraMenuItem.Text = "Generate &Infrastructure Layer";
        _generateInfraMenuItem.Click += GenerateInfrastructureLayer_Click;
        // 
        // _generateAppMenuItem
        // 
        _generateAppMenuItem.Name = "_generateAppMenuItem";
        _generateAppMenuItem.Size = new Size(226, 22);
        _generateAppMenuItem.Text = "Generate &Application Layer";
        _generateAppMenuItem.Click += GenerateApplicationLayer_Click;
        // 
        // _generatePresentationMenuItem
        // 
        _generatePresentationMenuItem.Name = "_generatePresentationMenuItem";
        _generatePresentationMenuItem.Size = new Size(226, 22);
        _generatePresentationMenuItem.Text = "Generate &Presentation Layer";
        _generatePresentationMenuItem.Click += GeneratePresentationLayer_Click;
        // 
        // _generateMenuSeparator2
        // 
        _generateMenuSeparator2.Name = "_generateMenuSeparator2";
        _generateMenuSeparator2.Size = new Size(223, 6);
        // 
        // _generateDbScriptMenuItem
        // 
        _generateDbScriptMenuItem.Name = "_generateDbScriptMenuItem";
        _generateDbScriptMenuItem.Size = new Size(226, 22);
        _generateDbScriptMenuItem.Text = "Generate &Database Scripts";
        _generateDbScriptMenuItem.Click += GenerateDatabaseScripts_Click;
        // 
        // _viewMenu
        // 
        _viewMenu.DropDownItems.AddRange(new ToolStripItem[] { _refreshPreviewMenuItem, _clearLogMenuItem });
        _viewMenu.Name = "_viewMenu";
        _viewMenu.Size = new Size(44, 20);
        _viewMenu.Text = "&View";
        // 
        // _refreshPreviewMenuItem
        // 
        _refreshPreviewMenuItem.Name = "_refreshPreviewMenuItem";
        _refreshPreviewMenuItem.ShortcutKeys = Keys.F5;
        _refreshPreviewMenuItem.Size = new Size(32, 19);
        _refreshPreviewMenuItem.Text = "&Refresh Preview";
        _refreshPreviewMenuItem.Click += OnRefreshPreview;
        // 
        // _clearLogMenuItem
        // 
        _clearLogMenuItem.Name = "_clearLogMenuItem";
        _clearLogMenuItem.Size = new Size(32, 19);
        _clearLogMenuItem.Text = "&Clear Log";
        _clearLogMenuItem.Click += OnClearLog;
        // 
        // _helpMenu
        // 
        _helpMenu.DropDownItems.AddRange(new ToolStripItem[] { _aboutMenuItem });
        _helpMenu.Name = "_helpMenu";
        _helpMenu.Size = new Size(44, 20);
        _helpMenu.Text = "&Help";
        // 
        // _aboutMenuItem
        // 
        _aboutMenuItem.Name = "_aboutMenuItem";
        _aboutMenuItem.Size = new Size(32, 19);
        _aboutMenuItem.Text = "&About...";
        _aboutMenuItem.Click += OnAbout;
        // 
        // _toolStrip
        // 
        _toolStrip.Items.AddRange(new ToolStripItem[] { _newButton, _openButton, _saveButton, _separator1, _addEntityButton, _addPropertyButton, _separator2, _previewButton, _generateButton, _separator3, _settingsButton });
        _toolStrip.Location = new Point(0, 24);
        _toolStrip.Name = "_toolStrip";
        _toolStrip.Size = new Size(1384, 25);
        _toolStrip.TabIndex = 1;
        // 
        // _newButton
        // 
        _newButton.Name = "_newButton";
        _newButton.Size = new Size(35, 22);
        _newButton.Text = "New";
        _newButton.ToolTipText = "New Schema";
        _newButton.Click += OnNewSchema;
        // 
        // _openButton
        // 
        _openButton.Name = "_openButton";
        _openButton.Size = new Size(40, 22);
        _openButton.Text = "Open";
        _openButton.ToolTipText = "Open Schema";
        _openButton.Click += OnOpenSchema;
        // 
        // _saveButton
        // 
        _saveButton.Name = "_saveButton";
        _saveButton.Size = new Size(35, 22);
        _saveButton.Text = "Save";
        _saveButton.ToolTipText = "Save Schema";
        _saveButton.Click += OnSaveSchema;
        // 
        // _separator1
        // 
        _separator1.Name = "_separator1";
        _separator1.Size = new Size(6, 25);
        // 
        // _addEntityButton
        // 
        _addEntityButton.Name = "_addEntityButton";
        _addEntityButton.Size = new Size(66, 22);
        _addEntityButton.Text = "Add Entity";
        _addEntityButton.ToolTipText = "Add New Entity";
        _addEntityButton.Click += OnAddEntity;
        // 
        // _addPropertyButton
        // 
        _addPropertyButton.Name = "_addPropertyButton";
        _addPropertyButton.Size = new Size(81, 22);
        _addPropertyButton.Text = "Add Property";
        _addPropertyButton.ToolTipText = "Add New Property";
        _addPropertyButton.Click += OnAddProperty;
        // 
        // _separator2
        // 
        _separator2.Name = "_separator2";
        _separator2.Size = new Size(6, 25);
        // 
        // _previewButton
        // 
        _previewButton.Name = "_previewButton";
        _previewButton.Size = new Size(52, 22);
        _previewButton.Text = "Preview";
        _previewButton.ToolTipText = "Preview Generated Code (F5)";
        _previewButton.Click += OnPreviewAll;
        // 
        // _generateButton
        // 
        _generateButton.Name = "_generateButton";
        _generateButton.Size = new Size(58, 22);
        _generateButton.Text = "Generate";
        _generateButton.ToolTipText = "Generate All Code (F6)";
        _generateButton.Click += OnGenerateAll;
        // 
        // _separator3
        // 
        _separator3.Name = "_separator3";
        _separator3.Size = new Size(6, 25);
        // 
        // _settingsButton
        // 
        _settingsButton.Name = "_settingsButton";
        _settingsButton.Size = new Size(53, 22);
        _settingsButton.Text = "Settings";
        _settingsButton.ToolTipText = "Open Settings";
        _settingsButton.Click += OnOpenSettings;
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
        _mainSplitContainer.Panel1.Controls.Add(_domainSchemaEditor);
        // 
        // _mainSplitContainer.Panel2
        // 
        _mainSplitContainer.Panel2.Controls.Add(_previewTabControl);
        _mainSplitContainer.Size = new Size(1384, 790);
        _mainSplitContainer.SplitterDistance = 1116;
        _mainSplitContainer.TabIndex = 0;
        // 
        // _domainSchemaEditor
        // 
        _domainSchemaEditor.Dock = DockStyle.Fill;
        _domainSchemaEditor.Location = new Point(0, 0);
        _domainSchemaEditor.Name = "_domainSchemaEditor";
        _domainSchemaEditor.Size = new Size(1116, 790);
        _domainSchemaEditor.TabIndex = 0;
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
        outputTab.Size = new Size(256, 762);
        outputTab.TabIndex = 1;
        outputTab.Text = "Output Structure";
        // 
        // _outputTreeView
        // 
        _outputTreeView.Dock = DockStyle.Fill;
        _outputTreeView.Location = new Point(0, 0);
        _outputTreeView.Name = "_outputTreeView";
        _outputTreeView.Size = new Size(256, 762);
        _outputTreeView.TabIndex = 0;
        // 
        // previewTab
        // 
        previewTab.Controls.Add(_previewTextBox);
        previewTab.Location = new Point(4, 24);
        previewTab.Name = "previewTab";
        previewTab.Size = new Size(256, 762);
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
        _previewTextBox.Size = new Size(256, 762);
        _previewTextBox.TabIndex = 0;
        _previewTextBox.Text = "";
        _previewTextBox.WordWrap = false;
        // 
        // userControlTab
        // 
        userControlTab.Controls.Add(_userControlPreviewPanel);
        userControlTab.Location = new Point(4, 24);
        userControlTab.Name = "userControlTab";
        userControlTab.Size = new Size(256, 762);
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
        _userControlPreviewPanel.Size = new Size(256, 762);
        _userControlPreviewPanel.TabIndex = 0;
        // 
        // logTab
        // 
        logTab.Controls.Add(_logListBox);
        logTab.Location = new Point(4, 24);
        logTab.Name = "logTab";
        logTab.Size = new Size(256, 762);
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
        _logListBox.Size = new Size(256, 762);
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
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _toolStrip.ResumeLayout(false);
        _toolStrip.PerformLayout();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _mainSplitContainer.Panel1.ResumeLayout(false);
        _mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).EndInit();
        _mainSplitContainer.ResumeLayout(false);
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
