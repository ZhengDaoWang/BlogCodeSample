using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqSample
{
    public static class MyListExtension
    {
        public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> enumable, Func<T, bool> func)
        {
            foreach (var item in enumable)
            {
                if (func(item))
                {
                    yield return item;
                }
            }
        }
    }
}
