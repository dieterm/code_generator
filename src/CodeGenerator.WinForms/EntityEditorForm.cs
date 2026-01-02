using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.WinForms;

public partial class EntityEditorForm : Form
{
    public string EntityName { get; private set; } = string.Empty;
    public EntityDefinition Entity { get; private set; } = new();

    public EntityEditorForm(string? existingName = null, EntityDefinition? existingEntity = null)
    {
        if (existingName != null) EntityName = existingName;
        if (existingEntity != null) Entity = existingEntity;

        InitializeComponent();

        if (existingEntity != null)
        {
            LoadEntity();
        }
    }

    private void LoadEntity()
    {
        _nameTextBox.Text = EntityName;
        _titleTextBox.Text = Entity.Title;
        _descriptionTextBox.Text = Entity.Description;
        _tableNameTextBox.Text = Entity.DatabaseMetadata?.TableName;
        _schemaTextBox.Text = Entity.DatabaseMetadata?.Schema ?? "dbo";
        _baseClassTextBox.Text = Entity.CodeGenMetadata?.BaseClass;
        _isAbstractCheckBox.Checked = Entity.CodeGenMetadata?.IsAbstract ?? false;
        _isSealedCheckBox.Checked = Entity.CodeGenMetadata?.IsSealed ?? false;
        _isOwnedTypeCheckBox.Checked = Entity.CodeGenMetadata?.IsOwnedType ?? false;
        _generateRepoCheckBox.Checked = Entity.CodeGenMetadata?.GenerateRepository ?? true;
        _generateControllerCheckBox.Checked = Entity.CodeGenMetadata?.GenerateController ?? true;
        _generateViewModelCheckBox.Checked = Entity.CodeGenMetadata?.GenerateViewModel ?? true;
        _generateViewCheckBox.Checked = Entity.CodeGenMetadata?.GenerateView ?? true;
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            MessageBox.Show("Entity name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        EntityName = _nameTextBox.Text.Trim();

        Entity = new EntityDefinition
        {
            Type = "object",
            Title = _titleTextBox.Text,
            Description = _descriptionTextBox.Text,
            Properties = Entity.Properties ?? new Dictionary<string, PropertyDefinition>(),
            Required = Entity.Required,
            DatabaseMetadata = new EntityDatabaseMetadata
            {
                TableName = string.IsNullOrEmpty(_tableNameTextBox.Text) ? EntityName : _tableNameTextBox.Text,
                Schema = _schemaTextBox.Text
            },
            CodeGenMetadata = new EntityCodeGenMetadata
            {
                ClassName = EntityName,
                BaseClass = string.IsNullOrEmpty(_baseClassTextBox.Text) ? null : _baseClassTextBox.Text,
                IsAbstract = _isAbstractCheckBox.Checked,
                IsSealed = _isSealedCheckBox.Checked,
                IsOwnedType = _isOwnedTypeCheckBox.Checked,
                GenerateRepository = _generateRepoCheckBox.Checked,
                GenerateController = _generateControllerCheckBox.Checked,
                GenerateViewModel = _generateViewModelCheckBox.Checked,
                GenerateView = _generateViewCheckBox.Checked
            }
        };
    }
}
