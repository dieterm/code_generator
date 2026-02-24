using System.ComponentModel;

namespace CodeGenerator.Shared.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged, IDisposable
    {
        event EventHandler? Disposed;
        
        void DisposeViewModel();
    }
}