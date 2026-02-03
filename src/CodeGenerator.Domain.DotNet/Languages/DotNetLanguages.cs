using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public static class DotNetLanguages
    {
        public static readonly DotNetLanguage CSharp = new DotNetLanguage("csharp", "CSharp", "C#", "csproj", "csharp");
        public static readonly DotNetLanguage FSharp = new DotNetLanguage("fsharp", "FSharp", "F#", "fsproj", "fsharp");
        public static readonly DotNetLanguage VisualBasic = new DotNetLanguage("visualbasic", "Visual Basic", "VB", "vbproj", "vb");
        public static readonly DotNetLanguage Python = new DotNetLanguage("python", "Python", "Python", "pyproj", "python");

        public static readonly List<DotNetLanguage> AllLanguages = new List<DotNetLanguage>
        {
            CSharp,
            FSharp,
            VisualBasic,
            Python
        };

        public static DotNetLanguage GetByCommandLineArgument(string? language)
        {
            if(language==null) throw new ArgumentNullException(nameof(language));
            return AllLanguages.Single(l => l.DotNetCommandLineArgument.Equals(language, StringComparison.OrdinalIgnoreCase));
        }
    }
}
