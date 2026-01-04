using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controllers;

/// <summary>
/// Controller for EntityEditorForm
/// </summary>
public class EntityEditorController : ControllerBase<EntityEditorViewModel>
{
    public EntityEditorController(EntityEditorViewModel viewModel) 
        : base(viewModel)
    {
    }

    /// <summary>
    /// Save the entity
    /// </summary>
    public bool Save()
    {
        if (!Validate())
        {
            return false;
        }

        // Additional business logic can be added here
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
