
using System;
using System.Linq;

namespace Yahv.Linq
{
    /// <summary>
    /// 一般视图基类
    /// </summary>
    /// <typeparam name="TEntity">目标对象</typeparam>
    /// <typeparam name="TReponsitory">Linq支持者</typeparam>
    abstract public class QueryView<TEntity, TReponsitory> : QueryBase<TEntity>, IDisposable
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        /// <summary>
        ///  已经释放
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// Linq支持者
        /// </summary>
        protected TReponsitory Reponsitory { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public QueryView()
        {
            this.Reponsitory = new TReponsitory();
        }


        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected QueryView(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected QueryView(TReponsitory reponsitory, IQueryable<TEntity> iQueryable):base(iQueryable)
        {
            this.Reponsitory = reponsitory;
            
            
        }

        /// <summary>
        /// 实现 释放函数
        /// </summary>
        virtual public void Dispose()
        {
            this.Disposed = true;

            if (this.Reponsitory == null)
            {
                return;
            }
            this.Reponsitory.Dispose();
        }
    }
}
