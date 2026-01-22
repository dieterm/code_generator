using CodeGenerator.Core.DomainSchema.Schema;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.ViewModels
{
    public class DomainSchemaTreeViewModel : ViewModelBase
    {
        private DomainSchema _domainSchema;
        public DomainSchema DomainSchema
        {
            get => _domainSchema;
            set => SetProperty(ref _domainSchema, value);
        }
    }
}
