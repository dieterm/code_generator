using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views.Domains
{
    /// <summary>
    /// View for editing EntityEditViewArtifact properties
    /// </summary>
    public partial class EntityEditViewEditView : UserControl, IView<EntityEditViewEditViewModel>
    {
        private EntityEditViewEditViewModel? _viewModel;

        public EntityEditViewEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(EntityEditViewEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.PreviewChanged -= ViewModel_PreviewChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            cmbEntityState.BindViewModel(_viewModel.EntityStateField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.PreviewChanged += ViewModel_PreviewChanged;
            
            RefreshPreviewPanel();
        }

        private void ViewModel_PreviewChanged(object? sender, EventArgs e)
        {
            // Ensure we're on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshPreviewPanel));
            }
            else
            {
                RefreshPreviewPanel();
            }
        }

        private void RefreshPreviewPanel()
        {
            previewPanel.SuspendLayout();
            previewPanel.Controls.Clear();

            if (_viewModel == null || _viewModel.PreviewFields.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No fields defined. Add fields via the context menu on the Edit View node.",
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    ForeColor = SystemColors.GrayText,
                    Padding = new Padding(10)
                };
                previewPanel.Controls.Add(emptyLabel);
                previewPanel.ResumeLayout();
                return;
            }

            var previewContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                Padding = new Padding(5)
            };
            previewContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int rowIndex = 0;
            foreach (var field in _viewModel.PreviewFields)
            {
                previewContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                var control = CreatePreviewControl(field);
                if (control != null)
                {
                    control.Dock = DockStyle.Top;
                    control.Enabled = !field.IsReadOnly;
                    previewContainer.Controls.Add(control, 0, rowIndex);
                    rowIndex++;
                }
            }

            previewContainer.RowCount = rowIndex;
            previewPanel.Controls.Add(previewContainer);
            previewPanel.ResumeLayout();
        }

        private Control? CreatePreviewControl(EntityEditViewFieldPreviewModel field)
        {
            var labelText = field.IsRequired ? $"{field.Label} *" : field.Label;

            switch (field.ControlType)
            {
                case DataFieldControlType.SingleLineTextField:
                case DataFieldControlType.EmailField:
                case DataFieldControlType.PhoneField:
                case DataFieldControlType.UrlField:
                case DataFieldControlType.PasswordField:
                    var textField = new SingleLineTextField
                    {
                        Label = labelText,
                        Height = 50
                    };
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(textField, field.Tooltip);
                    }
                    return textField;

                case DataFieldControlType.MultiLineTextField:
                case DataFieldControlType.RichTextField:
                    var multiLinePanel = new Panel { Height = 100 };
                    var multiLineLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var multiLineText = new TextBox
                    {
                        Multiline = true,
                        Dock = DockStyle.Fill,
                        ScrollBars = ScrollBars.Vertical
                    };
                    multiLinePanel.Controls.Add(multiLineText);
                    multiLinePanel.Controls.Add(multiLineLabel);
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(multiLineText, field.Tooltip);
                    }
                    return multiLinePanel;

                case DataFieldControlType.IntegerField:
                    var intField = new IntegerField
                    {
                        Label = labelText,
                        Height = 50
                    };
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(intField, field.Tooltip);
                    }
                    return intField;

                case DataFieldControlType.DecimalField:
                    var decimalPanel = new Panel { Height = 50 };
                    var decimalLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var decimalInput = new NumericUpDown
                    {
                        DecimalPlaces = 2,
                        Dock = DockStyle.Top
                    };
                    decimalPanel.Controls.Add(decimalInput);
                    decimalPanel.Controls.Add(decimalLabel);
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(decimalInput, field.Tooltip);
                    }
                    return decimalPanel;

                case DataFieldControlType.BooleanField:
                    var boolField = new BooleanField
                    {
                        Label = labelText,
                        Height = 30
                    };
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(boolField, field.Tooltip);
                    }
                    return boolField;

                case DataFieldControlType.DateField:
                case DataFieldControlType.DateTimeField:
                    var datePanel = new Panel { Height = 50 };
                    var dateLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var datePicker = new DateTimePicker
                    {
                        Dock = DockStyle.Top,
                        Format = field.ControlType == DataFieldControlType.DateField 
                            ? DateTimePickerFormat.Short 
                            : DateTimePickerFormat.Long
                    };
                    datePanel.Controls.Add(datePicker);
                    datePanel.Controls.Add(dateLabel);
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(datePicker, field.Tooltip);
                    }
                    return datePanel;

                case DataFieldControlType.TimeField:
                    var timePanel = new Panel { Height = 50 };
                    var timeLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var timePicker = new DateTimePicker
                    {
                        Dock = DockStyle.Top,
                        Format = DateTimePickerFormat.Time,
                        ShowUpDown = true
                    };
                    timePanel.Controls.Add(timePicker);
                    timePanel.Controls.Add(timeLabel);
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(timePicker, field.Tooltip);
                    }
                    return timePanel;

                case DataFieldControlType.ComboboxField:
                    var comboField = new ComboboxField
                    {
                        Label = labelText,
                        Height = 50
                    };
                    if (!string.IsNullOrEmpty(field.Tooltip))
                    {
                        var toolTip = new ToolTip();
                        toolTip.SetToolTip(comboField, field.Tooltip);
                    }
                    return comboField;

                case DataFieldControlType.RadioButtonField:
                    var radioPanel = new GroupBox
                    {
                        Text = labelText,
                        Height = 60,
                        Dock = DockStyle.Top
                    };
                    var radio1 = new RadioButton { Text = "Option 1", Location = new Point(10, 20) };
                    var radio2 = new RadioButton { Text = "Option 2", Location = new Point(100, 20) };
                    radioPanel.Controls.Add(radio1);
                    radioPanel.Controls.Add(radio2);
                    return radioPanel;

                case DataFieldControlType.FileField:
                    var filePanel = new Panel { Height = 50 };
                    var fileLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var fileButton = new Button
                    {
                        Text = "Browse...",
                        Dock = DockStyle.Top
                    };
                    filePanel.Controls.Add(fileButton);
                    filePanel.Controls.Add(fileLabel);
                    return filePanel;

                case DataFieldControlType.ImageField:
                    var imagePanel = new Panel { Height = 80 };
                    var imageLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var imagePlaceholder = new PictureBox
                    {
                        Height = 50,
                        Dock = DockStyle.Top,
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.CenterImage
                    };
                    imagePanel.Controls.Add(imagePlaceholder);
                    imagePanel.Controls.Add(imageLabel);
                    return imagePanel;

                case DataFieldControlType.ColorField:
                    var colorPanel = new Panel { Height = 50 };
                    var colorLabel = new Label
                    {
                        Text = labelText,
                        Dock = DockStyle.Top,
                        AutoSize = true
                    };
                    var colorButton = new Button
                    {
                        Text = "Select Color",
                        Dock = DockStyle.Top,
                        BackColor = Color.White
                    };
                    colorPanel.Controls.Add(colorButton);
                    colorPanel.Controls.Add(colorLabel);
                    return colorPanel;

                default:
                    var defaultField = new SingleLineTextField
                    {
                        Label = labelText,
                        Height = 50
                    };
                    return defaultField;
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((EntityEditViewEditViewModel)(object)viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    _viewModel.PreviewChanged -= ViewModel_PreviewChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
