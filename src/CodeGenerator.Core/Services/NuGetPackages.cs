using CodeGenerator.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Services
{
    public static class NuGetPackages
    {
        public static NuGetPackageInfo Microsoft_EntityFrameworkCore { get; } = new NuGetPackageInfo { PackageId = "Microsoft.EntityFrameworkCore", Version = "8.0.11" };
        public static NuGetPackageInfo Microsoft_EntityFrameworkCore_Relational { get; } = new NuGetPackageInfo { PackageId = "Microsoft.EntityFrameworkCore.Relational", Version = "8.0.11" };
        public static NuGetPackageInfo Microsoft_Extensions_Configuration { get; } = new NuGetPackageInfo { PackageId = "Microsoft.Extensions.Configuration", Version = "8.0.0" };
        public static NuGetPackageInfo Microsoft_Extensions_Configuration_Json { get; } = new NuGetPackageInfo { PackageId = "Microsoft.Extensions.Configuration.Json", Version = "8.0.1" };
        public static NuGetPackageInfo Microsoft_Extensions_DependencyInjection { get; } = new NuGetPackageInfo { PackageId = "Microsoft.Extensions.DependencyInjection", Version = "8.0.1" };
        public static NuGetPackageInfo Microsoft_Extensions_Hosting { get; } = new NuGetPackageInfo { PackageId = "Microsoft.Extensions.Hosting", Version = "8.0.1" };
        public static NuGetPackageInfo Microsoft_Extensions_Logging { get; } = new NuGetPackageInfo { PackageId = "Microsoft.Extensions.Logging", Version = "8.0.1" };
        public static NuGetPackageInfo Microsoft_Extensions_Logging_Debug { get; } = new NuGetPackageInfo { PackageId = "Microsoft.Extensions.Logging.Debug", Version = "8.0.1" };
        public static NuGetPackageInfo Syncfusion_Core_WinForms { get; } = new NuGetPackageInfo { PackageId = "Syncfusion.Core.WinForms", Version = "32.1.20" };
        public static NuGetPackageInfo Syncfusion_SfDataGrid_WinForms { get; } = new NuGetPackageInfo { PackageId = "Syncfusion.SfDataGrid.WinForms", Version = "32.1.20" };
        public static NuGetPackageInfo Syncfusion_SfInput_WinForms { get; } = new NuGetPackageInfo { PackageId = "Syncfusion.SfInput.WinForms", Version = "32.1.20" };
        public static NuGetPackageInfo Syncfusion_SfListView_WinForms { get; } = new NuGetPackageInfo { PackageId = "Syncfusion.SfListView.WinForms", Version = "32.1.20" };
        public static NuGetPackageInfo Syncfusion_Shared_Base { get; } = new NuGetPackageInfo { PackageId = "Syncfusion.Shared.Base", Version = "32.1.20" };
        public static NuGetPackageInfo Syncfusion_Tools_Windows { get; } = new NuGetPackageInfo { PackageId = "Syncfusion.Tools.Windows", Version = "32.1.20" };
    }
}
