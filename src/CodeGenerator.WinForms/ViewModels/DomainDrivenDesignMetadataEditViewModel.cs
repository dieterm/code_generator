using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class DomainDrivenDesignMetadataEditViewModel : ValidationViewModelBase
{
    private DomainDrivenDesignMetadata? _metadata;
    private string _boundedContext = string.Empty;

    public DomainDrivenDesignMetadataEditViewModel()
    {
    }

    public DomainDrivenDesignMetadataEditViewModel(DomainDrivenDesignMetadata metadata)
    {
        LoadFromMetadata(metadata);
    }

    public string BoundedContext
    {
        get => _boundedContext;
        set => SetProperty(ref _boundedContext, value);
    }

    public void LoadFromMetadata(DomainDrivenDesignMetadata metadata)
    {
        _metadata = metadata;
        BoundedContext = metadata.BoundedContext ?? string.Empty;
    }

    public void UpdateMetadata()
    {
        if (_metadata != null)
        {
            _metadata.BoundedContext = string.IsNullOrWhiteSpace(BoundedContext) ? null : BoundedContext;
        }
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        return IsValid;
    }
}
