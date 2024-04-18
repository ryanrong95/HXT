using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class PvWsOrderBaseOrderView : UniqueView<Order, ScCustomReponsitory>
    {
        public PvWsOrderBaseOrderView()
        {

        }

        public PvWsOrderBaseOrderView(ScCustomReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            return from order in Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PvWsOrderBaseOrderView>()
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
