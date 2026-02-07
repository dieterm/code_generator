namespace CodeGenerator.Shared.Controllers
{
    public interface IControllerBase
    {
        void Dispose();
        void Initialize();
    }
}