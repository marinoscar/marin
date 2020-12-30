using System;

namespace Luval.Workflow
{
    public interface IActivityName
    {
        Type ActivityType { get; }
        string DisplayName { get;  }
        string Id { get; }
    }
}