namespace CodeGenerator.Domain.CodeArchitecture
{
    public class HexagonCodeArchitecture : CodeArchitecture<IHexagonArchitectureLayerFactory>
    {
        public const string ARCHITECTURE_ID = "hexagon";

        public const string CORE_LAYER = "Core";
        public const string PORTS_LAYER = "Ports";
        public const string ADAPTERS_LAYER = "Adapters";

        public HexagonCodeArchitecture() 
            : base(ARCHITECTURE_ID, "Hexagonal Architecture")
        {
        }

        public IHexagonArchitectureLayerFactory CoreLayer { get { return this.Layers.Single(l => l.LayerName == CORE_LAYER); } }
        public IHexagonArchitectureLayerFactory PortsLayer { get { return this.Layers.Single(l => l.LayerName == PORTS_LAYER); } }
        public IHexagonArchitectureLayerFactory AdaptersLayer { get { return this.Layers.Single(l => l.LayerName == ADAPTERS_LAYER); } }
    }
}