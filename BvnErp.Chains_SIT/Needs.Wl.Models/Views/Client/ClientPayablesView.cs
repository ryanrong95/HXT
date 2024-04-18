using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 客户应付款统计的视图
    /// </summary>
    public class ClientPayablesView : View<Models.ClientPayables, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientPayablesView(string clientID)
        {
            this.ClientID = clientID;
        }

        internal ClientPayablesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientPayables> GetIQueryable()
        {
            var productPayableQuery = from player in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                      join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on player.OrderID equals order.ID
                                      where order.IsLoan && player.FeeType == (int)Needs.Wl.Models.Enums.OrderFeeType.Product && player.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                                      group player by player.ClientID into playerGroup
                                      select new
                                      {
                                          ClientID = playerGroup.Key,
                                          ProductPayable = playerGroup.Sum(x => x.Amount),
                                      };

            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join payable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t=>t.OrderID.Length != 17) on client.ID equals payable.ClientID into payables
                   join productPayable in productPayableQuery on client.ID equals productPayable.ClientID into productPayables
                   from product in productPayables.DefaultIfEmpty()
                   where client.ID == this.ClientID
                   select new Models.ClientPayables
                   {
                       ClientID = client.ID,
                       ProductPayable = product.ProductPayable,
                       AgencyFeePayable = payables.Where(x => x.FeeType == (int)Needs.Wl.Models.Enums.OrderFeeType.AgencyFee && x.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(x => x.Amount),
                       IncidentalPayable = payables.Where(x => x.FeeType == (int)Needs.Wl.Models.Enums.OrderFeeType.Incidental && x.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(x => x.Amount),
                       TaxPayable = payables.Where(x => x.FeeType == (int)Needs.Wl.Models.Enums.OrderFeeType.Tariff || x.FeeType == (int)Needs.Wl.Models.Enums.OrderFeeType.AddedValueTax && x.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(x => x.Amount)
                   };
        }
    }
}