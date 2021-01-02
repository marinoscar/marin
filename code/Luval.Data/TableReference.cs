using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Luval.Data
{
    public class TableReference
    {
        public SqlColumnSchema SourceColumn { get; set; }
        public SqlTableSchema ReferenceTable { get; set; }
        public string ReferenceTableKey { get; set; }

        public Type EntityType { get; set; }
        public bool IsChild { get; set; }
    }
}
