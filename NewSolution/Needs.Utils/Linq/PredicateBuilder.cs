using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Linq
{
    /// <summary>
    /// Linq条件建立扩展类
    /// </summary>
    public static class PredicateBuilder
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
    }
}
