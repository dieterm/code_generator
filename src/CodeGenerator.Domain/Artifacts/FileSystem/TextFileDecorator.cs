namespace CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Decorator for text files with specific mime type and encoding
/// </summary>
public class TextFileDecorator : FileDecorator
{
    /// <summary>
    /// MIME type of the text content
    /// eg: "text/plain", "application/json"
    /// </summary>
    public string MimeType
    {
        get => Artifact?.GetProperty<string>("MimeType") ?? "text/plain";
        set => Artifact?.SetProperty("MimeType", value);
    }

    /// <summary>
    /// Encoding used for the text content
    /// eg.: "utf-8", "ascii"
    /// </summary>
    public string Encoding
    {
        get => Artifact?.GetProperty<string>("Encoding") ?? "utf-8";
        set => Artifact?.SetProperty("Encoding", value);
    }
}
