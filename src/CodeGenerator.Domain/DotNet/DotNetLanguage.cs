using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public record DotNetLanguage(string DotNetCommandLineArgument, string ProjectFileExtension, string ImageKey);

}
