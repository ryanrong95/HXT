using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Needs.Underly;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace MvcApp.Buyer.Services.Rates
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

        FlushTimer<FloatRates> floatRate;

        public FloatRates FloatRates
        {
            get
            {
                return this.floatRate.Current;
            }
        }

        public UnifyRates()
        {
            this.taxRates = new TaxRates();
            this.floatRate = new FlushTimer<FloatRates>();
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
