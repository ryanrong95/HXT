using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq.Generic
{
    /// <summary>
    /// 
    /// </summary>
    abstract public class Query1Classics<Tentity, Treponsitory> : IDisposable
      where Treponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        protected Treponsitory Reponsitory { get; private set; }

        public Query1Classics()
        {
            this.Reponsitory = new Treponsitory();
        }

        protected Query1Classics(Treponsitory reponsitory)
        {
            this.Reponsitory = reponsitory == null ? new Treponsitory() : reponsitory;
        }

        /// <summary>
        /// 指定条数的
        /// </summary>
        /// <param name="top"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public Tentity[] GetTop(int top, Expression<Func<Tentity, bool>> expression
            , params LambdaExpression[] expressions)
        {
            //在订单有订单项目的产品名称搜索时，还是有可能会超越Expression<Func<Tentity, bool>>条件的情况。
            var linq = this.GetIQueryable(expression, expressions ?? new LambdaExpression[0]);

            var arry = linq.Take(top).ToArray();

            return this.OnReadShips(arry).ToArray();
        }

        /// <summary>
        /// 获取所有数据,带有多个条件的
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Tentity[] GetAlls(Expression<Func<Tentity, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable(expression, expressions ?? new LambdaExpression[0]);
            var arry = this.OnReadShips(linq.ToArray()).ToArray();
            return arry;
        }

        /// <summary>
        /// 分页的
        /// </summary>
        /// <param name="pageSize">单页数据数量</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="expressions">条件表达式</param>
        /// <returns></returns>
        public PageList<Tentity> GetPageList(int pageIndex, int pageSize, Expression<Func<Tentity, bool>> expression
            , params LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable(expression, expressions ?? new LambdaExpression[0]);

            var arry = linq.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();

            var afters = this.OnReadShips(arry);
            return new PageList<Tentity>(pageIndex, pageSize, afters, linq.Count());
        }

        abstract protected IQueryable<Tentity> GetIQueryable(Expression<Func<Tentity, bool>> expression, params LambdaExpression[] expressions);

        abstract protected IEnumerable<Tentity> OnReadShips(Tentity[] results);

        public void Dispose()
        {
            this.Reponsitory.Dispose();
        }
    }
}
