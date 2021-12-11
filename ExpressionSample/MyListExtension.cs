using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace ExpressionSample
{
    public static  class MyListExtension
    {
        public static IQueryable<T> MyWhere<T>(this IQueryable<T> source, Expression<Func<T, bool>> func)
        {
            var lamda= func.Compile();
            return source.Where(lamda).AsQueryable();
        }
    }
}
