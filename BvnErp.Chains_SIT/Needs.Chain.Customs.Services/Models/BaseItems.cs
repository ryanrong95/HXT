using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 公共方法 linq 中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    abstract public class BaseItems<T> : IEnumerable<T>
    {
        Action<T> action;
        List<T> list;

        protected BaseItems(IEnumerable<T> enums)
        {
            this.list = new List<T>(enums);
        }
        protected BaseItems(IEnumerable<T> enums, Action<T> action) : this(enums)
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
            this.action(item);

            this.list.Add(item);
        }

        /// <summary>
        /// 移除已有项
        /// </summary>
        /// <param name="item"></param>
        virtual public void Remove(T item)
        {
            this.list.Remove(item);
        }

        /// <summary>
        /// 移除所有项
        /// </summary>
        virtual public void RemoveAll()
        {
            this.list.Clear();
        }
    }
}
