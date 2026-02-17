using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Workspace.Artifacts
{
    public class CsvValueObjectReaderArtifact : WorkspaceArtifactBase
    {
        public CsvValueObjectReaderArtifact(CodeFileElement codeFileElement)
        {
            CodeFileElement = codeFileElement;
        }

        public CsvValueObjectReaderArtifact(ArtifactState state, List<string> errors) : base(state, errors)
        {
            CodeFileElement = CodeFileElement.FromJson((string)state.Properties[nameof(CodeFileElement)]!);
        }

        public override string TreeNodeText => "CsvValueObjectReader<T>";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("braces");

        private CodeFileElement? _cachedCodeFile;
        public CodeFileElement? CodeFileElement {
            get { return _cachedCodeFile; }
            set {
                if (_cachedCodeFile != value) {
                    _cachedCodeFile = value; 
                    if(value!=null)
                        SetValue(nameof(CodeFileElement), value.ToJson());
                    else 
                        SetValue<string>(nameof(CodeFileElement), null);
                }
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
