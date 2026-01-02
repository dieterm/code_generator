using System.Text.Json;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Generators;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.WinForms;

public partial class MainForm : Form
{
    private readonly ISchemaParser _schemaParser;
    private readonly ISettingsService _settingsService;
    private readonly ITemplateEngine _templateEngine;
    private readonly GeneratorOrchestrator _orchestrator;
    private readonly ILogger<MainForm> _logger;
    private const string ConfigFileName = "AppConfig.json";

    private DomainSchema? _currentSchema;
    private DomainContext? _currentContext;
    private GeneratorSettings _settings;
    private string? _currentFilePath;
    private bool _isDirty;

    public MainForm(
        ISchemaParser schemaParser,
        ISettingsService settingsService,
        ITemplateEngine templateEngine,
        GeneratorOrchestrator orchestrator,
        ILogger<MainForm> logger)
    {
        _schemaParser = schemaParser;
        _settingsService = settingsService;
        _templateEngine = templateEngine;
        _orchestrator = orchestrator;
        _logger = logger;

        InitializeComponent();
        SetupEventHandlers();
        LoadSettingsAsync();
    }

    private async void LoadSettingsAsync()
    {
        try
        {
            var configPath = GetConfigFilePath();
            
            if (File.Exists(configPath))
            {
                _settings = await _settingsService.LoadSettingsAsync(configPath);
                Log($"Loaded settings from: {configPath}");
            }
            else
            {
                _settings = _settingsService.GetDefaultSettings();
                Log("Using default settings (no saved configuration found)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings, using defaults");
            _settings = _settingsService.GetDefaultSettings();
            Log("Failed to load settings, using defaults");
        }
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

    private void SetupEventHandlers()
    {
        _schemaTreeView.AfterSelect += SchemaTreeView_AfterSelect;
        _schemaTreeView.NodeMouseDoubleClick += SchemaTreeView_NodeMouseDoubleClick;
        _jsonEditor.TextChanged += JsonEditor_TextChanged;
        _outputTreeView.AfterSelect += OutputTreeView_AfterSelect;
        FormClosing += MainForm_FormClosing;
    }

    private void SchemaTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag != null)
        {
            _propertyGrid.SelectedObject = e.Node.Tag;
        }
    }

