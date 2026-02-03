using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Generators
{
    public abstract class GeneratorBase : IMessageBusAwareGenerator
    {
        public GeneratorMessageBus? MessageBus { get; private set; }
        public GeneratorSettingsDescription SettingsDescription { get; set; }

        /// <inheritdoc/>
        public string Id => SettingsDescription.Id;

        /// <summary>
        /// Base constructor.
        /// Initializes the SettingsDescription by calling ConfigureSettingsDescription().
        /// </summary>
        protected GeneratorBase()
        {
            SettingsDescription = ConfigureSettingsDescription();
        }

        protected abstract GeneratorSettingsDescription ConfigureSettingsDescription();

        protected virtual GeneratorSettings GetSettings()
        {
            var settingsManager = ServiceProviderHolder.GetRequiredService<GeneratorSettingsManager>();
            
            var settings = settingsManager.GetGeneratorSettings(this.SettingsDescription.Id);
            return settings;
        }

        /// <summary>
        /// Returns whether this generator is enabled in the settings.<br />
        /// (will automatically update on settings change)
        /// </summary>
        public bool Enabled { get; private set; }
        
        private void RefreshEnabledState()
        {
            var settings = GetSettings();
            Enabled = settings.Enabled;
        }

        public virtual void Initialize(GeneratorMessageBus messageBus)
        {
            MessageBus = messageBus;
            // set initial Enabled state
            RefreshEnabledState();

            var settingsManager = ServiceProviderHolder.GetRequiredService<GeneratorSettingsManager>();
            // subscribe to settings saved event to update Enabled state
            settingsManager.SettingsSaved += (sender, e) => RefreshEnabledState();

        }

        public abstract void SubscribeToEvents(GeneratorMessageBus messageBus);

        public abstract void UnsubscribeFromEvents(GeneratorMessageBus messageBus);

        /// <summary>
        /// Publish CreatingArtifactEvent -> Add child to parent -> Publish CreatedArtifactEvent
        /// </summary>
        protected void AddChildArtifactToParent(IArtifact parent, IArtifact child, GenerationResult result)
        {
            MessageBus.Publish(new CreatingArtifactEventArgs(result, child));
            parent.AddChild(child);
            MessageBus.Publish(new CreatedArtifactEventArgs(result, child));
            //VisitArtifactChildrenAndPublishCreatingEvents(child, result);
            //VisitArtifactChildrenAndPublishCreatedEvents(child, result);
        }

        protected void VisitArtifactChildrenAndPublishCreatingEvents(IArtifact artifact, GenerationResult result)
        {
            foreach (var child in artifact.Children)
            {
                NotifyArtifactCreating(child, result);
                VisitArtifactChildrenAndPublishCreatingEvents(child, result);
            }
        }

        protected void VisitArtifactChildrenAndPublishCreatedEvents(IArtifact artifact, GenerationResult result)
        {
            foreach (var child in artifact.Children)
            {
                NotifyArtifactCreated(child, result);
                VisitArtifactChildrenAndPublishCreatedEvents(child, result);
            }
        }

        protected void NotifyArtifactCreated(IArtifact artifact, GenerationResult result)
        {
            MessageBus.Publish(new CreatedArtifactEventArgs(result, artifact));
        }

        protected void NotifyArtifactCreating(IArtifact artifact, GenerationResult result)
        {
            MessageBus.Publish(new CreatingArtifactEventArgs(result, artifact));
        }

        protected TTemplateInstance GetTemplateInstance<TTemplate, TTemplateInstance>(string templateName, params string[] subfolders)
            where TTemplate : class, ITemplate
            where TTemplateInstance : class, ITemplateInstance
        {
            var templateId = TemplateIdParser.BuildGeneratorTemplateId(this.Id, templateName, subfolders);
            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
            var template = templateManager.GetTemplateById(templateId) as TTemplate;
            if (template == null) throw new ApplicationException($"{templateName} template with id '{templateId}' not found.");
            var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
            var templateEngine = templateEngineManager.GetTemplateEnginesForTemplate(template).FirstOrDefault();
            if (templateEngine == null) throw new ApplicationException($"No template engine found for template id '{templateId}'.");
            var templateInstance = templateEngine.CreateTemplateInstance(template) as TTemplateInstance;
            if (templateInstance == null) throw new ApplicationException($"Template instance was not created or not of type {nameof(TTemplateInstance)}.");

            return templateInstance;
        }

        protected void AddTemplateOutputToArtifact(TemplateOutput output, IArtifact parentArtifact, GenerationResult result)
        {
            if (output.Succeeded)
            {
                foreach (var artifact in output.Artifacts)
                {
                    AddChildArtifactToParent(parentArtifact, artifact, result);
                }
            }
            else
            {
                result.Errors.AddRange(output.Errors);
            }
        }
    }
}
