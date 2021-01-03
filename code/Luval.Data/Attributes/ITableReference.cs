namespace Luval.Data.Attributes
{
    public interface ITableReference
    {
        string ParentColumnKey { get; set; }
        string ReferenceTableKey { get; set; }
    }
}