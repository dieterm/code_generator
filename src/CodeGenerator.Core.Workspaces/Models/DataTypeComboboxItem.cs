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
        public bool UseMaxLength { get; set; }
        public bool UsePrecision { get; set; }
        public bool UseScale { get; set; }
    }
}
