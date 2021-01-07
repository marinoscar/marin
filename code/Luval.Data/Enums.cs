/// <summary>
/// Provides the action to be enforced on a data record
/// </summary>
public enum DataAction { Insert, Update, Delete }

/// <summary>
/// Determines how entities are loaded by the adapter
/// </summary>
public enum EntityLoadMode
{
    /// <summary>
    /// Only loads the fields on the entity and not the related references
    /// </summary>
    Lazy,
    /// <summary>
    /// Loads all of the entity fields, plus the references
    /// </summary>
    Eager
}

/// <summary>
/// Indicates the state of an entity
/// </summary>
public enum EntityState
{
    New, Modified, Deleted
}