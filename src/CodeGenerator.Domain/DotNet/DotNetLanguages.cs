using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public static class DotNetLanguages
    {
        public static readonly DotNetLanguage CSharp = new DotNetLanguage("C#", "csproj", "csharp");
        public static readonly DotNetLanguage FSharp = new DotNetLanguage("F#", "fsproj", "fsharp");
        public static readonly DotNetLanguage VisualBasic = new DotNetLanguage("VB", "vbproj", "vb");
        public static readonly DotNetLanguage Python = new DotNetLanguage("Python", "pyproj", "python");

        public static readonly List<DotNetLanguage> AllLanguages = new List<DotNetLanguage>
        {
            CSharp,
            FSharp,
            VisualBasic,
            Python
        };

        public static DotNetLanguage GetByCommandLineArgument(string language)
        {
            if(language==null) throw new ArgumentNullException(nameof(language));
            return AllLanguages.Single(l => l.DotNetCommandLineArgument.Equals(language, StringComparison.OrdinalIgnoreCase));
        }
    }
}
