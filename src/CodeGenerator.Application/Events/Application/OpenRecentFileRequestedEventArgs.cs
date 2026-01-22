using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Events.Application
{
    public class OpenRecentFileRequestedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public OpenRecentFileRequestedEventArgs(string filePath)
        {
            FilePath = filePath;
        }
    }
}
