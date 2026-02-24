using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.CodeArchitecture.OnionArchitecture;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts
{
    public class CsvValueObjectReaderImplementationArtifact : WorkspaceArtifactBase
    {
        public CsvValueObjectReaderImplementationArtifact(CodeFileElement codeFileElement, string valueTypeId)
        {
            CodeFileElement = codeFileElement;
            ValueTypeId = valueTypeId;
            UpdateValueTypeNameAndNameSpace();
            AttachedToWorkspace += CsvValueObjectReaderImplementationArtifact_AttachedToWorkspace;
        }

        
        public CsvValueObjectReaderImplementationArtifact(ArtifactState state, List<string> errors) : base(state, errors)
        {
            CodeFileElement = CodeFileElement.FromJson((string)state.Properties[nameof(CodeFileElement)]!);
            UpdateValueTypeNameAndNameSpace();
            AttachedToWorkspace += CsvValueObjectReaderImplementationArtifact_AttachedToWorkspace;
        }
        private void CsvValueObjectReaderImplementationArtifact_AttachedToWorkspace(object? sender, EventArgs e)
        {
            ObserveValueTypeArtifact();
            UpdateValueTypeNameAndNameSpace();
        }

        public override string TreeNodeText => Name ?? string.Empty;

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");



        private CodeFileElement? _cachedCodeFile;
        public CodeFileElement? CodeFileElement
        {
            get { return _cachedCodeFile; }
            set
            {
                if (_cachedCodeFile != value)
                {
                    _cachedCodeFile = value;
                    if (value != null)
                        SetValue(nameof(CodeFileElement), value.ToJson());
                    else
                        SetValue<string>(nameof(CodeFileElement), null);
                }
            }
        }

        /// <summary>
        /// The name of the class-element inside the CodeFileElement. 
        /// It assumes that there is only one namespace and one type in the CodeFileElement. 
        /// This is a simplification for this specific use case, where we are generating a single class in a file, but it might need to be changed if we want to support more complex scenarios.
        /// </summary>
        public string? Name
        {
            get { return ClassElement?.Name; }
            set
            {
                if (value != ClassElement?.Name)
                {
                    if (CodeFileElement!= null && ClassElement != null) { 
                        ClassElement.Name = value;
                        CodeFileElement.FileName = value?? CodeFileElement.FileName;
                    }
                    RaisePropertyChangedEvent(nameof(Name));
                    RaisePropertyChangedEvent(nameof(TreeNodeText));
                }
            }
        }
        /// <summary>
        /// The class-element inside the CodeFileElement. 
        /// It assumes that there is only one namespace and one type in the CodeFileElement. 
        /// This is a simplification for this specific use case, where we are generating a single class in a file, but it might need to be changed if we want to support more complex scenarios.
        /// </summary>
        public ClassElement? ClassElement
        {
            get { return CodeFileElement?.Namespaces.SingleOrDefault()?.Types.SingleOrDefault() as ClassElement; }
        }

        public string? ValueTypeId
        {
            get { return GetValue<string?>(nameof(ValueTypeId)); }
            set { 
                if (SetValue<string?>(nameof(ValueTypeId), value))
                {
                    RaisePropertyChangedEvent(nameof(ValueType));
                    ObserveValueTypeArtifact();
                }
            } 
        }

        public string? Description
        {
            get { return GetValue<string?>(nameof(Description)); }
            set { 
                if (SetValue<string?>(nameof(Description), value)) 
                { 
                    if(ClassElement!=null)
                        ClassElement.Documentation = value;
                }
            }
        }

        private ValueTypeArtifact? _observingValueTypeArtifact;
        private void ObserveValueTypeArtifact()
        {
            if (_observingValueTypeArtifact != null)
                _observingValueTypeArtifact.PropertyChanged -= ValueTypeArtifact_PropertyChanged;

            _observingValueTypeArtifact = null;

            var valueType = ResolveValueTypeArtifact();
            if (valueType != null)
            {
                _observingValueTypeArtifact = valueType;
                _observingValueTypeArtifact.PropertyChanged += ValueTypeArtifact_PropertyChanged;
            }
        }

        /// <summary>
        /// Resolves the ValueTypeArtifact by ID. Falls back to ancestor-based lookup
        /// when the workspace tree is not yet fully constructed (e.g. during deserialization),
        /// since FindDescendantById may not find artifacts in branches that haven't been 
        /// fully attached yet.
        /// </summary>
        private ValueTypeArtifact? ResolveValueTypeArtifact()
        {
            if (string.IsNullOrEmpty(ValueTypeId))
                return null;

            // Primary: lookup via workspace descendants
            var result = Workspace?.FindDescendantById<ValueTypeArtifact>(ValueTypeId);
            if (result != null)
                return result;

            // Fallback: navigate via ancestor ScopeArtifact — needed when AttachedToWorkspace fires
            // before the full tree is constructed (FindDescendantById traverses Children which may
            // not yet contain all restored artifacts at that point)
            var scope = this.FindAncesterOfType<OnionScopeArtifact>();
            if (scope != null)
            {
                foreach (var domain in scope.Domains)
                {
                    var match = domain.ValueTypes.FirstOrDefault(vt => vt.Id == ValueTypeId);
                    if (match != null)
                        return match;
                }
            }
            
            return null;
        }

        /// <summary>
        /// The ValueTypeArtifact's Name is used as the generic type argument for the reader implementation
        /// </summary>
        private void ValueTypeArtifact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ValueTypeArtifact.Name) || e.PropertyName == nameof(ValueTypeArtifact.Context))
            {
                UpdateValueTypeNameAndNameSpace();
            }
        }

        private void UpdateValueTypeNameAndNameSpace()
        {
            if (ValueType != null && ClassElement != null)
            {
                var genericyTypeArgument = ClassElement.BaseTypes.FirstOrDefault()?.GenericArguments.FirstOrDefault();
                if (genericyTypeArgument != null)
                {
                    var valueTypeContext = ValueType.Context;
                    if (valueTypeContext != null)
                    {
                        genericyTypeArgument.TypeName = valueTypeContext.ClassName ?? genericyTypeArgument.TypeName;
                        genericyTypeArgument.Namespace = valueTypeContext.Namespace;
                        CodeFileElement?.AddUsing(valueTypeContext.Namespace);
                    }
                }
            }
        }

        public ValueTypeArtifact? ValueType
        {
            get
            {
                if (ValueTypeId == null)
                    return null;
                return ResolveValueTypeArtifact();
            }
        }


        public override IMementoState CaptureState()
        {
            var state = base.CaptureState();
            state.Properties[nameof(CodeFileElement)] = CodeFileElement?.ToJson();
            return state;
        }
    }
}
