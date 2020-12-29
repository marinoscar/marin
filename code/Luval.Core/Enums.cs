/// <summary>
/// Type of result of the activity execution
/// </summary>
public enum ResultType
{
    /// <summary>
    /// The activity was completed succesfuly
    /// </summary>
    Success,
    /// <summary>
    /// The activity execution failed
    /// </summary>
    Failed,
    /// <summary>
    /// The activity execution was stopped by the caller
    /// </summary>
    Stopped
}