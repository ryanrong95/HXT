using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 根据 OrderID 查询用户本次申请金额列表
    /// </summary>
    public class UserCurrentPayApplyListView : View<UserCurrentPayApplyListModel, ScCustomsReponsitory>
    {
        private string OrderID { get; set; } = string.Empty;

        public UserCurrentPayApplyListView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<UserCurrentPayApplyListModel> GetIQueryable()
        {
            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();

            return from payExchangeApplyItem in payExchangeApplyItems
                   join payExchangeApply in payExchangeApplies
                            on new
                            {
                                PayExchangeApplyID = payExchangeApplyItem.PayExchangeApplyID,
                                PayExchangeApplyDataStatus = (int)Enums.Status.Normal,
                                //PayExchangeApplyStatus = (int)Enums.PayExchangeApplyStatus.Approvaled,
                            }
                            equals new
                            {
                                PayExchangeApplyID = payExchangeApply.ID,
                                PayExchangeApplyDataStatus = payExchangeApply.Status,
                                //PayExchangeApplyStatus = payExchangeApply.PayExchangeApplyStatus,
                            }
                   where payExchangeApplyItem.OrderID == this.OrderID
                      && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                      && payExchangeApplyItem.ApplyStatus == (int)Enums.ApplyItemStatus.Appling
                      && (payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Approvaled
                            || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Completed)
                   orderby payExchangeApplyItem.CreateDate
                   select new UserCurrentPayApplyListModel
                   {
                       PayExchangeApplyItemID = payExchangeApplyItem.ID,
                       CurrentPayApplyAmount = payExchangeApplyItem.Amount * ConstConfig.TransPremiumInsurance,
                       ApplyTime = payExchangeApplyItem.CreateDate,
                   };
        }
    }

    /// <summary>
    /// 根据 DecHeadID 查询用户本次申请金额列表
    /// </summary>
    public class UserCurrentPayApplyListByDecHeeadIDView : View<UserCurrentPayApplyListModel, ScCustomsReponsitory>
    {
        private string[] DecHeadIDs { get; set; }

        public UserCurrentPayApplyListByDecHeeadIDView(string[] decHeadIDs)
        {
            this.DecHeadIDs = decHeadIDs;
            this.AllowPaging = false;
        }

        protected override IQueryable<UserCurrentPayApplyListModel> GetIQueryable()
        {
            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            return from payExchangeApplyItem in payExchangeApplyItems
                   join decHead in decHeads on payExchangeApplyItem.OrderID equals decHead.OrderID
                   join payExchangeApply in payExchangeApplies
                            on new
                            {
                                PayExchangeApplyID = payExchangeApplyItem.PayExchangeApplyID,
                                PayExchangeApplyDataStatus = (int)Enums.Status.Normal,
                                //PayExchangeApplyStatus = (int)Enums.PayExchangeApplyStatus.Approvaled,
                            }
                            equals new
                            {
                                PayExchangeApplyID = payExchangeApply.ID,
                                PayExchangeApplyDataStatus = payExchangeApply.Status,
                                //PayExchangeApplyStatus = payExchangeApply.PayExchangeApplyStatus,
                            }
                   where payExchangeApplyItem.Status == (int)Enums.Status.Normal
                      && payExchangeApplyItem.ApplyStatus == (int)Enums.ApplyItemStatus.Appling
                      && this.DecHeadIDs.Contains(decHead.ID)
                      && decHead.CusDecStatus != "04"
                      && (payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Approvaled
                            || payExchangeApply.PayExchangeApplyStatus == (int)Enums.PayExchangeApplyStatus.Completed)
                   orderby payExchangeApplyItem.CreateDate
                   select new UserCurrentPayApplyListModel
                   {
                       DecHeadID = decHead.ID,
                       PayExchangeApplyItemID = payExchangeApplyItem.ID,
                       CurrentPayApplyAmount = payExchangeApplyItem.Amount * ConstConfig.TransPremiumInsurance,
                       ApplyTime = payExchangeApplyItem.CreateDate,
                   };
        }
    }

    public class UserCurrentPayApplyListModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        public string DecHeadID { get; set; } = string.Empty;

        public string PayExchangeApplyItemID { get; set; } = string.Empty;

        public decimal CurrentPayApplyAmount { get; set; }

        public DateTime ApplyTime { get; set; }
    }


}
