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
        public static readonly TargetFramework Net10 = new TargetFramework("net10_0", "net10.0", ".NET 10");
        public static readonly TargetFramework Net10WindowsOnly = new TargetFramework("net10_0_windows", "net10.0-windows", ".NET 10 (Windows only)");
        public static readonly TargetFramework Net9 = new TargetFramework("net9_0", "net9.0", ".NET 9");
        public static readonly TargetFramework Net9WindowsOnly = new TargetFramework("net9_0_windows", "net9.0-windows", ".NET 9 (Windows only)");
        public static readonly TargetFramework Net8 = new TargetFramework("net8_0", "net8.0", ".NET 8");
        public static readonly TargetFramework Net8WindowsOnly = new TargetFramework("net8_0_windows", "net8.0-windows", ".NET 8 (Windows only)");
        public static readonly TargetFramework Net7 = new TargetFramework("net7_0", "net7.0", ".NET 7");
        public static readonly TargetFramework Net7WindowsOnly = new TargetFramework("net7_0_windows", "net7.0-windows", ".NET 7 (Windows only)");
        public static readonly TargetFramework Net6 = new TargetFramework("net6_0", "net6.0", ".NET 6");
        public static readonly TargetFramework Net6WindowsOnly = new TargetFramework("net6_0_windows", "net6.0-windows", ".NET 6 (Windows only)");
        public static readonly TargetFramework Net5 = new TargetFramework("net5_0", "net5.0", "Target net5.0");
        // .NET Standard
        public static readonly TargetFramework NetStandard21 = new TargetFramework("netstandard2_1", "netstandard2.1", ".NET Standard 2.1");
        public static readonly TargetFramework NetStandard20 = new TargetFramework("netstandard2_0", "netstandard2.0", ".NET Standard 2.0");
        // NET Core
        public static readonly TargetFramework NetCore30 = new TargetFramework("netcoreapp3_0", "netcoreapp3.0", ".NET Core 3.0");
        public static readonly TargetFramework NetCore31 = new TargetFramework("netcoreapp3_1", "netcoreapp3.1", ".NET Core 3.1");
        // .NET Framework
        public static readonly TargetFramework Net48 = new TargetFramework("net48", "net48", ".NET Framework 4.8");
        public static readonly TargetFramework Net481 = new TargetFramework("net481", "net481", ".NET Framework 4.8.1");
        public static readonly TargetFramework Net47 = new TargetFramework("net47", "net47", ".NET Framework 4.7");
        public static readonly TargetFramework Net471 = new TargetFramework("net471", "net471", ".NET Framework 4.7.1");
        public static readonly TargetFramework Net472 = new TargetFramework("net472", "net472", ".NET Framework 4.7.2");
        public static readonly TargetFramework Net462 = new TargetFramework("net462", "net462", ".NET Framework 4.6.2");

        public static TargetFramework[] AllFrameworks = new TargetFramework[]
        {
            Net10,
            Net10WindowsOnly,
            Net9,
            Net9WindowsOnly,
            Net8,
            Net8WindowsOnly,
            Net7,
            Net7WindowsOnly,
            Net6,
            Net6WindowsOnly,
            Net5,
            NetStandard21,
            NetStandard20,
            NetCore31,
            Net48,
            Net481,
            Net47,
            Net471,
            Net472,
            Net462
        };
    }
}
