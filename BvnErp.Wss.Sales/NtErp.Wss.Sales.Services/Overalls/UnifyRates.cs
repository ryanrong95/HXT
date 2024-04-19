using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Overalls
{
    public partial class UnifyRates
    {
        TaxRates taxRates;

        public TaxRates TaxRates
        {
            get
            {
                return this.taxRates;
            }
        }

        Underly.FlushTimer<Rates.FloatRates> floatRate;

        public Rates.FloatRates FloatRates
        {
            get
            {
                return this.floatRate.Current;
            }
        }

        public UnifyRates()
        {
            this.taxRates = new TaxRates();
            this.floatRate = new Underly.FlushTimer<Rates.FloatRates>();
        }


        static object lockcurrent = new object();
        static UnifyRates runtime;

        static public UnifyRates Current
        {
            get
            {
                if (runtime == null)
                {
                    lock (lockcurrent)
                    {
                        if (runtime == null)
                        {
                            runtime = new UnifyRates();
                        }
                    }
                }
                return runtime;
            }
        }
    }
}
