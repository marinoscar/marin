using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Luval.Data
{
    /// <summary>
    /// Provides an abstraction to the operations in a relational Sql Server database extending the implementation of the 
    /// <see cref="Database"/> class
    /// </summary>
    public class SqlServerDatabase : Database
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SqlServerDatabase" class with the provided connection string/>
        /// </summary>
        /// <param name="connectionString">The parameters to create the Sql Server connection</param>
        public SqlServerDatabase(string connectionString) : base((() => { return new SqlConnection(connectionString); }))
        {
        }
    }
}
