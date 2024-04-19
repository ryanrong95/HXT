using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Products.Prices
{
    /// <summary>
    /// 阶梯价集合
    /// </summary>
    abstract public class BasePricebreaks : NtErp.Wss.Sales.Services.Underly.IForSerializers<Pricebreak>
    {
        List<Pricebreak> source;

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        protected BasePricebreaks()
        {
            this.source = new List<Pricebreak>();
        }

        public BasePricebreaks(IEnumerable<Pricebreak> ienums)
        {
            this.source = new List<Pricebreak>(ienums);
        }

        public Pricebreak this[int index]
        {
            get { return this.source[index]; }
        }


        public void Add(Pricebreak t)
        {
            StackTrace trace = new StackTrace();
            //获取是哪个类来调用的  
            var type = trace.GetFrame(1).GetMethod().DeclaringType;

            this.source.Add(t);
        }

        virtual public IEnumerator<Pricebreak> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        protected IEnumerable<Pricebreak> GetIEnumerable()
        {
            return this.source;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(IEnumerable<Pricebreak> collection)
        {
            this.source.AddRange(collection);
        }
    }
}
