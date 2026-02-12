using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public class UsingElementArtifact : CodeElementArtifactBase<UsingElement>
    {
        public UsingElementArtifact(UsingElement usingElement) 
            : base(usingElement)
        {
            
        }

        public UsingElementArtifact(ArtifactState artifactState) : base(artifactState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// BooleanField
        /// </summary>
        public bool IsGlobal
        {
            get { return CodeElement.IsGlobal; }
            set
            {
                if (CodeElement.IsGlobal != value)
                {
                    CodeElement.IsGlobal = value;
                    RaisePropertyChangedEvent(nameof(IsGlobal));
                }
            }
        }

        /// <summary>
        /// SingleLineTextField
        /// </summary>
        public string Namespace
        {
            get { return CodeElement.Namespace; }
            set
            {
                if (CodeElement.Namespace != value)
                {
                    CodeElement.Namespace = value;
                    RaisePropertyChangedEvent(nameof(Namespace));
                }
            }
        }

        /// <summary>
        /// SingleLineTextField
        /// </summary>
        public string? Alias
        {
            get { return CodeElement.Alias; }
            set
            {
                if (CodeElement.Alias != value)
                {
                    CodeElement.Alias = value;
                    RaisePropertyChangedEvent(nameof(Alias));
                }
            }
        }


    }
}
