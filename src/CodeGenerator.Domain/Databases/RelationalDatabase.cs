using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Domain.DataTypes;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    /// <summary>
    /// Abstract base class for relational database definitions
    /// </summary>
    public abstract class RelationalDatabase : Database
    {
        /// <summary>
        /// Default schema name for this database
        /// </summary>
        public virtual string DefaultSchema => "dbo";

        /// <summary>
        /// Supports schemas/databases for organization
        /// </summary>
        public virtual bool SupportsSchemas => true;

        /// <summary>
        /// Default character set for string types
        /// </summary>
        public virtual string? DefaultCharacterSet => null;

        /// <summary>
        /// Default collation for string types
        /// </summary>
        public virtual string? DefaultCollation => null;

        /// <summary>
        /// Supports CHECK constraints
        /// </summary>
        public virtual bool SupportsCheckConstraints => true;

        /// <summary>
        /// Supports foreign key constraints
        /// </summary>
        public virtual bool SupportsForeignKeyConstraints => true;

        /// <summary>
        /// Supports unique constraints
        /// </summary>
        public virtual bool SupportsUniqueConstraints => true;

        /// <summary>
        /// Supports computed/generated columns
        /// </summary>
        public virtual bool SupportsGeneratedColumns => false;

        /// <summary>
        /// Supports common table expressions (CTEs / WITH clause)
        /// </summary>
        public virtual bool SupportsCTE => true;

        /// <summary>
        /// Supports window functions
        /// </summary>
        public virtual bool SupportsWindowFunctions => false;

        /// <summary>
        /// Supports JSON data type
        /// </summary>
        public virtual bool SupportsJsonDataType => false;

        /// <summary>
        /// Supports XML data type
        /// </summary>
        public virtual bool SupportsXmlDataType => false;

        /// <summary>
        /// Maximum column name length
        /// </summary>
        public virtual int MaxColumnNameLength => 255;

        /// <summary>
        /// Maximum table name length
        /// </summary>
        public virtual int MaxTableNameLength => 255;

        /// <summary>
        /// Maximum identifier length
        /// </summary>
        public virtual int MaxIdentifierLength => 255;

        /// <summary>
        /// Generate a CREATE TABLE statement
        /// </summary>
        public virtual string GenerateCreateTableStatement(
            string tableName,
            string schema,
            IEnumerable<(string name, string type, bool isNullable, bool isPrimaryKey)> columns,
            string? charset = null,
            string? collation = null)
        {
            throw new NotImplementedException($"GenerateCreateTableStatement is not implemented for {GetType().Name}");
        }

        /// <summary>
        /// Generate a CREATE INDEX statement
        /// </summary>
        public virtual string GenerateCreateIndexStatement(
            string tableName,
            string indexName,
            IEnumerable<string> columnNames,
            bool isUnique = false,
            string? schema = null)
        {
            throw new NotImplementedException($"GenerateCreateIndexStatement is not implemented for {GetType().Name}");
        }
    }
}
