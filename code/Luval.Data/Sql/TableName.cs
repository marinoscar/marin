using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public class TableName
    {
        public TableName()
        {
        }

        public TableName(string name) : this (name, "dbo")
        {

        }

        public TableName(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        public string Name { get; set; }
        public string Schema { get; set; }

        public string GetFullTableName()
        {
            if (string.IsNullOrWhiteSpace(Schema)) return "[" + Name + "]";
            return string.Format("[{0}].[{1}]", Schema, Name);
        }
    }
}
