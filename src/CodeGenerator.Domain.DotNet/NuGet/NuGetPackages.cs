using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public static class NuGetPackages
    {
        public static NuGetPackage Microsoft_EntityFrameworkCore { get; } = new NuGetPackage { PackageId = "Microsoft.EntityFrameworkCore", Version = "8.0.11" };
        public static NuGetPackage Microsoft_EntityFrameworkCore_Relational { get; } = new NuGetPackage { PackageId = "Microsoft.EntityFrameworkCore.Relational", Version = "8.0.11" };
        public static NuGetPackage Microsoft_Extensions_Configuration { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.Configuration", Version = "8.0.0" };
        public static NuGetPackage Microsoft_Extensions_Configuration_Json { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.Configuration.Json", Version = "8.0.1" };
        public static NuGetPackage Microsoft_Extensions_DependencyInjection { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.DependencyInjection", Version = "8.0.1" };
        public static NuGetPackage Microsoft_Extensions_DependencyInjection_Abstractions { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.DependencyInjection.Abstractions", Version = "8.0.2" };
        public static NuGetPackage Microsoft_Extensions_Hosting { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.Hosting", Version = "8.0.1" };
        public static NuGetPackage Microsoft_Extensions_Logging { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.Logging", Version = "8.0.1" };
        public static NuGetPackage Microsoft_Extensions_Logging_Debug { get; } = new NuGetPackage { PackageId = "Microsoft.Extensions.Logging.Debug", Version = "8.0.1" };
       
        public const string Syncfusion_Version = "32.1.20";

        public static NuGetPackage Syncfusion_Core_WinForms { get; } = new NuGetPackage { PackageId = "Syncfusion.Core.WinForms", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_SfDataGrid_WinForms { get; } = new NuGetPackage { PackageId = "Syncfusion.SfDataGrid.WinForms", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_SfInput_WinForms { get; } = new NuGetPackage { PackageId = "Syncfusion.SfInput.WinForms", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_SfListView_WinForms { get; } = new NuGetPackage { PackageId = "Syncfusion.SfListView.WinForms", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_Shared_Base { get; } = new NuGetPackage { PackageId = "Syncfusion.Shared.Base", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_Tools_Windows { get; } = new NuGetPackage { PackageId = "Syncfusion.Tools.Windows", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_Edit_Windows { get; } = new NuGetPackage { PackageId = "Syncfusion.Edit.Windows", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_Grouping_Base { get; } = new NuGetPackage { PackageId = "Syncfusion.Grouping.Base", Version = Syncfusion_Version };
        public static NuGetPackage Syncfusion_Office2019Theme_WinForms { get; } = new NuGetPackage { PackageId = "Syncfusion.Office2019Theme.WinForms", Version = Syncfusion_Version };
        
    }
}
