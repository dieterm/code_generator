using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Domain.ProgrammingLanguages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeElements
{
    public class CodeFileDecorator : ArtifactDecorator
    {
        public CodeFileDecorator(ArtifactDecoratorState state)
            : base(state)
        {
        }

        public CodeFileDecorator(string key) 
            : base(key)
        {
        }

        public override bool CanGenerate()
        {
            return true;
        }

        public override async Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            var codeFileArtifact = this.Artifact as CodeFileArtifact;
            if(codeFileArtifact==null) throw new InvalidOperationException("Artifact is not a CodeFileArtifact");

            var codeContent = codeFileArtifact.GetTextContent();

            var fileArtifact = Artifact.GetDecoratorOfType<FileArtifactDecorator>() ?? throw new InvalidOperationException("Artifact does not have a FileArtifactDecorator");
            if (string.IsNullOrWhiteSpace(fileArtifact.FileName))
                throw new InvalidOperationException("FileArtifactDecorator does not have a FileName set");

            var folderPath = Artifact.GetFullPath();

            var filePath = Path.Combine(folderPath, fileArtifact.FileName);
            var content = codeContent;
            await System.IO.File.WriteAllTextAsync(filePath, content, cancellationToken);
        }
    }
}
