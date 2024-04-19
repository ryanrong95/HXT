using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Collections
{
    public class Alert<T> : IEnumerable<T> where T : class, IAlter, new()
    {
        List<T> list;

        public Alert()
        {
            this.list = new List<T>();
        }

        public int Count { get { return this.list.Count; } }

        T modifier;
        protected T Modifier
        {
            get
            {
                lock (this)
                {
                    if (modifier == null)
                    {
                        modifier = this.Tinit();
                        this.list.ForEach(item =>
                        {
                            item.Status = AlterStatus.Altered;
                        });
                        this.Add(modifier);
                    }
                }
                return modifier;
            }
        }

        protected T Current
        {
            get
            {
                var view = this.list.Where(item => item.Status == AlterStatus.Normal);
                var first = view.FirstOrDefault();
                if (first == null)
                {
                    first = this.Tinit();
                    //this.Add(first = this.Tinit());
                }
                return first;
            }
        }

        /// <summary>
        /// 泛型初始化
        /// </summary>
        /// <returns>返回希望的初始化数据</returns>
        virtual protected T Tinit()
        {
            return new T
            {
                AlterDate = DateTime.Now,
                Status = AlterStatus.Normal,
            };
        }

        public void Add(T consignee)
        {
            this.list.Add(consignee);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}
