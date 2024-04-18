using System;
using System.Linq;
using System.Reflection;

namespace Yahv.Linq
{
    /// <summary>
    /// 一般视图转换器
    /// </summary>
    /// <typeparam name="TEntity">目标对象</typeparam>
    /// <typeparam name="Tview">原始视图</typeparam>
    public abstract class QueryAdapter<TEntity, Tview> : QueryBase<TEntity>, IDisposable
        where Tview : class, IQueryable, IDisposable
    {
        /// <summary>
        /// 视图访问器
        /// </summary>
        protected Tview View { get; private set; }


        Layers.Linq.IReponsitory reponsitory;

        /// <summary>
        /// Linq支持者
        /// </summary>
        protected Layers.Linq.IReponsitory Reponsitory
        {
            get
            {
                if (this.reponsitory == null)
                {
                    var property = typeof(Tview).GetProperty(nameof(this.Reponsitory), BindingFlags.IgnoreCase
                        | BindingFlags.Instance
                        | BindingFlags.NonPublic);

                    this.reponsitory = property.GetValue(this.View) as Layers.Linq.IReponsitory;
                }
                return this.reponsitory;
            }
        }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public QueryAdapter()
        {
            var view = this.View = Activator.CreateInstance(typeof(Tview), true) as Tview;
        }

        /// <summary>
        /// 实现 释放函数
        /// </summary>
        virtual public void Dispose()
        {
            if (this.Reponsitory == null)
            {
                return;
            }
            this.Reponsitory.Dispose();
        }
    }
}
