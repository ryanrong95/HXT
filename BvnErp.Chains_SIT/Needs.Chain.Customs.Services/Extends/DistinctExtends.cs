using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class DistinctExtends
    {
        /// <summary>
        /// 自定义Distinct扩展方法
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source">要去重的对象类</param>
        /// <param name="keySelector">自定义去重字段的委托</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IQueryable<TSource> DistinctBy<TSource, TKey>(this IQueryable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var result = source.AsEnumerable().DistinctBy(keySelector);
            return result.AsQueryable();
        }
    }
}
