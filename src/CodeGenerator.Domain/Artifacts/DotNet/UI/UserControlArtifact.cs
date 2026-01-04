namespace CodeGenerator.Domain.Artifacts.DotNet.UI;

using CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Decorator that marks an artifact as a WinForms UserControl
/// </summary>
public class UserControlDecorator : PreviewableDecorator
{
    /// <summary>
    /// Class name of the UserControl
    /// </summary>
    public string ClassName
    {
        get => Artifact?.GetProperty<string>("ClassName") ?? Artifact?.Name ?? string.Empty;
        set => Artifact?.SetProperty("ClassName", value);
    }

    /// <summary>
    /// Namespace of the UserControl
    /// </summary>
    public string Namespace
    {
        get => Artifact?.GetProperty<string>("Namespace") ?? string.Empty;
        set => Artifact?.SetProperty("Namespace", value);
    }

    /// <summary>
    /// The main code file (.cs)
    /// </summary>
    public Artifact? CodeFile
    {
        get => Artifact?.GetProperty<Artifact>("CodeFile");
        set => Artifact?.SetProperty("CodeFile", value);
    }

    /// <summary>
    /// The designer file (.Designer.cs)
    /// </summary>
    public Artifact? DesignerFile
    {
        get => Artifact?.GetProperty<Artifact>("DesignerFile");
        set => Artifact?.SetProperty("DesignerFile", value);
    }

    /// <summary>
    /// The resource file (.resx)
    /// </summary>
    public Artifact? ResourceFile
    {
        get => Artifact?.GetProperty<Artifact>("ResourceFile");
        set => Artifact?.SetProperty("ResourceFile", value);
    }

    /// <summary>
    /// Base class for the UserControl
    /// </summary>
    public string BaseClass
    {
        get => Artifact?.GetProperty<string>("BaseClass") ?? "UserControl";
        set => Artifact?.SetProperty("BaseClass", value);
    }

    /// <summary>
    /// Full type name including namespace
    /// </summary>
    public string FullTypeName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

    public override object? CreatePreview()
    {
        if (Artifact == null) return null;
        return new UserControlPreviewRequest(Artifact);
    }
}
