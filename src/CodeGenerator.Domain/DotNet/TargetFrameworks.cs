using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public static class TargetFrameworks
    {
        // Modern .NET
        public const string Net8 = "net8.0";
        public const string Net7 = "net7.0";
        public const string Net6 = "net6.0";

        // .NET Standard
        public const string NetStandard21 = "netstandard2.1";
        public const string NetStandard20 = "netstandard2.0";

        // .NET Framework
        public const string Net48 = "net48";
        public const string Net472 = "net472";

        public static string[] AllFrameworks = new string[]
        {
            Net8,
            Net7,
            Net6,
            NetStandard21,
            NetStandard20,
            Net48,
            Net472
        };
    }
}
