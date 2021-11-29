using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Luval.Common
{
    public static class Extensions
    {
        #region String Operations
        
        /// <summary>
        /// Formats a string
        /// </summary>
        /// <param name="s">String to be formated</param>
        /// <param name="args">Arguments</param>
        /// <returns>Formatted string</returns>
        public static string Format(this string s, params object[] args)
        {
            return Format(s, CultureInfo.CurrentCulture, args);
        }

        /// <summary>
        /// Formats a string
        /// </summary>
        /// <param name="s">String to be formated</param>
        /// <param name="args">Arguments</param>
        /// <returns>Formatted string</returns>
        public static string Format(this string s, IFormatProvider provider, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return string.Format(provider, s, args);
        } 

        #endregion
    }
}
