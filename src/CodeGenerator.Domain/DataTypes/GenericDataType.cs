using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DataTypes
{
    /// <summary>
    /// Represents an abstract/generic data type that can be mapped to specific implementations
    /// in different databases and programming languages.
    /// </summary>
    public class GenericDataType
    {
        /// <summary>
        /// Unique identifier for this data type
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name (e.g., "Integer", "String", "Decimal")
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the data type
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of the data type
        /// </summary>
        public DataTypeCategory Category { get; }

        /// <summary>
        /// Indicates if this type supports a 'max length' parameter
        /// </summary>
        public bool SupportsMaxLength { get; set; }

        /// <summary>
        /// Indicates if this type supports an 'allowed values' parameter
        /// </summary>
        public bool SupportsAllowedValues { get; set; }

        /// <summary>
        /// Indicates if this type supports value type reference (e.g., enum types)
        /// </summary>
        public bool SupportsValueTypeReference { get; set; }

        /// <summary>
        /// Indicates if this type supports 'precision' parameter
        /// </summary>
        public bool SupportsPrecision { get; set; }

        /// <summary>
        /// Indicates if this type supports 'scale' parameter
        /// </summary>
        public bool SupportsScale { get; set; }

        /// <summary>
        /// Default max length (if applicable)
        /// </summary>
        public int? DefaultMaxLength { get; set; }

        /// <summary>
        /// Default precision (if applicable)
        /// </summary>
        public int? DefaultPrecision { get; set; }

        /// <summary>
        /// Default scale (if applicable)
        /// </summary>
        public int? DefaultScale { get; set; }

        public GenericDataType(string id, string name, DataTypeCategory category)
        {
            Id = id;
            Name = name;
            Category = category;
        }
    }
}
