using CodeGenerator.Core.MessageBus;
using CodeGenerator.Application.ViewModels;

namespace CodeGenerator.Application.Events.DomainSchema
{
    public class DomainSchemaLoadedEvent : ApplicationEventArg
    {
        /// <summary>
        /// If FilePath is empty, a new DomainSchema was created
        /// </summary>
        public string? FilePath { get; }
        public DomainSchemaTreeViewModel TreeViewModel { get; }
        public Core.DomainSchema.Schema.DomainSchema DomainSchema { get; }
        public DomainSchemaLoadedEvent(string? filePath, Core.DomainSchema.Schema.DomainSchema domainSchema, DomainSchemaTreeViewModel treeViewModel)
        {
            FilePath = filePath;
            DomainSchema = domainSchema;
            TreeViewModel = treeViewModel;
        }
    }
}
