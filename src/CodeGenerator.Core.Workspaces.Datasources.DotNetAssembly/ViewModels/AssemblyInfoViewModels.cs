using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Services;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;

/// <summary>
/// ViewModel for assembly type display in the tree view
/// </summary>
public class AssemblyTypeInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Namespace { get; set; }
    public string TypeKind { get; set; } = string.Empty;
    public bool IsInterface { get; set; }
    public bool IsEnum { get; set; }
    public bool IsClass { get; set; }
    public bool IsStruct { get; set; }
    public bool IsAbstract { get; set; }
    public int PropertyCount { get; set; }
    public List<AssemblyPropertyInfoViewModel> Properties { get; set; } = new();

    public static AssemblyTypeInfoViewModel FromAssemblyTypeInfo(AssemblyTypeInfo info)
    {
        return new AssemblyTypeInfoViewModel
        {
            Name = info.Name,
            FullName = info.FullName,
            Namespace = info.Namespace,
            TypeKind = info.TypeKind,
            IsInterface = info.IsInterface,
            IsEnum = info.IsEnum,
            IsClass = info.IsClass,
            IsStruct = info.IsStruct,
            IsAbstract = info.IsAbstract,
            PropertyCount = info.Properties.Count,
            Properties = info.Properties
                .Select(AssemblyPropertyInfoViewModel.FromAssemblyPropertyInfo)
                .ToList()
        };
    }

    /// <summary>
    /// Icon key for the type
    /// </summary>
    public string IconKey
    {
        get
        {
            if (IsInterface) return "circle-dot";
            if (IsEnum) return "list-ordered";
            if (IsStruct) return "box";
            return "braces";
        }
    }
}

/// <summary>
/// ViewModel for property display
/// </summary>
public class AssemblyPropertyInfoViewModel
{
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
    public bool IsComplexType { get; set; }
    public string InferredDataType { get; set; } = string.Empty;

    public string TypeDisplay => IsNullable ? $"{TypeName}?" : TypeName;

    public static AssemblyPropertyInfoViewModel FromAssemblyPropertyInfo(AssemblyPropertyInfo info)
    {
        return new AssemblyPropertyInfoViewModel
        {
            Name = info.Name,
            TypeName = info.TypeName,
            IsNullable = info.IsNullable,
            IsCollection = info.IsCollection,
            IsComplexType = info.IsComplexType,
            InferredDataType = info.InferredDataType
        };
    }
}

/// <summary>
/// ViewModel for assembly info display
/// </summary>
public class AssemblyInfoViewModel
{
    public string FileName { get; set; } = string.Empty;
    public string? AssemblyFullName { get; set; }
    public int TypeCount { get; set; }
    public int NamespaceCount { get; set; }
    public List<string> Namespaces { get; set; } = new();
    public List<AssemblyTypeInfoViewModel> Types { get; set; } = new();

    public string DisplayInfo => $"{TypeCount} types in {NamespaceCount} namespaces";

    public static AssemblyInfoViewModel FromAssemblyInfo(AssemblyInfo info)
    {
        return new AssemblyInfoViewModel
        {
            FileName = info.FileName,
            AssemblyFullName = info.AssemblyFullName,
            TypeCount = info.TypeCount,
            NamespaceCount = info.NamespaceCount,
            Namespaces = info.Namespaces,
            Types = info.Types
                .Select(AssemblyTypeInfoViewModel.FromAssemblyTypeInfo)
                .ToList()
        };
    }
}
