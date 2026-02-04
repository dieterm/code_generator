using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public abstract class CodeArchitectureLayerArtifact : WorkspaceArtifactBase, IDisposable
    {
        private ScopeArtifact? _scope;
        private string _initialScopeName;
        public CodeArchitectureLayerArtifact(string layerName, string initialScopeName)
        {
            LayerName = layerName;
            _initialScopeName = initialScopeName;
            ParentChanged += CodeArchitectureLayerArtifact_ParentChanged;
        }

        public CodeArchitectureLayerArtifact(ArtifactState state)
            : base(state)
        {
            ParentChanged += CodeArchitectureLayerArtifact_ParentChanged;
        }

        private void CodeArchitectureLayerArtifact_ParentChanged(object? sender, ParentChangedEventArgs e)
        {
            if(_scope!=null)
                _scope.PropertyChanged -= Scope_PropertyChanged;

            if(e.NewParent is ScopeArtifact newScope)
            {
                _scope = newScope;
                _scope.PropertyChanged += Scope_PropertyChanged;
            }
            else
            {
                throw new InvalidOperationException("CodeArchitectureLayerArtifact must have a ScopeArtifact as parent.");
            }
        }

        private void Scope_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ScopeArtifact.Name))
            {
                RaisePropertyChangedEvent(nameof(ScopeName));
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }

        /// <summary>
        /// Name of the layer (e.g., "Application", "Domain", "Infrastructure")
        /// </summary>
        public string? LayerName
        {
            get { return GetValue<string>(nameof(LayerName)); }
            private set { 
                SetValue<string>(nameof(LayerName), value);
                RaisePropertyChangedEvent(nameof(TreeNodeText));
            }
        }
        /// <summary>
        /// Scope of the layer (e.g., "Shared", "Application", "ProjectA", "ModuleB")
        /// </summary>
        public string ScopeName { get { return Scope?.Name?? _initialScopeName; } }

        public ScopeArtifact Scope { get { return (Parent as ScopeArtifact)!; } }

        public override string TreeNodeText {  get { return $"{LayerName} Layer ({Scope})"; } }

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("layers");

        protected override WorkspaceArtifactContext? GetOwnContext()
        {
            return new WorkspaceArtifactContext
            {
                Namespace = Scope.Context!.Namespace + "." + LayerName
            };
        }

        public void Dispose()
        {
            if(_scope!=null)
            {
                _scope.PropertyChanged -= Scope_PropertyChanged;
            }   
        }
    }
}
