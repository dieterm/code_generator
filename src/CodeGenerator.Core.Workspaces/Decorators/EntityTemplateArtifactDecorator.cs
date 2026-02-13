using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Decorators
{
    public class EntityTemplateArtifactDecorator : ArtifactDecorator
    {
        public const string DECORATOR_KEY = "EntityTemplate";
        public EntityTemplateArtifactDecorator(ArtifactDecoratorState state) : base(state)
        {
        }

        public EntityTemplateArtifactDecorator(string key) : base(key)
        {
        }

        public string? TemplateId { 
            get => GetValue<string>(nameof(TemplateId));
            set => SetValue(nameof(TemplateId), value);
        }

        public override bool CanGenerate()
        {
            return !string.IsNullOrEmpty(TemplateId);
        }

        public override Task GenerateAsync(IProgress<ArtifactGenerationProgress> progress, CancellationToken cancellationToken = default)
        {
            OnGenerating();
            OnGenerated();
            return Task.CompletedTask;
        }
    }
}
