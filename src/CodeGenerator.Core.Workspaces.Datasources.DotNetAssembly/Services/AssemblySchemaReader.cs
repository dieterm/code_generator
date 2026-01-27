using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Domain.DataTypes;
using System.Reflection;

namespace CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.Services;

/// <summary>
/// Service for reading type information from .NET assemblies using reflection
/// </summary>
public class AssemblySchemaReader
{
    private const int MaxNestedDepth = 5;

    /// <summary>
    /// Get information about an assembly file
    /// </summary>
    public Task<AssemblyInfo> GetAssemblyInfoAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = new AssemblyInfo
        {
            FileName = Path.GetFileName(filePath)
        };

        if (!File.Exists(filePath))
            return Task.FromResult(result);

        try
        {
            // Load assembly in reflection-only context
            var assembly = Assembly.LoadFrom(filePath);
            result.AssemblyFullName = assembly.FullName;

            // Get all public types
            var types = assembly.GetExportedTypes()
                .Where(t => !t.IsNested) // Exclude nested types at top level
                .OrderBy(t => t.Namespace)
                .ThenBy(t => t.Name)
                .ToList();

            result.TypeCount = types.Count;
            result.Namespaces = types
                .Select(t => t.Namespace ?? "(No Namespace)")
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            result.NamespaceCount = result.Namespaces.Count;

            foreach (var type in types)
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.Types.Add(CreateTypeInfo(type));
            }
        }
        catch (Exception ex)
        {
            // If we can't load the assembly, return basic info with error
            result.AssemblyFullName = $"Error: {ex.Message}";
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Import types from an assembly as TableArtifacts
    /// </summary>
    public Task<List<TableArtifact>> ImportTypesAsync(
        string filePath,
        string datasourceName,
        IEnumerable<string>? typeFilter = null,
        CancellationToken cancellationToken = default)
    {
        var tables = new List<TableArtifact>();

        if (!File.Exists(filePath))
            return Task.FromResult(tables);

        try
        {
            var assembly = Assembly.LoadFrom(filePath);
            var types = assembly.GetExportedTypes()
                .Where(t => !t.IsNested)
                .ToList();

            // Apply filter if provided
            if (typeFilter != null && typeFilter.Any())
            {
                var filterSet = typeFilter.ToHashSet(StringComparer.OrdinalIgnoreCase);
                types = types.Where(t => filterSet.Contains(t.FullName ?? t.Name)).ToList();
            }

            var processedTypes = new HashSet<string>();

            foreach (var type in types)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var table = CreateTableFromType(type, datasourceName, processedTypes, 0);
                tables.Add(table);
            }
        }
        catch (Exception)
        {
            // Return empty list on error
        }

        return Task.FromResult(tables);
    }

    /// <summary>
    /// Import a single type as a TableArtifact
    /// </summary>
    public Task<TableArtifact> ImportTypeAsync(
        string filePath,
        string typeName,
        string datasourceName,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Assembly file not found", filePath);

        var assembly = Assembly.LoadFrom(filePath);
        var type = assembly.GetExportedTypes()
            .FirstOrDefault(t => t.FullName == typeName || t.Name == typeName);

        if (type == null)
            throw new InvalidOperationException($"Type '{typeName}' not found in assembly");

        var processedTypes = new HashSet<string>();
        return Task.FromResult(CreateTableFromType(type, datasourceName, processedTypes, 0));
    }

    private AssemblyTypeInfo CreateTypeInfo(Type type)
    {
        var info = new AssemblyTypeInfo
        {
            FullName = type.FullName ?? type.Name,
            Name = type.Name,
            Namespace = type.Namespace,
            IsClass = type.IsClass && !type.IsAbstract,
            IsInterface = type.IsInterface,
            IsEnum = type.IsEnum,
            IsStruct = type.IsValueType && !type.IsEnum && !type.IsPrimitive,
            IsAbstract = type.IsAbstract && type.IsClass,
            IsSealed = type.IsSealed,
            IsGeneric = type.IsGenericType,
            BaseTypeName = type.BaseType?.Name
        };

        // Get implemented interfaces
        info.ImplementedInterfaces = type.GetInterfaces()
            .Where(i => i.IsPublic)
            .Select(i => i.Name)
            .ToList();

        // Get properties
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            info.Properties.Add(CreatePropertyInfo(prop));
        }

        return info;
    }

    private AssemblyPropertyInfo CreatePropertyInfo(PropertyInfo property)
    {
        var propType = property.PropertyType;
        var (isNullable, underlyingType) = GetNullableInfo(propType);
        var actualType = underlyingType ?? propType;

        var info = new AssemblyPropertyInfo
        {
            Name = property.Name,
            TypeName = GetFriendlyTypeName(actualType),
            FullTypeName = actualType.FullName ?? actualType.Name,
            IsNullable = isNullable,
            CanRead = property.CanRead,
            CanWrite = property.CanWrite,
            IsCollection = IsCollectionType(actualType),
            IsComplexType = IsComplexType(actualType),
            InferredDataType = InferDataType(actualType)
        };

        if (info.IsCollection)
        {
            info.CollectionElementTypeName = GetCollectionElementTypeName(actualType);
        }

        return info;
    }

    private TableArtifact CreateTableFromType(Type type, string datasourceName, HashSet<string> processedTypes, int depth)
    {
        var typeName = type.Name;
        var table = new TableArtifact(typeName, type.Namespace ?? string.Empty);

        // Add decorator to mark as existing
        table.AddDecorator(new ExistingTableDecorator
        {
            OriginalTableName = typeName,
            OriginalSchema = type.Namespace ?? string.Empty,
            ImportedAt = DateTime.UtcNow,
            SourceDatasourceName = datasourceName
        });

        // Mark this type as processed to avoid circular references
        var typeKey = type.FullName ?? type.Name;
        if (processedTypes.Contains(typeKey))
            return table;
        processedTypes.Add(typeKey);

        int ordinal = 1;

        // For enums, add the enum values as columns
        if (type.IsEnum)
        {
            foreach (var value in Enum.GetNames(type))
            {
                var column = new ColumnArtifact(value, GenericDataTypes.Int.Id, false)
                {
                    OrdinalPosition = ordinal++
                };
                column.AddDecorator(new ExistingColumnDecorator
                {
                    OriginalName = value,
                    OriginalDataType = GenericDataTypes.Int.Id,
                    OriginalOrdinalPosition = column.OrdinalPosition,
                    OriginalIsNullable = false
                });
                table.AddChild(column);
            }
            return table;
        }

        // Get properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var propType = prop.PropertyType;
            var (isNullable, underlyingType) = GetNullableInfo(propType);
            var actualType = underlyingType ?? propType;
            var dataType = InferDataType(actualType);

            var column = new ColumnArtifact(prop.Name, dataType, isNullable)
            {
                OrdinalPosition = ordinal++
            };

            column.AddDecorator(new ExistingColumnDecorator
            {
                OriginalName = prop.Name,
                OriginalDataType = dataType,
                OriginalOrdinalPosition = column.OrdinalPosition,
                OriginalIsNullable = isNullable
            });

            table.AddChild(column);

            // If property is a complex type and we haven't reached max depth, add nested table
            if (depth < MaxNestedDepth && IsComplexType(actualType))
            {
                var nestedType = actualType;
                
                // For collections, get the element type
                if (IsCollectionType(actualType))
                {
                    nestedType = GetCollectionElementType(actualType);
                }

                if (nestedType != null && IsComplexType(nestedType) && !processedTypes.Contains(nestedType.FullName ?? nestedType.Name))
                {
                    var nestedTable = CreateTableFromType(nestedType, datasourceName, processedTypes, depth + 1);
                    column.AddChild(nestedTable);
                }
            }
        }

        return table;
    }

    private (bool isNullable, Type? underlyingType) GetNullableInfo(Type type)
    {
        // Check for Nullable<T>
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return (true, Nullable.GetUnderlyingType(type));
        }

        // Reference types are always nullable by default
        if (!type.IsValueType)
        {
            return (true, null);
        }

        return (false, null);
    }

    private string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            var genericTypeName = type.Name;
            var backtickIndex = genericTypeName.IndexOf('`');
            if (backtickIndex > 0)
            {
                genericTypeName = genericTypeName.Substring(0, backtickIndex);
            }

            var genericArgs = type.GetGenericArguments()
                .Select(GetFriendlyTypeName);

            return $"{genericTypeName}<{string.Join(", ", genericArgs)}>";
        }

        return type.Name;
    }

    private bool IsCollectionType(Type type)
    {
        if (type == typeof(string))
            return false;

        return type.IsArray ||
               (type.IsGenericType && (
                   type.GetGenericTypeDefinition() == typeof(List<>) ||
                   type.GetGenericTypeDefinition() == typeof(IList<>) ||
                   type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                   type.GetGenericTypeDefinition() == typeof(HashSet<>) ||
                   type.GetGenericTypeDefinition() == typeof(ISet<>)
               )) ||
               typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
    }

    private bool IsComplexType(Type type)
    {
        if (type == null)
            return false;

        // Exclude primitive types, strings, common BCL types
        if (type.IsPrimitive ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan) ||
            type == typeof(Guid) ||
            type.IsEnum)
        {
            return false;
        }

        // Check for Nullable<T> of primitive types
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
        {
            return IsComplexType(underlyingType);
        }

        // Complex types are classes or interfaces (excluding collections of primitives)
        if (IsCollectionType(type))
        {
            var elementType = GetCollectionElementType(type);
            return elementType != null && IsComplexType(elementType);
        }

        return type.IsClass || type.IsInterface;
    }

    private Type? GetCollectionElementType(Type type)
    {
        if (type.IsArray)
            return type.GetElementType();

        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length == 1)
                return genericArgs[0];
        }

        // Try to get from IEnumerable<T>
        var enumerable = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerable?.GetGenericArguments().FirstOrDefault();
    }

    private string? GetCollectionElementTypeName(Type type)
    {
        var elementType = GetCollectionElementType(type);
        return elementType != null ? GetFriendlyTypeName(elementType) : null;
    }

    private string InferDataType(Type type)
    {
        // Handle nullable
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
            type = underlyingType;

        // Map .NET types to generic data types
        if (type == typeof(bool))
            return GenericDataTypes.Boolean.Id;
        if (type == typeof(byte))
            return GenericDataTypes.TinyInt.Id;
        if (type == typeof(short) || type == typeof(ushort))
            return GenericDataTypes.SmallInt.Id;
        if (type == typeof(int) || type == typeof(uint))
            return GenericDataTypes.Int.Id;
        if (type == typeof(long) || type == typeof(ulong))
            return GenericDataTypes.BigInt.Id;
        if (type == typeof(float))
            return GenericDataTypes.Float.Id;
        if (type == typeof(double))
            return GenericDataTypes.Double.Id;
        if (type == typeof(decimal))
            return GenericDataTypes.Decimal.Id;
        if (type == typeof(string))
            return GenericDataTypes.VarChar.Id;
        if (type == typeof(char))
            return GenericDataTypes.Char.Id;
        if (type == typeof(DateTime))
            return GenericDataTypes.DateTime.Id;
        if (type == typeof(DateTimeOffset))
            return GenericDataTypes.DateTimeOffset.Id;
        if (type == typeof(DateOnly))
            return GenericDataTypes.Date.Id;
        if (type == typeof(TimeOnly) || type == typeof(TimeSpan))
            return GenericDataTypes.Time.Id;
        if (type == typeof(Guid))
            return GenericDataTypes.Guid.Id;
        if (type == typeof(byte[]))
            return GenericDataTypes.Binary.Id;

        // Enums map to int
        if (type.IsEnum)
            return GenericDataTypes.Int.Id;

        // Complex types map to JSON (for nested objects)
        if (type.IsClass || type.IsInterface)
            return GenericDataTypes.Json.Id;

        // Default to varchar
        return GenericDataTypes.VarChar.Id;
    }
}
