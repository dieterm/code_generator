namespace CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// .NET project types
/// </summary>
public enum DotNetProjectType
{
    /// <summary>
    /// Console application
    /// </summary>
    ConsoleApp,

    /// <summary>
    /// Class library
    /// </summary>
    ClassLibrary,

    /// <summary>
    /// ASP.NET Core Web API
    /// </summary>
    WebApi,

    /// <summary>
    /// Blazor application
    /// </summary>
    BlazorApp,

    /// <summary>
    /// Blazor WebAssembly application
    /// </summary>
    BlazorWasm,

    /// <summary>
    /// Windows Forms application
    /// </summary>
    WinForms,

    /// <summary>
    /// Windows Forms control library
    /// </summary>
    WinFormsControlLibrary,

    /// <summary>
    /// WPF application
    /// </summary>
    Wpf,

    /// <summary>
    /// WPF control library
    /// </summary>
    WpfControlLibrary,

    /// <summary>
    /// WinUI application
    /// </summary>
    WinUI,

    /// <summary>
    /// MAUI application
    /// </summary>
    Maui,

    /// <summary>
    /// Unit test project
    /// </summary>
    UnitTest,

    /// <summary>
    /// Worker service
    /// </summary>
    WorkerService
}
