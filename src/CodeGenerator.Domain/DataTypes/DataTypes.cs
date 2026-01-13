using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DataTypes
{
    public static class DataTypes
    {
        private static List<DataType> _dataTypes = new List<DataType>();
        public static IReadOnlyList<DataType> All => _dataTypes.AsReadOnly();

        static DataTypes()
        {
            // Initialize common data types
            _dataTypes.AddRange(new List<DataType>
            {
                new DataType
                {
                    Name = "SmallInt",
                    Description = $"Range {short.MinValue} to {short.MaxValue}",
                    Code = "smallint",
                    DotNetType = typeof(short),
                    UseMaxLength = false,
                    UsePrecision = false,
                    UseScale = false,
                    MinValue = short.MinValue,
                    MaxValue = short.MaxValue
                },
                new DataType
                {
                    Name = "String",
                    Description = "Variable-length string",
                    Code = "string",
                    DotNetType = typeof(string),
                    UseMaxLength = true,
                    UsePrecision = false,
                    UseScale = false
                },
                // Add more data types as needed
                new DataType {
                    Name = "Integer",
                    Description = $"Range {int.MinValue} to {int.MaxValue}",
                    Code = "int",
                    DotNetType = typeof(int),
                    UseMaxLength = false,
                    UsePrecision = false,
                    UseScale = false,
                    MinValue = int.MinValue,
                    MaxValue = int.MaxValue
                },
            });
        }
    }
}
