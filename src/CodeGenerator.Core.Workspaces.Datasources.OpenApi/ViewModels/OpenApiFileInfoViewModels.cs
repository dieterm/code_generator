using CodeGenerator.Core.Workspaces.Datasources.OpenApi.Services;

namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;

/// <summary>
/// ViewModel for OpenAPI file info display
/// </summary>
public class OpenApiFileInfoViewModel
{
    public string FileName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public int SchemaCount { get; set; }
    public List<OpenApiSchemaInfoViewModel> Schemas { get; set; } = new();

    public string DisplayName => $"{Title ?? FileName} v{Version} ({SchemaCount} schemas)";

    public static OpenApiFileInfoViewModel FromFileInfo(OpenApiFileInfo info)
    {
        return new OpenApiFileInfoViewModel
        {
            FileName = info.FileName,
            Title = info.Title,
            Version = info.Version,
            Description = info.Description,
            SchemaCount = info.SchemaCount,
            Schemas = info.Schemas
                .Select(OpenApiSchemaInfoViewModel.FromSchemaInfo)
                .ToList()
        };
    }
}

/// <summary>
/// ViewModel for schema display
/// </summary>
public class OpenApiSchemaInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SchemaType { get; set; } = "object";
    public bool IsEnum { get; set; }
    public List<string>? EnumValues { get; set; }
    public int PropertyCount { get; set; }
    public List<OpenApiPropertyInfoViewModel> Properties { get; set; } = new();

    public string TypeDisplay => IsEnum ? "enum" : SchemaType;

    public string IconKey => IsEnum ? "list-ordered" : "braces";

    public static OpenApiSchemaInfoViewModel FromSchemaInfo(OpenApiSchemaInfo info)
    {
        return new OpenApiSchemaInfoViewModel
        {
            Name = info.Name,
            Description = info.Description,
            SchemaType = info.SchemaType,
            IsEnum = info.IsEnum,
            EnumValues = info.EnumValues,
            PropertyCount = info.PropertyCount,
            Properties = info.Properties
                .Select(OpenApiPropertyInfoViewModel.FromPropertyInfo)
                .ToList()
        };
    }
}

/// <summary>
/// ViewModel for property display
/// </summary>
public class OpenApiPropertyInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public string? Format { get; set; }
    public bool IsNullable { get; set; }
    public bool IsRequired { get; set; }
    public bool IsArray { get; set; }
    public bool IsReference { get; set; }
    public string? ReferencedSchemaName { get; set; }

    public string TypeDisplay
    {
        get
        {
            var type = IsReference ? ReferencedSchemaName ?? DataType : DataType;
            if (!string.IsNullOrEmpty(Format))
                type = $"{type} ({Format})";
            if (IsArray)
                type = $"{type}[]";
            return type;
        }
    }

    public static OpenApiPropertyInfoViewModel FromPropertyInfo(OpenApiPropertyInfo info)
    {
        return new OpenApiPropertyInfoViewModel
        {
            Name = info.Name,
            Description = info.Description,
            DataType = info.DataType,
            Format = info.Format,
            IsNullable = info.IsNullable,
            IsRequired = info.IsRequired,
            IsArray = info.IsArray,
            IsReference = info.IsReference,
            ReferencedSchemaName = info.ReferencedSchemaName
        };
    }
}
