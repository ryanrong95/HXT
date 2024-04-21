using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments.Tools
{
    public class Carloads
    {
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="ton">吨数</param>
        /// <returns></returns>
        public PayTool this[int ton]
        {
            get
            {
                return PaymentTools.Data.SingleOrDefault(item => item.Conduct == "代仓储"
                     && item.Catalog == "杂费"
                     && item.Subject == "包车费" + ton + "吨车"
                     && item.Type == PayItemType.Receivables);
            }
        }


        internal Carloads()
        {

        }
    }
}
