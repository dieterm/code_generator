using CodeGenerator.Domain.Databases.RelationalDatabases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DataTypes
{
    public class DataTypeMappings : IEnumerable<DataTypeMapping>
    {
        private readonly IEnumerable<DataTypeMapping> _dataTypeMappings;
        public DataTypeMappings(IEnumerable<DataTypeMapping> dataTypeMappings   ) {
            _dataTypeMappings = dataTypeMappings;
        }

        public DataTypeMapping? GetMappingByGenericDataType(string genericDataTypeId)
        {
            return _dataTypeMappings.FirstOrDefault(m => m.GenericType.Id == genericDataTypeId);
        }

        public DataTypeMapping? GetMappingByGenericDataType(GenericDataType genericDataType)
        {
            return _dataTypeMappings.FirstOrDefault(m => m.GenericType.Id == genericDataType.Id);
        }

        public IEnumerator<DataTypeMapping> GetEnumerator()
        {
            return _dataTypeMappings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dataTypeMappings).GetEnumerator();
        }

        public IEnumerable<DataTypeMapping> AllMappings { get { return _dataTypeMappings; } }

        // Integer types
        public DataTypeMapping TinyInt => GetMappingByGenericDataType(GenericDataTypes.TinyInt)!;
        public DataTypeMapping SmallInt => GetMappingByGenericDataType(GenericDataTypes.SmallInt)!;
        public DataTypeMapping Int => GetMappingByGenericDataType(GenericDataTypes.Int)!;
        public DataTypeMapping BigInt => GetMappingByGenericDataType(GenericDataTypes.BigInt)!;

        // Decimal types
        public DataTypeMapping Decimal => GetMappingByGenericDataType(GenericDataTypes.Decimal)!;
        public DataTypeMapping Float => GetMappingByGenericDataType(GenericDataTypes.Float)!;
        public DataTypeMapping Double => GetMappingByGenericDataType(GenericDataTypes.Double)!;
        public DataTypeMapping Money => GetMappingByGenericDataType(GenericDataTypes.Money)!;

        // String types
        public DataTypeMapping Char => GetMappingByGenericDataType(GenericDataTypes.Char)!;
        public DataTypeMapping VarChar => GetMappingByGenericDataType(GenericDataTypes.VarChar)!;
        public DataTypeMapping Text => GetMappingByGenericDataType(GenericDataTypes.Text)!;
        public DataTypeMapping NChar => GetMappingByGenericDataType(GenericDataTypes.NChar)!;
        public DataTypeMapping NVarChar => GetMappingByGenericDataType(GenericDataTypes.NVarChar)!;
        public DataTypeMapping NText => GetMappingByGenericDataType(GenericDataTypes.NText)!;
        // Boolean
        public DataTypeMapping Boolean => GetMappingByGenericDataType(GenericDataTypes.Boolean)!;
        public DataTypeMapping Bit => GetMappingByGenericDataType(GenericDataTypes.Bit)!;

        // Date/Time types
        public DataTypeMapping Date => GetMappingByGenericDataType(GenericDataTypes.Date)!;
        public DataTypeMapping Time => GetMappingByGenericDataType(GenericDataTypes.Time)!;
        public DataTypeMapping DateTime => GetMappingByGenericDataType(GenericDataTypes.DateTime)!;
        public DataTypeMapping DateTime2 => GetMappingByGenericDataType(GenericDataTypes.DateTime2)!;
        public DataTypeMapping Timestamp => GetMappingByGenericDataType(GenericDataTypes.Timestamp)!;
        public DataTypeMapping DateTimeOffset => GetMappingByGenericDataType(GenericDataTypes.DateTimeOffset)!;

        // Binary types
        public DataTypeMapping Binary => GetMappingByGenericDataType(GenericDataTypes.Binary)!;
        public DataTypeMapping VarBinary => GetMappingByGenericDataType(GenericDataTypes.VarBinary)!;
        public DataTypeMapping Blob => GetMappingByGenericDataType(GenericDataTypes.Blob)!;

        // Other types
        public DataTypeMapping Guid => GetMappingByGenericDataType(GenericDataTypes.Guid)!;
        public DataTypeMapping Xml => GetMappingByGenericDataType(GenericDataTypes.Xml)!;
        public DataTypeMapping Json => GetMappingByGenericDataType(GenericDataTypes.Json)!;

    }
}
