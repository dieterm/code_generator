
using CodeGenerator.Shared.Memento;
using System.ComponentModel;

namespace CodeGenerator.Core.Artifacts
{
    public abstract class ArtifactDecorator : MementoObjectBase<ArtifactDecoratorState>, IArtifactDecorator
    {
        /// <summary>
        /// Constructor for restoring state from memento
        /// </summary>
        public ArtifactDecorator(ArtifactDecoratorState state) 
            : base(state)
        {
            
        }
        protected ArtifactDecorator(string key)
        {
            Key = key;
        }

        public string Key { 
            get { return base.GetValue<string>(nameof(Key)); } 
            set { SetValue(nameof(Key), value); }
        }
        public Artifact? Artifact { get; private set; }

         public virtual void Attach(Artifact artifact)
        {
            Artifact = artifact;
        }

        public virtual void Detach()
        {
            Artifact = null;
        }

        /// <summary>
        /// Overwite in derived classes to indicate if this decorator can generate content
        /// </summary>
        /// <returns></returns>
        public virtual bool CanGenerate()
        {
            return false;
        }

        /// <summary>
        /// Overwrite in derived classes to implement generation logic
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }
}
