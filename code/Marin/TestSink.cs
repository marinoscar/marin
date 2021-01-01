using Luval.Data;
using Luval.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marin
{
    public static class TestSink
    {
        public static void GetSql()
        {
            var inv = new Invoice(5);
            var sqlDialect = new SqlServerDialectProvider(SqlTableSchema.Create(typeof(Invoice)));
            var sqlResult = sqlDialect.GetCreateCommand(DictionaryDataRecord.FromEntity(inv), true);
            Console.WriteLine(sqlResult);
        }
    }

    public class Invoice
    {

        public Invoice(int detailCount)
        {
            Id = Guid.NewGuid().ToString();
            Date = DateTime.Now;
            InvoiceDetails = new List<InvoiceDetail>();
            for (int i = 0; i < detailCount; i++)
            {
                InvoiceDetails.Add(new InvoiceDetail(Id));
            }
        }

        public string Id { get; set; }
        public DateTime Date { get; set; }
        [TableReference]
        public List<InvoiceDetail> InvoiceDetails { get; set; }
    }

    public class InvoiceDetail
    {

        public InvoiceDetail(string invoiceId)
        {
            InvoiceId = invoiceId;
            Id = Guid.NewGuid().ToString();
            Quantity = new Random().Next(1, 15);
            Item = GetItem();
            Price = new Random().Next(12, 78);
        }

        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public decimal Quantity { get; set; }
        public string Item { get; set; }
        public double Price { get; set; }

        private string GetItem()
        {
            return (new[] { "CELL PHONE", "CUP", "COMPUTER", "PROJECTOR", "LAMP", "CHARGER", "NOTEBOOK", "DESK", "TELEVISION", "KEYBOARD", "COUCH" })[new Random().Next(0, 10)];
        }
    }
}
