using Luval.Data.Attributes;
using Luval.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Data.Sql
{

    /// <summary>
    /// Provides an abstraction to the operations in a relational database
    /// </summary>
    public class Database
    {

        private readonly Func<IDbConnection> _connectionFactory;
        private readonly IDataRecordMapper _mapper;

        /// <summary>
        /// Creates a new instance of the database class
        /// </summary>
        /// <param name="connectionFactory">The function that will create a new instance of the <see cref="IDbConnection"/> object</param>
        public Database(Func<IDbConnection> connectionFactory): this (connectionFactory, new ReflectionDataRecordMapper())
        { 
        }

        /// <summary>
        /// Creates a new instance of the database class
        /// </summary>
        /// <param name="connectionFactory">The function that will create a new instance of the <see cref="IDbConnection"/> object</param>
        /// <param name="dataRecordMapper">A <see cref="IDataRecordMapper"/> implementation to convert <see cref="IDataRecord"/> into data entities</param>
        public Database(Func<IDbConnection> connectionFactory, IDataRecordMapper dataRecordMapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = dataRecordMapper;
        }

        /// <summary>
        /// Encapsulates the use of a <see cref="IDbConnection"/> object
        /// </summary>
        /// <param name="doSomething">Action that would use the <see cref="IDbConnection"/> object</param>
        public void WithConnection(Action<IDbConnection> doSomething)
        {
            using (var conn = _connectionFactory())
            {
                if (conn == null) throw new ArgumentNullException(nameof(conn), "Connection is not properly provided");
                doSomething(conn);
            }
        }

        /// <summary>
        /// Encapsulates the use of a <see cref="IDbTransaction"/> object
        /// </summary>
        /// <param name="doSomething">Action that would use the <see cref="IDbTransaction"/> object</param>
        public void WithTransaction(Action<IDbTransaction> doSomething)
        {
            WithConnection((conn) =>
            {
                try
                {
                    OpenConnection(conn);
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            doSomething(tran);
                            WorkTransaction(tran, () => { tran.Commit(); });
                        }
                        catch (Exception ex)
                        {
                            WorkTransaction(tran, () => { tran.Rollback(); });

                            if (!typeof(DatabaseException).IsAssignableFrom(ex.GetType()))
                                throw new DatabaseException("Failed to complete the transaction", ex);
                            throw ex;
                        }
                    }
                    CloseConnection(conn);
                }
                catch (Exception ex)
                {
                    if (!typeof(DatabaseException).IsAssignableFrom(ex.GetType()))
                        throw new DatabaseException("Failed to begin or rollback the transaction", ex);
                    throw ex;
                }
            });
        }

        /// <summary>
        /// Encapsulates the use of a <see cref="IDbCommand"/> object
        /// </summary>
        /// <param name="doSomething">Action that would use the <see cref="IDbCommand"/> object</param>
        public void WithCommand(Action<IDbCommand> doSomething)
        {

            WithTransaction((tran) =>
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    try
                    {
                        doSomething(cmd);
                    }
                    catch (Exception ex)
                    {
                        throw new DatabaseException("Unable to execute sql command.",
                            new DatabaseException(string.Format("COMMAND FAILURE: {0}", cmd.CommandText), ex));
                    }
                }
            });
        }

        /// <summary>
        /// Encapsulates the reading of a <see cref="IDataReader"/> and its <see cref="IDataRecord"/>
        /// </summary>
        /// <param name="cmd">The <see cref="IDbCommand"/> to use for the <see cref="IDataReader"/></param>
        /// <param name="behavior">The <see cref="CommandBehavior"/> to use</param>
        /// <param name="doSomething">Action that would be used for the <see cref="IDataRecord"/></param>
        public void WhileReading(IDbCommand cmd, CommandBehavior behavior, Action<IDataRecord> doSomething)
        {
            using (var reader = cmd.ExecuteReader(behavior))
            {
                while (reader.Read())
                {
                    doSomething(reader);
                }
            }
        }

        /// <summary>
        /// Encapsulates the reading of a <see cref="IDataReader"/> and its <see cref="IDataRecord"/>
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="behavior">The <see cref="CommandBehavior"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <param name="doSomething">Action that would be used for the <see cref="IDataRecord"/></param>
        public void WhileReading(string query, CommandType type, CommandBehavior behavior, IDataParameterCollection parameters, Action<IDataRecord> doSomething)
        {
            WithCommand((cmd) =>
            {
                PrepareCommand(cmd, query, type, 0, parameters);
                WhileReading(cmd, behavior, doSomething);
            });
        }

        /// <summary>
        /// Encapsulates the reading of a <see cref="IDataReader"/> and its <see cref="IDataRecord"/>
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="behavior">The <see cref="CommandBehavior"/> to use</param>
        /// <param name="doSomething">Action that would be used for the <see cref="IDataRecord"/></param>
        public void WhileReading(string query, CommandType type, CommandBehavior behavior, Action<IDataRecord> doSomething)
        {
            WhileReading(query, type, behavior, null, doSomething);
        }

        /// <summary>
        /// Encapsulates the reading of a <see cref="IDataReader"/> and its <see cref="IDataRecord"/>
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="doSomething">Action that would be used for the <see cref="IDataRecord"/></param>
        public void WhileReading(string query, CommandType type, Action<IDataRecord> doSomething)
        {
            WhileReading(query, type, CommandBehavior.Default, doSomething);
        }

        /// <summary>
        /// Encapsulates the reading of a <see cref="IDataReader"/> and its <see cref="IDataRecord"/>
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="doSomething">Action that would be used for the <see cref="IDataRecord"/></param>
        public void WhileReading(string query, Action<IDataRecord> doSomething)
        {
            WhileReading(query, CommandType.Text, doSomething);
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>The resulting scalar value of the command</returns>
        public object ExecuteScalar(string query, CommandType type, IDataParameterCollection parameters)
        {
            var result = default(object);
            WithCommand((cmd) =>
            {
                PrepareCommand(cmd, query, type, 0, parameters);
                result = cmd.ExecuteScalar();
            });
            return result;
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>The resulting scalar value of the command</returns>
        public async Task<object> ExecuteScalarAsync(string query, CommandType type, IDataParameterCollection parameters, CancellationToken cancellationToken)
        {
            return await new Task<object>(() => { return ExecuteScalar(query, type, parameters); }, cancellationToken);
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>The resulting scalar value of the command</returns>
        public async Task<object> ExecuteScalarAsync(string query, CommandType type, IDataParameterCollection parameters)
        {
            return await new Task<object>(() => { return ExecuteScalar(query, type, parameters); });
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>The resulting scalar value of the command</returns>
        public object ExecuteScalar(string query, CommandType type)
        {
            return ExecuteScalar(query, type, null);
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>The resulting scalar value of the command</returns>
        public async Task<object> ExecuteScalarAsync(string query, CommandType type, CancellationToken cancellationToken)
        {
            return await new Task<object>(() => { return ExecuteScalar(query, type, null); }, cancellationToken);
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>The resulting scalar value of the command</returns>
        public async Task<object> ExecuteScalarAsync(string query, CommandType type)
        {
            return await new Task<object>(() => { return ExecuteScalar(query, type, null); });
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>The resulting scalar value of the command</returns>
        public object ExecuteScalar(string query)
        {
            return ExecuteScalar(query, CommandType.Text);
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>The resulting scalar value of the command</returns>
        public async Task<object> ExecuteScalarAsync(string query, CancellationToken cancellationToken)
        {
            return await new Task<object>(() => { return ExecuteScalar(query, CommandType.Text); }, cancellationToken);
        }

        /// <summary>
        /// Gets the scalar value of a command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>The resulting scalar value of the command</returns>
        public async Task<object> ExecuteScalarAsync(string query)
        {
            return await new Task<object>(() => { return ExecuteScalar(query, CommandType.Text); });
        }


        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>number of records affected by the non query command</returns>
        public int ExecuteNonQuery(string query, CommandType type, IDataParameterCollection parameters)
        {
            var result = 0;
            WithCommand((cmd) =>
            {
                PrepareCommand(cmd, query, type, 0, parameters);
                result = cmd.ExecuteNonQuery();
            });
            return result;
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>number of records affected by the non query command</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, CommandType type, IDataParameterCollection parameters, CancellationToken cancellationToken)
        {
            return await new Task<int>(() => { return ExecuteNonQuery(query, type, parameters); }, cancellationToken);
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>number of records affected by the non query command</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, CommandType type, IDataParameterCollection parameters)
        {
            return await new Task<int>(() => { return ExecuteNonQuery(query, type, parameters); });
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>number of records affected by the non query command</returns>
        public int ExecuteNonQuery(string query, CommandType type)
        {
            return ExecuteNonQuery(query, type, null);
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>number of records affected by the non query command</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, CommandType type, CancellationToken cancellationToken)
        {
            return await new Task<int>(() => { return ExecuteNonQuery(query, type); }, cancellationToken);
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>number of records affected by the non query command</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, CommandType type)
        {
            return await new Task<int>(() => { return ExecuteNonQuery(query, type); });
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>number of records affected by the non query command</returns>
        public int ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, CommandType.Text);
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>number of records affected by the non query command</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, CancellationToken cancellationToken)
        {
            return await new Task<int>(() => { return ExecuteNonQuery(query, CommandType.Text); }, cancellationToken);
        }

        /// <summary>
        /// Gets the number of records affected by the non query command
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>number of records affected by the non query command</returns>
        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            return await new Task<int>(() => { return ExecuteNonQuery(query, CommandType.Text); });
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="cmd">The <see cref="IDbCommand"/> to execute</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public List<Dictionary<string, object>> ExecuteToDictionaryList(IDbCommand cmd)
        {
            var result = new List<Dictionary<string, object>>();
            WhileReading(cmd, CommandBehavior.CloseConnection, (row) =>
            {
                LoadRecordIntoDictionaryList(result, row);
            });
            return result;
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="cmd">The <see cref="IDbCommand"/> to execute</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(IDbCommand cmd, CancellationToken cancellationToken)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(cmd); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="cmd">The <see cref="IDbCommand"/> to execute</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(IDbCommand cmd)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(cmd); });
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public List<Dictionary<string, object>> ExecuteToDictionaryList(string query, CommandType type, IDataParameterCollection parameters)
        {
            var result = new List<Dictionary<string, object>>();
            WhileReading(query, type, CommandBehavior.Default, parameters, (row) =>
            {
                LoadRecordIntoDictionaryList(result, row);
            });
            return result;
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(string query, CommandType type, IDataParameterCollection parameters, CancellationToken cancellationToken)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(query, type, parameters); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(string query, CommandType type, IDataParameterCollection parameters)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(query, type, parameters); });
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public List<Dictionary<string, object>> ExecuteToDictionaryList(string query, CommandType type)
        {
            return ExecuteToDictionaryList(query, type, null);
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(string query, CommandType type, CancellationToken cancellationToken)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(query, type, null); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(string query, CommandType type)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(query, type, null); });
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public List<Dictionary<string, object>> ExecuteToDictionaryList(string query)
        {
            return ExecuteToDictionaryList(query, CommandType.Text);
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(string query, CancellationToken cancellationToken)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(query, CommandType.Text); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of records from a command into a <see cref="Dictionary{TKey, TValue}"/> of string for the field and object for the value
        /// </summary>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>A <see cref="List{T}"/> of records from a command into a <see cref="<Dictionary"/> of string for the field and object for the value</returns>
        public async Task<List<Dictionary<string, object>>> ExecuteToDictionaryListAsync(string query)
        {
            return await new Task<List<Dictionary<string, object>>>(() => { return ExecuteToDictionaryList(query, CommandType.Text); });
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="cmd">The <see cref="IDbCommand"/> to execute</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public List<T> ExecuteToEntityList<T>(IDbCommand cmd)
        {
            var result = new List<T>();
            WhileReading(cmd, CommandBehavior.CloseConnection, (r) =>
            {
                result.Add(_mapper.FromDataRecord<T>(r));
            });
            return result;
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="cmd">The <see cref="IDbCommand"/> to execute</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(IDbCommand cmd, CancellationToken cancellationToken)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(cmd); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="cmd">The <see cref="IDbCommand"/> to execute</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(IDbCommand cmd)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(cmd); });
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public List<T> ExecuteToEntityList<T>(string query, CommandType type, IDataParameterCollection parameters)
        {
            var result = new List<T>();
            WhileReading(query, type, CommandBehavior.CloseConnection, parameters, (r) =>
            {
                result.Add(_mapper.FromDataRecord<T>(r));
            });
            return result;
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{Object}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <param name="entityType">The data type to use to create the entity</param>
        /// <returns>A collection of entities into a <see cref="List{Object}"/></returns>
        public List<object> ExecuteToEntityList(string query, CommandType type, IDataParameterCollection parameters, Type entityType)
        {
            var result = new List<object>();
            WhileReading(query, type, CommandBehavior.CloseConnection, parameters, (r) =>
            {
                result.Add(_mapper.FromDataRecord(r, entityType));
            });
            return result;
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{Object}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A collection of entities into a <see cref="List{Object}"/></returns>
        public async Task<List<object>> ExecuteToEntityListAsync(string query, CommandType type, IDataParameterCollection parameters, Type entityType, CancellationToken cancellationToken)
        {
            return await new Task<List<object>>(() => { return ExecuteToEntityList(query, type, parameters, entityType); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{Object}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>A collection of entities into a <see cref="List{Object}"/></returns>
        public async Task<List<object>> ExecuteToEntityListAsync(string query, CommandType type, IDataParameterCollection parameters, Type entityType)
        {
            return await new Task<List<object>>(() => { return ExecuteToEntityList(query, type, parameters, entityType); });
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(string query, CommandType type, IDataParameterCollection parameters, CancellationToken cancellationToken)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(query, type, parameters); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="parameters">The <see cref="IDataParameterCollection"/> to use by the command</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(string query, CommandType type, IDataParameterCollection parameters)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(query, type, parameters); });
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public List<T> ExecuteToEntityList<T>(string query, CommandType type)
        {
            return ExecuteToEntityList<T>(query, type, null);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(string query, CommandType type, CancellationToken cancellationToken)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(query, type, null); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="type">The <see cref="CommandType"/> to use</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(string query, CommandType type)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(query, type, null); });
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public List<T> ExecuteToEntityList<T>(string query)
        {
            return ExecuteToEntityList<T>(query, CommandType.Text);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <param name="cancellationToken">Cancellation token for the task</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(string query, CancellationToken cancellationToken)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(query, CommandType.Text); }, cancellationToken);
        }

        /// <summary>
        /// Gets a collection of entities into a <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">The entity type to populate</typeparam>
        /// <param name="query">The query to execute in the command</param>
        /// <returns>A collection of entities into a <see cref="List{T}"/></returns>
        public async Task<List<T>> ExecuteToEntityListAsync<T>(string query)
        {
            return await new Task<List<T>>(() => { return ExecuteToEntityList<T>(query, CommandType.Text); });
        }

        /// <summary>
        /// Tries to see if the connection is valid
        /// </summary>
        /// <param name="errorMessage">Error message in case of a failure</param>
        /// <returns>True if the connection is valid otherwise false</returns>
        public bool TryConnection(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                WithConnection((c) => { c.Open(); c.Close(); });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to see if the connection is valid
        /// </summary>
        /// <returns>True if the connection is valid otherwise false</returns>
        public bool TryConnection()
        {
            var msg = string.Empty;
            return TryConnection(out msg);
        }


        #region Private Helper Methods


        private void LoadRecordIntoDictionaryList(List<Dictionary<string, object>> recordSet, IDataRecord row)
        {
            var record = new Dictionary<string, object>();
            for (int i = 0; i < row.FieldCount; i++)
            {
                record.Add(row.GetName(i), row.GetValue(i));
            }
            recordSet.Add(record);
        }

        private void OpenConnection(IDbConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Unable to open the connection", ex);
            }
        }

        private void CloseConnection(IDbConnection conn)
        {
            try
            {
                if (conn != null && conn.State != ConnectionState.Closed) conn.Close();
            }
            catch (Exception ex)
            {

                throw new DatabaseException("Unable to close the connection", ex);
            }
        }

        private void WorkTransaction(IDbTransaction tran, Action action)
        {
            if (tran == null || tran.Connection == null || tran.Connection.State == ConnectionState.Closed) return;
            action();
        }

        private void PrepareCommand(IDbCommand cmd, string query, CommandType type, int timeout, IDataParameterCollection parameters)
        {
            cmd.CommandText = query;
            cmd.CommandType = type;
            if (parameters != null && parameters.Count > 0)
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
            if (timeout > 0) cmd.CommandTimeout = timeout;
            else cmd.CommandTimeout = cmd.Connection.ConnectionTimeout;
        }

        #endregion
    }
}
