namespace CodeGenerator.Shared;
/// <summary>
/// Holds the currently authenticated user for the application session
/// </summary>
public static class CurrentUser
{
    /// <summary>
    /// Gets or sets the currently authenticated user
    /// </summary>
    public static object? User { get; set; }

    /// <summary>
    /// Gets whether a user is currently authenticated
    /// </summary>
    public static bool IsAuthenticated => User != null;

    /// <summary>
    /// Clears the current user (logout)
    /// </summary>
    public static void Logout()
    {
        User = null;
    }
}
