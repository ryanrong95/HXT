
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Collections
{
    public class CumulateList<T> : IEnumerable<T>
    {
        protected List<T> source;

        public CumulateList()
        {
            this.source = new List<T>();
        }

        public CumulateList(IEnumerable<T> source)
        {
            this.source = source.ToList();
        }

        public T this[int index]
        {
            get
            {
                return this.source[index];
            }

            set
            {
                this.source[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        public void Add(T item)
        {
            this.source.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
