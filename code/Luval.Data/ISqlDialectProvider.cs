using System.Data;

namespace Luval.Data
{
    public interface ISqlDialectProvider
    {
        SqlTableSchema Schema { get; }
        string GetCreateCommand(IDataRecord record, bool incudeChildren);
        string GetReadCommand(IDataRecord record);
        string GetUpdateCommand(IDataRecord record);
        string GetDeleteCommand(IDataRecord record);
        string GetReadAllCommand();
    }
}