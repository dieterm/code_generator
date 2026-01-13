using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DataTypes
{
    public class DataType
    {
        /// <summary>
        /// eg. "SmallInt", "String", etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// eg. "Range -32768 to 32767"
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// eg. "smallint"
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// eg. "System.Int32", "System.String", etc.
        /// </summary>
        //public Type DotNetType { get; set; }
        /// <summary>
        /// Indicate if the UI should show max length input
        /// eg. true if the data type supports max length (string)
        /// </summary>
        public bool UseMaxLength { get; set; }
        /// <summary>
        /// Indiciate if the UI should show precision input
        /// eg. true if the data type supports precision (decimal)
        /// </summary>
        public bool UsePrecision { get; set; }
        /// <summary>
        /// Indicate if the UI should show scale input
        /// eg. true if the data type supports scale (decimal)
        /// </summary>
        public bool UseScale { get; set; }
        /// <summary>
        /// minimum value for the data type
        /// eg. -32768 for SmallInt
        /// </summary>
        public object? MinValue { get; set; }
        /// <summary>
        /// maximum value for the data type
        /// eg. 32767 for SmallInt
        /// </summary>
        public object? MaxValue { get; set; }
    }
}
