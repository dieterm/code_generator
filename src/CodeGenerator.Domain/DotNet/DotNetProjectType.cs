using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public class DotNetProjectType
    {
        public const string ClassLib = "classlib";
        public const string WinFormsLib = "winforms";
        public const string WpfLib = "wpf";
        public const string ConsoleApp = "console";
        public const string BlazorWebApp = "blazor";
        public const string MsTestProject = "mstest";
    }
}
