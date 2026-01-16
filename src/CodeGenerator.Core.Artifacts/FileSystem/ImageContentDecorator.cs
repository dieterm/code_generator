using System.Drawing;

namespace CodeGenerator.Core.Artifacts.FileSystem;

/// <summary>
/// Decorator for artifacts with image content, supports preview
/// </summary>
public class ImageContentDecorator : BinaryContentDecorator, IPreviewableDecorator
{
    public ImageContentDecorator(string key)
        : base(key)
    {
    }

    public ImageContentDecorator(ArtifactDecoratorState state) : base(state) { }

    public bool CanPreview => Content != null && Content.Length > 0;

    public object? CreatePreview()
    {
        if (Content == null || Content.Length == 0)
            return null;

        return Image.FromStream(new MemoryStream(Content));
    }
}
