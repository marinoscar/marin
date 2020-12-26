using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Luval.Data
{
    public class DataRecordAction
    {
        public DataAction Action { get; set; }
        public IDataRecord Record { get; set; }
    }
}
