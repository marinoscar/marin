using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Luval.Data.Sql
{
    public class DatabaseException : DbException
    {
        public DatabaseException() : this(null, null)
        {

        }

        public DatabaseException(string message) : this(message, null)
        {

        }

        public DatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
