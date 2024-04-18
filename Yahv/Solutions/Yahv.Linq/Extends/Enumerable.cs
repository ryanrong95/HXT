using System;
using System.Collections.Generic;
using System.Linq;



namespace Yahv.Linq.Extends
{
    /// <summary>
    /// Enumerable 扩展
    /// </summary>
    /// <remarks>
    /// Func[IQueryable[T], object[]]
    /// Func[IQueryable[T], T[]]
    /// Func[IQueryable[T], OutT[]]
    /// 的实现建议开发在Face层，陈翰想配合颗粒化
    /// 以上是建议：随时接受讨论
    /// </remarks>
    static public class Enumerable
    {
        static public T[] ToArray<T>(this IQueryable<T> queryable,
            Action<T[]> action)
        {
            var arry = System.Linq.Enumerable.ToArray(queryable);
            action(arry);
            return arry;
        }

        static public outT ToArray<T, outT>(this IQueryable<T> queryable,
            Func<T[], outT> func)
        {
            var arry = System.Linq.Enumerable.ToArray(queryable);
            return func(arry);
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        static public object Paging<T>(this IQueryable<T> queryable,
            int pageIndex = 1,
            int pageSize = 20,
        Func<T, object> converter = null)
        {
            int total = queryable.Count();
            var query = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (converter == null)
            {
                return new
                {
                    rows = query.ToArray(),
                    total = total
                };
            }
            else
            {
                var tquery = query.ToArray().Select(converter);

                return new
                {
                    rows = tquery.ToArray(),
                    total = total
                };
            }
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        static public object Paging<T>(this IEnumerable<T> queryable,
            int pageIndex = 1,
            int pageSize = 20,
            Func<T, object> converter = null)
        {
            int total = queryable.Count();
            var query = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            if (converter == null)
            {
                return new
                {
                    rows = query.ToArray(),
                    total = total
                };
            }
            else
            {
                return new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                };
            }
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="OutT"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="func">分页数据的Query的处理函数</param>
        /// <returns></returns>
        static public object Paging<T>(this IQueryable<T> queryable,
            int pageIndex = 1,
            int pageSize = 20,
            Func<IQueryable<T>, object[]> func = null) where T : class
        {
            int total = queryable.Count();

            var query = queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (func == null)
            {
                return new
                {
                    rows = query.ToArray(),
                    total = total
                };
            }
            else
            {
                return new
                {
                    rows = func(query),
                    total = total
                };
            }
        }

    }
}
