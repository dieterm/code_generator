using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts;

public class TypeReferenceArtifact : CodeElementArtifactBase
{
    public TypeReference TypeReference { get; }

    public TypeReferenceArtifact(TypeReference typeReference)
    {
        TypeReference = typeReference;
        Name = typeReference.TypeName;
    }

    public TypeReferenceArtifact(ArtifactState artifactState, List<string> errors) : base(artifactState, errors)
    {
        TypeReference = new TypeReference();
    }

    public override string TreeNodeText => GetDisplayText();
    public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

    public string TypeName
    {
        get => TypeReference.TypeName;
        set
        {
            if (TypeReference.TypeName != value)
            {
                TypeReference.TypeName = value;
                Name = value;
                RaisePropertyChangedEvent(nameof(TypeName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    public bool IsNullable
    {
        get => TypeReference.IsNullable;
        set
        {
            if (TypeReference.IsNullable != value)
            {
                TypeReference.IsNullable = value;
                RaisePropertyChangedEvent(nameof(IsNullable));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    public bool IsArray
    {
        get => TypeReference.IsArray;
        set
        {
            if (TypeReference.IsArray != value)
            {
                TypeReference.IsArray = value;
                RaisePropertyChangedEvent(nameof(IsArray));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    public int ArrayRank
    {
        get => TypeReference.ArrayRank;
        set
        {
            if (TypeReference.ArrayRank != value)
            {
                TypeReference.ArrayRank = value;
                RaisePropertyChangedEvent(nameof(ArrayRank));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
    }

    public string? Namespace
    {
        get => TypeReference.Namespace;
        set
        {
            if (TypeReference.Namespace != value)
            {
                TypeReference.Namespace = value;
                RaisePropertyChangedEvent(nameof(Namespace));
            }
        }
    }

    public List<TypeReference> GenericArguments => TypeReference.GenericArguments;

    private string GetDisplayText()
    {
        var text = TypeReference.TypeName;

        if (TypeReference.GenericArguments.Count > 0)
            text += $"<{string.Join(", ", TypeReference.GenericArguments.Select(a => a.TypeName))}>";

        if (TypeReference.IsNullable)
            text += "?";

        if (TypeReference.IsArray)
            text += TypeReference.ArrayRank > 1 ? $"[{new string(',', TypeReference.ArrayRank - 1)}]" : "[]";

        return text;
    }
}
