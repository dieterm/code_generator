using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Controllers
{
    public abstract class ControllerBase : IDisposable, IControllerBase
    {
        public abstract void Initialize();
        /// <summary>
        /// Override in derived classes 
        /// </summary>
        public abstract void Dispose();
    }
}
