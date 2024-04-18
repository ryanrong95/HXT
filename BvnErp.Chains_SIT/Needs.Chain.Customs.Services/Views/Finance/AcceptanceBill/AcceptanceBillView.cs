using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AcceptanceBillView : UniqueView<Models.AcceptanceBill, ScCustomsReponsitory>
    {
        public AcceptanceBillView()
        {
        }

        internal AcceptanceBillView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.AcceptanceBill> GetIQueryable()
        {
            var accountsView = new Views.FinanceAccountsView(this.Reponsitory);
            var adminsView = new Views.AdminsTopView(this.Reponsitory);

            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MoneyOrders>()
                         join inAccount in accountsView
                         on c.PayeeAccountID equals inAccount.ID
                         join outAccount in accountsView
                         on c.PayerAccountID equals outAccount.ID
                         join admin in adminsView on c.CreatorID equals admin.ID
                         select new Models.AcceptanceBill
                         {
                             ID = c.ID,
                             Type = (Enums.MoneyOrderType)c.Type,
                             Code = c.Code,
                             Name = c.Name,
                             BankCode = c.BankCode,
                             BankName = c.BankName,
                             BankNo = c.BankNo,
                             Currency = c.Currency,
                             Price = c.Price,
                             IsTransfer = c.IsTransfer,
                             IsMoney = c.IsMoney,
                             PayeeAccount = inAccount,
                             PayerAccount = outAccount,
                             StartDate = c.StartDate,
                             EndDate = c.EndDate,
                             Nature = (Enums.MoneyOrderNature)c.Nature,
                             ExchangePrice = c.ExchangePrice,
                             ExchangeDate = c.ExchangeDate,
                             Creator = admin,
                             BillStatus = (Enums.MoneyOrderStatus)c.BillStatus,
                             Endorser = c.Endorser,
                             Status = (Enums.Status)c.Status,
                             CreateDate = c.CreateDate,
                             UpdateDate = c.UpdateDate,
                             Summary = c.Summary,   
                             AccCreSta = c.AccCreSta,
                             AccCreWord = c.AccCreWord,
                             AccCreNo = c.AccCreNo,
                             BuyCreSta = c.BuyCreSta,
                             BuyCreWord = c.BuyCreWord,
                             BuyCreNo = c.BuyCreNo,
                             ReceiveBank = c.ReceiveBank,
                             RequestID = c.RequestID,
                             BuyRequestID = c.BuyRequestID,
                             AcceptedDate = c.AcceptedDate
                         };

            return result;
        }
    }
}
