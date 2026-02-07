using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.SharedScope.Generators
{
    public class DotNetSharedScopeGeneratorSettings
    {
        private readonly GeneratorSettings _settings;

        public DotNetSharedScopeGeneratorSettings(GeneratorSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public static GeneratorSettingsDescription CreateDescription(DotNetSharedScopeGenerator generator)
        {
            var id = nameof(DotNetSharedScopeGenerator);
            var name = ".NET shared scope generator";
            var description = "Generates base classes in the shared scope.";
            var descriptionObj = new GeneratorSettingsDescription(id, name, description);
            var architectureManager = ServiceProviderHolder.GetRequiredService<CodeArchitectureManager>();
            foreach(var architecture in architectureManager.GetAllArchitectures())
            {
                foreach(var layer in architecture.Layers)
                {
                    var templateId = TemplateIdParser.BuildGeneratorTemplateId(id, layer.LayerName, architecture.Id);
                    descriptionObj.Templates.Add(new TemplateRequirement(templateId, $"{architecture.Name}{layer.LayerName} base class template",null, TemplateType.Folder));
                }
            }
            return descriptionObj;
        }
    }
}
