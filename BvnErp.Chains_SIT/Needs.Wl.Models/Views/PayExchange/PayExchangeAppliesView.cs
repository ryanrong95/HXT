using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class PayExchangeAppliesView : View<Models.PayExchangeApply, ScCustomsReponsitory>
    {
        public PayExchangeAppliesView()
        {

        }

        internal PayExchangeAppliesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeApply> GetIQueryable()
        {
            return from payApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on payApply.ClientID equals client.ID
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>() on payApply.ID equals item.PayExchangeApplyID into items
                   join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on payApply.UserID equals userTable.ID into users
                   from user in users.DefaultIfEmpty()
                   join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on payApply.AdminID equals adminTable.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where payApply.Status == (int)Enums.Status.Normal
                   select new Models.PayExchangeApply
                   {
                       ID = payApply.ID,
                       ClientID = payApply.ClientID,
                       SupplierName = payApply.SupplierName,
                       SupplierEnglishName = payApply.SupplierEnglishName,
                       SupplierAddress = payApply.SupplierAddress,
                       BankName = payApply.BankName,
                       BankAccount = payApply.BankAccount,
                       BankAddress = payApply.BankAddress,
                       SwiftCode = payApply.SwiftCode,
                       Currency = payApply.Currency,
                       ExchangeRate = payApply.ExchangeRate,
                       ExchangeRateType = (Enums.ExchangeRateType)payApply.ExchangeRateType,
                       ExpectPayDate = payApply.ExpectPayDate,
                       SettlemenDate = payApply.SettlemenDate,
                       PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)payApply.PayExchangeApplyStatus,
                       PaymentType = (Enums.PaymentType)payApply.PaymentType,
                       OtherInfo = payApply.OtherInfo,
                       Items = new PayExchangeApplyItems(items.Select(item => new PayExchangeApplyItem
                       {
                           ID = item.ID,
                           PayExchangeApplyID = item.PayExchangeApplyID,
                           OrderID = item.OrderID,
                           Amount = item.Amount,
                           Status = item.Status,
                           CreateDate = item.CreateDate,
                           UpdateDate = item.UpdateDate,
                           Summary = item.Summary
                       })),
                       User = user == null ? null : new User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName
                       },
                       Admin = admin == null ? null : new Admin()
                       {
                           ID = admin.ID,
                           UserName = admin.UserName,
                           RealName = admin.RealName
                       },
                       Status = payApply.Status,
                       CreateDate = payApply.CreateDate,
                       UpdateDate = payApply.UpdateDate,
                       Summary = payApply.Summary
                   };
        }
    }
}