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
    private Label _solutionRootFolderLabel;
    private TextBox _solutionRootTextBox;
    private Button _solutionBrowseBtn;
    private Label _rootNamespaceLabel;
    private TextBox _namespaceTextBox;
    private Label _templateFolderLabel;
    private TextBox _templateFolderTextBox;
    private Button _templateBrowseBtn;
    private Label _outputFolderLabel;
    private TextBox _outputFolderTextBox;
    private Button _outputBrowseBtn;
    private Label _targetFrameworkLabel;
    private TextBox _targetFrameworkTextBox;
    private CheckBox _overwriteCheckBox;
    private CheckBox _backupCheckBox;
    private DataGridView _generatorsGrid;
    private DataGridView _packagesGrid;
    private Button _saveButton;
    private Button _cancelButton;
    private Button _loadDefaultsButton;
    private DataGridViewTextBoxColumn _packagesPackageIdColumn;
    private DataGridViewTextBoxColumn _packagesVersionColumn;
    private DataGridViewTextBoxColumn _packagesLayersColumn;
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
        _solutionRootFolderLabel = new Label();
        _solutionRootTextBox = new TextBox();
        _solutionBrowseBtn = new Button();
        _rootNamespaceLabel = new Label();
        _namespaceTextBox = new TextBox();
        _templateFolderLabel = new Label();
        _templateFolderTextBox = new TextBox();
        _templateBrowseBtn = new Button();
        _outputFolderLabel = new Label();
        _outputFolderTextBox = new TextBox();
        _outputBrowseBtn = new Button();
        _targetFrameworkLabel = new Label();
        _targetFrameworkTextBox = new TextBox();
        _overwriteCheckBox = new CheckBox();
        _backupCheckBox = new CheckBox();
        _generatorsTab = new TabPage();
        _packagesTab = new TabPage();
        _packagesGrid = new DataGridView();
        _packagesPackageIdColumn = new DataGridViewTextBoxColumn();
        _packagesVersionColumn = new DataGridViewTextBoxColumn();
        _packagesLayersColumn = new DataGridViewTextBoxColumn();
        _buttonPanel = new Panel();
        _saveButton = new Button();
        _cancelButton = new Button();
        _loadDefaultsButton = new Button();
        _tabControl.SuspendLayout();
        _generalTab.SuspendLayout();
        _generalTabLayout.SuspendLayout();
        _packagesTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_packagesGrid).BeginInit();
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
        _generalTabLayout.Controls.Add(_solutionRootFolderLabel, 0, 0);
        _generalTabLayout.Controls.Add(_solutionRootTextBox, 1, 0);
        _generalTabLayout.Controls.Add(_solutionBrowseBtn, 2, 0);
        _generalTabLayout.Controls.Add(_rootNamespaceLabel, 0, 1);
        _generalTabLayout.Controls.Add(_namespaceTextBox, 1, 1);
        _generalTabLayout.Controls.Add(_templateFolderLabel, 0, 2);
        _generalTabLayout.Controls.Add(_templateFolderTextBox, 1, 2);
        _generalTabLayout.Controls.Add(_templateBrowseBtn, 2, 2);
        _generalTabLayout.Controls.Add(_outputFolderLabel, 0, 3);
        _generalTabLayout.Controls.Add(_outputFolderTextBox, 1, 3);
        _generalTabLayout.Controls.Add(_outputBrowseBtn, 2, 3);
        _generalTabLayout.Controls.Add(_targetFrameworkLabel, 0, 4);
        _generalTabLayout.Controls.Add(_targetFrameworkTextBox, 1, 4);
        _generalTabLayout.Controls.Add(_overwriteCheckBox, 1, 5);
        _generalTabLayout.Controls.Add(_backupCheckBox, 1, 6);
        _generalTabLayout.Dock = DockStyle.Fill;
        _generalTabLayout.Location = new Point(0, 0);
        _generalTabLayout.Name = "_generalTabLayout";
        _generalTabLayout.Padding = new Padding(10);
        _generalTabLayout.RowCount = 8;
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        _generalTabLayout.RowStyles.Add(new RowStyle());
        _generalTabLayout.Size = new Size(776, 483);
        _generalTabLayout.TabIndex = 0;
        // 
        // _solutionRootFolderLabel
        // 
        _solutionRootFolderLabel.Anchor = AnchorStyles.Left;
        _solutionRootFolderLabel.Location = new Point(13, 20);
        _solutionRootFolderLabel.Name = "_solutionRootFolderLabel";
        _solutionRootFolderLabel.Size = new Size(100, 20);
        _solutionRootFolderLabel.TabIndex = 10;
        _solutionRootFolderLabel.Text = "Solution Root Folder:";
        // 
        // _solutionRootTextBox
        // 
        _solutionRootTextBox.Dock = DockStyle.Fill;
        _solutionRootTextBox.Location = new Point(163, 13);
        _solutionRootTextBox.Name = "_solutionRootTextBox";
        _solutionRootTextBox.Size = new Size(520, 23);
        _solutionRootTextBox.TabIndex = 0;
        // 
        // _solutionBrowseBtn
        // 
        _solutionBrowseBtn.Dock = DockStyle.Fill;
        _solutionBrowseBtn.Location = new Point(689, 13);
        _solutionBrowseBtn.Name = "_solutionBrowseBtn";
        _solutionBrowseBtn.Size = new Size(74, 34);
        _solutionBrowseBtn.TabIndex = 1;
        _solutionBrowseBtn.Text = "Browse";
        _solutionBrowseBtn.Click += SolutionBrowseBtn_Click;
        // 
        // _rootNamespaceLabel
        // 
        _rootNamespaceLabel.Anchor = AnchorStyles.Left;
        _rootNamespaceLabel.Location = new Point(13, 60);
        _rootNamespaceLabel.Name = "_rootNamespaceLabel";
        _rootNamespaceLabel.Size = new Size(100, 20);
        _rootNamespaceLabel.TabIndex = 11;
        _rootNamespaceLabel.Text = "Root Namespace:";
        // 
        // _namespaceTextBox
        // 
        _namespaceTextBox.Dock = DockStyle.Fill;
        _namespaceTextBox.Location = new Point(163, 53);
        _namespaceTextBox.Name = "_namespaceTextBox";
        _namespaceTextBox.Size = new Size(520, 23);
        _namespaceTextBox.TabIndex = 2;
        // 
        // _templateFolderLabel
        // 
        _templateFolderLabel.Anchor = AnchorStyles.Left;
        _templateFolderLabel.Location = new Point(13, 100);
        _templateFolderLabel.Name = "_templateFolderLabel";
        _templateFolderLabel.Size = new Size(100, 20);
        _templateFolderLabel.TabIndex = 12;
        _templateFolderLabel.Text = "Template Folder:";
        // 
        // _templateFolderTextBox
        // 
        _templateFolderTextBox.Dock = DockStyle.Fill;
        _templateFolderTextBox.Location = new Point(163, 93);
        _templateFolderTextBox.Name = "_templateFolderTextBox";
        _templateFolderTextBox.Size = new Size(520, 23);
        _templateFolderTextBox.TabIndex = 3;
        // 
        // _templateBrowseBtn
        // 
        _templateBrowseBtn.Dock = DockStyle.Fill;
        _templateBrowseBtn.Location = new Point(689, 93);
        _templateBrowseBtn.Name = "_templateBrowseBtn";
        _templateBrowseBtn.Size = new Size(74, 34);
        _templateBrowseBtn.TabIndex = 4;
        _templateBrowseBtn.Text = "Browse";
        _templateBrowseBtn.Click += TemplateBrowseBtn_Click;
        // 
        // _outputFolderLabel
        // 
        _outputFolderLabel.Anchor = AnchorStyles.Left;
        _outputFolderLabel.Location = new Point(13, 140);
        _outputFolderLabel.Name = "_outputFolderLabel";
        _outputFolderLabel.Size = new Size(100, 20);
        _outputFolderLabel.TabIndex = 13;
        _outputFolderLabel.Text = "Output Folder:";
        // 
        // _outputFolderTextBox
        // 
        _outputFolderTextBox.Dock = DockStyle.Fill;
        _outputFolderTextBox.Location = new Point(163, 133);
        _outputFolderTextBox.Name = "_outputFolderTextBox";
        _outputFolderTextBox.Size = new Size(520, 23);
        _outputFolderTextBox.TabIndex = 5;
        // 
        // _outputBrowseBtn
        // 
        _outputBrowseBtn.Dock = DockStyle.Fill;
        _outputBrowseBtn.Location = new Point(689, 133);
        _outputBrowseBtn.Name = "_outputBrowseBtn";
        _outputBrowseBtn.Size = new Size(74, 34);
        _outputBrowseBtn.TabIndex = 6;
        _outputBrowseBtn.Text = "Browse";
        _outputBrowseBtn.Click += OutputBrowseBtn_Click;
        // 
        // _targetFrameworkLabel
        // 
        _targetFrameworkLabel.Anchor = AnchorStyles.Left;
        _targetFrameworkLabel.Location = new Point(13, 180);
        _targetFrameworkLabel.Name = "_targetFrameworkLabel";
        _targetFrameworkLabel.Size = new Size(100, 20);
        _targetFrameworkLabel.TabIndex = 14;
        _targetFrameworkLabel.Text = "Target Framework:";
        // 
        // _targetFrameworkTextBox
        // 
        _targetFrameworkTextBox.Dock = DockStyle.Fill;
        _targetFrameworkTextBox.Location = new Point(163, 173);
        _targetFrameworkTextBox.Name = "_targetFrameworkTextBox";
        _targetFrameworkTextBox.Size = new Size(520, 23);
        _targetFrameworkTextBox.TabIndex = 7;
        // 
        // _overwriteCheckBox
        // 
        _overwriteCheckBox.Anchor = AnchorStyles.Left;
        _generalTabLayout.SetColumnSpan(_overwriteCheckBox, 2);
        _overwriteCheckBox.Location = new Point(163, 220);
        _overwriteCheckBox.Name = "_overwriteCheckBox";
        _overwriteCheckBox.Size = new Size(104, 20);
        _overwriteCheckBox.TabIndex = 8;
        _overwriteCheckBox.Text = "Overwrite existing files";
        // 
        // _backupCheckBox
        // 
        _backupCheckBox.Anchor = AnchorStyles.Left;
        _generalTabLayout.SetColumnSpan(_backupCheckBox, 2);
        _backupCheckBox.Location = new Point(163, 260);
        _backupCheckBox.Name = "_backupCheckBox";
        _backupCheckBox.Size = new Size(104, 20);
        _backupCheckBox.TabIndex = 9;
        _backupCheckBox.Text = "Create backup before overwriting";
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
        _packagesTab.Controls.Add(_packagesGrid);
        _packagesTab.Location = new Point(4, 24);
        _packagesTab.Name = "_packagesTab";
        _packagesTab.Size = new Size(776, 483);
        _packagesTab.TabIndex = 2;
        _packagesTab.Text = "NuGet Packages";
        // 
        // _packagesGrid
        // 
        _packagesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _packagesGrid.Columns.AddRange(new DataGridViewColumn[] { _packagesPackageIdColumn, _packagesVersionColumn, _packagesLayersColumn });
        _packagesGrid.Dock = DockStyle.Fill;
        _packagesGrid.Location = new Point(0, 0);
        _packagesGrid.Name = "_packagesGrid";
        _packagesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _packagesGrid.Size = new Size(776, 483);
        _packagesGrid.TabIndex = 0;
        // 
        // _packagesPackageIdColumn
        // 
        _packagesPackageIdColumn.HeaderText = "Package ID";
        _packagesPackageIdColumn.Name = "_packagesPackageIdColumn";
        // 
        // _packagesVersionColumn
        // 
        _packagesVersionColumn.HeaderText = "Version";
        _packagesVersionColumn.Name = "_packagesVersionColumn";
        // 
        // _packagesLayersColumn
        // 
        _packagesLayersColumn.HeaderText = "Layers";
        _packagesLayersColumn.Name = "_packagesLayersColumn";
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
        _packagesTab.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_packagesGrid).EndInit();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

}
