using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public abstract class UsingsContainerArtifact<T> : CodeElementArtifactBase<T>, IEnumerable<UsingElementArtifact>, IUsingsContainerArtifact 
        where T : CodeElement, ICodeElementWithUsings
    {

        public UsingsContainerArtifact(T codeElement)
            : base(codeElement)
        {
            foreach (var usingElement in codeElement.Usings)
            {
                AddChild(new UsingElementArtifact(usingElement));
            }
        }

        public UsingsContainerArtifact(ArtifactState artifactState) : base(artifactState)
        {
        }

        public override string TreeNodeText => "Usings";
        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("folder");

        public IEnumerator<UsingElementArtifact> GetEnumerator()
        {
            return Children.OfType<UsingElementArtifact>().GetEnumerator();
        }

        public void RemoveUsing(UsingElementArtifact artifact)
        {
            CodeElement.Usings.Remove(artifact.CodeElement);
            RemoveChild(artifact);
        }

        public void AddNewUsing()
        {
            var newUsing = new UsingElement { Namespace = "MyProduct.MyNamespace" };
            CodeElement.Usings.Add(newUsing);
            AddChild(new UsingElementArtifact(newUsing));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
