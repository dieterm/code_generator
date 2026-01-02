using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;

namespace CodeGenerator.WinForms;

public partial class SettingsForm : Form
{
    private readonly ISettingsService _settingsService;
    public GeneratorSettings Settings { get; private set; }

    public SettingsForm()
    {
        InitializeComponent();
    }

    public SettingsForm(GeneratorSettings settings, ISettingsService settingsService)
    {
        Settings = CloneSettings(settings);
        _settingsService = settingsService;

        InitializeComponent();
        CreateGeneralTab();
        CreateGeneratorsTab();
        CreatePackagesTab();
        LoadSettings();

        _solutionBrowseBtn.Click += (s, e) => BrowseFolder(_solutionRootTextBox);
        _templateBrowseBtn.Click += (s, e) => BrowseFolder(_templateFolderTextBox);
        _outputBrowseBtn.Click += (s, e) => BrowseFolder(_outputFolderTextBox);
    }

    private void CreateGeneralTab(TabPage tab)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 3,
            RowCount = 8
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));

        int row = 0;

        layout.Controls.Add(new Label { Text = "Solution Root Folder:", Anchor = AnchorStyles.Left }, 0, row);
        _solutionRootTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_solutionRootTextBox, 1, row);
        var solutionBrowseBtn = new Button { Text = "Browse", Dock = DockStyle.Fill };
        solutionBrowseBtn.Click += (s, e) => BrowseFolder(_solutionRootTextBox);
        layout.Controls.Add(solutionBrowseBtn, 2, row++);

        layout.Controls.Add(new Label { Text = "Root Namespace:", Anchor = AnchorStyles.Left }, 0, row);
        _namespaceTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_namespaceTextBox, 1, row++);
        layout.SetColumnSpan(_namespaceTextBox, 2);

        layout.Controls.Add(new Label { Text = "Template Folder:", Anchor = AnchorStyles.Left }, 0, row);
        _templateFolderTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_templateFolderTextBox, 1, row);
        var templateBrowseBtn = new Button { Text = "Browse", Dock = DockStyle.Fill };
        templateBrowseBtn.Click += (s, e) => BrowseFolder(_templateFolderTextBox);
        layout.Controls.Add(templateBrowseBtn, 2, row++);

        layout.Controls.Add(new Label { Text = "Output Folder:", Anchor = AnchorStyles.Left }, 0, row);
        _outputFolderTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_outputFolderTextBox, 1, row);
        var outputBrowseBtn = new Button { Text = "Browse", Dock = DockStyle.Fill };
        outputBrowseBtn.Click += (s, e) => BrowseFolder(_outputFolderTextBox);
        layout.Controls.Add(outputBrowseBtn, 2, row++);

        layout.Controls.Add(new Label { Text = "Target Framework:", Anchor = AnchorStyles.Left }, 0, row);
        _targetFrameworkTextBox = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_targetFrameworkTextBox, 1, row++);
        layout.SetColumnSpan(_targetFrameworkTextBox, 2);

        _overwriteCheckBox = new CheckBox { Text = "Overwrite existing files", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_overwriteCheckBox, 1, row++);
        layout.SetColumnSpan(_overwriteCheckBox, 2);

        _backupCheckBox = new CheckBox { Text = "Create backup before overwriting", Anchor = AnchorStyles.Left };
        layout.Controls.Add(_backupCheckBox, 1, row++);
        layout.SetColumnSpan(_backupCheckBox, 2);

        tab.Controls.Add(layout);
    }

    private void CreateGeneratorsTab(TabPage tab)
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

        tab.Controls.Add(_generatorsGrid);
    }

    private void CreatePackagesTab(TabPage tab)
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

        tab.Controls.Add(_packagesGrid);
    }

    private void LoadSettings()
    {
        _solutionRootTextBox.Text = Settings.SolutionRootFolder;
        _namespaceTextBox.Text = Settings.RootNamespace;
        _templateFolderTextBox.Text = Settings.TemplateFolder;
        _outputFolderTextBox.Text = Settings.OutputFolder;
        _targetFrameworkTextBox.Text = Settings.TargetFramework;
        _overwriteCheckBox.Checked = Settings.OverwriteExisting;
        _backupCheckBox.Checked = Settings.CreateBackup;

        _generatorsGrid.Rows.Clear();
        foreach (var (id, config) in Settings.Generators)
        {
            _generatorsGrid.Rows.Add(config.Enabled, config.Id, config.Name, config.Layer.ToString(), config.OutputPathPattern);
        }

        _packagesGrid.Rows.Clear();
        foreach (var package in Settings.NuGetPackages)
        {
            var layers = string.Join(", ", package.Layers.Select(l => l.ToString()));
            _packagesGrid.Rows.Add(package.PackageId, package.Version, layers);
        }
    }

    private void SaveSettings()
    {
        Settings.SolutionRootFolder = _solutionRootTextBox.Text;
        Settings.RootNamespace = _namespaceTextBox.Text;
        Settings.TemplateFolder = _templateFolderTextBox.Text;
        Settings.OutputFolder = _outputFolderTextBox.Text;
        Settings.TargetFramework = _targetFrameworkTextBox.Text;
        Settings.OverwriteExisting = _overwriteCheckBox.Checked;
        Settings.CreateBackup = _backupCheckBox.Checked;

        foreach (DataGridViewRow row in _generatorsGrid.Rows)
        {
            if (row.IsNewRow) continue;

            var id = row.Cells["Id"].Value?.ToString();
            if (id != null && Settings.Generators.TryGetValue(id, out var config))
            {
                config.Enabled = (bool)(row.Cells["Enabled"].Value ?? false);
                config.OutputPathPattern = row.Cells["OutputPath"].Value?.ToString() ?? config.OutputPathPattern;
            }
        }
        
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        SaveSettings();
    }

    private void LoadDefaultsButton_Click(object? sender, EventArgs e)
    {
        Settings = _settingsService.GetDefaultSettings();
        LoadSettings();
    }

    private void BrowseFolder(TextBox textBox)
    {
        using var dialog = new FolderBrowserDialog
        {
            SelectedPath = textBox.Text
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            textBox.Text = dialog.SelectedPath;
        }
    }

    private GeneratorSettings CloneSettings(GeneratorSettings settings)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(settings);
        return System.Text.Json.JsonSerializer.Deserialize<GeneratorSettings>(json) ?? new GeneratorSettings();
    }
}
