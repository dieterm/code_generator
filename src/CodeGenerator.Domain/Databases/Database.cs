using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Domain.Databases
{
    /// <summary>
    /// Abstract base class for all database definitions
    /// </summary>
    public abstract class Database
    {
        /// <summary>
        /// Unique identifier for this database type
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Display name of the database
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Database vendor/manufacturer
        /// </summary>
        public abstract string Vendor { get; }

        /// <summary>
        /// Available versions of this database
        /// </summary>
        public abstract IReadOnlyList<DatabaseVersion> Versions { get; }

        /// <summary>
        /// Default/recommended version
        /// </summary>
        public abstract DatabaseVersion DefaultVersion { get; }

        /// <summary>
        /// Data type mappings for this database
        /// </summary>
        //public abstract IReadOnlyList<DataTypeMapping> DataTypeMappings { get; }

        private Lazy<DataTypeMappings> _dataTypeMappingsWrapper;
        public DataTypeMappings DataTypeMappings => _dataTypeMappingsWrapper.Value;

        protected Database()
        {
            _dataTypeMappingsWrapper = new Lazy<DataTypeMappings>(() => CreateDataTypeMappings());
        }

        protected abstract DataTypeMappings CreateDataTypeMappings();
        /// <summary>
        /// Get the mapping for a generic data type
        /// </summary>
        public virtual DataTypeMapping? GetMapping(GenericDataType genericType)
        {
            return DataTypeMappings.FirstOrDefault(m => m.GenericType.Id == genericType.Id);
        }

        /// <summary>
        /// Get the mapping for a generic data type by id
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
        /// Generate a column type definition string
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

        /// <summary>
        /// Generate a column type definition from a native type name
        /// </summary>
        public virtual string GenerateTypeDef(
            string nativeTypeName,
            int? maxLength = null,
            int? precision = null,
            int? scale = null)
        {
            var mapping = FindMappingByNativeType(nativeTypeName);
            return mapping?.GenerateTypeDef(maxLength, precision, scale) ?? nativeTypeName;
        }
    }
}
