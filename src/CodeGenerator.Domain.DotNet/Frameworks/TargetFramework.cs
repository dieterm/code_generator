using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public record TargetFramework(string Id, string DotNetCommandLineArgument, string Name);
    
}
