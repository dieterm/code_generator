using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public class ArtifactCollection : IEnumerable<IArtifact>
    {
        private readonly List<IArtifact> _artifacts = new List<IArtifact>();

        public void AddRange(IEnumerable<IArtifact> artifacts)
        {
            if (artifacts == null) throw new ArgumentNullException(nameof(artifacts));
            foreach (var artifact in artifacts)
            {
                Add(artifact);
            }
        }

        public void Add(IArtifact artifact)
        {
            if (artifact == null) throw new ArgumentNullException(nameof(artifact));
            _artifacts.Add(artifact);
        }

        public bool Remove(IArtifact artifact)
        {
            if (artifact == null) throw new ArgumentNullException(nameof(artifact));
            return _artifacts.Remove(artifact);
        }

        public IEnumerator<IArtifact> GetEnumerator()
        {
            return _artifacts.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _artifacts.GetEnumerator();
        }
        /// <summary>
        /// Visit all artifacts and child-artifacts recursively and save them as a flat serializable list state
        /// </summary>
        /// <returns></returns>
        public ArtifactCollectionState CaptureState()
        {
            var state = new ArtifactCollectionState();
            SaveArtifactsToCollectionState(_artifacts, state);
            return state;
        }

        public void SaveArtifactsToCollectionState(IEnumerable<IArtifact> artifacts, ArtifactCollectionState collectionState)
        {
            foreach (var artifact in artifacts)
            {
                collectionState.Artifacts.Add((ArtifactState)artifact.CaptureState());
                SaveArtifactsToCollectionState(artifact.Children, collectionState);
            }
        }
    }
}