    private void SchemaTreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        // Open entity editor or property editor
    }

    private void JsonEditor_TextChanged(object? sender, EventArgs e)
    {
        _isDirty = true;
        UpdateTitle();
    }

    private async void OutputTreeView_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is FilePreview preview)
        {
            _previewTextBox.Text = preview.Content;
            _previewTabControl.SelectedIndex = 2; // Switch to Preview tab
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isDirty)
        {
            var result = MessageBox.Show(
                "You have unsaved changes. Do you want to save before closing?",
                "Unsaved Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                OnSaveSchema(this, EventArgs.Empty);
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }

    private void OnNewSchema(object? sender, EventArgs e)
    {
        if (_isDirty && !ConfirmDiscardChanges()) return;

        _currentSchema = new DomainSchema
        {
            Schema = "https://json-schema.org/draft/2020-12/schema",
            Title = "New Domain Schema",
            Description = "Domain-driven design schema for code generation",
            Definitions = new Dictionary<string, EntityDefinition>(),
            CodeGenMetadata = new CodeGenMetadata
            {
                Namespace = "MyCompany.MyProject",
                TargetLanguage = "CSharp",
                DataLayerTechnology = "EntityFrameworkCore"
            },
            DatabaseMetadata = new DatabaseMetadata
            {
                DatabaseName = "MyDatabase",
                Schema = "dbo",
                Provider = "SqlServer"
            }
        };

        _currentFilePath = null;
        _isDirty = false;
        RefreshUI();
        UpdateTitle();
        Log("Created new schema");
    }

    private async void OnOpenSchema(object? sender, EventArgs e)
    {
        if (_isDirty && !ConfirmDiscardChanges()) return;

        using var dialog = new OpenFileDialog
        {
            Filter = "JSON Schema Files (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Open Domain Schema"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            await LoadSchemaAsync(dialog.FileName);
        }
    }

    private async Task LoadSchemaAsync(string filePath)
    {
        try
        {
            SetBusy(true, "Loading schema...");

            var json = await File.ReadAllTextAsync(filePath);
            _currentSchema = JsonSerializer.Deserialize<DomainSchema>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            _currentContext = await _schemaParser.ParseAsync(filePath);
            _currentFilePath = filePath;
            _isDirty = false;

            RefreshUI();
            UpdateTitle();
            Log($"Loaded schema from: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load schema");
            MessageBox.Show($"Failed to load schema: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnSaveSchema(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            OnSaveSchemaAs(sender, e);
            return;
        }

        await SaveSchemaAsync(_currentFilePath);
    }

    private async void OnSaveSchemaAs(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "JSON Schema Files (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Save Domain Schema",
            DefaultExt = "json"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            await SaveSchemaAsync(dialog.FileName);
        }
    }

    private async Task SaveSchemaAsync(string filePath)
    {
        try
        {
            SetBusy(true, "Saving schema...");

            var json = JsonSerializer.Serialize(_currentSchema, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);

            _currentFilePath = filePath;
            _isDirty = false;
            UpdateTitle();
            Log($"Saved schema to: {filePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save schema");
            MessageBox.Show($"Failed to save schema: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void OnOpenSettings(object? sender, EventArgs e)
    {
        using var dialog = new SettingsForm(_settings, _settingsService);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _settings = dialog.Settings;
            Log("Settings updated");
        }
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Close();
    }

    private void OnAddEntity(object? sender, EventArgs e)
    {
        if (_currentSchema == null)
        {
            MessageBox.Show("Please create or open a schema first.", "No Schema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new EntityEditorForm();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _currentSchema.Definitions ??= new Dictionary<string, EntityDefinition>();
            _currentSchema.Definitions[dialog.EntityName] = dialog.Entity;
            _isDirty = true;
            RefreshUI();
            Log($"Added entity: {dialog.EntityName}");
        }
    }

    private void OnAddProperty(object? sender, EventArgs e)
    {
        var selectedNode = _schemaTreeView.SelectedNode;
        if (selectedNode?.Tag is not EntityDefinition entity && selectedNode?.Parent?.Tag is not EntityDefinition)
        {
            MessageBox.Show("Please select an entity first.", "No Entity Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new PropertyEditorForm();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var targetEntity = selectedNode.Tag as EntityDefinition ?? selectedNode.Parent?.Tag as EntityDefinition;
            if (targetEntity != null)
            {
                targetEntity.Properties ??= new Dictionary<string, PropertyDefinition>();
                targetEntity.Properties[dialog.PropertyName] = dialog.Property;
                _isDirty = true;
                RefreshUI();
                Log($"Added property: {dialog.PropertyName}");
            }
        }
    }

    private void OnDeleteSelected(object? sender, EventArgs e)
    {
        var selectedNode = _schemaTreeView.SelectedNode;
        if (selectedNode == null || _currentSchema == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{selectedNode.Text}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            if (selectedNode.Tag is EntityDefinition && selectedNode.Parent == null)
            {
                // Delete entity
                var entityName = selectedNode.Text;
                _currentSchema.Definitions?.Remove(entityName);
                Log($"Deleted entity: {entityName}");
            }
            else if (selectedNode.Tag is PropertyDefinition && selectedNode.Parent?.Tag is EntityDefinition parentEntity)
            {
                // Delete property
                var propName = selectedNode.Text;
                parentEntity.Properties?.Remove(propName);
                Log($"Deleted property: {propName}");
            }

            _isDirty = true;
            RefreshUI();
        }
    }

    private async void OnPreviewAll(object? sender, EventArgs e)
    {
        if (_currentFilePath == null || _currentSchema == null)
        {
            MessageBox.Show("Please save the schema first.", "Schema Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            SetBusy(true, "Generating preview...");
            _settings.SchemaFilePath = _currentFilePath;

            var preview = await _orchestrator.PreviewAllAsync(_settings);

            RefreshOutputTree(preview);
            _previewTabControl.SelectedIndex = 1; // Switch to Output Structure tab

            Log($"Preview generated: {preview.TotalFiles} files, {preview.TotalProjects} projects");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate preview");
            MessageBox.Show($"Failed to generate preview: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnGenerateAll(object? sender, EventArgs e)
    {
        if (_currentFilePath == null || _currentSchema == null)
        {
            MessageBox.Show("Please save the schema first.", "Schema Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show(
            "This will generate code for all configured layers. Continue?",
            "Confirm Generation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes) return;

        await GenerateCodeAsync(null);
    }

    private async void OnGenerateLayer(string generatorId)
    {
        await GenerateCodeAsync(generatorId);
    }

    private async Task GenerateCodeAsync(string? generatorId)
    {
        if (_currentFilePath == null)
        {
            MessageBox.Show("Please save the schema first.", "Schema Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            SetBusy(true, "Generating code...");
            _settings.SchemaFilePath = _currentFilePath;

            GenerationResult genResult;
            if (string.IsNullOrEmpty(generatorId))
            {
                genResult = await _orchestrator.GenerateAllAsync(_settings);
            }
            else
            {
                genResult = await _orchestrator.GenerateAsync(generatorId, _settings);
            }

            foreach (var msg in genResult.Messages)
                Log(msg);

            foreach (var warn in genResult.Warnings)
                Log($"Warning: {warn}");

            foreach (var err in genResult.Errors)
                Log($"Error: {err}");

            if (genResult.Success)
            {
                Log($"Generation completed successfully. {genResult.Files.Count} files generated.");
                MessageBox.Show(
                    $"Code generation completed successfully!\n\nGenerated {genResult.Files.Count} files in {genResult.Duration.TotalSeconds:F2} seconds.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Code generation completed with errors.\n\n{string.Join("\n", genResult.Errors.Take(5))}",
                    "Errors",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Code generation failed");
            MessageBox.Show($"Code generation failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void OnRefreshPreview(object? sender, EventArgs e)
    {
        OnPreviewAll(sender, e);
    }

    private void OnClearLog(object? sender, EventArgs e)
    {
        _logListBox.Items.Clear();
    }

    private void OnAbout(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Code Generator - Domain Driven Design\n\n" +
            "A versatile code generation tool for DDD architectures.\n\n" +
            "Supports:\n" +
            "• Multiple programming languages (C#, SQL, JavaScript, etc.)\n" +
            "• Multiple presentation technologies (WinForms, WPF, WinUI, React, etc.)\n" +
            "• Multiple data layer technologies (EF Core, Dapper, etc.)\n\n" +
            "Version 1.0.0",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void RefreshUI()
    {
        RefreshSchemaTree();
        RefreshJsonEditor();
    }

    private void RefreshSchemaTree()
    {
        _schemaTreeView.BeginUpdate();
        _schemaTreeView.Nodes.Clear();

        if (_currentSchema == null)
        {
            _schemaTreeView.EndUpdate();
            return;
        }

        var rootNode = new TreeNode(_currentSchema.Title ?? "Domain Schema")
        {
            ImageKey="Schema",
            SelectedImageKey="Schema",
            Tag = _currentSchema
        };

        if(_currentSchema.CodeGenMetadata==null)
            _currentSchema.CodeGenMetadata = new CodeGenMetadata();

        if (_currentSchema.CodeGenMetadata != null)
        {
            var codeGenNode = new TreeNode("Code Generation Settings") { 
                Tag = _currentSchema.CodeGenMetadata ,
                ImageKey= "CodeGenSettings",
                SelectedImageKey= "CodeGenSettings"
            };
            rootNode.Nodes.Add(codeGenNode);
        }
        if (_currentSchema.DatabaseMetadata == null)
            _currentSchema.DatabaseMetadata = new DatabaseMetadata();

        if (_currentSchema.DatabaseMetadata != null)
        {
            var dbNode = new TreeNode("Database Settings") { 
                Tag = _currentSchema.DatabaseMetadata,
                ImageKey= "DatabaseSettings",
                SelectedImageKey= "DatabaseSettings"
            };
            rootNode.Nodes.Add(dbNode);
        }
        if (_currentSchema.DomainDrivenDesignMetadata == null)
            _currentSchema.DomainDrivenDesignMetadata = new DomainDrivenDesignMetadata();

        if (_currentSchema.DomainDrivenDesignMetadata != null)
        {
            var dbNode = new TreeNode("Domain Driven Design Settings") {
                Tag = _currentSchema.DomainDrivenDesignMetadata,
                ImageKey= "DDDSettings",
                SelectedImageKey= "DDDSettings"
            };
            rootNode.Nodes.Add(dbNode);
        }

        if (_currentSchema.Definitions != null)
        {
            var valueTypeNodes = new TreeNode("Value Types") {
                ImageKey= "ValueTypes",
                SelectedImageKey= "ValueTypes"
            };
            foreach (var (name, entity) in _currentSchema.Definitions.Where(d => d.Value.DomainDrivenDesignMetadata != null && d.Value.DomainDrivenDesignMetadata.ValueObject==true))
            {
                var valueTypeNode = new TreeNode(name) { Tag = entity };

                if (entity.Properties != null)
                {
                    foreach (var (propName, prop) in entity.Properties)
                    {
                        var propNode = new TreeNode($"{propName}: {prop.Type ?? "object"}")
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

            foreach (var (name, entity) in _currentSchema.Definitions.Where(d => d.Value.DomainDrivenDesignMetadata == null || d.Value.DomainDrivenDesignMetadata.ValueObject==false))
            {
                var entityNode = new TreeNode(name) { Tag = entity };

                if (entity.Properties != null)
                {
                    foreach (var (propName, prop) in entity.Properties)
                    {
                        var propNode = new TreeNode($"{propName}: {prop.Type ?? "object"}")
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

    private void RefreshJsonEditor()
    {
        if (_currentSchema == null)
        {
            _jsonEditor.Text = string.Empty;
            return;
        }

        var json = JsonSerializer.Serialize(_currentSchema, new JsonSerializerOptions { WriteIndented = true });
        _jsonEditor.Text = json;
    }

    private void RefreshOutputTree(GenerationPreview preview)
    {
        _outputTreeView.BeginUpdate();
        _outputTreeView.Nodes.Clear();

        AddFolderNode(_outputTreeView.Nodes, preview.RootFolder);

        if (_outputTreeView.Nodes.Count > 0)
        {
            _outputTreeView.Nodes[0].ExpandAll();
        }

        _outputTreeView.EndUpdate();
    }

    private void AddFolderNode(TreeNodeCollection parentNodes, FolderNode folder)
    {
        var node = new TreeNode(folder.Name);

        foreach (var subFolder in folder.Folders)
        {
            AddFolderNode(node.Nodes, subFolder);
        }

        foreach (var file in folder.Files)
        {
            var fileNode = new TreeNode(file.FileName)
            {
                Tag = file,
                ForeColor = file.WillOverwrite ? Color.Orange : Color.Green
            };
            node.Nodes.Add(fileNode);
        }

        parentNodes.Add(node);
    }

    private void UpdateTitle()
    {
        var fileName = string.IsNullOrEmpty(_currentFilePath) ? "Untitled" : Path.GetFileName(_currentFilePath);
        var dirty = _isDirty ? " *" : "";
        Text = $"Code Generator - {fileName}{dirty}";
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _progressBar.Visible = busy;
        _statusLabel.Text = message ?? "Ready";
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        Enabled = !busy;
        Application.DoEvents();
    }

    private void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        _logListBox.Items.Add($"[{timestamp}] {message}");
        _logListBox.TopIndex = _logListBox.Items.Count - 1;
    }

    private bool ConfirmDiscardChanges()
    {
        return MessageBox.Show(
            "You have unsaved changes. Discard them?",
            "Unsaved Changes",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning) == DialogResult.Yes;
    }
}
