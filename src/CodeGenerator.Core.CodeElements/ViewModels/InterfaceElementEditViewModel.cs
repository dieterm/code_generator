using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class InterfaceElementEditViewModel : CodeElementEditViewModel
{
    private InterfaceElementArtifact? _artifact;

    public InterfaceElementEditViewModel()
    {
    }

    public InterfaceElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        // No additional fields beyond base
    }
}
