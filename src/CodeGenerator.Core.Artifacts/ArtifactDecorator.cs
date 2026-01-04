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
