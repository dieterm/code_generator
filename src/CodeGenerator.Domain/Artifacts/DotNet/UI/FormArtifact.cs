namespace CodeGenerator.Domain.Artifacts.DotNet.UI;

/// <summary>
/// Decorator that marks an artifact as a WinForms Form
/// </summary>
public class FormDecorator : UserControlDecorator
{
    /// <summary>
    /// Title of the form
    /// </summary>
    public string FormTitle
    {
        get => Artifact?.GetProperty<string>("FormTitle") ?? string.Empty;
        set => Artifact?.SetProperty("FormTitle", value);
    }

    /// <summary>
    /// Whether this is the main/startup form
    /// </summary>
    public bool IsMainForm
    {
        get => Artifact?.GetProperty<bool>("IsMainForm") ?? false;
        set => Artifact?.SetProperty("IsMainForm", value);
    }

    /// <summary>
    /// Form start position
    /// </summary>
    public string StartPosition
    {
        get => Artifact?.GetProperty<string>("StartPosition") ?? "CenterScreen";
        set => Artifact?.SetProperty("StartPosition", value);
    }

    /// <summary>
    /// Whether to show the form in taskbar
    /// </summary>
    public bool ShowInTaskbar
    {
        get => Artifact?.GetProperty<bool>("ShowInTaskbar") ?? true;
        set => Artifact?.SetProperty("ShowInTaskbar", value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        // Forms have Form as base class by default
        if (string.IsNullOrEmpty(Artifact?.GetProperty<string>("BaseClass")))
        {
            BaseClass = "Form";
        }
    }
}
