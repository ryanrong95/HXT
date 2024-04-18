using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{

    public class PayExchangeAppliesView<T> : UniqueView<T, ScCustomsReponsitory> where T : Models.PayExchangeApply, new()
    {
        public PayExchangeAppliesView()
        {

        }

        internal PayExchangeAppliesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<T> GetIQueryable()
        {
            var clientView = new ClientsView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);

            //var TotalAmount = from payApplyItems in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
            //                  group payApplyItems by payApplyItems.PayExchangeApplyID into payApplyItem
            //                  select new
            //                  {
            //                      PayExchangeApplyID = payApplyItem.Key,
            //                      TotalAmount = payApplyItem.Sum(t => t.Amount),
            //                  };

            return from payApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                       // join totalAmount in TotalAmount on payApply.ID equals totalAmount.PayExchangeApplyID
                   join client in clientView on payApply.ClientID equals client.ID
                   join user in usersView on payApply.UserID equals user.ID into users
                   from _user in users.DefaultIfEmpty()
                   join admin in adminsView on payApply.AdminID equals admin.ID into admins
                   from _admin in admins.DefaultIfEmpty()
                   where payApply.Status == (int)Enums.Status.Normal
                   select new T
                   {
                       ID = payApply.ID,
                       SupplierName = payApply.SupplierName,
                       SupplierEnglishName = payApply.SupplierEnglishName,
                       SupplierAddress = payApply.SupplierAddress,
                       BankName = payApply.BankName,
                       BankAccount = payApply.BankAccount,
                       BankAddress = payApply.BankAddress,
                       SwiftCode = payApply.SwiftCode,
                       ClientID = payApply.ClientID,
                       Client = client,
                       Currency = payApply.Currency,
                       ExchangeRate = payApply.ExchangeRate,
                       ExchangeRateType = (Enums.ExchangeRateType)payApply.ExchangeRateType,
                       ExpectPayDate = payApply.ExpectPayDate,
                       SettlemenDate = payApply.SettlemenDate,
                       PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)payApply.PayExchangeApplyStatus,
                       PaymentType = (Enums.PaymentType)payApply.PaymentType,
                       OtherInfo = payApply.OtherInfo,
                       User = _user,
                       Admin = _admin,
                       Status = (Enums.Status)payApply.Status,
                       CreateDate = payApply.CreateDate,
                       UpdateDate = payApply.UpdateDate,
                       Summary = payApply.Summary,
                       // TotalAmount = totalAmount.TotalAmount,
                       ABA = payApply.ABA,
                       IBAN = payApply.IBAN,
                       FatherID = payApply.FatherID,
                       IsAdvanceMoney = payApply.IsAdvanceMoney,
                       DyjID = payApply.DyjID,
                       HandlingFeePayerType = payApply.HandlingFeePayerType,
                       HandlingFee = payApply.HandlingFee,
                       USDRate = payApply.USDRate
                   };
        }
    }

    /// <summary>
    /// 付汇申请视图（管理端）
    /// </summary>
    public class AdminPayExchangeApplyView : PayExchangeAppliesView<Models.AdminPayExchangeApply>
    {
        public AdminPayExchangeApplyView() : base()
        {
        }

        internal AdminPayExchangeApplyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.AdminPayExchangeApply> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    /// <summary>
    /// 付汇申请视图（待审核）
    /// </summary>
    public class UnAuditedPayExchangeApplyView : PayExchangeAppliesView<Models.UnAuditedPayExchangeApply>
    {
        protected override IQueryable<Models.UnAuditedPayExchangeApply> GetIQueryable()
        {
            return from apply in base.GetIQueryable()
                   where apply.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Auditing
                   select apply;
        }
    }

    /// <summary>
    /// 付汇申请视图（待审批）
    /// </summary>
    public class UnApprovalPayExchangeApplyView : PayExchangeAppliesView<Models.UnApprovalPayExchangeApply>
    {
        protected override IQueryable<Models.UnApprovalPayExchangeApply> GetIQueryable()
        {
            return from apply in base.GetIQueryable()
                   where apply.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Audited
                   select apply;
        }
    }

    /// <summary>
    /// 付汇申请视图（待完成）
    /// 
    /// 
    ///         
    /// </summary>
    public class UnCompletePayExchangeApplyView : PayExchangeAppliesView<Models.UnCompletePayExchangeApply>
    {
        protected override IQueryable<Models.UnCompletePayExchangeApply> GetIQueryable()
        {
            return from apply in base.GetIQueryable()
                   where apply.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Approvaled
                   select apply;
        }
    }
}