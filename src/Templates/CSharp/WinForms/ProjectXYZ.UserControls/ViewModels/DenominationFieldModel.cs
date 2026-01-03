using ProjectXYZ.Shared.Models;
using ProjectXYZ.Shared.ViewModels;

namespace ProjectXYZ.UserControls.ViewModels
{
    public class DenominationFieldModel : FieldViewModelBase
    {
        public SingleLineTextFieldModel Dutch { get; private set; }
        public SingleLineTextFieldModel French { get; private set; }
        public SingleLineTextFieldModel English { get; private set; }
        public SingleLineTextFieldModel German { get; private set; }
        public DenominationFieldModel()
        {
            Dutch = new SingleLineTextFieldModel();
            French = new SingleLineTextFieldModel();
            English = new SingleLineTextFieldModel();
            German = new SingleLineTextFieldModel();

            Dutch.PropertyChanged += AnyField_PropertyChanged;
            French.PropertyChanged += AnyField_PropertyChanged;
            English.PropertyChanged += AnyField_PropertyChanged;
            German.PropertyChanged += AnyField_PropertyChanged;
        }

        private void AnyField_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Value))
            {
                OnPropertyChanged(nameof(Value));
            }
        }

        public override T GetValue<T>()
        {
            if(typeof(T) != typeof(Denomination))
            {
                throw new InvalidCastException($"Cannot cast Denomination to {typeof(T).FullName}");
            }
            return (T)(object)new Denomination(Dutch.GetValue<string>(), French.GetValue<string>(), English.GetValue<string>(), German.GetValue<string>());
        }

        public override bool SetValue<T>(T value)
        {
            if (typeof(T) != typeof(Denomination))
            {
                throw new InvalidCastException($"Cannot cast Denomination to {typeof(T).FullName}");
            }
            var denomination = value as Denomination;
            return Dutch.SetValue<string>(denomination?.Nl) ||
                   French.SetValue<string>(denomination?.Fr) ||
                   English.SetValue<string>(denomination?.En) ||
                   German.SetValue<string>(denomination?.De);
        }
    }
}
