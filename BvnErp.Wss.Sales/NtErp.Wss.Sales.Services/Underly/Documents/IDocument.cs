using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public interface IDocument<T> : IEnumerable<T>
    {
        int Count { get; }

        Elements this[string index] { get; set; }
    }

    public interface IXJsonConvert
    {

    }

    public class PageData : IXJsonConvert
    {
        public int total { set; get; }
        public Document[] rows { set; get; }
    }

    public class PageData<T>
    {
        public int total { set; get; }
        public T[] rows { set; get; }
    }
}
