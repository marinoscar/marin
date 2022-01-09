using Luval.Common;
using Luval.DataStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoalTracker.Entities
{
    public class BaseEntity : StringAuditedEntity
    {
        public BaseEntity()
        {
            Id = CodeGenerator.GetCode();
        }
    }
}
