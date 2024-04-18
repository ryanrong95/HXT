using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Linq
{
    //[Obsolete("已经商议废弃")]
    abstract public class DepthViewBase<TEntity, TRuturn>
           where TEntity : IUnique
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
                return arry.Data.FirstOrDefault();
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
            return page.Data.ToArray();
        }


        public MyPage<TRuturn> ToMyPage(int? pageIndex = null, int? pageSize = null)
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


            return new MyPage<TRuturn>()
            {
                Total = total ?? 0,
                Size = pageSize.Value,
                Index = pageIndex ?? 1,
                Data = this.OnMyPage(iquery, pageIndex.HasValue && pageSize.HasValue),
            };
        }

        /// <summary>
        /// 分页
        /// </summary>
        ///<param name="iquery">查询集</param>
        ///<param name="isPaging">是否分页</param>
        /// <returns></returns>
        abstract protected IEnumerable<TRuturn> OnMyPage(IQueryable<TEntity> iquery, bool isPaging);



        public DepthViewBase()
        {

        }

        public DepthViewBase(IQueryable<TEntity> iQueryable)
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
    abstract public class DepthView<TEntity, TRuturn, TReponsitory> : DepthViewBase<TEntity, TRuturn>, IDisposable
        where TEntity : IUnique
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        /// <summary>
        /// Linq支持者
        /// </summary>
        internal protected TReponsitory Reponsitory { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public DepthView()
        {
            this.Reponsitory = new TReponsitory();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected DepthView(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected DepthView(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(iQueryable)
        {
            this.Reponsitory = reponsitory;
        }

        public DepthView(IQueryable<TEntity> iQueryable) : base(iQueryable)
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
    abstract public class DepthView<TEntity, TReponsitory> : DepthView<TEntity, object, TReponsitory>, IDisposable
        where TEntity : IUnique
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public DepthView()
        {
        }

        protected DepthView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected DepthView(TReponsitory reponsitory, IQueryable<TEntity> iQueryable) : base(reponsitory, iQueryable)
        {
        }
    }

    /// <summary>
    /// 分页对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class MyPage<T>
    {
        public int Total { get; set; }
        public int Size { get; set; }
        public int? Index { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
