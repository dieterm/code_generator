using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.ViewModels
{
    public class CodeElementsEditorViewModel : ViewModelBase
    {
        public CodeElementsEditorViewModel() { }

        private string? _text;
        public string? Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages Syntax { get; set; } = Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages.CSharp;
    }
}
