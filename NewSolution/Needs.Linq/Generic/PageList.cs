using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq.Generic
{
    public class PageList<T> : IEnumerable<T>
    {
        public int PageSize { get; private set; }
        public int PageIndex { get; private set; }
        public int Total { get; private set; }

        IEnumerable<T> data;

        public PageList(int pageIndex, int pageSize, IEnumerable<T> data, int total)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.data = data;
            this.Total = total;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
