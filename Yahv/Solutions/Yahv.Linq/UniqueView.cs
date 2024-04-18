using System;
using System.Linq;

namespace Yahv.Linq
{

    /// <summary>
    /// 唯一性对象视图街垒
    /// </summary>
    /// <typeparam name="TEntity">目标对象</typeparam>
    /// <typeparam name="TReponsitory">Linq支持者</typeparam>
    abstract public class UniqueView<TEntity, TReponsitory> : UniqueBase<TEntity>, IDisposable
        where TEntity : IUnique
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        ///  已经释放
        /// </summary>
        public bool Disposed { get; private set; } = false;


        /// <summary>
        /// Linq支持者
        /// </summary>
        internal protected TReponsitory Reponsitory { get; private set; }


        /// <summary>
        /// 默认构造器
        /// </summary>
        public UniqueView()
        {
            this.Reponsitory = new TReponsitory();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected UniqueView(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="iQueryable">替换可查询集</param>
        protected UniqueView(IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected UniqueView(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(iQueryable)
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

    /// <summary>
    /// 唯一性对象视图街垒
    /// </summary>
    /// <typeparam name="TEntity">目标对象</typeparam>
    /// <typeparam name="TReponsitory">Linq支持者</typeparam>
    abstract public class UniqueView2<TEntity, TReponsitory> : UniqueBase<TEntity>, IDisposable
        where TEntity : IUnique
        where TReponsitory : Layers.Linq.IReponsitory, IDisposable
    {
        /// <summary>
        ///  已经释放
        /// </summary>
        public bool Disposed { get; private set; } = false;


        /// <summary>
        /// Linq支持者
        /// </summary>
        internal protected TReponsitory Reponsitory { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected UniqueView2(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="iQueryable">替换可查询集</param>
        protected UniqueView2(IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected UniqueView2(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(iQueryable)
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
