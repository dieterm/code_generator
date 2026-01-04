using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class DomainSchemaEditViewModel : ValidationViewModelBase
{
    private DomainSchema? _schema;
    private string _schemaUrl = string.Empty;
    private string _title = string.Empty;
    private string _description = string.Empty;

    public DomainSchemaEditViewModel()
    {
    }

    public DomainSchemaEditViewModel(DomainSchema schema)
    {
        LoadFromSchema(schema);
    }

    public string SchemaUrl
    {
        get => _schemaUrl;
        set => SetProperty(ref _schemaUrl, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public void LoadFromSchema(DomainSchema schema)
    {
        _schema = schema;
        SchemaUrl = schema.Schema ?? string.Empty;
        Title = schema.Title ?? string.Empty;
        Description = schema.Description ?? string.Empty;
    }

    public void UpdateSchema()
    {
        if (_schema != null)
        {
            _schema.Schema = SchemaUrl;
            _schema.Title = Title;
            _schema.Description = Description;
        }
    }

    public override bool Validate()
    {
        ClearValidationErrors();

        if (string.IsNullOrWhiteSpace(Title))
        {
            AddValidationError(nameof(Title), "Title is required.");
        }

        return IsValid;
    }
}
