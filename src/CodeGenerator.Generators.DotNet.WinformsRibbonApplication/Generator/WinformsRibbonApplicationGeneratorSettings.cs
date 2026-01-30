using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Generators.DotNet.Generators;
using CodeGenerator.Generators.DotNet.Generators.PresentationLayer.ApplicationScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication
{
    public class WinformsRibbonApplicationGeneratorSettings
    {
        public const string TEMPLATE_ID = "WinformsRibbonApplication";
        private readonly GeneratorSettings _settings;

        public WinformsRibbonApplicationGeneratorSettings(GeneratorSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public static GeneratorSettingsDescription CreateDescription(WinformsRibbonApplicationGenerator generator)
        {
            var id = nameof(WinformsRibbonApplicationGenerator);
            var name = nameof(WinformsRibbonApplicationGenerator);
            var description = "Generates a Winforms application with a ribbon interface.";
            var descriptionObj = new GeneratorSettingsDescription(id, name, description);
            var templateId = TemplateIdParser.BuildGeneratorTemplateId(id, TEMPLATE_ID);
            descriptionObj.Templates.Add(new TemplateRequirement(templateId, TEMPLATE_ID, null, TemplateType.Folder));
            descriptionObj.DependingGeneratorIds.Add(PresentationLayerApplicationScopeDotNetProjectGenerator.GENERATOR_ID);
            return descriptionObj;
        }
    }
}
