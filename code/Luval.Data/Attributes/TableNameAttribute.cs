using Luval.Data.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Attributes
{
    /// <summary>
    ///  Specifies the name of a table for an entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string name) : this(new TableName(name))
        {

        }

        public TableNameAttribute(string name, string schema) : this(new TableName(name, schema))
        {
        }

        public TableNameAttribute(TableName name)
        {
            TableName = name;
        }

        public TableName TableName { get; private set; }
    }
}
