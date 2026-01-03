namespace ProjectXYZ.Shared.Audit;

public class BusinessOperationContext
{
    /// <summary>
    /// eg. "CREATE_ORGANIZATION"
    /// </summary>
    public string OperationType { get; set; }
    /// <summary>
    /// eg. "Organization"
    /// </summary>
    public string EntityType { get; set; }
    /// <summary>
    /// User identifier performing the operation
    /// </summary>
    public string UserId { get; set; }

    public long OperationId { get; set; }

    public void Reset()
    {
        OperationType = string.Empty;
        EntityType = string.Empty;
        UserId = string.Empty;
        OperationId = 0;
    }
}