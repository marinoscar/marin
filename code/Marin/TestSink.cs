using Luval.BlobStorage;
using Luval.Data;
using Luval.Data.Attributes;
using Luval.Data.Sql;
using Luval.Web.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marin
{
    public static class TestSink
    {
        private static string SqlConn = ConfigurationManager.AppSettings["Db.UserProfile"];
        public static void TestUser()
        {
            var repo = new ApplicationUserRepository(new DbUnitOfWorkFactory(new SqlServerDatabase(SqlConn), new SqlServerDialectFactory()));
            var t = repo.GetUserByMailAsync("oscar.marin.saenz@outlook.com");
            t.Wait();
        }

        public static void TestSql()
        {
            var db = new SqlServerDatabase(SqlConn);
            var id = Guid.NewGuid().ToString();
            db.ExecuteNonQuery(string.Format("INSERT INTO BlogPost (Id, Title, Slug, Content, UtcCreatedOn, UtcUpdatedOn, CreatedByUserId, UpdatedByUserId) VALUES ('{0}', 'Sample', 'sample', '', GETDATE(), GETDATE(), 'MARIN', 'MARIN');", id));
        }

        public static void GetFiles()
        {
            var cnn = "";
            var storage = new AzureBlobStorage(cnn, "misc");
            var t = storage.GetBlobsAsync("Sample/", CancellationToken.None);
            t.Wait();
            var info = t.Result;
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
