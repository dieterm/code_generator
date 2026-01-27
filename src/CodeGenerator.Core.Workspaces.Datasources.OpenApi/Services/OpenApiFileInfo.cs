namespace CodeGenerator.Core.Workspaces.Datasources.OpenApi.Services;

/// <summary>
/// Information about an OpenAPI specification file
/// </summary>
public class OpenApiFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public int SchemaCount { get; set; }
    public List<OpenApiSchemaInfo> Schemas { get; set; } = new();

    public string DisplayName => $"{Title ?? FileName} v{Version} ({SchemaCount} schemas)";
}

/// <summary>
/// Information about an OpenAPI schema (from components/schemas)
/// </summary>
public class OpenApiSchemaInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SchemaType { get; set; } = "object";
    public bool IsEnum { get; set; }
    public List<string>? EnumValues { get; set; }
    public int PropertyCount { get; set; }
    public List<OpenApiPropertyInfo> Properties { get; set; } = new();

    public string TypeDisplay => IsEnum ? "enum" : SchemaType;
}

/// <summary>
/// Information about a property in an OpenAPI schema
/// </summary>
public class OpenApiPropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public string? Format { get; set; }
    public bool IsNullable { get; set; } = true;
    public bool IsRequired { get; set; }
    public bool IsArray { get; set; }
    public bool IsReference { get; set; }
    public string? ReferencedSchemaName { get; set; }
    public string InferredGenericType { get; set; } = string.Empty;

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
}
