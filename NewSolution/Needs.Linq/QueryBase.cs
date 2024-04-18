using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    abstract public class QueryBase<T> : IQueryable<T>
    {

        IQueryable<T> iQueryable;

        protected IQueryable<T> IQueryable
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

        public Type ElementType
        {
            get
            {
                return this.IQueryable.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return this.IQueryable.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.IQueryable.Provider;
            }
        }

        public QueryBase()
        {
        }

        protected QueryBase(IQueryable<T> iQueryable)
        {
            this.iQueryable = iQueryable;
        }

        abstract protected IQueryable<T> GetIQueryable();

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return this.IQueryable.ToString();
        }

        #region Paging(AllowPaging RecordCount PageSize PageIndex PageCount)

        /// <summary>
        /// 是否启用分页
        /// 默认值：true
        /// </summary>
        public bool AllowPaging { get; set; } = true;

        /// <summary>
        /// 获取总记录数
        /// </summary>
        public int RecordCount
        {
            get
            {
                return this.GetIQueryable().Count();
            }
        }

        /// <summary>
        /// 获取或设置分类大小
        /// 默认值：10
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 获取或设置当前页索引
        /// 默认值：0
        /// </summary>
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 获取总页数
        /// </summary>
        public int PageCount
        {
            get { return (int)Math.Ceiling((double)RecordCount / (double)PageSize); }
        }

        #endregion

        #region OrderBy

        /// <summary>
        /// 获取或设置升序排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 获取或设置倒(降)序排序字段
        /// </summary>
        public string OrderByDescending { get; set; }

        #endregion

        #region Predicate

        private Expression<Func<T, bool>> predicate;

        /// <summary>
        /// 获取或设置查询条件
        /// </summary>
        public Expression<Func<T, bool>> Predicate
        {
            get
            {
                if (predicate == null)
                {
                    predicate = PredicateBuilder.Create<T>();
                }

                return predicate;
            }
            set
            {
                predicate = value;
            }
        }

        #endregion
    }
}
