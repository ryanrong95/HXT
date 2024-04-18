using Layers.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yahv.Linq
{
    /// <summary>
    /// 附加价值   
    /// </summary>
    /// 
    [Obsolete("按照王亚要求重新开发")]
    public class LinqEnumerable<TEntity> : IEnumerable<TEntity> where TEntity : IUnique
    {
        List<TEntity> data;
        List<TEntity> deletes;

        /// <summary>
        /// 受保护的构造器
        /// </summary>
        protected LinqEnumerable()
        {
            this.data = new List<TEntity>();
            this.deletes = new List<TEntity>();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="data">数据</param>
        public LinqEnumerable(IEnumerable<TEntity> data) : this()
        {
            if (data is List<TEntity>)
            {
                this.data = data as List<TEntity>;
            }
            else
            {
                this.data = new List<TEntity>(data);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对象</returns>
        virtual public TEntity this[int index]
        {
            get { return this.data[index]; }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对象</returns>
        virtual public TEntity this[string index]
        {
            get { return this.data.SingleOrDefault(item => item.ID == index); }
        }

        /// <summary>
        /// 已重写可枚举访问器
        /// </summary>
        /// <returns></returns>
        virtual public IEnumerator<TEntity> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        protected List<TEntity> GetDeletes()
        {
            return this.deletes;
        }


        //virtual public IEnumerator<TEntity> _GetEnumerator()
        //{
        //    if (true)
        //    {
        //        return this.deletes.GetEnumerator();
        //    }

        //    return this.data.GetEnumerator();
        //}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        public int Count { get { return this.data.Count; } }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="arry">数据</param>
        protected void add(params TEntity[] arry)
        {
            this.data.AddRange(arry.Where(item => item != null));
        }

        /// <summary>
        /// 标记删除
        /// </summary>
        /// <param name="arry">数据</param>
        public void remove(params TEntity[] arry)
        {
            this.deletes.AddRange(arry.Where(item => item != null));
            this.data.RemoveAll(item => arry.Contains(item));
        }

        /// <summary>
        /// 标记删除
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        public void remove(Func<TEntity, bool> predicate)
        {
            this.deletes.AddRange(this.data.Where(predicate));
            this.data.RemoveAll(item => predicate(item));
        }

        ///// <summary>
        ///// 废弃被标记删除的项
        ///// </summary>
        ///// <param name="arry">数据</param>
        //public void Abandon()
        //{
        //    this.data.RemoveAll(item => deletes.Contains(item));
        //    this.deletes.Clear();
        //}
    }


    /// <summary>
    /// 附加价值
    /// </summary>

    [Obsolete("暂时废弃")]
    abstract public class LinqEnumerable<TEntity, TReponsitory> : LinqEnumerable<TEntity>, IDisposable
       where TEntity : IUnique
       where TReponsitory : IReponsitory, new()
    {
        IEnumerable<TEntity> query;

        List<TEntity> data;

        /// <summary>
        /// 数据源
        /// </summary>
        protected List<TEntity> Data
        {
            get
            {
                if (query == null)
                {
                    this.query = this.GetIQueryable();
                }

                if (data == null)
                {
                    this.data = new List<TEntity>(this.query);
                }

                return data;
            }
        }

        TReponsitory reponsitory;

        /// <summary>
        /// linq 支持者
        /// </summary>
        protected TReponsitory Reponsitory
        {
            get
            {
                if (this.reponsitory == null)
                {
                    this.reponsitory = new TReponsitory();
                }

                return this.reponsitory;
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="data">数据</param>
        public LinqEnumerable()
        {

        }

        /// <summary>
        /// 附加运费表 可查询集
        /// </summary>
        /// <param name="reponsitory">支持者</param>
        /// <param name="arry">参数构造</param>
        /// <returns>可查询集</returns>
        abstract protected IQueryable<TEntity> GetIQueryable();

        /// <summary>
        /// 已重写可枚举访问器
        /// </summary>
        /// <returns></returns>
        override public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }

        /// <summary>
        /// 已重写 释放
        /// </summary>
        public void Dispose()
        {
            if (this.Reponsitory == null)
            {
                return;
            }

            this.Reponsitory.Dispose();
        }
    }

}
