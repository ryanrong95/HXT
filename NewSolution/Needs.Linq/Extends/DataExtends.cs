using Needs.Linq.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Needs.Linq.Extends
{

    public enum RadiateType
    {
        Father,
    }

    /// <summary>
    /// 测试使用
    /// </summary>
    static public class DataExtends
    {
        static public IPageList<T> Paging<T>(this IQueryable<T> queryable, int pageIndex = 1, int pageSize = 20)
        {
            
            int total = queryable.Count();
            var query = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new PageList<T>(query)
            {
                Index = pageIndex,
                Size = pageSize,
                Total = total
            };
        }

        static public IPageList<T> Paging<T>(this IEnumerable<T> queryable, int pageIndex = 1, int pageSize = 20)
        {
            int total = queryable.Count();
            var query = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            return new PageList<T>(query)
            {
                Index = pageIndex,
                Size = pageSize,
                Total = total
            };
        }

        /// <summary>
        /// 辐射
        /// </summary>
        /// <typeparam name="Target">目标类型</typeparam>
        /// <typeparam name="Tobject">设置类型</typeparam>
        /// <param name="target">来源实例</param>
        /// <param name="tobject">设置实例</param>
        /// <param name="feild">这是字段类型</param>
        /// <returns></returns>
        static public Target Radiate<Target, Tobject>(this Target target, Tobject tobject, string feildName = "father")
        {
            Type type = typeof(Target);
            var field = type.GetField(feildName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
            if (field == null)
            {
                return target;
            }
            field.SetValue(target, tobject);
            return target;
        }
    }
}
