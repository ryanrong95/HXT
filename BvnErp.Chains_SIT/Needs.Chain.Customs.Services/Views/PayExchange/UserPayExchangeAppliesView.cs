using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{

    public class UserPayExchangeAppliesView : UniqueView<Models.UserPayExchangeApply, ScCustomsReponsitory>
    {
        public UserPayExchangeAppliesView()
        {

        }

        internal UserPayExchangeAppliesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.UserPayExchangeApply> GetIQueryable()
        {
            var clientView = new ClientsView(this.Reponsitory);
            var usersView = new UsersView(this.Reponsitory);
            var logsView = new PayExchangeLogsView(this.Reponsitory);

            return from payApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>() on payApply.ID equals item.PayExchangeApplyID into items
                   join client in clientView on payApply.ClientID equals client.ID
                   join user in usersView on payApply.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   join log in logsView on payApply.ID equals log.PayExchangeApplyID into logs
                   where payApply.Status == (int)Enums.Status.Normal
                   select new Models.UserPayExchangeApply
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
                       User = user,
                       Status = (Enums.Status)payApply.Status,
                       CreateDate = payApply.CreateDate,
                       UpdateDate = payApply.UpdateDate,
                       Summary = payApply.Summary,
                       CompletedDate = logs.Where(t => t.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Completed).FirstOrDefault().CreateDate,
                       TotalAmount = items.Select(c => c.Amount).Sum(),
                   };
        }
    }
}