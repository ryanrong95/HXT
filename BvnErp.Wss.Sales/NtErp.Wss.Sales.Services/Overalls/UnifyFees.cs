using NtErp.Wss.Sales.Services.Overalls.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Overalls
{
    public class UnifyFees
    {
        public OrderPremiums Order { get; private set; }

        public UnifyFees()
        {
            this.Order = new OrderPremiums();
        }

        static object lockcurrent = new object();
        static UnifyFees runtime;

        static public UnifyFees Current
        {
            get
            {
                if (runtime == null)
                {
                    lock (lockcurrent)
                    {
                        if (runtime == null)
                        {
                            runtime = new UnifyFees();
                        }
                    }
                }
                return runtime;
            }
        }
    }
}
