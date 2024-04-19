using Needs.Underly;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Rates
{
    public class UnifyFees
    {

        public UnifyFees()
        {
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
