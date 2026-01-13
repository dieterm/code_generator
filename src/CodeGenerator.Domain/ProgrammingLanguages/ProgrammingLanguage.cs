using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Domain.NamingConventions;

namespace CodeGenerator.Domain.ProgrammingLanguages
{
    /// <summary>
    /// Represents a programming language with its characteristics and type mappings
    /// </summary>
    public abstract class ProgrammingLanguage
    {
        /// <summary>
        /// Unique identifier for this language
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Display name of the language
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// File extension for source files (e.g., ".cs", ".java")
        /// </summary>
        public abstract string FileExtension { get; }

        /// <summary>
        /// Available versions of this language
        /// </summary>
        public abstract IReadOnlyList<ProgrammingLanguageVersion> Versions { get; }

        /// <summary>
        /// Default/recommended version
        /// </summary>
        public abstract ProgrammingLanguageVersion DefaultVersion { get; }

        /// <summary>
        /// Naming conventions for this language
        /// </summary>
        public abstract NamingConventions.NamingConventions NamingConventions { get; }

        /// <summary>
        /// Data type mappings for this language
        /// </summary>
        public abstract IReadOnlyList<DataTypeMapping> DataTypeMappings { get; }

        /// <summary>
        /// Get the native type for a generic data type
        /// </summary>
        public virtual DataTypeMapping? GetMapping(GenericDataType genericType)
        {
            return DataTypeMappings.FirstOrDefault(m => m.GenericType.Id == genericType.Id);
        }

        /// <summary>
        /// Get the native type for a generic data type by id
        /// </summary>
        public virtual DataTypeMapping? GetMapping(string genericTypeId)
        {
            return DataTypeMappings.FirstOrDefault(m => 
                m.GenericType.Id.Equals(genericTypeId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Find a mapping that matches a native type name
        /// </summary>
        public virtual DataTypeMapping? FindMappingByNativeType(string nativeTypeName)
        {
            return DataTypeMappings.FirstOrDefault(m => m.Matches(nativeTypeName));
        }

        /// <summary>
        /// Generate a type definition string
        /// </summary>
        public virtual string GenerateTypeDef(
            GenericDataType genericType,
            int? maxLength = null,
            int? precision = null,
            int? scale = null)
        {
            var mapping = GetMapping(genericType);
            return mapping?.GenerateTypeDef(maxLength, precision, scale) ?? genericType.Name;
        }
    }
}
