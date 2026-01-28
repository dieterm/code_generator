using CodeGenerator.Domain.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    public class MysqlDatatypeMappings : DataTypeMappings
    {
        private readonly static List<DataTypeMapping> _dataTypeMappings;

        static MysqlDatatypeMappings()
        {
            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "TINYINT", "TINYINT", "TINYINT(1)", "TINYINT(2)", "TINYINT(4)"),
                new(GenericDataTypes.SmallInt, "SMALLINT", "SMALLINT", "SMALLINT(6)"),
                new(GenericDataTypes.Int, "INT", "INT", "INTEGER", "INT(11)"),
                new(GenericDataTypes.BigInt, "BIGINT", "BIGINT", "BIGINT(20)"),

                // Decimal types
                new(GenericDataTypes.Decimal, "DECIMAL", "DECIMAL({precision},{scale})", "NUMERIC")
                {
                    MaxPrecision = 65,
                    MaxScale = 30
                },
                new(GenericDataTypes.Float, "FLOAT", "FLOAT", "FLOAT(M,D)"),
                new(GenericDataTypes.Double, "DOUBLE", "DOUBLE", "REAL"),
                new(GenericDataTypes.Money, "DECIMAL", "DECIMAL(10,2)"),

                // String types
                new(GenericDataTypes.Char, "CHAR", "CHAR({maxlength})", "CHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 255
                },
                new(GenericDataTypes.VarChar, "VARCHAR", "VARCHAR({maxlength})", "VARCHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 65535,
                    UnlimitedLengthKeyword = "MAX"
                },
                new(GenericDataTypes.NChar, "NCHAR", "NCHAR({maxlength})", "NATIONAL CHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 255
                },
                new(GenericDataTypes.NVarChar, "NVARCHAR", "NVARCHAR({maxlength})", "NATIONAL VARCHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 65535,
                    UnlimitedLengthKeyword = "MAX"
                },
                new(GenericDataTypes.Text, "TEXT", "TEXT", "LONGTEXT", "MEDIUMTEXT")
                {
                    Notes = "TEXT supports max 65,535 characters"
                },
                new(GenericDataTypes.NText, "LONGTEXT", "LONGTEXT", "NTEXT")
                {
                    Notes = "LONGTEXT supports up to 4GB of data"
                },

                // Enum types
                new(GenericDataTypes.Enum, "ENUM", "ENUM({allowedvalues})", "ENUM")
                {
                    Notes = "MySQL native ENUM type. Values are specified as comma-separated quoted strings."
                },

                // Boolean types
                new(GenericDataTypes.Boolean, "TINYINT", "TINYINT(1)", "BOOL", "BOOLEAN"),
                new(GenericDataTypes.Bit, "BIT", "BIT(1)", "BIT"),

                // Date/Time types
                new(GenericDataTypes.Date, "DATE", "DATE"),
                new(GenericDataTypes.Time, "TIME", "TIME", "TIME(6)"),
                new(GenericDataTypes.DateTime, "DATETIME", "DATETIME", "DATETIME(6)"),
                new(GenericDataTypes.DateTime2, "DATETIME", "DATETIME(6)"),
                new(GenericDataTypes.Timestamp, "TIMESTAMP", "TIMESTAMP", "TIMESTAMP(6)"),
                new(GenericDataTypes.DateTimeOffset, "DATETIME", "DATETIME(6)")
                {
                    Notes = "MySQL does not have built-in timezone support; use DATETIME with application-level handling"
                },

                // Binary types
                new(GenericDataTypes.Binary, "BINARY", "BINARY({maxlength})", "BINARY")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 255
                },
                new(GenericDataTypes.VarBinary, "VARBINARY", "VARBINARY({maxlength})", "VARBINARY")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 65535
                },
                new(GenericDataTypes.Blob, "LONGBLOB", "LONGBLOB", "BLOB", "MEDIUMBLOB")
                {
                    Notes = "BLOB supports up to 65KB, MEDIUMBLOB up to 16MB, LONGBLOB up to 4GB"
                },

                // Other types
                new(GenericDataTypes.Guid, "CHAR", "CHAR(36)", "VARCHAR(36)")
                {
                    Notes = "MySQL/MariaDB does not have native UUID/GUID type; use CHAR(36) for UUID format"
                },
                new(GenericDataTypes.Xml, "LONGTEXT", "LONGTEXT", "TEXT"),
                new(GenericDataTypes.Json, "JSON", "JSON")
                {
                    Notes = "Native JSON support available in MySQL 5.7.8+, MariaDB 10.2.3+"
                },
            };
        }

        public MysqlDatatypeMappings()
            : base(_dataTypeMappings)
        {

            
        }
    }
}
