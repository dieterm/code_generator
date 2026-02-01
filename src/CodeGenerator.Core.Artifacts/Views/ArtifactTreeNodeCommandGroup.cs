using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts
{
    public class ArtifactTreeNodeCommandGroup
    {
        /// <summary>
        /// Group for rename commands
        /// </summary>
        public const string COMMAND_GROUP_RENAME = "Rename";
        /// <summary>
        /// Default group for commands
        /// </summary>
        public const string COMMAND_GROUP_DEFAULT = "Default";
        /// <summary>
        /// Group for cut/copy/paste commands
        /// </summary>
        public const string COMMAND_GROUP_EDIT = "Edit";
        /// <summary>
        /// Group for delete commands
        /// </summary>
        public const string COMMAND_GROUP_DELETE = "Delete";
        /// <summary>
        /// Group for manage commands. 
        /// E.g., manage templates, manage connections, etc.
        /// </summary>
        public const string COMMAND_GROUP_MANAGE = "Manage";

        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; init; }

        public int SortOrder { get; init; }
        public List<ArtifactTreeNodeCommand> Commands { get; } = new List<ArtifactTreeNodeCommand>();

        public ArtifactTreeNodeCommandGroup(string groupName)
        {
            Name = groupName;
            SortOrder = GetSortOrder(groupName);
        }
        private static readonly Dictionary<string, int> _sortOrderCache = new();
        private static int _customGroupSortOrderCounter = 101;
        static ArtifactTreeNodeCommandGroup()
        {
            _sortOrderCache[COMMAND_GROUP_DEFAULT] = 0;
            _sortOrderCache[COMMAND_GROUP_MANAGE] = 100;
            // custom groups go from 101 to 799
            _sortOrderCache[COMMAND_GROUP_RENAME] = 800;
            _sortOrderCache[COMMAND_GROUP_EDIT] = 900;
            _sortOrderCache[COMMAND_GROUP_DELETE] = 1000;
        }
        public static int GetSortOrder(string groupName)
        {
            if (_sortOrderCache.TryGetValue(groupName, out int sortOrder))
            {
                return sortOrder;
            }
            else
            {
                // Assign a new sort order for custom groups
                _sortOrderCache[groupName] = _customGroupSortOrderCounter++;
                return _sortOrderCache[groupName];
            }
        }
    }
}
