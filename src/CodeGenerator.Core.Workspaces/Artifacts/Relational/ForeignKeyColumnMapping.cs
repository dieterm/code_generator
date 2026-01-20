using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using System.Text.Json.Serialization;

namespace CodeGenerator.Core.Workspaces.Artifacts.Relational
{
    /// <summary>
    /// Represents a column mapping in a foreign key
    /// </summary>
    public class ForeignKeyColumnMapping
    {
        public ForeignKeyColumnMapping()
        {
            SourceColumnId = string.Empty;
            ReferencedColumnId = string.Empty;
        }

        public ForeignKeyColumnMapping(string sourceColumnId, string referencedColumnId)
        {
            SourceColumnId = sourceColumnId;
            ReferencedColumnId = referencedColumnId;
        }

        /// <summary>
        /// The Id of the source column (in the table that owns the foreign key)
        /// </summary>
        public string SourceColumnId { get; set; }
        [JsonIgnore]
        public ColumnArtifact? SourceColumn { get { return string.IsNullOrWhiteSpace(SourceColumnId) ? null : ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>().CurrentWorkspace?.FindDescendantById<ColumnArtifact>(SourceColumnId); } }

        /// <summary>
        /// The Id of the referenced column (in the referenced table)
        /// </summary>
        public string ReferencedColumnId { get; set; }
        [JsonIgnore]
        public ColumnArtifact? ReferencedColumn { get { return string.IsNullOrWhiteSpace(ReferencedColumnId) ? null : ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>().CurrentWorkspace?.FindDescendantById<ColumnArtifact>(ReferencedColumnId); } }
    }
}
