using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public abstract class FileBasedTemplateEngine<TTemplate, TTemplateInstance> : TemplateEngine<TTemplate, TTemplateInstance>, IFileBasedTemplateEngine
        where TTemplate : ITemplate
        where TTemplateInstance : ITemplateInstance
    {
        public string[] SupportedFileExtensions { get; } = Array.Empty<string>();
        public abstract string DefaultFileExtension { get; }

        public FileBasedTemplateEngine(ILogger logger, string id, string displayName, TemplateType supportedTemplateType, string[]? supportedFileExtensions = null)
            : base(logger, id, displayName, supportedTemplateType)
        {
            if (supportedFileExtensions != null)
            {
                SupportedFileExtensions = supportedFileExtensions;
            }
        }
       


        /// <summary>
        /// This method checks whether the given fileOrFolderName ends with one of the supported file extensions.<br />
        /// It does not check if the file or folder actually exists.<br />
        /// These checks need to be done by the derived classes if needed.
        /// </summary>
        public virtual bool SupportsTemplatePath(string fileOrFolderName)
        {
            if (string.IsNullOrWhiteSpace(fileOrFolderName)) throw new ArgumentNullException(nameof(fileOrFolderName));
            return SupportedFileExtensions.Any(ext => fileOrFolderName.EndsWith($".{ext}", StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public virtual bool SupportsTemplateFileExtension(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension)) throw new ArgumentNullException(nameof(fileExtension));
            return SupportedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public abstract ITemplate CreateTemplateFromFile(string filePath);
    }
}
