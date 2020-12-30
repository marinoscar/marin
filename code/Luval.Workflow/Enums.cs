/// <summary>
/// The status of an activity
/// </summary>
public enum ActivityStatus
{
    /// <summary>
    /// Activity has been created
    /// </summary>
    Created,
    /// <summary>
    /// Activity is in progress
    /// </summary>
    InProgress,
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