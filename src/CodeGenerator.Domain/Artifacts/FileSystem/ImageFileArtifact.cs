namespace CodeGenerator.Domain.Artifacts.FileSystem;

/// <summary>
/// Decorator for image files
/// </summary>
public class ImageFileDecorator : FileDecorator
{
    /// <summary>
    /// Raw image data
    /// </summary>
    public byte[]? ImageData
    {
        get => Artifact?.GetProperty<byte[]>("ImageData");
        set => Artifact?.SetProperty("ImageData", value);
    }

    /// <summary>
    /// Thumbnail data for preview
    /// </summary>
    public byte[]? ThumbnailData
    {
        get => Artifact?.GetProperty<byte[]>("ThumbnailData");
        set => Artifact?.SetProperty("ThumbnailData", value);
    }

    /// <summary>
    /// Image width in pixels
    /// </summary>
    public int Width
    {
        get => Artifact?.GetProperty<int>("Width") ?? 0;
        set => Artifact?.SetProperty("Width", value);
    }

    /// <summary>
    /// Image height in pixels
    /// </summary>
    public int Height
    {
        get => Artifact?.GetProperty<int>("Height") ?? 0;
        set => Artifact?.SetProperty("Height", value);
    }

    /// <summary>
    /// Image format (e.g., "png", "jpg", "gif")
    /// </summary>
    public string ImageFormat
    {
        get => Artifact?.GetProperty<string>("ImageFormat") ?? "png";
        set => Artifact?.SetProperty("ImageFormat", value);
    }

    public override object? CreatePreview() => ImageData;
}
