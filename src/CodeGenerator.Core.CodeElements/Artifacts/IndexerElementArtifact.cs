using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class IndexerElementArtifact : CodeElementArtifactBase<IndexerElement>
{
    public IndexerElementArtifact(IndexerElement indexerElement) : base(indexerElement)
    {
        AddChild(new ParametersContainerArtifact(indexerElement.Parameters));
    }

    public IndexerElementArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => $"this[{string.Join(", ", CodeElement.Parameters.Select(p => p.Type.TypeName))}]";

    public string TypeName
    {
        get => CodeElement.Type.TypeName;
        set => CodeElement.Type.TypeName = value;
    }

    public bool HasGetter
    {
        get => CodeElement.HasGetter;
        set => CodeElement.HasGetter = value;
    }

    public bool HasSetter
    {
        get => CodeElement.HasSetter;
        set => CodeElement.HasSetter = value;
    }

    public ParametersContainerArtifact Parameters => Children.OfType<ParametersContainerArtifact>().Single();
}
