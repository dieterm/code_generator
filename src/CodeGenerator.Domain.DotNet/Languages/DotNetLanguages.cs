using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public static class DotNetLanguages
    {
        public static readonly DotNetLanguage CSharp = new DotNetLanguage("csharp", "CSharp", "C#", "csproj", "csharp", "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        public static readonly DotNetLanguage FSharp = new DotNetLanguage("fsharp", "FSharp", "F#", "fsproj", "fsharp", "F2A71F9B-5D33-465A-A702-920D77279786");
        public static readonly DotNetLanguage VisualBasic = new DotNetLanguage("visualbasic", "Visual Basic", "VB", "vbproj", "vb", "F184B08F-C81C-45F6-A57F-5ABD9991F28F");
        public static readonly DotNetLanguage Python = new DotNetLanguage("python", "Python", "Python", "pyproj", "python", "888888A0-9F3D-457C-B088-3A5042F75D52");
        public static readonly DotNetLanguage CPlusPlus = new DotNetLanguage("cplusplus", "C++", "C++", "cppproj", "cpp", "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942");
        public static readonly DotNetLanguage JSharp = new DotNetLanguage("jsharp", "JSharp", "J#", "jsharpproj", "jsharp", "E6FDF86B-0D7D-4E3B-8B8B-8B8B8B8B8B8B");

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
