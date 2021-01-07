using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface IUpdatedEntity
    {
        DateTime UtcUpdatedOn { get; set; }
        string UpdatedByUserId { get; set; }
    }
}
