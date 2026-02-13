using CodeGenerator.Domain.ProgrammingLanguages;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeGenerator.Domain.CodeElements.Serialization
{
    /// <summary>
    /// JsonConverter for CodeElement and all its abstract subtypes (TypeElement, StatementElement).
    /// Uses AssemblyTypeFullyQualifiedName to resolve the concrete type on deserialization,
    /// following the same principle as MementoObjectFactory.CreateMementoObject().
    /// 
    /// Serialization: manually writes each property, recursing via this converter for any
    /// CodeElement-valued properties to ensure runtime-type serialization.
    /// 
    /// Deserialization: reads the assemblyTypeFullyQualifiedName discriminator to resolve the concrete type,
    /// then manually deserializes each property. Get-only collection properties (e.g. CompositeStatement.Statements,
    /// MethodElement.Body) are populated after construction.
    /// </summary>
    public class CodeElementJsonConverter : JsonConverter<CodeElement>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(CodeElement).IsAssignableFrom(typeToConvert);
        }

        public override CodeElement? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            return DeserializeElement(root, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, CodeElement value, JsonSerializerOptions options)
        {
            SerializeElement(writer, value, options);
        }

        #region Serialization

        private void SerializeElement(Utf8JsonWriter writer, CodeElement value, JsonSerializerOptions options)
        {
            var concreteType = value.GetType();
            var innerOptions = GetInnerOptions(options);

            writer.WriteStartObject();

            var properties = GetSerializableProperties(concreteType);
            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(value);
                var jsonPropName = options.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name;
                writer.WritePropertyName(jsonPropName);

                if (propValue == null)
                {
                    writer.WriteNullValue();
                }
                else if (propValue is CodeElement ce)
                {
                    SerializeElement(writer, ce, options);
                }
                else if (IsCodeElementList(prop.PropertyType, out _))
                {
                    writer.WriteStartArray();
                    foreach (var item in (System.Collections.IEnumerable)propValue)
                    {
                        if (item is CodeElement ceItem)
                            SerializeElement(writer, ceItem, options);
                        else
                            JsonSerializer.Serialize(writer, item, item.GetType(), innerOptions);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    JsonSerializer.Serialize(writer, propValue, propValue.GetType(), innerOptions);
                }
            }

            writer.WriteEndObject();
        }

        private static PropertyInfo[] GetSerializableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0 && !IsComputedProperty(p))
                .ToArray();
        }

        private static bool IsComputedProperty(PropertyInfo prop)
        {
            if (prop.CanWrite) return false;

            // Get-only collections and CodeElement properties we DO serialize
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                return false;
            if (typeof(CodeElement).IsAssignableFrom(prop.PropertyType))
                return false;

            return true;
        }

        #endregion

        #region Deserialization

        private CodeElement? DeserializeElement(JsonElement json, Type typeToConvert, JsonSerializerOptions options)
        {
            if (json.ValueKind == JsonValueKind.Null)
                return null;

            var concreteType = ResolveConcreteType(json, typeToConvert);
            var innerOptions = GetInnerOptions(options);

            // Create instance via parameterless constructor
            var instance = (CodeElement)Activator.CreateInstance(concreteType)!;

            // Populate properties from JSON
            var properties = concreteType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (!prop.CanRead) continue;
                if (prop.GetIndexParameters().Length > 0) continue;
                if (IsComputedProperty(prop)) continue;

                var jsonPropName = options.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name;
                if (!json.TryGetProperty(jsonPropName, out var jsonValue)) continue;

                // Case 1: Single CodeElement property
                if (typeof(CodeElement).IsAssignableFrom(prop.PropertyType))
                {
                    var childElement = DeserializeElement(jsonValue, prop.PropertyType, options);
                    if (prop.CanWrite)
                        prop.SetValue(instance, childElement);
                    // Get-only CodeElement: handled below for Body
                    continue;
                }

                // Case 2: List<T> where T is a CodeElement
                if (IsCodeElementList(prop.PropertyType, out var elementType))
                {
                    if (jsonValue.ValueKind != JsonValueKind.Array) continue;

                    // Get the list (either from setter-created instance or get-only initializer)
                    var list = prop.GetValue(instance) as System.Collections.IList;
                    if (list == null && prop.CanWrite)
                    {
                        list = (System.Collections.IList)Activator.CreateInstance(prop.PropertyType)!;
                        prop.SetValue(instance, list);
                    }
                    if (list == null) continue;

                    list.Clear();
                    foreach (var itemJson in jsonValue.EnumerateArray())
                    {
                        var item = DeserializeElement(itemJson, elementType!, options);
                        if (item != null) list.Add(item);
                    }
                    continue;
                }

                // Case 3: Non-CodeElement property with a setter
                if (prop.CanWrite)
                {
                    try
                    {
                        var propValue = JsonSerializer.Deserialize(jsonValue.GetRawText(), prop.PropertyType, innerOptions);
                        prop.SetValue(instance, propValue);
                    }
                    catch
                    {
                        // Skip properties that fail to deserialize (e.g., computed properties that
                        // sneaked through, or unsupported types)
                    }
                }
            }

            return instance;
        }

        private static Type ResolveConcreteType(JsonElement root, Type fallbackType)
        {
            string? assemblyTypeName = null;
            if (root.TryGetProperty("assemblyTypeFullyQualifiedName", out var prop))
                assemblyTypeName = prop.GetString();
            else if (root.TryGetProperty("AssemblyTypeFullyQualifiedName", out var prop2))
                assemblyTypeName = prop2.GetString();

            if (!string.IsNullOrEmpty(assemblyTypeName))
            {
                var resolved = Type.GetType(assemblyTypeName);
                if (resolved != null && typeof(CodeElement).IsAssignableFrom(resolved))
                    return resolved;
            }

            if (!fallbackType.IsAbstract)
                return fallbackType;

            throw new JsonException($"Cannot deserialize abstract type '{fallbackType.Name}': " +
                $"assemblyTypeFullyQualifiedName is missing or could not be resolved (value: '{assemblyTypeName}').");
        }

        #endregion

        #region Helpers

        private static bool IsCodeElementList(Type type, out Type? elementType)
        {
            elementType = null;
            if (!type.IsGenericType) return false;
            if (type.GetGenericTypeDefinition() != typeof(List<>)) return false;
            elementType = type.GetGenericArguments()[0];
            return typeof(CodeElement).IsAssignableFrom(elementType);
        }

        private static readonly object _optionsCacheLock = new();
        private static readonly Dictionary<JsonSerializerOptions, JsonSerializerOptions> _innerOptionsCache = new();

        /// <summary>
        /// Creates a copy of the options without CodeElementJsonConverter.
        /// Used only for serializing/deserializing non-CodeElement properties (strings, enums, TypeReference, etc.).
        /// </summary>
        private static JsonSerializerOptions GetInnerOptions(JsonSerializerOptions source)
        {
            lock (_optionsCacheLock)
            {
                if (_innerOptionsCache.TryGetValue(source, out var cached))
                    return cached;

                var newOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = source.PropertyNamingPolicy,
                    WriteIndented = source.WriteIndented,
                    DefaultIgnoreCondition = source.DefaultIgnoreCondition,
                    PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive,
                };
                foreach (var converter in source.Converters)
                {
                    if (converter is not CodeElementJsonConverter)
                        newOptions.Converters.Add(converter);
                }

                _innerOptionsCache[source] = newOptions;
                return newOptions;
            }
        }

        #endregion
    }

    /// <summary>
    /// JsonConverter for ProgrammingLanguage. Serializes as just the language Id string,
    /// and deserializes by looking up the singleton instance via ProgrammingLanguages.FindById().
    /// </summary>
    public class ProgrammingLanguageJsonConverter : JsonConverter<ProgrammingLanguage>
    {
        public override ProgrammingLanguage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var languageId = reader.GetString();
            if (string.IsNullOrEmpty(languageId))
                return null;

            return ProgrammingLanguages.ProgrammingLanguages.FindById(languageId)
                ?? throw new JsonException($"Programming language with ID '{languageId}' not found.");
        }

        public override void Write(Utf8JsonWriter writer, ProgrammingLanguage value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }
}
