using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Common
{
    public static class Extensions
    {
        public static Uri GetRequestRootUri(this Controller c)
        {
            return new Uri(string.Format("{0}://{1}", c.Request.Scheme, c.Request.Host));
        }
    }
}
