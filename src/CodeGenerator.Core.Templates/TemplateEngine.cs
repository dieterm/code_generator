using CodeGenerator.Core.Interfaces;
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
        protected string[] SupportedFileExtensions { get; } = Array.Empty<string>();
        protected TemplateType SupportedTemplateType { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string DisplayName { get; }
        protected ILogger Logger { get; }

        protected TemplateEngine(ILogger logger, string id, string displayName, TemplateType supportedTemplateType, string[]? supportedFileExtensions = null)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            SupportedTemplateType = supportedTemplateType;
            if (supportedFileExtensions != null)
            {
                SupportedFileExtensions = supportedFileExtensions;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool SupportsTemplate(ITemplate template)
        {
            return template is TTemplate && template.TemplateType == SupportedTemplateType;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool SupportsTemplateType(TemplateType templateType)
        {
            return templateType == SupportedTemplateType;
        }
       
        /// <summary>
        /// This method checks whether the given fileOrFolderName ends with one of the supported file extensions.<br />
        /// It does not check if the file or folder actually exists.<br />
        /// These checks need to be done by the derived classes if needed.
        /// </summary>
        public virtual bool SupportsTemplatePath(string fileOrFolderName)
        {
            if(string.IsNullOrWhiteSpace(fileOrFolderName)) throw new ArgumentNullException(nameof(fileOrFolderName));
            return SupportedFileExtensions.Any(ext => fileOrFolderName.EndsWith($".{ext}", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual bool SupportsTemplateFileExtension(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension)) throw new ArgumentNullException(nameof(fileExtension));
            return SupportedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public abstract Task<TemplateOutput> RenderAsync(TTemplateInstance templateInstance, CancellationToken cancellationToken);
        Task<TemplateOutput> ITemplateEngine.RenderAsync(ITemplateInstance templateInstance, CancellationToken cancellationToken)
        {
            return RenderAsync((TTemplateInstance)templateInstance, cancellationToken);
        }

        public abstract ITemplate CreateTemplateFromFile(string filePath);
        public abstract ITemplateInstance CreateTemplateInstance(ITemplate template);
    }
}
