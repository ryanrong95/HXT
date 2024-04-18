using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    public static class LinqExpression
    {
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string orderByProperty)
        {
            var type = typeof(TSource);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { type, property.PropertyType },
                                   source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TSource>(resultExpression);
        }

        public static IQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string orderByProperty)
        {
            var type = typeof(TSource);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { type, property.PropertyType },
                                   source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TSource>(resultExpression);
        }
    }
}