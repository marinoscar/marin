using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Data.Interfaces
{
    /// <summary>
    /// Interface to execute a query command on a data storage
    /// </summary>
    public interface IQueryCommand
    {
        T Get<T>();
    }
}
