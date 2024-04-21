using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Wms.Services.Views
{
    /// <summary>
    /// 订单通用视图
    /// </summary>
    public class OrdersTopView : QueryView<WsOrder, PvWmsRepository>
    {
        public OrdersTopView()
        {

        }

        public OrdersTopView(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<WsOrder> GetIQueryable()
        {
            return new Yahv.Services.Views.WsOrdersTopView<PvWmsRepository>();
        }
    }
}