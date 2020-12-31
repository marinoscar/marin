using Luval.Workflow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.UtilityTasks
{
    public static class Extensions
    {
        public static T GetData<T>(this SessionContext context, string keyName, T defaultValue)
        {
            if (!context.Data.ContainsKey(keyName)) return defaultValue;
            return (T)context.Data[keyName];
        }

        public static T GetData<T>(this SessionContext context, string keyName)
        {
            return GetData<T>(context, keyName, default(T));
        }
    }
}
