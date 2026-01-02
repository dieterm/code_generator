namespace CodeGenerator.WinForms;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;

    private TabControl _tabControl;
    private TabPage _generalTab;
    private TabPage _generatorsTab;
    private TabPage _packagesTab;
    private Panel _buttonPanel;
    private TableLayoutPanel _generalTabLayout;
    private TextBox _solutionRootTextBox;
    private TextBox _namespaceTextBox;
    private TextBox _templateFolderTextBox;
    private TextBox _outputFolderTextBox;
    private TextBox _targetFrameworkTextBox;
    private CheckBox _overwriteCheckBox;
    private CheckBox _backupCheckBox;
    private DataGridView _generatorsGrid;
    private DataGridView _packagesGrid;
    private Button _saveButton;
    private Button _cancelButton;
    private Button _loadDefaultsButton;
    private Button _solutionBrowseBtn;
    private Button _templateBrowseBtn;
    private Button _outputBrowseBtn;
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
        _tabControl = new TabControl();
        _generalTab = new TabPage();
        _generalTabLayout = new TableLayoutPanel();
        _solutionRootTextBox = new TextBox();
        _solutionBrowseBtn = new Button();
        _namespaceTextBox = new TextBox();
        _templateFolderTextBox = new TextBox();
        _templateBrowseBtn = new Button();
        _outputFolderTextBox = new TextBox();
        _outputBrowseBtn = new Button();
        _targetFrameworkTextBox = new TextBox();
        _overwriteCheckBox = new CheckBox();
        _backupCheckBox = new CheckBox();
        _generatorsTab = new TabPage();
        _packagesTab = new TabPage();
        _buttonPanel = new Panel();
        _saveButton = new Button();
        _cancelButton = new Button();
        _loadDefaultsButton = new Button();
        _tabControl.SuspendLayout();
        _generalTab.SuspendLayout();
        _generalTabLayout.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _tabControl
        // 
        _tabControl.Controls.Add(_generalTab);
        _tabControl.Controls.Add(_generatorsTab);
        _tabControl.Controls.Add(_packagesTab);
        _tabControl.Dock = DockStyle.Fill;
        _tabControl.Location = new Point(0, 0);
        _tabControl.Name = "_tabControl";
        _tabControl.SelectedIndex = 0;
        _tabControl.Size = new Size(784, 511);
        _tabControl.TabIndex = 0;
        // 
        // _generalTab
        // 
        _generalTab.Controls.Add(_generalTabLayout);
        _generalTab.Location = new Point(4, 24);
        _generalTab.Name = "_generalTab";
        _generalTab.Size = new Size(776, 483);
        _generalTab.TabIndex = 0;
        _generalTab.Text = "General";
        // 
        // _generalTabLayout
        // 
        _generalTabLayout.ColumnCount = 3;
        _generalTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        _generalTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _generalTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
        _generalTabLayout.Controls.Add(_solutionRootTextBox, 1, 0);
        _generalTabLayout.Controls.Add(_solutionBrowseBtn, 2, 0);
        _generalTabLayout.Controls.Add(_namespaceTextBox, 1, 1);
        _generalTabLayout.Controls.Add(_templateFolderTextBox, 1, 2);
        _generalTabLayout.Controls.Add(_templateBrowseBtn, 2, 2);
        _generalTabLayout.Controls.Add(_outputFolderTextBox, 1, 3);
        _generalTabLayout.Controls.Add(_outputBrowseBtn, 2, 3);
        _generalTabLayout.Controls.Add(_targetFrameworkTextBox, 1, 4);
        _generalTabLayout.Controls.Add(_overwriteCheckBox, 1, 5);
        _generalTabLayout.Controls.Add(_backupCheckBox, 1, 6);
        _generalTabLayout.Dock = DockStyle.Fill;
        _generalTabLayout.Location = new Point(0, 0);
        _generalTabLayout.Name = "_generalTabLayout";
        _generalTabLayout.Padding = new Padding(10);
        _generalTabLayout.RowCount = 8;
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        _generalTabLayout.Size = new Size(776, 483);
        _generalTabLayout.TabIndex = 0;
        // 
        // _solutionRootTextBox
        // 
        _solutionRootTextBox.Location = new Point(163, 13);
        _solutionRootTextBox.Name = "_solutionRootTextBox";
        _solutionRootTextBox.Size = new Size(100, 23);
        _solutionRootTextBox.TabIndex = 1;
        // 
        // _solutionBrowseBtn
        // 
        _solutionBrowseBtn.Dock = DockStyle.Fill;
        _solutionBrowseBtn.Location = new Point(689, 13);
        _solutionBrowseBtn.Name = "_solutionBrowseBtn";
        _solutionBrowseBtn.Size = new Size(74, 14);
        _solutionBrowseBtn.TabIndex = 2;
        _solutionBrowseBtn.Text = "Browse";
        // 
        // _namespaceTextBox
        // 
        _generalTabLayout.SetColumnSpan(_namespaceTextBox, 2);
        _namespaceTextBox.Location = new Point(163, 33);
        _namespaceTextBox.Name = "_namespaceTextBox";
        _namespaceTextBox.Size = new Size(100, 23);
        _namespaceTextBox.TabIndex = 4;
        // 
        // _templateFolderTextBox
        // 
        _templateFolderTextBox.Location = new Point(163, 53);
        _templateFolderTextBox.Name = "_templateFolderTextBox";
        _templateFolderTextBox.Size = new Size(100, 23);
        _templateFolderTextBox.TabIndex = 6;
        // 
        // _templateBrowseBtn
        // 
        _templateBrowseBtn.Dock = DockStyle.Fill;
        _templateBrowseBtn.Location = new Point(689, 53);
        _templateBrowseBtn.Name = "_templateBrowseBtn";
        _templateBrowseBtn.Size = new Size(74, 14);
        _templateBrowseBtn.TabIndex = 7;
        _templateBrowseBtn.Text = "Browse";
        // 
        // _outputFolderTextBox
        // 
        _outputFolderTextBox.Location = new Point(163, 73);
        _outputFolderTextBox.Name = "_outputFolderTextBox";
        _outputFolderTextBox.Size = new Size(100, 23);
        _outputFolderTextBox.TabIndex = 9;
        // 
        // _outputBrowseBtn
        // 
        _outputBrowseBtn.Dock = DockStyle.Fill;
        _outputBrowseBtn.Location = new Point(689, 73);
        _outputBrowseBtn.Name = "_outputBrowseBtn";
        _outputBrowseBtn.Size = new Size(74, 14);
        _outputBrowseBtn.TabIndex = 10;
        _outputBrowseBtn.Text = "Browse";
        // 
        // _targetFrameworkTextBox
        // 
        _generalTabLayout.SetColumnSpan(_targetFrameworkTextBox, 2);
        _targetFrameworkTextBox.Location = new Point(163, 93);
        _targetFrameworkTextBox.Name = "_targetFrameworkTextBox";
        _targetFrameworkTextBox.Size = new Size(100, 23);
        _targetFrameworkTextBox.TabIndex = 12;
        // 
        // _overwriteCheckBox
        // 
        _generalTabLayout.SetColumnSpan(_overwriteCheckBox, 2);
        _overwriteCheckBox.Location = new Point(163, 113);
        _overwriteCheckBox.Name = "_overwriteCheckBox";
        _overwriteCheckBox.Size = new Size(104, 14);
        _overwriteCheckBox.TabIndex = 13;
        // 
        // _backupCheckBox
        // 
        _generalTabLayout.SetColumnSpan(_backupCheckBox, 2);
        _backupCheckBox.Location = new Point(163, 133);
        _backupCheckBox.Name = "_backupCheckBox";
        _backupCheckBox.Size = new Size(104, 14);
        _backupCheckBox.TabIndex = 14;
        // 
        // _generatorsTab
        // 
        _generatorsTab.Location = new Point(4, 24);
        _generatorsTab.Name = "_generatorsTab";
        _generatorsTab.Size = new Size(776, 483);
        _generatorsTab.TabIndex = 1;
        _generatorsTab.Text = "Generators";
        // 
        // _packagesTab
        // 
        _packagesTab.Location = new Point(4, 24);
        _packagesTab.Name = "_packagesTab";
        _packagesTab.Size = new Size(776, 483);
        _packagesTab.TabIndex = 2;
        _packagesTab.Text = "NuGet Packages";
        // 
        // _buttonPanel
        // 
        _buttonPanel.Controls.Add(_saveButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.Controls.Add(_loadDefaultsButton);
        _buttonPanel.Dock = DockStyle.Bottom;
        _buttonPanel.Location = new Point(0, 511);
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.Size = new Size(784, 50);
        _buttonPanel.TabIndex = 1;
        // 
        // _saveButton
        // 
        _saveButton.DialogResult = DialogResult.OK;
        _saveButton.Location = new Point(600, 12);
        _saveButton.Name = "_saveButton";
        _saveButton.Size = new Size(80, 28);
        _saveButton.TabIndex = 0;
        _saveButton.Text = "Save";
        _saveButton.Click += SaveButton_Click;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Location = new Point(690, 12);
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(80, 28);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        // 
        // _loadDefaultsButton
        // 
        _loadDefaultsButton.Location = new Point(12, 12);
        _loadDefaultsButton.Name = "_loadDefaultsButton";
        _loadDefaultsButton.Size = new Size(100, 28);
        _loadDefaultsButton.TabIndex = 2;
        _loadDefaultsButton.Text = "Load Defaults";
        _loadDefaultsButton.Click += LoadDefaultsButton_Click;
        // 
        // SettingsForm
        // 
        AcceptButton = _saveButton;
        CancelButton = _cancelButton;
        ClientSize = new Size(784, 561);
        Controls.Add(_tabControl);
        Controls.Add(_buttonPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Generator Settings";
        _tabControl.ResumeLayout(false);
        _generalTab.ResumeLayout(false);
        _generalTabLayout.ResumeLayout(false);
        _generalTabLayout.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void CreateGeneralTab()
    {
        var _generalTabLayout = new TableLayoutPanel();
        _generalTabLayout.Dock = DockStyle.Fill;
        _generalTabLayout.Padding = new Padding(10);
        _generalTabLayout.ColumnCount = 3;
        _generalTabLayout.RowCount = 8;

        _generalTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        _generalTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _generalTabLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));

        //int row = 0;

        _generalTabLayout.Controls.Add(new Label { Text = "Solution Root Folder:", Anchor = AnchorStyles.Left }, 0, 0);
        _solutionRootTextBox = new TextBox { Dock = DockStyle.Fill };
        _generalTabLayout.Controls.Add(_solutionRootTextBox, 1, 0);
        var solutionBrowseBtn = new Button { Text = "Browse", Dock = DockStyle.Fill };
        solutionBrowseBtn.Click += (s, e) => BrowseFolder(_solutionRootTextBox);
        _generalTabLayout.Controls.Add(solutionBrowseBtn, 2, 0);

        _generalTabLayout.Controls.Add(new Label { Text = "Root Namespace:", Anchor = AnchorStyles.Left }, 0, 1);
        _namespaceTextBox = new TextBox { Dock = DockStyle.Fill };
        _generalTabLayout.Controls.Add(_namespaceTextBox, 1, 1);
        _generalTabLayout.SetColumnSpan(_namespaceTextBox, 2);

        _generalTabLayout.Controls.Add(new Label { Text = "Template Folder:", Anchor = AnchorStyles.Left }, 0, 2);
        _templateFolderTextBox = new TextBox { Dock = DockStyle.Fill };
        _generalTabLayout.Controls.Add(_templateFolderTextBox, 1, 2);
        var templateBrowseBtn = new Button { Text = "Browse", Dock = DockStyle.Fill };
        templateBrowseBtn.Click += (s, e) => BrowseFolder(_templateFolderTextBox);
        _generalTabLayout.Controls.Add(templateBrowseBtn, 2, 2);

        _generalTabLayout.Controls.Add(new Label { Text = "Output Folder:", Anchor = AnchorStyles.Left }, 0, 3);
        _outputFolderTextBox = new TextBox { Dock = DockStyle.Fill };
        _generalTabLayout.Controls.Add(_outputFolderTextBox, 1, 3);
        var outputBrowseBtn = new Button { Text = "Browse", Dock = DockStyle.Fill };
        outputBrowseBtn.Click += (s, e) => BrowseFolder(_outputFolderTextBox);
        _generalTabLayout.Controls.Add(outputBrowseBtn, 2, 3);

        _generalTabLayout.Controls.Add(new Label { Text = "Target Framework:", Anchor = AnchorStyles.Left }, 0, 4);
        _targetFrameworkTextBox = new TextBox { Dock = DockStyle.Fill };
        _generalTabLayout.Controls.Add(_targetFrameworkTextBox, 1, 4);
        _generalTabLayout.SetColumnSpan(_targetFrameworkTextBox, 2);

        _overwriteCheckBox = new CheckBox { Text = "Overwrite existing files", Anchor = AnchorStyles.Left };
        _generalTabLayout.Controls.Add(_overwriteCheckBox, 1, 5);
        _generalTabLayout.SetColumnSpan(_overwriteCheckBox, 2);

        _backupCheckBox = new CheckBox { Text = "Create backup before overwriting", Anchor = AnchorStyles.Left };
        _generalTabLayout.Controls.Add(_backupCheckBox, 1, 6);
        _generalTabLayout.SetColumnSpan(_backupCheckBox, 2);
        _generalTab.Controls.Clear();
        _generalTab.Controls.Add(_generalTabLayout);
    }

    private void CreateGeneratorsTab()
    {
        _generatorsGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        _generatorsGrid.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewCheckBoxColumn { Name = "Enabled", HeaderText = "Enabled", Width = 60 },
            new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "Layer", HeaderText = "Layer", ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "OutputPath", HeaderText = "Output Path" }
        });

        _generatorsTab.Controls.Add(_generatorsGrid);
    }

    private void CreatePackagesTab()
    {
        _packagesGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = true,
            AllowUserToDeleteRows = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        _packagesGrid.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "PackageId", HeaderText = "Package ID" },
            new DataGridViewTextBoxColumn { Name = "Version", HeaderText = "Version" },
            new DataGridViewTextBoxColumn { Name = "Layers", HeaderText = "Layers" }
        });

        _packagesTab.Controls.Add(_packagesGrid);
    }
}
