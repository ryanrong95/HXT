using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyPayExchangeAppliesView : View<Needs.Wl.Client.Services.Models.UserPayExchangeApply, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyPayExchangeAppliesView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.Models.UserPayExchangeApply> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return this.MainUserPayExchangeApplies();
            }
            else
            {
                return this.UserPayExchangeApplies();
            }
        }

        private IQueryable<Needs.Wl.Client.Services.Models.UserPayExchangeApply> UserPayExchangeApplies()
        {
            return from payApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on payApply.ClientID equals client.ID
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>() on new { payApplyID = payApply.ID, itemStatus = (int)Needs.Wl.Models.Enums.Status.Normal } equals new { payApplyID = item.PayExchangeApplyID, itemStatus = item.Status } into items
                   join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on payApply.UserID equals userTable.ID into users
                   from user in users.DefaultIfEmpty()
                   join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on payApply.AdminID equals adminTable.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where payApply.Status == (int)Needs.Wl.Models.Enums.Status.Normal && payApply.UserID == this.User.ID
                   orderby payApply.CreateDate descending
                   select new Needs.Wl.Client.Services.Models.UserPayExchangeApply
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
                       ExchangeRateType = (Needs.Wl.Models.Enums.ExchangeRateType)payApply.ExchangeRateType,
                       ExpectPayDate = payApply.ExpectPayDate,
                       SettlemenDate = payApply.SettlemenDate,
                       PayExchangeApplyStatus = (Needs.Wl.Models.Enums.PayExchangeApplyStatus)payApply.PayExchangeApplyStatus,
                       PaymentType = (Needs.Wl.Models.Enums.PaymentType)payApply.PaymentType,
                       OtherInfo = payApply.OtherInfo,
                       Items = new Needs.Wl.Models.PayExchangeApplyItems(items.Select(item => new Needs.Wl.Models.PayExchangeApplyItem
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
                       User = user == null ? null : new Needs.Wl.Models.User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName
                       },
                       Admin = admin == null ? null : new Needs.Wl.Models.Admin()
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

        private IQueryable<Needs.Wl.Client.Services.Models.UserPayExchangeApply> MainUserPayExchangeApplies()
        {
            return from payApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on payApply.ClientID equals client.ID
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>() on new { payApplyID = payApply.ID, itemStatus = (int)Needs.Wl.Models.Enums.Status.Normal } equals new { payApplyID = item.PayExchangeApplyID, itemStatus = item.Status } into items
                   join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on payApply.UserID equals userTable.ID into users
                   from user in users.DefaultIfEmpty()
                   join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on payApply.AdminID equals adminTable.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where payApply.Status == (int)Needs.Wl.Models.Enums.Status.Normal && payApply.ClientID == this.User.Client.ID
                   orderby payApply.CreateDate descending
                   select new Needs.Wl.Client.Services.Models.UserPayExchangeApply
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
                       ExchangeRateType = (Needs.Wl.Models.Enums.ExchangeRateType)payApply.ExchangeRateType,
                       ExpectPayDate = payApply.ExpectPayDate,
                       SettlemenDate = payApply.SettlemenDate,
                       PayExchangeApplyStatus = (Needs.Wl.Models.Enums.PayExchangeApplyStatus)payApply.PayExchangeApplyStatus,
                       PaymentType = (Needs.Wl.Models.Enums.PaymentType)payApply.PaymentType,
                       OtherInfo = payApply.OtherInfo,
                       Items = new Needs.Wl.Models.PayExchangeApplyItems(items.Select(item => new Needs.Wl.Models.PayExchangeApplyItem
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
                       User = user == null ? null : new Needs.Wl.Models.User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName
                       },
                       Admin = admin == null ? null : new Needs.Wl.Models.Admin()
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