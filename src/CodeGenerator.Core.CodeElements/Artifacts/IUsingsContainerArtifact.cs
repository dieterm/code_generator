namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public interface IUsingsContainerArtifact
    {
        void AddNewUsing();
        void RemoveUsing(UsingElementArtifact artifact);
    }
}