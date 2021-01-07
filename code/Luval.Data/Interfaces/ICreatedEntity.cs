using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    public interface ICreatedEntity
    {
        DateTime UtcCreatedOn { get; set; }
        string CreatedByUserId { get; set; }

    }
}
