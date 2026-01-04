using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Audit
{
    public abstract class UnitOfWorkBase
    {
        protected readonly BusinessOperationContext _operationContext;

        protected UnitOfWorkBase(BusinessOperationContext operationContext)
        {
            _operationContext = operationContext;
        }

    }
}
