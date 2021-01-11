using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Sql
{
    public class SqlQueryCommand : IQueryCommand
    {
        private string _query;

        public SqlQueryCommand(string sqlQuery)
        {
            _query = sqlQuery;
        }

        public T Get<T>()
        {
            return (T)Convert.ChangeType(_query, typeof(T));
        }
    }
}
