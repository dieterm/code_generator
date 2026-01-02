namespace CodeGenerator.WinForms.Controllers;

/// <summary>
/// Controller for PropertyEditorForm
/// </summary>
public class PropertyEditorController : ControllerBase<ViewModels.PropertyEditorViewModel>
{
    public PropertyEditorController(ViewModels.PropertyEditorViewModel viewModel) 
        : base(viewModel)
    {
    }

    /// <summary>
    /// Save the property
    /// </summary>
    public bool Save()
    {
        if (!Validate())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get the first validation error message
    /// </summary>
    public string? GetFirstValidationError()
    {
        return ViewModel.ValidationErrors.Values.FirstOrDefault();
    }
}
