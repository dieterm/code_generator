using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.Templates
{
    public abstract class TemplateDatasourceProviderDecorator : ArtifactDecorator
    {
        public TemplateDatasourceProviderDecorator(ArtifactDecoratorState state) 
            : base(state)
        {
        }
        public TemplateDatasourceProviderDecorator(string key) 
            : base(key)
        {
        }
        public abstract string DisplayName { get; }
        public abstract string FullPath { get; }
        public abstract Task<IEnumerable<dynamic>> LoadDataAsync(ILogger Logger, string? filter, int? maxRows, CancellationToken cancellationToken);

    }
}
