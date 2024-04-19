using Needs.Underly.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;
using NtErp.Wss.Sales.Services.Underly.Collections;

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    public sealed class Premiums : CumulateList<Premium>
    {

        public Premiums()
        { }

        public Premiums(IEnumerable<Premium> source)
        {
            base.source = new List<Premium>(source);
        }

        /// <summary>
        /// 附加费总价
        /// </summary>
        [XmlIgnore]
        public decimal Total
        {
            get
            {
                if (this.Count == 0)
                {
                    return 0;
                }
                return this.Sum(item => item.SubTotal);
            }
        }
    }
}
