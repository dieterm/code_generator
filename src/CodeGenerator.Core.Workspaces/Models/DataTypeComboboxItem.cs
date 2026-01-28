using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Models
{
    public class DataTypeComboboxItem : ComboboxItem
    {
        /// <summary>
        /// extra info for generic datatype, shown to the user under the combobox
        /// eg. for int -> show the minimum and maximum value
        /// </summary>
        public string? TypeDescription { get; set; }
        public bool UseMaxLength { get; set; }
        public bool UsePrecision { get; set; }
        public bool UseScale { get; set; }
        public bool UseAllowedValues { get; set; }
        /// <summary>
        /// warnings or notes about this data type
        /// </summary>
        public string? TypeNotes { get; set; }

        public override string ToString()
        {
            return $"{DisplayName} ({Value})";
        }
    }
}
