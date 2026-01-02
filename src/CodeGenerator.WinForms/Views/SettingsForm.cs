using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;

namespace CodeGenerator.WinForms;

public partial class SettingsForm : Form
{
    private readonly ISettingsService _settingsService;
    private const string ConfigFileName = "AppConfig.json";
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
        CreateGeneratorsTab();
        LoadSettings();
    }

    private void SolutionBrowseBtn_Click(object? sender, EventArgs e)
    {
        BrowseFolder(_solutionRootTextBox);
    }

    private void TemplateBrowseBtn_Click(object? sender, EventArgs e)
    {
        BrowseFolder(_templateFolderTextBox);
    }

    private void OutputBrowseBtn_Click(object? sender, EventArgs e)
    {
        BrowseFolder(_outputFolderTextBox);
    }

    private void CreateGeneratorsTab()
    {
        _generatorsGrid = new DataGridView();
        _generatorsGrid.Dock = DockStyle.Fill;
        _generatorsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _generatorsGrid.AllowUserToAddRows = false;
        _generatorsGrid.AllowUserToDeleteRows = false;
        _generatorsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _generatorsGrid.MultiSelect = false;

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

    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            SaveSettings();
            
            var configPath = GetConfigFilePath();
            await _settingsService.SaveSettingsAsync(Settings, configPath);
            
            MessageBox.Show(
                $"Settings successfully saved to:\n{configPath}",
                "Settings Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to save settings:\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void LoadDefaultsButton_Click(object? sender, EventArgs e)
    {
        Settings = _settingsService.GetDefaultSettings();
        LoadSettings();
    }

    private void BrowseFolder(TextBox textBox)
    {
        using var dialog = new FolderBrowserDialog();
        dialog.SelectedPath = textBox.Text;

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

    private string GetConfigFilePath()
    {
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataFolder, "CodeGenerator");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        return Path.Combine(appFolder, ConfigFileName);
    }
}
