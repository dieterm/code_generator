namespace CodeGenerator.Core.Artifacts.FileSystem;

/// <summary>
/// Decorator for artifacts with binary content
/// </summary>
public class BinaryContentDecorator : ArtifactDecorator
{
    public BinaryContentDecorator(string key)
        : base(key)
    {
    }

    public BinaryContentDecorator(ArtifactDecoratorState state) : base(state) { }

    public byte[]? Content
    {
        get { return GetValue<byte[]>(nameof(Content)); }
        set { SetValue(nameof(Content), value); }
    }

    public override bool CanGenerate()
    {
        return !string.IsNullOrWhiteSpace(Artifact?.GetDecorator<FileArtifactDecorator>()?.FileName);
    }

    public override async Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
    {
        var fileArtifact = Artifact?.GetDecorator<FileArtifactDecorator>() 
            ?? throw new InvalidOperationException("Artifact does not have a FileArtifactDecorator");
        
        if (string.IsNullOrWhiteSpace(fileArtifact.FileName))
            throw new InvalidOperationException("FileArtifactDecorator does not have a FileName set");

        var folderPath = Artifact!.GetFullPath();
        var filePath = Path.Combine(folderPath, fileArtifact.FileName);
        var content = Content ?? Array.Empty<byte>();
        
        await File.WriteAllBytesAsync(filePath, content, cancellationToken);
    }
}
