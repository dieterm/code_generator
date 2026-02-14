using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class DotNetSolutionFolder
    {
        public string Id { get; }
        public string Name { get; }
        public bool IsRoot { get { return Name == null; } }
        public string SolutionTypeGuid { get { return "2150E333-8FDC-42A3-9474-1A3956D46DE8"; } }
        public DotNetSolutionFolder()
            : this(null, null)
        {
            
        }
        public DotNetSolutionFolder(string name, DotNetSolutionFolder? parentFolder)
            : this(Guid.NewGuid().ToString().ToUpper(), name, parentFolder)
        {

        }

        private DotNetSolutionFolder(string id, string name, DotNetSolutionFolder? parentFolder)
        {
            Id = id;
            Name = name;
            ParentFolder = parentFolder;
        }


        public DotNetSolutionFolder? ParentFolder { get; }

        public List<DotNetSolutionFolder> SubFolders { get; } = new List<DotNetSolutionFolder>();

        public List<DotNetProjectReference> Projects { get; } = new List<DotNetProjectReference>();

        public List<DotNetSolutionItem> Items { get; } = new List<DotNetSolutionItem>();

        public DotNetSolutionFolder CreateSubFolder(string name)
        {
            var subFolder = new DotNetSolutionFolder(name, this);
            SubFolders.Add(subFolder);
            return subFolder;
        }
    }
}
