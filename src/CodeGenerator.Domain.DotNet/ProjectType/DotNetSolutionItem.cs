using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class DotNetSolutionItem
    {
        /// <summary>
        /// absolute path of the file, including file extension
        /// </summary>
        public string FilePath { get;  }

        public DotNetSolutionItem(string filePath)
        {
            FilePath = filePath;
        }
    }
}
