namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Services;

/// <summary>
/// Information about a type in a .NET assembly
/// </summary>
public class AssemblyTypeInfo
{
    /// <summary>
    /// Full name of the type (namespace.typeName)
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the type without namespace
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Namespace of the type
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Whether this is a class
    /// </summary>
    public bool IsClass { get; set; }

    /// <summary>
    /// Whether this is an interface
    /// </summary>
    public bool IsInterface { get; set; }

    /// <summary>
    /// Whether this is an enum
    /// </summary>
    public bool IsEnum { get; set; }

    /// <summary>
    /// Whether this is a struct
    /// </summary>
    public bool IsStruct { get; set; }

    /// <summary>
    /// Whether this is abstract
    /// </summary>
    public bool IsAbstract { get; set; }

    /// <summary>
    /// Whether this is sealed
    /// </summary>
    public bool IsSealed { get; set; }

    /// <summary>
    /// Whether this is a generic type
    /// </summary>
    public bool IsGeneric { get; set; }

    /// <summary>
    /// Base type name (if any)
    /// </summary>
    public string? BaseTypeName { get; set; }

    /// <summary>
    /// Implemented interface names
    /// </summary>
    public List<string> ImplementedInterfaces { get; set; } = new();

    /// <summary>
    /// Properties of this type
    /// </summary>
    public List<AssemblyPropertyInfo> Properties { get; set; } = new();

    /// <summary>
    /// Type kind for display
    /// </summary>
    public string TypeKind
    {
        get
        {
            if (IsInterface) return "Interface";
            if (IsEnum) return "Enum";
            if (IsStruct) return "Struct";
            if (IsAbstract) return "Abstract Class";
            return "Class";
        }
    }
}

/// <summary>
/// Information about a property in a .NET type
/// </summary>
public class AssemblyPropertyInfo
{
    /// <summary>
    /// Name of the property
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type name of the property
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// Full type name including namespace
    /// </summary>
    public string FullTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the property type is nullable
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether the property has a getter
    /// </summary>
    public bool CanRead { get; set; }

    /// <summary>
    /// Whether the property has a setter
    /// </summary>
    public bool CanWrite { get; set; }

    /// <summary>
    /// Whether the property type is a collection
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Whether the property type is a complex type (class/interface)
    /// </summary>
    public bool IsComplexType { get; set; }

    /// <summary>
    /// Element type for collections
    /// </summary>
    public string? CollectionElementTypeName { get; set; }

    /// <summary>
    /// The inferred generic data type ID
    /// </summary>
    public string InferredDataType { get; set; } = string.Empty;
}

/// <summary>
/// Information about an assembly
/// </summary>
public class AssemblyInfo
{
    /// <summary>
    /// Assembly file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Assembly full name
    /// </summary>
    public string? AssemblyFullName { get; set; }

    /// <summary>
    /// Number of types in the assembly
    /// </summary>
    public int TypeCount { get; set; }

    /// <summary>
    /// Number of namespaces
    /// </summary>
    public int NamespaceCount { get; set; }

    /// <summary>
    /// All public types in the assembly
    /// </summary>
    public List<AssemblyTypeInfo> Types { get; set; } = new();

    /// <summary>
    /// All namespaces found
    /// </summary>
    public List<string> Namespaces { get; set; } = new();
}
