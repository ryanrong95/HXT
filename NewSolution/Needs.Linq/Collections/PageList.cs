using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq.Collections
{

    public interface IPageList<T> : IEnumerable<T>
    {
        int Total { get; }
        int Index { get; }
        int Size { get; }
    }

    public class PageList<T> : IPageList<T>
    {
        public int Total { get; set; }
        public int Index { get; set; }
        public int Size { get; set; }

        public T this[int index]
        {
            get { return this.source[index]; }
        }

        T[] source;
        internal PageList(IQueryable<T> source)
        {
            this.source = source.ToArray();
        }

        internal PageList(IEnumerable<T> source)
        {
            this.source = source.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.source.Select(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
