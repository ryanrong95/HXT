using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Yahv.Linq
{
    /// <summary>
    /// 一般视图基类
    /// </summary>
    /// <typeparam name="T">目标对象</typeparam>
    abstract public class QueryBase<T> : IQueryable<T>
    {
        IQueryable<T> iQueryable;

        /// <summary>
        /// 可查询集访问器
        /// </summary>
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

        /// <summary>
        /// ElementType
        /// </summary>
        public Type ElementType
        {
            get
            {
                return this.IQueryable.ElementType;
            }
        }

        /// <summary>
        /// Expression
        /// </summary>
        public Expression Expression
        {
            get
            {
                return this.IQueryable.Expression;
            }
        }

        /// <summary>
        /// Provider
        /// </summary>
        public IQueryProvider Provider
        {
            get
            {
                return this.IQueryable.Provider;
                //return new MyQueryProvider(this.IQueryable.Provider);
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="iQueryable">替换可查询集</param>
        /// <param name="reponsitory">Linq支持者</param>
        protected QueryBase(IQueryable<T> iQueryable)
        {
            this.iQueryable = iQueryable;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        protected QueryBase()
        {
        }

        /// <summary>
        /// 抽象实现 目标对象的可查询集获取方法
        /// </summary>
        /// <returns>目标对象的可查询集获取方法</returns>
        abstract protected IQueryable<T> GetIQueryable();

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>支持在泛型集合上进行简单迭代</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>支持在泛型集合上进行简单迭代</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 已重写 ToString()
        /// </summary>
        /// <returns>linq 表达式</returns>
        public override string ToString()
        {
            //return this.Provider.ToString(); 
            return this.IQueryable.ToString();
        }
    }
}
