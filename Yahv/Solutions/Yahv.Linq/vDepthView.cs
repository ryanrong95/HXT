using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Linq
{
    abstract public class vDepthViewBase<TEntity, TRuturn>
           where TEntity : IUnique, IDataEntity
    {

        IQueryable<TEntity> iQueryable;

        /// <summary>
        /// 可查询集访问器
        /// </summary>
        protected IQueryable<TEntity> IQueryable
        {
            get
            {
                if (this.iQueryable == null)
                {
                    iQueryable = this.GetIQueryable();
                }

                return iQueryable;
            }
        }

        abstract public void Dispose();

        /// <summary>
        /// 抽象实现 目标对象的可查询集获取方法
        /// </summary>
        /// <returns>目标对象的可查询集获取方法</returns>
        abstract protected IQueryable<TEntity> GetIQueryable();

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>目标对象指定ID的单一实例</returns>
        virtual public TRuturn this[string id]
        {
            get
            {
                this.iQueryable = this.GetIQueryable().Where(item => item.ID == id);
                var arry = this.ToMyPage(null, 1);
                return arry.rows.FirstOrDefault();
            }
        }

        /// <summary>
        /// 补全数据
        /// </summary>
        /// <param name="top">获取头N条</param>
        /// <returns>头N条数据</returns>
        virtual public TRuturn[] ToMyArray(int top = 50)
        {
            var page = this.ToMyPage(null, top);
            return page.rows.ToArray();
        }

        public vMyPage<TRuturn> ToMyPage(int? pageIndex = 1, int? pageSize = 20)
        {
            var iquery = this.IQueryable;
            int? total = null;

            if (pageIndex.HasValue)
            {
                total = iquery.Count();
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }
            else
            {
                iquery = iquery.Take(pageSize.Value);
            }

            return new vMyPage<TRuturn>()
            {
                total = total ?? 0,
                Size = pageSize.Value,
                Index = pageIndex ?? 1,
                rows = this.OnMyPage(iquery),
            };
        }

        public vMyPage<object> ToMyPage(Func<TRuturn, object> selector, int? pageIndex = 1, int? pageSize = 20)
        {
            var view = this.ToMyPage(pageIndex, pageSize);
            return new vMyPage<object>()
            {
                total = view.total,
                Size = pageSize.Value,
                Index = pageIndex ?? 1,
                rows = view.rows.Select(selector),
            };
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        abstract protected IEnumerable<TRuturn> OnMyPage(IQueryable<TEntity> iquery);

        public vDepthViewBase()
        {

        }

        public vDepthViewBase(IQueryable<TEntity> iQueryable)
        {
            this.iQueryable = iQueryable;
        }

    }




    /// <summary>
    /// 深度复杂对象视图
    /// </summary>
    /// <typeparam name="TEntity">循环使用的内部对象</typeparam>
    /// <typeparam name="TRuturn">返回对象</typeparam>
    /// <typeparam name="TReponsitory">支持者</typeparam>
    abstract public class vDepthViewBase<TEntity, TRuturn, TReponsitory> : vDepthViewBase<TEntity, TRuturn>, IDisposable
         where TEntity : IUnique, IDataEntity
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        /// <summary>
        /// Linq支持者
        /// </summary>
        internal protected TReponsitory Reponsitory { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public vDepthViewBase()
        {
            this.Reponsitory = new TReponsitory();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected vDepthViewBase(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected vDepthViewBase(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
            this.Reponsitory = reponsitory;
        }

        public vDepthViewBase(IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
        }
        override public void Dispose()
        {
            if (this.Reponsitory != null)
            {
                this.Reponsitory.Dispose();
            }
        }
    }


    /// <summary>
    /// 深度复杂对象视图
    /// </summary>
    /// <typeparam name="TEntity">循环使用的内部对象</typeparam>
    /// <typeparam name="TRuturn">返回对象</typeparam>
    /// <typeparam name="TReponsitory">支持者</typeparam>
    abstract public class vDepthView<TEntity, TRuturn, TReponsitory> : vDepthViewBase<TEntity, TRuturn>, IDisposable
         where TEntity : IUnique, IDataEntity
         where TRuturn : IEntity
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        /// <summary>
        /// Linq支持者
        /// </summary>
        internal protected TReponsitory Reponsitory { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public vDepthView()
        {
            this.Reponsitory = new TReponsitory();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected vDepthView(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected vDepthView(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
            this.Reponsitory = reponsitory;
        }

        public vDepthView(IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
        }
        override public void Dispose()
        {
            if (this.Reponsitory != null)
            {
                this.Reponsitory.Dispose();
            }
        }
    }



    /// <summary>
    /// 深度复杂对象视图
    /// </summary>
    /// <typeparam name="TEntity">循环使用的内部对象</typeparam>
    /// <typeparam name="TReponsitory">支持者</typeparam>
    abstract public class vDepthView<TEntity, TReponsitory> : vDepthViewBase<TEntity, object, TReponsitory>, IDisposable
        where TEntity : IUnique, IDataEntity
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public vDepthView()
        {
        }

        protected vDepthView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected vDepthView(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(reponsitory, iQueryable)
        {
        }
    }

    /// <summary>
    /// 分页对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class vMyPage<T>
    {
        public int total { get; set; }
        public int Size { get; set; }
        public int? Index { get; set; }
        public IEnumerable<T> rows { get; set; }
    }
}
