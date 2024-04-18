using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yahv.Linq.Extends
{
    /// <summary>
    /// Linq条件建立扩展类
    /// </summary>
    public static class PredicateExtends
    {
        /// <summary>
        /// 永真条件
        /// </summary>
        /// <typeparam name="T">目标条件类型</typeparam>
        /// <returns>指定目标条件类型永真条件</returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        /// <summary>
        /// 永假条件
        /// </summary>
        /// <typeparam name="T">目标条件类型</typeparam>
        /// <returns>指定目标条件类型永假条件</returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        /// <summary>
        /// Or 并条件
        /// </summary>
        /// <typeparam name="T">目标条件类型</typeparam>
        /// <param name="left">目标条件</param>
        /// <param name="right">连接条件</param>
        /// <returns>连接后的条件</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            var invokedExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.Or(left.Body, invokedExpr), left.Parameters);
        }
        /// <summary>
        /// And 并条件
        /// </summary>
        /// <typeparam name="T">目标条件类型</typeparam>
        /// <param name="left">目标条件</param>
        /// <param name="right">连接条件</param>
        /// <returns>连接后的条件</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {

            //马上需要修改

            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            var invokedExpr = Expression.Invoke(right, left.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.And(left.Body, invokedExpr), left.Parameters);
        }

        /// <summary>
        /// 视图使用 expression 作为条件
        /// </summary>
        /// <typeparam name="TEntity">目标对象类型</typeparam>
        /// <param name="expressions">条件表达式组</param>
        /// <param name="iQueryable">目标可查询集</param>
        /// <returns>指定目标条件类型永真条件</returns>

        [Obsolete("原有分页开发，不啊哟使用")]
        public static IQueryable<TEntity> TryWhere<TEntity>(this IQueryable<TEntity> iQueryable, params LambdaExpression[] expressions)
        {
            if (expressions == null || expressions.Length == 0)
            {
                return iQueryable;
            }

            var rezult = iQueryable;

            for (int index = 0; index < expressions.Length; index++)
            {
                if (expressions[index] is Expression<Func<TEntity, bool>>)
                {
                    rezult = rezult.Where((Expression<Func<TEntity, bool>>)expressions[index]);
                }
            }

            return rezult;
        }

        /// <summary>
        /// LinqToSql去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy2<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
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
       
    }
}
