using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Attributes
{
    /// <summary>
    /// Specifies the name of a column for an entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : NameBaseAttribute
    {
        public ColumnNameAttribute(string name):base(name)
        {
        }
    }
}
