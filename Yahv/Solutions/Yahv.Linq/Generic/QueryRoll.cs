using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yahv.Linq.Generic
{
    /// <summary>
    /// 查询数据基类
    /// </summary>
    /// <typeparam name="TRetrun">目标对象</typeparam>
    /// <typeparam name="Treponsitory">Linq支持者访问器</typeparam>
    /// <typeparam name="TFirst">第一条件目标对象</typeparam>
    abstract public class QueryRoll<TRetrun, TFirst, Treponsitory> : IDisposable
      where Treponsitory : Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// Linq支持者访问器
        /// </summary>
        protected Treponsitory Reponsitory { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public QueryRoll()
        {
            this.Reponsitory = new Treponsitory();
        }

        /// <summary>
        /// 指定条数的
        /// </summary>
        /// <param name="top">返回条数限制</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="expressions">其他条件表达式</param>
        /// <returns>查询结果数组</returns>
        public TRetrun[] GetTop(int top, Expression<Func<TFirst, bool>> expression
            , params LambdaExpression[] expressions)
        {
            //在订单有订单项目的产品名称搜索时，还是有可能会超越Expression<Func<Tentity, bool>>条件的情况。
            var linq = this.GetIQueryable(expression, expressions ?? new LambdaExpression[0]);

            var arry = linq.Take(top).ToArray();
            return this.OnReadShips(arry).ToArray();
        }

        /// <summary>
        /// 分页的
        /// </summary>
        /// <param name="pageSize">单页数据数量</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="expression">条件表达式</param>
        /// <param name="expressions">其他条件表达式</param>
        /// <returns>查询结果分页数据</returns>
        public PageList<TRetrun> GetPageList(int pageIndex, int pageSize, Expression<Func<TFirst, bool>> expression
            , params LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable(expression, expressions ?? new LambdaExpression[0]);

            var arry = linq.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();
            var afters = this.OnReadShips(arry);
            return new PageList<TRetrun>(pageIndex, pageSize, afters, linq.Count());
        }

        /// <summary>
        /// 抽象实现 可查询集
        /// </summary>
        /// <param name="expression">条件表达式</param>
        /// <param name="expressions">其他条件表达式</param>
        /// <returns>可查询集</returns>
        abstract protected IQueryable<TFirst> GetIQueryable(Expression<Func<TFirst, bool>> expression, params LambdaExpression[] expressions);

        /// <summary>
        /// 抽象实现 读取关系
        /// </summary>
        /// <param name="results">查询结果数组</param>
        /// <returns>读取后的关系集合</returns>
        abstract protected IEnumerable<TRetrun> OnReadShips(TFirst[] results);

        /// <summary>
        /// 实现 释放函数
        /// </summary>
        public void Dispose()
        {
            this.Reponsitory.Dispose();
        }
    }
}
