using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments.Tools
{
    public enum CarloadSpecs
    {
        /// <summary>
        /// 
        /// </summary>
        Big,
        Small
    }

    public class FreightDetails
    {
        public PayTool this[string area, CarloadSpecs spec]
        {
            get
            {
                string name;
                switch (spec)
                {
                    case CarloadSpecs.Big:
                        name = "大车";
                        break;
                    case CarloadSpecs.Small:
                        name = "小车";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return PaymentTools.Data.SingleOrDefault(item => item.Conduct == "代仓储"
                     && item.Catalog == "杂费"
                     && item.Subject == area + name
                     && item.Type == PayItemType.Receivables);
            }
        }

        internal FreightDetails()
        {

        }
    }
}
