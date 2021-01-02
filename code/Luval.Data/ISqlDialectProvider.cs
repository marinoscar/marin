using System;
using System.Data;
using System.Linq.Expressions;

namespace Luval.Data
{
    public interface ISqlDialectProvider
    {
        SqlTableSchema Schema { get; }
        string GetCreateCommand(IDataRecord record, bool incudeChildren);
        string GetReadCommand(IDataRecord record);
        string GetUpdateCommand(IDataRecord record);
        string GetDeleteCommand(IDataRecord record);
        string GetEntityQuery<TEntity>(Expression<Func<TEntity, bool>> expression);
        string GetReadAllCommand();
    }
}