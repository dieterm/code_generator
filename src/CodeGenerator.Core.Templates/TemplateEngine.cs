using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public abstract class TemplateEngine<TTemplate, TTemplateInstance> : ITemplateEngine
        where TTemplate : ITemplate
        where TTemplateInstance : ITemplateInstance
    {
        protected TemplateType SupportedTemplateType { get; }

        /// <inheritdoc/>
        public string Id { get; }
        /// <inheritdoc/>
        public string DisplayName { get; }
        protected ILogger Logger { get; }
        
        public TemplateEngineSettingsDescription SettingsDescription { get; }
        protected TemplateEngine(ILogger logger, string id, string displayName, TemplateType supportedTemplateType)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            SupportedTemplateType = supportedTemplateType;
            SettingsDescription = CreateSettingsDescription();
        }

        /// <summary>
        /// Override to provide a custom settings description.
        /// </summary>
        protected virtual TemplateEngineSettingsDescription CreateSettingsDescription()
        {
            return new TemplateEngineSettingsDescription(Id, DisplayName, DisplayName);
        }

        /// <inheritdoc/>
        public virtual bool SupportsTemplateType(TemplateType templateType)
        {
            return templateType == SupportedTemplateType;
        }

        /// <inheritdoc/>
        public virtual bool SupportsTemplate(ITemplate template)
        {
            return template is TTemplate && template.TemplateType == SupportedTemplateType;
        }

        /// <inheritdoc/>
        public abstract Task<TemplateOutput> RenderAsync(TTemplateInstance templateInstance, CancellationToken cancellationToken);

        /// <inheritdoc/>
        Task<TemplateOutput> ITemplateEngine.RenderAsync(ITemplateInstance templateInstance, CancellationToken cancellationToken)
        {
            return RenderAsync((TTemplateInstance)templateInstance, cancellationToken);
        }

        /// <inheritdoc/>
        public abstract ITemplateInstance CreateTemplateInstance(ITemplate template);

        /// <inheritdoc/>
        public virtual void Initialize()
        {
            // Default implementation does nothing
        }

        protected virtual TemplateEngineSettings GetSettings()
        {
            var settingsManager = ServiceProviderHolder.GetRequiredService<TemplateEngineSettingsManager>();
            var settings = settingsManager.GetTemplateEngineSettings(this.SettingsDescription.Id);
            return settings;
        }
    }
}
