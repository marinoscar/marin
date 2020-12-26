using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Luval.Data
{
    public class EntityAdapter
    {
        public EntityAdapter(Database database, ISqlDialectProvider dialectProvider)
        {
            Database = database;
            DialectProvider = dialectProvider;
        }

        public ISqlDialectProvider DialectProvider { get; private set; }
        public Database Database { get; private set; }

        private int DoAction(IDataRecord record, Func<IDataRecord, string> action)
        {
            return Database.ExecuteNonQuery(action(record));
        }

        public int Insert(object entity)
        {
            return Insert(DictionaryDataRecord.FromEntity(entity));
        }

        public int Insert(IDataRecord record)
        {
            return DoAction(record, DialectProvider.GetCreateCommand);
        }

        public int Update(object entity)
        {
            return Update(DictionaryDataRecord.FromEntity(entity));
        }


        public int Update(IDataRecord record)
        {
            return DoAction(record, DialectProvider.GetUpdateCommand);
        }

        public int Delete(IDataRecord record)
        {
            return DoAction(record, DialectProvider.GetDeleteCommand);
        }

        public int Delete(object entity)
        {
            return Delete(DictionaryDataRecord.FromEntity(entity));
        }

        public T Read<T>(IDataRecord record)
        {
            return Database.ExecuteToEntityList<T>(DialectProvider.GetReadCommand(record)).SingleOrDefault();
        }

        public T Read<T>(object entity)
        {
            return Read<T>(DictionaryDataRecord.FromEntity(entity));
        }

        public int ExecuteInTransaction(IEnumerable<DataRecordAction> records)
        {
            var count = 0;
            Database.WithCommand((cmd) => {
                cmd.CommandTimeout = cmd.Connection.ConnectionTimeout;
                foreach(var record in records)
                {
                    switch (record.Action)
                    {
                        case DataAction.Insert:
                            cmd.CommandText = DialectProvider.GetCreateCommand(record.Record);
                            count += cmd.ExecuteNonQuery();
                            break;
                        case DataAction.Update:
                            cmd.CommandText = DialectProvider.GetUpdateCommand(record.Record);
                            count += cmd.ExecuteNonQuery();
                            break;
                        case DataAction.Delete:
                            cmd.CommandText = DialectProvider.GetDeleteCommand(record.Record);
                            count += cmd.ExecuteNonQuery();
                            break;
                    }
                }
            });
            return count;
        }


    }
}
