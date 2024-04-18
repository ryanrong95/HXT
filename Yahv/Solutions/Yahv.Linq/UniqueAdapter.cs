using System;
using System.Linq;

namespace Yahv.Linq
{
    /// <summary>
    /// 一般视图转换器
    /// </summary>
    /// <typeparam name="TEntity">目标对象</typeparam>
    /// <typeparam name="Tview">原始视图</typeparam>
    public abstract class UniqueAdapter<TEntity, Tview> : QueryAdapter<TEntity, Tview>
        where TEntity : IUnique
        where Tview : class, IQueryable, IDisposable
    {

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>目标对象指定ID的单一实例</returns>
        public TEntity this[string id]
        {
            get
            {
                return this.Single(item => item.ID == id);
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public UniqueAdapter() : base()
        {


        }
    }
}
