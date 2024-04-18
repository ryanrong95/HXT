using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class OrderBaseOrigin : UniqueView<Order, PvWsOrderReponsitory>
    {
        public OrderBaseOrigin()
        {

        }

        public OrderBaseOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            return from order in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>()
                   select new Order
                   {
                       ID = order.ID,
                       Type = (OrderType)order.Type,
                       ClientID = order.ClientID,
                       InvoiceID = order.InvoiceID,
                       PayeeID = order.PayeeID,
                       BeneficiaryID = order.BeneficiaryID,
                       SupplierID = order.SupplierID,
                       Summary = order.Summary,
                       CreateDate = order.CreateDate,
                       ModifyDate = order.ModifyDate,
                       CreatorID = order.CreatorID,
                       SettlementCurrency = (Currency?)order.SettlementCurrency,
                   };
        }
    }
}
