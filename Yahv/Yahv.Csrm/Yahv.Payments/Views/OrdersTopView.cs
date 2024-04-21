using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 订单视图
    /// </summary>
    public class WsOrdersTopView : QueryView<WsOrder, PvbCrmReponsitory>
    {
        public WsOrdersTopView()
        {

        }

        public WsOrdersTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<WsOrder> GetIQueryable()
        {
            return new Yahv.Services.Views.WsOrdersTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public WsOrder this[string orderId]
        {
            get { return this.SingleOrDefault(item => item.ID == orderId); }
        }
    }
}
