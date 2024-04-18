using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yahv.Linq.Extends
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, false);
        }
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool desc)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, desc);
        }
        static class QueryableHelper<T>
        {
            private static Dictionary<string, LambdaExpression> cache = new Dictionary<string, LambdaExpression>();
            public static IQueryable<T> OrderBy(IQueryable<T> queryable, string propertyName, bool desc)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);
                return desc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
            }
            private static LambdaExpression GetLambdaExpression(string propertyName)
            {
                Type type = typeof(T);
                string key = string.Concat(type.Name, "-", propertyName);
                if (cache.ContainsKey(key)) return cache[key];
                var param = Expression.Parameter(typeof(T));
                var body = Expression.Property(param, propertyName);
                var keySelector = Expression.Lambda(body, param);
                cache[key] = keySelector;
                return keySelector;
            }
        }
    }
}
