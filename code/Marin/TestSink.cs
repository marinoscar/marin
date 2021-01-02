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
            var inv = new Invoice(5);
            var adapter = new SqlEntityAdapter<Invoice>(new SqlServerDatabase(ConfigurationManager.AppSettings["Db.UserProfile"]), new SqlServerDialectFactory());
            //adapter.Read(i => i.Id == "Oscar Marin", o => o.Id, true);
        }

        public static void ValidateUser()
        {
            var claims = new List<Claim>(new[] { 
                new Claim(ClaimTypes.Email, "oscar.marin.saenz@outlook.com"),
                new Claim(ClaimTypes.Name, "Oscar"),
                new Claim(ClaimTypes.Surname, "Marin"),
                new Claim(ClaimTypes.GivenName, "Oscar Marin"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            });
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var repo = new ApplicationUserRepository(new SqlEntityAdapterFactory(new SqlServerDatabase(ConfigurationManager.AppSettings["Db.UserProfile"]), new SqlServerDialectFactory()) );
            var task = repo.ValidateAndUpdateUserAccess(claimsPrincipal);
            task.Wait();

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
