using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 客户应付款统计的视图
    /// </summary>
    public class ClientAccountPayablesView : UniqueView<Models.ClientAccountPayable, ScCustomsReponsitory>
    {
        public ClientAccountPayablesView()
        {
        }

        internal ClientAccountPayablesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientAccountPayable> GetIQueryable()
        {
            var payablesView = new OrderReceiptsAllsView(this.Reponsitory);
            var agreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => item.Status == Enums.Status.Normal);

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join agreement in agreementsView on entity.ID equals agreement.ClientID
                   join payable in payablesView on entity.ID equals payable.ClientID into payables
                   let productPayable = payables.Where(item => item.FeeType == Enums.OrderFeeType.Product && item.IsLoan)
                                         .Sum(item => (decimal?)item.Amount).GetValueOrDefault()
                   select new Models.ClientAccountPayable
                   {
                       ID = entity.ID,
                       Agreement = agreement,
                       //应付款统计
                       ProductPayable = productPayable < 0 ? 0 : productPayable,
                       TaxPayable = payables.Where(item => item.FeeType == Enums.OrderFeeType.Tariff || item.FeeType == Enums.OrderFeeType.AddedValueTax)
                                     .Sum(item => (decimal?)item.Amount).GetValueOrDefault(),
                       AgencyPayable = payables.Where(item => item.FeeType == Enums.OrderFeeType.AgencyFee)
                                        .Sum(item => (decimal?)item.Amount).GetValueOrDefault(),
                       IncidentalPayable = payables.Where(item => item.FeeType == Enums.OrderFeeType.Incidental)
                                            .Sum(item => (decimal?)item.Amount).GetValueOrDefault()
                   };
        }
    }

    /// <summary>
    /// 订单费用明细的视图（用于订单实收费用维护）
    /// </summary>
    public class ClientPayableDetailsView : UniqueView<Models.ClientPayableDetail, ScCustomsReponsitory>
    {
        public ClientPayableDetailsView()
        {
        }

        internal ClientPayableDetailsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientPayableDetail> GetIQueryable()
        {
            var payablesView = new OrderReceiptsAllsView(this.Reponsitory);
            var linq = from entity in payablesView
                       group entity by new { entity.ClientID } into gClient
                       select new Models.ClientPayableDetail
                       {
                           ID = gClient.Key.ClientID,
                           //应付款明细
                           Payables = from payable in gClient
                                      group payable by new { payable.OrderID, payable.FeeSourceID, payable.FeeType } into gPayable
                                      where gPayable.Sum(x => x.Amount) > 0
                                      select new Models.ClientFeeModel
                                      {
                                          OrderID = gPayable.Key.OrderID,
                                          FeeSourceID = gPayable.Key.FeeSourceID,
                                          Type = gPayable.Key.FeeType,
                                          Amount = gPayable.Key.FeeSourceID == null ?
                                                   gClient.Where(item => item.OrderID == gPayable.Key.OrderID && item.FeeType == gPayable.Key.FeeType && item.FeeSourceID == null)
                                                          .Sum(item => item.Amount * item.Rate) :
                                                   gClient.Where(item => item.OrderID == gPayable.Key.OrderID && item.FeeSourceID == gPayable.Key.FeeSourceID)
                                                          .Sum(item => item.Amount * item.Rate)
                                      }
                       };

            return linq;
        }
    }
}
