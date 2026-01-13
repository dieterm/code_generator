using CodeGenerator.Domain.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    public class SqlServerDataTypeMappings : DataTypeMappings
    {
        private readonly static List<DataTypeMapping> _dataTypeMappings;

        static SqlServerDataTypeMappings()
        {
            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "TINYINT", "TINYINT"),
                new(GenericDataTypes.SmallInt, "SMALLINT", "SMALLINT"),
                new(GenericDataTypes.Int, "INT", "INT", "INTEGER"),
                new(GenericDataTypes.BigInt, "BIGINT", "BIGINT"),

                // Decimal types
                new(GenericDataTypes.Decimal, "DECIMAL", "DECIMAL({precision},{scale})", "NUMERIC")
                {
                    MaxPrecision = 38,
                    MaxScale = 38
                },
                new(GenericDataTypes.Float, "FLOAT", "FLOAT", "FLOAT(53)"),
                new(GenericDataTypes.Double, "REAL", "REAL"),
                new(GenericDataTypes.Money, "MONEY", "MONEY", "SMALLMONEY"),

                // String types
                new(GenericDataTypes.Char, "CHAR", "CHAR({maxlength})")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 8000
                },
                new(GenericDataTypes.VarChar, "VARCHAR", "VARCHAR({maxlength})", "VARCHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 8000,
                    UnlimitedLengthKeyword = "MAX"
                },
                new(GenericDataTypes.NChar, "NCHAR", "NCHAR({maxlength})")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 4000
                },
                new(GenericDataTypes.NVarChar, "NVARCHAR", "NVARCHAR({maxlength})", "NVARCHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 4000,
                    UnlimitedLengthKeyword = "MAX"
                },
                new(GenericDataTypes.Text, "VARCHAR", "VARCHAR(MAX)", "TEXT")
                {
                    Notes = "TEXT is deprecated; use VARCHAR(MAX) instead"
                },
                new(GenericDataTypes.NText, "NVARCHAR", "NVARCHAR(MAX)", "NTEXT")
                {
                    Notes = "NTEXT is deprecated; use NVARCHAR(MAX) instead"
                },

                // Boolean types
                new(GenericDataTypes.Boolean, "BIT", "BIT"),
                new(GenericDataTypes.Bit, "BIT", "BIT"),

                // Date/Time types
                new(GenericDataTypes.Date, "DATE", "DATE"),
                new(GenericDataTypes.Time, "TIME", "TIME"),
                new(GenericDataTypes.DateTime, "DATETIME", "DATETIME", "DATETIME2"),
                new(GenericDataTypes.DateTime2, "DATETIME2", "DATETIME2", "DATETIME2(7)"),
                new(GenericDataTypes.Timestamp, "ROWVERSION", "ROWVERSION", "TIMESTAMP")
                {
                    Notes = "TIMESTAMP is deprecated; use ROWVERSION instead"
                },
                new(GenericDataTypes.DateTimeOffset, "DATETIMEOFFSET", "DATETIMEOFFSET", "DATETIMEOFFSET(7)"),

                // Binary types
                new(GenericDataTypes.Binary, "BINARY", "BINARY({maxlength})")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 8000
                },
                new(GenericDataTypes.VarBinary, "VARBINARY", "VARBINARY({maxlength})", "VARBINARY")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = 8000,
                    UnlimitedLengthKeyword = "MAX"
                },
                new(GenericDataTypes.Blob, "VARBINARY", "VARBINARY(MAX)", "IMAGE")
                {
                    Notes = "IMAGE is deprecated; use VARBINARY(MAX) instead"
                },

                // Other types
                new(GenericDataTypes.Guid, "UNIQUEIDENTIFIER", "UNIQUEIDENTIFIER"),
                new(GenericDataTypes.Xml, "XML", "XML"),
                new(GenericDataTypes.Json, "NVARCHAR", "NVARCHAR(MAX)")
                {
                    Notes = "JSON is stored as NVARCHAR(MAX) with JSON constraints; native JSON support available in SQL Server 2016+"
                },
            };
        }

        public SqlServerDataTypeMappings() 
            : base(_dataTypeMappings)
        {
        }
    }
}
