using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System.Collections;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class IndexersContainerArtifact : CodeElementArtifactBase, IEnumerable<IndexerElementArtifact>
{
    private readonly List<IndexerElement> _indexers;

    public IndexersContainerArtifact(List<IndexerElement> indexers) : base()
    {
        _indexers = indexers;
        foreach (var indexer in indexers)
            AddChild(new IndexerElementArtifact(indexer));
    }

    public IndexersContainerArtifact(ArtifactState artifactState) : base(artifactState) { }

    public override string TreeNodeText => "Indexers";
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

    public IEnumerator<IndexerElementArtifact> GetEnumerator() => Children.OfType<IndexerElementArtifact>().GetEnumerator();

    public void AddIndexerElement(IndexerElement newIndexerElement)
    {
        _indexers.Add(newIndexerElement);
        AddChild(new IndexerElementArtifact(newIndexerElement));
    }

    public void RemoveIndexerElement(IndexerElementArtifact indexerElementArtifact)
    {
        RemoveChild(indexerElementArtifact);
        _indexers.Remove(indexerElementArtifact.CodeElement);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
