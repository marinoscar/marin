using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luval.Data.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable Cast(this IEnumerable self, Type innerType)
        {
            var methodInfo = typeof(Enumerable).GetMethod("Cast");
            var genericMethod = methodInfo.MakeGenericMethod(innerType);
            return genericMethod.Invoke(null, new[] { self }) as IEnumerable;
        }

        public static object ToList(this IEnumerable self, Type innerType)
        {
            var methodInfo = typeof(Enumerable).GetMethod("ToList");
            var genericMethod = methodInfo.MakeGenericMethod(innerType);
            var enumerable = Cast(self, innerType);
            var res = genericMethod.Invoke(null, new[] { enumerable });
            return res;
        }
    }
}
