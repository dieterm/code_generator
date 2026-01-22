using System.Collections.ObjectModel;

namespace CodeGenerator.Core.Artifacts
{
    public class ArtifactDecoratorCollection : KeyedCollection<string, IArtifactDecorator>
    {
         protected override string GetKeyForItem(IArtifactDecorator item)
        {
            return item.Key;
        }
    }
}
