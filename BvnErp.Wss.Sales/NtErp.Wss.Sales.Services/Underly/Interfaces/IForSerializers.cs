using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public interface IForSerializers<T> : IEnumerable<T>
    {
        int Count { get; }

        T this[int index] { get; }

        void Add(T t);

        void Add(IEnumerable<T> collection);
    }
}
