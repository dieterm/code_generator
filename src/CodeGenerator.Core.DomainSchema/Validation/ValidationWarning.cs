namespace CodeGenerator.Core.Interfaces;

public class ValidationWarning
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Path { get; set; }
}