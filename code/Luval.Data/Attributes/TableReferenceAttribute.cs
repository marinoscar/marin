using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Attributes
{
    /// <summary>
    /// Specifies a table reference
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableReferenceAttribute : Attribute
    {

        public TableReferenceAttribute()
        {

        }
        public TableReferenceAttribute(string referenceColumnKey)
        {
            ReferenceColumnKey = referenceColumnKey;
        }

        public string ReferenceColumnKey { get; set; }

    }
}
