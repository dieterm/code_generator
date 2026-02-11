using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.DotNet.DesignTools.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.UserControls.Views
{
    public partial class FieldCollection : UserControl, IView<FieldCollectionModel>
    {
        private FieldCollectionModel? _fieldCollectionModel;
        public FieldCollection()
        {
            InitializeComponent();
        }

        public void BindViewModel(FieldCollectionModel fieldCollection)
        {
            var viewFactory = ServiceProviderHolder.GetRequiredService<IViewFactory>();
            if (_fieldCollectionModel != null)
            {
                _fieldCollectionModel.FieldModels.CollectionChanged -= FieldModels_CollectionChanged;
                _fieldCollectionModel = null;
            }
            
            Controls.Clear();

            _fieldCollectionModel = fieldCollection;
            
            foreach (var fieldModel in fieldCollection.FieldModels)
            {
                var view = viewFactory.CreateView(fieldModel);

                if (view is UserControl detailsControl)
                {
                    detailsControl.Tag = fieldModel;
                    Controls.Add(detailsControl);
                    detailsControl.Dock = DockStyle.Fill;
                }
            }

            _fieldCollectionModel.FieldModels.CollectionChanged += FieldModels_CollectionChanged;
        }

        private void FieldModels_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var viewFactory = ServiceProviderHolder.GetRequiredService<IViewFactory>();

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    e.NewItems?.OfType<FieldViewModelBase>().ToList().ForEach(fieldModel =>
                    {
                        var view = viewFactory.CreateView(fieldModel);
                        if (view is UserControl detailsControl)
                        {
                            detailsControl.Tag = fieldModel;
                            Controls.Add(detailsControl);
                            detailsControl.Dock = DockStyle.Fill;
                        }
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    e.OldItems?.OfType<FieldViewModelBase>().ToList().ForEach(fieldModel =>
                    {
                        var controlToRemove = Controls.OfType<UserControl>().FirstOrDefault(c => c.Tag == fieldModel);
                        if (controlToRemove != null)
                        {
                            Controls.Remove(controlToRemove);
                            controlToRemove.Dispose();
                        }
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
            // Reorder controls to match the order of the field models
            foreach (var fieldModel in _fieldCollectionModel.FieldModels)
            {
                var control = Controls.OfType<UserControl>().FirstOrDefault(c => c.Tag == fieldModel);
                if (control != null)
                {
                    Controls.SetChildIndex(control, _fieldCollectionModel.FieldModels.IndexOf(fieldModel));
                }
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((FieldCollectionModel)(object)viewModel);
        }
    }
}
