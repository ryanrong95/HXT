using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class PaymentRecord : IUnique
    {
        public string ID { get; set; }

        public DateTime CreateDate { get; set; }

        public decimal Amount { get; set; }

        public List<PaymentDetail> PaymentDetails { get; set; }

        public class PaymentDetail
        {
            /// <summary>
            /// 订单号
            /// </summary>
            public string OrderID { get; set; }

            /// <summary>
            /// 货款
            /// </summary>
            public decimal? ProductFee { get; set; }

            /// <summary>
            /// 税代费
            /// </summary>
            public decimal? TaxAgencyFee { get; set; }
        }

    }
}
