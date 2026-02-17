using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.ValueTypes;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
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
        }

        public CsvValueObjectReaderImplementationArtifact(ArtifactState state, List<string> errors) : base(state, errors)
        {
            CodeFileElement = CodeFileElement.FromJson((string)state.Properties[nameof(CodeFileElement)]!);
        }

        public override string TreeNodeText => CodeFileElement?.FileName ?? string.Empty    ;

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

        public string? ValueTypeId
        {
            get { return GetValue<string?>(nameof(ValueTypeId)); }
            set { 
                if (SetValue<string?>(nameof(ValueTypeId), value))
                {
                    RaisePropertyChangedEvent(nameof(ValueType));
                }
            } 
        }

        public ValueTypeArtifact? ValueType
        {
            get
            {
                if (ValueTypeId == null)
                    return null;
                return Workspace?.FindDescendantById<ValueTypeArtifact>(ValueTypeId);
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
