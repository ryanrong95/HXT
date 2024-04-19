using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    public delegate void ItemStart<T>(T item);



    /// <summary>
    /// 公共方法 linq 中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    abstract public class BaseItems<T> : IEnumerable<T>
    {
        ItemStart<T> action;
        List<T> list;

        public BaseItems()
        {
            this.list = new List<T>();
        }

        protected BaseItems(IEnumerable<T> enums)
        {
            if (enums == null)
            {
                this.list = new List<T>();
                return;
            }

            if (enums is List<T>)
            {
                this.list = enums as List<T>;
                return;
            }

            this.list = new List<T>(enums);
        }
        protected BaseItems(IEnumerable<T> enums, ItemStart<T> action) : this(enums)
        {
            this.action = action;
        }
        protected BaseItems(IQueryable<T> enums, ItemStart<T> action) : this(enums.ToList())
        {
            this.action = action;
        }

        public int Count { get { return this.list.Count; } }

        public T this[int index]
        {
            get { return this.ElementAt(index); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var items = this.list.Select(item =>
            {
                this.action?.Invoke(item);
                return item;
            });

            return this.GetEnumerable(items).GetEnumerator();
        }

        virtual protected IEnumerable<T> GetEnumerable(IEnumerable<T> ienums)
        {
            return ienums;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 添加新项目
        /// </summary>
        /// <param name="item">项目</param>
        virtual public void Add(T item)
        {
            this.action?.Invoke(item);
            this.list.Add(item);
        }
    }


  
}
