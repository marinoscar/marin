using Luval.Data;
using Luval.Data.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Marin
{
    public class HtmlBuilder
    {
        public HtmlBuilder(Type type)
        {
            Properties = type.GetProperties();
        }

        protected IEnumerable<PropertyInfo> Properties { get; set; }
        
        public string InputFields()
        {
            var sw = new StringWriter();
            sw.WriteLine("<form>");
            foreach (var p in Properties.Where(i => ObjectExtensions.IsPrimitiveType(i.PropertyType)))
                sw.WriteLine(InputField(p));
            sw.WriteLine("</form>");
            return sw.ToString();
        }

        private string InputField(PropertyInfo prop)
        {
            var sw = new StringWriter();
            sw.WriteLine(@" <div class=""form-row"">");
            sw.WriteLine(@"     <div class=""form-group col-md-12"">");
            sw.WriteLine($@"        <label for=""{ prop.Name }"">{ prop.Name }</label>");
            sw.WriteLine($@"        <input type=""text"" class=""form-control"" id=""{ prop.Name }"" name=""{ prop.Name }"" placeholder=""{ prop.Name }"">");
            sw.WriteLine("      </div>");
            sw.WriteLine("  </div>");
            return sw.ToString();
        }
    }
}
