namespace CodeGenerator.Domain.CodeArchitecture
{
    public class NTierCodeArchitecture : CodeArchitecture<INTierArchitectureLayerFactory>
    {
        public const string ARCHITECTURE_ID = "ntier";

        public const string PRESENTATION_LAYER = "Presentation";
        public const string BUSINESS_LAYER = "Business";
        public const string DATA_ACCESS_LAYER = "DataAccess";

        public NTierCodeArchitecture() 
            : base(ARCHITECTURE_ID, "N-Tier Architecture")
        {
        }

        public INTierArchitectureLayerFactory PresentationLayer { get { return this.Layers.Single(l => l.LayerName == PRESENTATION_LAYER); } }
        public INTierArchitectureLayerFactory BusinessLayer { get { return this.Layers.Single(l => l.LayerName == BUSINESS_LAYER); } }
        public INTierArchitectureLayerFactory DataAccessLayer { get { return this.Layers.Single(l => l.LayerName == DATA_ACCESS_LAYER); } }
    }
}