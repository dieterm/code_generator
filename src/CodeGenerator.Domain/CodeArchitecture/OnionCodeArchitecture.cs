using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class OnionCodeArchitecture : CodeArchitecture<IOnionArchitectureLayerFactory>
    {
        public const string APPLICATION_LAYER = "Application";
        public const string DOMAIN_LAYER = "Domain";
        public const string INFRASTRUCTURE_LAYER = "Infrastructure";
        public const string PRESENTATION_LAYER = "Presentation";

        public OnionCodeArchitecture() 
            : base("onion", "Onion Architecture")
        {
        
        }

        public IOnionArchitectureLayerFactory ApplicationLayer { get { return this.Layers.Single(l => l.LayerName == APPLICATION_LAYER); } }
        public IOnionArchitectureLayerFactory DomainLayer { get { return this.Layers.Single(l => l.LayerName == DOMAIN_LAYER); } }
        public IOnionArchitectureLayerFactory InfrastructureLayer { get { return this.Layers.Single(l => l.LayerName == INFRASTRUCTURE_LAYER); } }
        public IOnionArchitectureLayerFactory PresentationLayer { get { return this.Layers.Single(l => l.LayerName == PRESENTATION_LAYER); } }
    }
}
