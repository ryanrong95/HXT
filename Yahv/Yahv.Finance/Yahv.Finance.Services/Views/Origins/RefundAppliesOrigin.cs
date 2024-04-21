using System.Linq;
using System.Security.Cryptography;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 退款申请 原始视图
    /// </summary>
    public class RefundAppliesOrigin : UniqueView<RefundApply, PvFinanceReponsitory>
    {
        public RefundAppliesOrigin()
        {
        }

        public RefundAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<RefundApply> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.RefundApplies>()
                   select new RefundApply()
                   {
                       AccountCatalogID = entity.AccountCatalogID,
                       ApplierID = entity.ApplierID,
                       ApproverID = entity.ApproverID,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Currency = (Currency)entity.Currency,
                       ExcuterID = entity.ExcuterID,
                       FlowID = entity.FlowID,
                       ID = entity.ID,
                       PayeeAccountID = entity.PayeeAccountID,
                       PayeeLeftID = entity.PayeeLeftID,
                       PayerAccountID = entity.PayerAccountID,
                       Price = entity.Price,
                       SenderID = entity.SenderID,
                       Status = (ApplyStauts)entity.Status,
                       Summary = entity.Summary,
                       Type = (FlowAccountType)entity.Type,
                   };
        }
    }
}