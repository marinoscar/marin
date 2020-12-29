using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Core
{
    public class ActivityResult : IActivityResult
    {
        public ResultType ResultType { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public ActivityInformation Information { get; set; }

        public T GetResultValue<T>(string keyName)
        {
            return default(T);
        }
    }
}
