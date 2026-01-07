
namespace CodeGenerator.Core.Artifacts
{
    public abstract class ArtifactDecorator : IArtifactDecorator
    {
        protected ArtifactDecorator(string key)
        {
            Key = key;
        }
        public string Key { get; }
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
        public virtual async Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            
        }

        /// <summary>
        /// Get a property value
        /// </summary>
        public T? GetProperty<T>(string name)
        {
            return Artifact.GetProperty<T>(this, name);
        }

        /// <summary>
        /// Set a property value
        /// </summary>
        public void SetProperty<T>(string name, T? value)
        {
            Artifact.SetProperty(this, name, value);
        }
    }
}
