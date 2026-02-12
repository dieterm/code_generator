using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Views.TreeNode;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public abstract class CodeElementArtifactBase : Artifact
    {
        protected CodeElementArtifactBase()
        { }

        protected CodeElementArtifactBase(ArtifactState artifactState) : base(artifactState) 
        { }
        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");
        public override string TreeNodeText => Name;
        public virtual string? Name { 
            get { return GetValue<string?>(nameof(Name)); }
            set {
                if(SetValue<string?>(nameof(Name), value))
                {
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                }
            } 
        }
        //public virtual AccessModifier AccessModifier { get; set; }
        //public virtual ElementModifiers Modifiers { get; set; }
        //public virtual string? Documentation { get; set; }
        //public virtual string? RawCode { get; set; }
    }

    public abstract class CodeElementArtifactBase<T> : CodeElementArtifactBase, IEditableTreeNode where T : CodeElement
    {
        public T CodeElement { get; protected set; }
        protected CodeElementArtifactBase(T codeElement) 
        {
            CodeElement = codeElement;
            Name = codeElement.Name;
            AddChild(new AttributesContainerArtifact(codeElement.Attributes));
        }
        protected CodeElementArtifactBase(ArtifactState artifactState) : base(artifactState) 
        { }

        public override string TreeNodeText => Name ?? string.Empty;

        /// <summary>
        /// SingleLineTextField
        /// </summary>
        public override string? Name
        {
            get { return CodeElement.Name; }
            set 
            { 
                if(value != CodeElement.Name)
                {
                    CodeElement.Name = value;
                    RaisePropertyChangedEvent(nameof(Name));
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                }
            }
        }


        /// <summary>
        /// ComboboxField with values from AccessModifier enum
        /// </summary>
        public virtual AccessModifier AccessModifier
        {
            get { return CodeElement.AccessModifier; }
            set
            {
                if (CodeElement.AccessModifier != value)
                {
                    CodeElement.AccessModifier = value;
                    RaisePropertyChangedEvent(nameof(AccessModifier));
                }
            }
        }
        /// <summary>
        /// Additional modifiers (static, abstract, virtual, etc.)
        /// </summary>
        public virtual ElementModifiers Modifiers
        {
            get { return CodeElement.Modifiers; }
            set
            {
                if (CodeElement.Modifiers != value)
                {
                    CodeElement.Modifiers = value;
                    RaisePropertyChangedEvent(nameof(Modifiers));
                }
            }
        }

        /// <summary>
        /// SingleLineTextField
        /// </summary>
        public virtual string? Documentation
        {
            get => CodeElement.Documentation;
            set
            {
                if (CodeElement.Documentation != value)
                {
                    CodeElement.Documentation = value;
                    RaisePropertyChangedEvent(nameof(Documentation));
                }
            }
        }

        /// <summary>
        /// SingleLineTextField
        /// </summary>
        public virtual string? RawCode
        {
            get => CodeElement.RawCode;
            set
            {
                var newValue = string.IsNullOrEmpty(value) ? null : value;
                if (CodeElement.RawCode != newValue)
                {
                    CodeElement.RawCode = newValue;
                    RaisePropertyChangedEvent(nameof(RawCode));
                }
            }
        }

        public virtual bool CanBeginEdit()
        {
            return true;
        }

        public virtual bool Validating(string newName)
        {
            return !string.IsNullOrWhiteSpace(newName);
        }

        public virtual void EndEdit(string oldName, string newName)
        {
            Name = newName;
        }

        public AttributesContainerArtifact Attributes => Children.OfType<AttributesContainerArtifact>().Single()!;
    }
}
