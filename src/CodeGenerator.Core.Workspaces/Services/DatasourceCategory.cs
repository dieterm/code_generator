using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Services
{
    /// <summary>
    /// Represents a category of datasource types with display metadata
    /// </summary>
    public class DatasourceCategory
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string IconKey { get; }

        private DatasourceCategory(string id, string displayName, string iconKey)
        {
            Id = id;
            DisplayName = displayName;
            IconKey = iconKey;
        }

        public static DatasourceCategory File { get; } = new("File", "File", "file");
        public static DatasourceCategory Directory { get; } = new("Directory", "Directory", "folder-open");
        public static DatasourceCategory RelationalDatabase { get; } = new("RelationalDatabase", "Relational Database", "database");
        public static DatasourceCategory NonRelationalDatabase { get; } = new("NonRelationalDatabase", "Non-Relational Database", "database");
        public static DatasourceCategory FileSystem { get; } = new("FileSystem", "File System", "folder-open");

        public override string ToString() => DisplayName;

        public override bool Equals(object? obj) => obj is DatasourceCategory other && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(DatasourceCategory? left, DatasourceCategory? right) =>
            ReferenceEquals(left, right) || (left is not null && left.Equals(right));

        public static bool operator !=(DatasourceCategory? left, DatasourceCategory? right) => !(left == right);
    }
}
