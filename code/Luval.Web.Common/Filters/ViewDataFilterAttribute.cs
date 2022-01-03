using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Web.Common.Filters
{
    public class ViewDataFilterAttribute : Attribute, IActionFilter
    {
        private readonly string _value;
        private readonly string _key;
        /// <summary>
        /// Sets the a value in the <seealso cref="Controller.ViewData"/>
        /// </summary>
        /// <param name="key">The key for the element</param>
        /// <param name="value">The element value</param>
        public ViewDataFilterAttribute(string key, string value)
        {
            _value = value;
            _key = key;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrWhiteSpace(_key)) return;
            var controller = context.Controller as Controller;
            if (controller != null)
            {
                controller.ViewData[_key] = _value;
            }
        }
    }
}
