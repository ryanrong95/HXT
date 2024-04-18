using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Linq.Extends
{
 
    /// <summary>
    /// 测试使用
    /// </summary>
    static public class DataExtends
    {
               
        static public object Paging<T>(this IQueryable<T> queryable, int pageIndex = 1, int pageSize = 20)
        {

            int total = queryable.Count();
            var query = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query,

            };
        }

        static public object Paging<T>(this IEnumerable<T> queryable, int pageIndex = 1, int pageSize = 20)
        {
            int total = queryable.Count();
            var query = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query,

            };
        }

    }
}
