using System.Linq;

namespace Yahv.Linq
{

    /// <summary>
    /// 唯一性视图基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract public class UniqueBase<T> : QueryBase<T>, ILinqUnique<T> where T : IUnique
    {
        /// <summary>
        /// 受保护的构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected UniqueBase()
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="iQueryable">替换可查询集</param>     
        /// <param name="reponsitory">Linq支持者</param>
        protected UniqueBase(IQueryable<T> iQueryable) : base(iQueryable)
        {
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>目标对象指定ID的单一实例</returns>
        virtual public T this[string id]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == id);
            }
        }
    }
}

