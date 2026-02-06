using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public static class DotNetProjectTypes
    {
        public static ClassLibProjectType ClassLib { get; } = new ClassLibProjectType();
        public static WinFormsLibProjectType WinFormsLib { get; } = new WinFormsLibProjectType();
        public static WinFormsExeProjectType WinFormsExe { get; } = new WinFormsExeProjectType();
        public static WpfLibProjectType WpfLib { get; } = new WpfLibProjectType();
        public static ConsoleAppProjectType ConsoleApp { get; } = new ConsoleAppProjectType();
        public static BlazorWebAppProjectType BlazorWebApp { get; } = new BlazorWebAppProjectType();
        public static MsTestProjectType MsTestProject { get; } = new MsTestProjectType();

        public static DotNetProjectType[] AllTypes { get; } = new DotNetProjectType[]
        {
            ClassLib,
            WinFormsLib,
            WinFormsExe,
            WpfLib,
            ConsoleApp,
            BlazorWebApp,
            MsTestProject
        };

        public static DotNetProjectType GetById(string v)
        {
            return AllTypes.First(t => t.Id == v);
        }
    }
}
