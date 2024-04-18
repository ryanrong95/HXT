using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 换汇通知的视图
    /// </summary>
    public class SwapNoticeView : UniqueView<Models.SwapNotice, ScCustomsReponsitory>
    {
        public SwapNoticeView()
        {
        }

        internal SwapNoticeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SwapNotice> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var AccountView = new FinanceAccountsView(this.Reponsitory);
            var ItemView = new SwapNoticeItemView(this.Reponsitory);

            //var CurrencyView = new BaseCurrenciesView(this.Reponsitory);

            var result = from swapNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>()
                         join admin in adminView on swapNotice.AdminID equals admin.ID
                         join outAccount in AccountView on swapNotice.OutFinanceAccountID equals outAccount.ID into outAccounts
                         from outAccount in outAccounts.DefaultIfEmpty()
                         join inAccount in AccountView on swapNotice.InFinanceAccountID equals inAccount.ID into inAccounts
                         from inAccount in inAccounts.DefaultIfEmpty()
                         join midAccount in AccountView on swapNotice.MidFinanceAccountID equals midAccount.ID into midAccounts
                         from midAccount in midAccounts.DefaultIfEmpty()
                         //join currency in CurrencyView on swapNotice.Currency equals currency.Code into currencies
                         //from  Currency in currencies.DefaultIfEmpty()
                         join item in ItemView on swapNotice.ID equals item.SwapNoticeID into items
                         select new Models.SwapNotice
                         {
                             ID = swapNotice.ID,
                             Admin = admin,
                             BankName = swapNotice.BankName,
                             //Currency = currencies.FirstOrDefault().Name,
                             Currency = swapNotice.Currency,
                             TotalAmount = swapNotice.TotalAmount,
                             ExchangeRate = swapNotice.ExchangeRate == null ? 0M : (decimal)swapNotice.ExchangeRate,
                             TotalAmountCNY = swapNotice.TotalAmountCNY == null ? 0M : (decimal)swapNotice.TotalAmountCNY,
                             SwapStatus = (Enums.SwapStatus)swapNotice.Status,
                             CreateDate = swapNotice.CreateDate,
                             UpdateDate = swapNotice.UpdateDate,
                             OutAccount = outAccount,
                             InAccount = inAccount,
                             MidAccount = midAccount,
                             Poundage = swapNotice.Poundage == null ? 0M : (decimal)swapNotice.Poundage,
                             Items = items,
                             ConsignorCode = swapNotice.ConsignorCode,
                             uid = swapNotice.uid
                         };
            return result;
        }
    }
}
