using System;

namespace Luval.Core
{
    public interface IActivityName
    {
        Type ActivityType { get; }
        string DisplayName { get;  }
        string Id { get; }
    }
}