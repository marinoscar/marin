using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FileSync.Core
{
    public class LocalSyncException : Exception
    {
        public LocalSyncException() : this(null, null)
        {

        }

        public LocalSyncException(string message) : this(message, null)
        {

        }

        public LocalSyncException(string messaage, Exception innerException) : base(messaage, innerException)
        {

        }
    }
}
