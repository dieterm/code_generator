using CodeGenerator.Domain.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.Databases.RelationalDatabases
{
    public class PostgreSqlDataTypeMappings : DataTypeMappings
    {
        private static readonly IEnumerable<DataTypeMapping> _dataTypeMappings;

        static PostgreSqlDataTypeMappings()
        {
            _dataTypeMappings = new List<DataTypeMapping>
            {
                // Integer types
                new(GenericDataTypes.TinyInt, "SMALLINT", "SMALLINT"),
                new(GenericDataTypes.SmallInt, "SMALLINT", "SMALLINT"),
                new(GenericDataTypes.Int, "INTEGER", "INTEGER", "INT", "INT4"),
                new(GenericDataTypes.BigInt, "BIGINT", "BIGINT", "INT8"),

                // Decimal types
                new(GenericDataTypes.Decimal, "NUMERIC", "NUMERIC({precision},{scale})", "DECIMAL")
                {
                    MaxPrecision = 131072,
                    MaxScale = 16383
                },
                new(GenericDataTypes.Float, "REAL", "REAL", "FLOAT4"),
                new(GenericDataTypes.Double, "DOUBLE PRECISION", "DOUBLE PRECISION", "FLOAT8"),
                new(GenericDataTypes.Money, "MONEY", "MONEY"),

                // String types
                new(GenericDataTypes.Char, "CHARACTER", "CHARACTER({maxlength})", "CHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = int.MaxValue
                },
                new(GenericDataTypes.VarChar, "CHARACTER VARYING", "CHARACTER VARYING({maxlength})", "VARCHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = int.MaxValue
                },
                new(GenericDataTypes.NChar, "CHARACTER", "CHARACTER({maxlength})", "CHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = int.MaxValue
                },
                new(GenericDataTypes.NVarChar, "CHARACTER VARYING", "CHARACTER VARYING({maxlength})", "VARCHAR")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = int.MaxValue
                },
                new(GenericDataTypes.Text, "TEXT", "TEXT"),
                new(GenericDataTypes.NText, "TEXT", "TEXT"),

                // Boolean types
                new(GenericDataTypes.Boolean, "BOOLEAN", "BOOLEAN", "BOOL"),
                new(GenericDataTypes.Bit, "BIT", "BIT({maxlength})", "BIT VARYING")
                {
                    MinMaxLength = 1,
                    MaxMaxLength = int.MaxValue
                },

                // Date/Time types
                new(GenericDataTypes.Date, "DATE", "DATE"),
                new(GenericDataTypes.Time, "TIME", "TIME", "TIME WITHOUT TIME ZONE"),
                new(GenericDataTypes.DateTime, "TIMESTAMP", "TIMESTAMP", "TIMESTAMP WITHOUT TIME ZONE"),
                new(GenericDataTypes.DateTime2, "TIMESTAMP", "TIMESTAMP(6)", "TIMESTAMP(6) WITHOUT TIME ZONE"),
                new(GenericDataTypes.Timestamp, "TIMESTAMP", "TIMESTAMP", "TIMESTAMP WITHOUT TIME ZONE")
                {
                    Notes = "Use TIMESTAMP WITH TIME ZONE for timezone-aware timestamps"
                },
                new(GenericDataTypes.DateTimeOffset, "TIMESTAMP WITH TIME ZONE", "TIMESTAMP WITH TIME ZONE", "TIMESTAMPTZ"),

                // Binary types
                new(GenericDataTypes.Binary, "BYTEA", "BYTEA"),
                new(GenericDataTypes.VarBinary, "BYTEA", "BYTEA"),
                new(GenericDataTypes.Blob, "BYTEA", "BYTEA"),

                // Other types
                new(GenericDataTypes.Guid, "UUID", "UUID"),
                new(GenericDataTypes.Xml, "XML", "XML"),
                new(GenericDataTypes.Json, "JSONB", "JSONB", "JSON")
                {
                    Notes = "Use JSONB for better performance (binary JSON); JSON is text-based"
                },
            };
        }
        public PostgreSqlDataTypeMappings()
            : base(_dataTypeMappings)
        {
        }
    }
}
