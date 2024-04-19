using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Collections
{
    public class ItemList<T> : CumulateList<T>
    {
        public ItemList()
        {

        }

        public void Remove(int index)
        {
            this.source.RemoveAt(index);
        }
    }
}
