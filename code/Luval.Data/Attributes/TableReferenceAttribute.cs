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

        public TableReferenceAttribute() : this(null, null)
        {

        }
        public TableReferenceAttribute(string referenceColumnKey) : this(null, "Id")
        {

        }
        public TableReferenceAttribute(string referenceColumnKey, string parentColumnKey)
        {
            ReferenceTableKey = referenceColumnKey;
            ParentColumnKey = parentColumnKey;
        }

        public string ReferenceTableKey { get; set; }
        public string ParentColumnKey { get; set; }

    }
}
