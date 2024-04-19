using NtErp.Wss.Sales.Services.Model.Orders;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Orders.Commodity
{
    /// <summary>
    /// 商品帐
    /// </summary>
    public class Commodity
    {

        internal Commodity()
        {

        }

        public Commodity(ServiceDetail detail)
        {
            var inputs = new CommodityInputsView().Where(t => t.ServiceOuputID == detail.ServiceOutputID).ToArray();
            var receivable = inputs.Sum(t => t.Count);
            var sent = new CommodityOutputsView().Where(t => t.ServiceInputID == detail.ServiceInputID).ToArray().Sum(t => t.Count);

            this.Receivable = receivable;
            this.Sent = sent;
        }

        /// <summary>
        /// 应收
        /// </summary>
        public int Receivable { get; protected set; }
        /// <summary>
        /// 实发
        /// </summary>
        public int Sent { get; protected set; }
        /// <summary>
        /// 未发送的
        /// </summary>
        public int Unsent
        {
            get
            {
                return this.Receivable - this.Sent;
            }
        }

    }
}
