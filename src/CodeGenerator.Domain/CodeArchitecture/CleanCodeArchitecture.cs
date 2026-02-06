namespace CodeGenerator.Domain.CodeArchitecture
{
    public class CleanCodeArchitecture : CodeArchitecture<ICleanArchitectureLayerFactory>
    {
        public const string ARCHITECTURE_ID = "clean";

        public const string ENTITIES_LAYER = "Entities";
        public const string USE_CASES_LAYER = "UseCases";
        public const string INTERFACE_ADAPTERS_LAYER = "InterfaceAdapters";
        public const string FRAMEWORKS_LAYER = "Frameworks";

        public CleanCodeArchitecture() 
            : base(ARCHITECTURE_ID, "Clean Architecture")
        {
        }

        public ICleanArchitectureLayerFactory EntitiesLayer { get { return this.Layers.Single(l => l.LayerName == ENTITIES_LAYER); } }
        public ICleanArchitectureLayerFactory UseCasesLayer { get { return this.Layers.Single(l => l.LayerName == USE_CASES_LAYER); } }
        public ICleanArchitectureLayerFactory InterfaceAdaptersLayer { get { return this.Layers.Single(l => l.LayerName == INTERFACE_ADAPTERS_LAYER); } }
        public ICleanArchitectureLayerFactory FrameworksLayer { get { return this.Layers.Single(l => l.LayerName == FRAMEWORKS_LAYER); } }
    }
}