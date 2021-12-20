using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common.Security
{
    public class SafeItem : BaseAuditEntity
    {
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
    }
}
