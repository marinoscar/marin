using Luval.Data;
using Luval.Data.Attributes;
using Luval.Web.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Marin
{
    public static class TestSink
    {
        public static void GetSql()
        {

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
            Invoice = new Invoice(0) { Id = invoiceId };
            Id = Guid.NewGuid().ToString();
            Quantity = new Random().Next(1, 15);
            Item = GetItem();
            Price = new Random().Next(12, 78);
            Invoice = new Invoice(0) { Id = invoiceId };
        }

        public string Id { get; set; }
        [TableReference]
        public Invoice Invoice { get; set; }
        public decimal Quantity { get; set; }
        public string Item { get; set; }
        public double Price { get; set; }

        private string GetItem()
        {
            return (new[] { "CELL PHONE", "CUP", "COMPUTER", "PROJECTOR", "LAMP", "CHARGER", "NOTEBOOK", "DESK", "TELEVISION", "KEYBOARD", "COUCH" })[new Random().Next(0, 10)];
        }
    }

    public class Product { public string Id { get; set; } public string Name { get; set; } }
    public class ProductSerial
    {
        public string Id { get; set; }
        [TableReference]
        public Product Product { get; set; }
        public string Name { get; set; }
    }

}
