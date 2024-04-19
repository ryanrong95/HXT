using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Premiums
{
    abstract public class UnifyPremium<T> : IEnumerable<T> where T : PremiumBase
    {
        protected UnifyPremium()
        {

        }

        abstract public IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
