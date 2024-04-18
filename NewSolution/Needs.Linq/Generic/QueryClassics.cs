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
    abstract public class QueryClassics<Tentity, Treponsitory>
      where Treponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        protected Treponsitory Reponsitory { get; private set; }

        public QueryClassics()
        {
            this.Reponsitory = new Treponsitory();
        }

        protected QueryClassics(Treponsitory reponsitory)
        {
            this.Reponsitory = reponsitory == null ? new Treponsitory() : reponsitory;
        }

        /// <summary>
        /// 指定条数的
        /// </summary>
        /// <param name="top"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public Tentity[] GetTop(int top, params LambdaExpression[] expressions)
        {
            var expression = expressions ?? new LambdaExpression[0];
            var linq = this.GetIQueryable(expression);

            var arry = linq.Take(top).ToArray();

            return this.OnReadShips(arry).ToArray();
        }

        /// <summary>
        /// 分页的
        /// </summary>
        /// <param name="pageSize">单页数据数量</param>
        /// <param name="pageIndex">页码（从1开始）</param>
        /// <param name="expressions">条件表达式</param>
        /// <returns></returns>
        public PageList<Tentity> GetPageList(int pageIndex, int pageSize, params LambdaExpression[] expressions)
        {
            var expression = expressions ?? new LambdaExpression[0];
            var linq = this.GetIQueryable(expression);

            var arry = linq.Skip(pageSize * pageIndex - 1).Take(pageSize).ToArray();

            var afters = this.OnReadShips(arry);
            return new PageList<Tentity>(pageIndex, pageSize, afters, linq.Count());

            //var expression1 = expressions.SingleOrDefault(item => item is Expression<Func<Tentity, bool>>);
        }

        abstract protected IQueryable<Tentity> GetIQueryable(params LambdaExpression[] expressions);

        abstract protected IEnumerable<Tentity> OnReadShips(Tentity[] results);
    }
}
