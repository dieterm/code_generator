using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.UserControls.ViewModels
{
    public class IntegerFieldModel : FieldViewModelBase
    {
        private int? _minimum;
        private int? _maximum;

        /// <summary>
        /// Gets or sets the minimum value
        /// </summary>
        public int? Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }

        /// <summary>
        /// Gets or sets the maximum value
        /// </summary>
        public int? Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }
    }
}
