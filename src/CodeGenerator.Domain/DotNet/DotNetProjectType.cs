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

        public static string[] AllTypes = new[]
        {
            ClassLib,
            WinFormsLib,
            WpfLib,
            ConsoleApp,
            BlazorWebApp,
            MsTestProject
        };

        public static Dictionary<string, DotNetLanguage[]> ProjectTypeSupportedLanguages = new Dictionary<string, DotNetLanguage[]>();

        public static Dictionary<string, TargetFramework[]> ProjectTypeSupportedFrameworks = new Dictionary<string, TargetFramework[]>();

        static DotNetProjectType()
        {
            // classlib Allowed values for '-lang' option are: 'C#', 'F#', 'VB'.
            ProjectTypeSupportedLanguages[ClassLib] = new[] {
                DotNetLanguages.CSharp,
                DotNetLanguages.FSharp,
                DotNetLanguages.VisualBasic
            };
            ProjectTypeSupportedLanguages[WinFormsLib] = new[] {
                DotNetLanguages.CSharp,
                DotNetLanguages.VisualBasic
            };
            ProjectTypeSupportedLanguages[WpfLib] = new[] {
                DotNetLanguages.CSharp,
                DotNetLanguages.VisualBasic
            };
            ProjectTypeSupportedLanguages[ConsoleApp] = new[] {
                DotNetLanguages.CSharp,
                DotNetLanguages.FSharp,
                DotNetLanguages.VisualBasic
            };
            ProjectTypeSupportedLanguages[BlazorWebApp] = new[] {
                DotNetLanguages.CSharp
            };
            ProjectTypeSupportedLanguages[MsTestProject] = new[] {
                DotNetLanguages.CSharp,
                DotNetLanguages.FSharp,
                DotNetLanguages.VisualBasic
            };

            ProjectTypeSupportedFrameworks[ClassLib] = new[] {
                TargetFrameworks.Net10,
                TargetFrameworks.Net9,
                TargetFrameworks.Net8,
                TargetFrameworks.Net7,
                TargetFrameworks.Net6,
                TargetFrameworks.Net5,
                TargetFrameworks.NetStandard21,
                TargetFrameworks.NetStandard20,
                TargetFrameworks.NetCore31,
            };
            ProjectTypeSupportedFrameworks[WinFormsLib] = new[] {
                TargetFrameworks.Net10,
                TargetFrameworks.Net9,
                TargetFrameworks.Net8,
                TargetFrameworks.Net7,
                TargetFrameworks.Net6,
                TargetFrameworks.Net5,
                TargetFrameworks.NetCore31,
            };
            ProjectTypeSupportedFrameworks[WpfLib] = new[] {
                TargetFrameworks.Net10,
                TargetFrameworks.Net9,
                TargetFrameworks.Net8,
                TargetFrameworks.Net7,
                TargetFrameworks.Net6,
                TargetFrameworks.Net5,
                TargetFrameworks.NetCore30,
                TargetFrameworks.NetCore31,
            };
            ProjectTypeSupportedFrameworks[ConsoleApp] = new[] {
                TargetFrameworks.Net10,
                TargetFrameworks.Net9,
                TargetFrameworks.Net8,
                TargetFrameworks.Net7,
                TargetFrameworks.Net6,
                TargetFrameworks.Net5,
                TargetFrameworks.NetCore31,
            };
            ProjectTypeSupportedFrameworks[BlazorWebApp] = new[] {
                TargetFrameworks.Net10,
                TargetFrameworks.Net9,
                TargetFrameworks.Net8,
            };
            ProjectTypeSupportedFrameworks[MsTestProject] = new[] {
                TargetFrameworks.Net10,
                TargetFrameworks.Net10WindowsOnly,
                TargetFrameworks.Net9,
                TargetFrameworks.Net9WindowsOnly,
                TargetFrameworks.Net8,
                TargetFrameworks.Net8WindowsOnly,
                TargetFrameworks.Net7,
                TargetFrameworks.Net7WindowsOnly,
                TargetFrameworks.Net6,
                TargetFrameworks.Net6WindowsOnly,
                TargetFrameworks.Net5,
                TargetFrameworks.NetCore31,
                TargetFrameworks.Net48,
                TargetFrameworks.Net481,
                TargetFrameworks.Net47,
                TargetFrameworks.Net471,
                TargetFrameworks.Net472,
                TargetFrameworks.Net462
            };
        }
    }
}
