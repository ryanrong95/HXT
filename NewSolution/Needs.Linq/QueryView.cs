using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Needs.Linq
{
    abstract public class QueryView<TSource, TReponsitory> : QueryBase<TSource>, IDisposable where TReponsitory : Layer.Linq.IReponsitory, IDisposable, new()
    {
        protected TReponsitory Reponsitory { get; private set; }

        public QueryView()
        {
            this.Reponsitory = new TReponsitory();
        }

        protected QueryView(TReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory == null ? new TReponsitory() : reponsitory;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected QueryView(TReponsitory reponsitory, IQueryable<TSource> iQueryable) : base(iQueryable)
        {
            this.Reponsitory = reponsitory;
        }

        virtual public void Dispose()
        {
            if (this.Reponsitory == null)
            {
                return;
            }
            this.Reponsitory.Dispose();
        }

        private IQueryable<TSource> GetQuery()
        {
            var query = this.GetIQueryable();

            query = query.Where(this.Predicate);

            if (string.IsNullOrEmpty(this.OrderBy) == false)
            {
                query = query.OrderBy(this.OrderBy);
            }

            if (string.IsNullOrEmpty(this.OrderByDescending) == false)
            {
                query = query.OrderByDescending(this.OrderByDescending);
            }

            if (this.AllowPaging)
            {
                query = query.Skip(this.PageSize * (this.PageIndex - 1)).Take(this.PageSize);
            }

            return query;
        }

        /// <summary>
        /// 返回数据集集合
        /// </summary>
        /// <returns></returns>
        public IList<TSource> ToList()
        {
            return this.GetQuery().ToList();
        }

        /// <summary>
        /// 使用异步方式获取数据集集合
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TSource>> ToListAsync()
        {
            return await Task.Run(() => GetQuery().ToList());
        }

        /// <summary>
        /// 返回数据集数组
        /// </summary>
        /// <returns></returns>
        public IList<TSource> ToArray()
        {
            return this.GetQuery().ToArray();
        }

        /// <summary>
        /// 使用异步方式获取数据集数组
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TSource>> ToArrayAsync()
        {
            return await Task.Run(() => GetQuery().ToArray());
        }

        /// <summary>
        /// 返回序列中的第一个元素
        /// </summary>
        /// <returns></returns>
        public TSource FirstOrDefault()
        {
            return this.GetQuery().FirstOrDefault();
        }

        /// <summary>
        /// 使用异步方式获取序列中的第一个元素
        /// </summary>
        /// <returns></returns>
        public async Task<TSource> FirstOrDefaultAsync()
        {
            return await Task.Run(() => GetQuery().FirstOrDefault());
        }
    }
}