using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AvailableProductFeeView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        private string ClientID { get; set; }

        public AvailableProductFeeView(string clientID)
        {
            this.Reponsitory = new ScCustomsReponsitory();
            this.ClientID = clientID;
        }

        public AvailableProductFeeView(ScCustomsReponsitory reponsitory, string clientID)
        {
            this.Reponsitory = reponsitory;
            this.ClientID = clientID;
        }

        public decimal GetProductUpperLimit()
        {
            decimal upperLimit = 0;

            var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            var clientFeeSettlements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>();

            var resultModel = (from clientAgreement in clientAgreements
                               join clientFeeSettlement in clientFeeSettlements on clientAgreement.ID equals clientFeeSettlement.AgreementID
                               where clientAgreement.ClientID == this.ClientID && clientAgreement.Status == (int)Enums.Status.Normal
                                  && clientFeeSettlement.FeeType == (int)Enums.FeeType.Product && clientFeeSettlement.Status == (int)Enums.Status.Normal
                               select new
                               {
                                   clientFeeSettlement.UpperLimit,
                               }).FirstOrDefault();

            if (resultModel != null && resultModel.UpperLimit != null)
            {
                upperLimit = (decimal)resultModel.UpperLimit;
            }

            return upperLimit;
        }

        //垫款可用额度
        public decimal GetProductAdvanceMoneyApply()
        {
            decimal advanceMoney = 0;

            var result = (from advanceMoneyApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>()
                          where advanceMoneyApply.ClientID == this.ClientID && advanceMoneyApply.Status == (int)Enums.AdvanceMoneyStatus.Effective
                          select new
                          {
                              advanceMoneyApply.Amount,
                              advanceMoneyApply.AmountUsed,
                          }).FirstOrDefault();

            if (result != null && result.Amount >= 0)
            {
                advanceMoney = result.Amount - result.AmountUsed;
            }

            return advanceMoney;
        }
        public decimal GetProductPayable()
        {
            decimal productPayable = 0;

            var productPayableModel = (from player in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on player.OrderID equals order.ID
                                       where order.IsLoan && player.FeeType == (int)Enums.OrderFeeType.Product && player.Status == (int)Enums.Status.Normal
                                          && player.ClientID == this.ClientID
                                       group player by player.ClientID into playerGroup
                                       select new
                                       {
                                           ClientID = playerGroup.Key,
                                           ProductPayable = playerGroup.Sum(x => x.Amount),
                                       }).FirstOrDefault();

            if (productPayableModel != null)
            {
                productPayable = productPayableModel.ProductPayable;
            }

            return productPayable;
        }

    }
}
