using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 承兑账户 原始视图
    /// </summary>
    public class MoneyOrdersOrigin : UniqueView<MoneyOrder, PvFinanceReponsitory>
    {
        public MoneyOrdersOrigin()
        {
        }

        public MoneyOrdersOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MoneyOrder> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MoneyOrders>()
                   select new MoneyOrder()
                   {
                       BankCode = entity.BankCode,
                       BankName = entity.BankName,
                       BankNo = entity.BankNo,
                       Code = entity.Code,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Currency = (Currency)entity.Currency,
                       EndDate = entity.EndDate,
                       ExchangeDate = entity.ExchangeDate,
                       ExchangePrice = entity.ExchangePrice,
                       ID = entity.ID,
                       Name = entity.Name,
                       PayerAccountID = entity.PayerAccountID,
                       Price = entity.Price,
                       IsTransfer = entity.IsTransfer,
                       ModifierID = entity.ModifierID,
                       ModifyDate = entity.ModifyDate,
                       Nature = (MoneyOrderNature)entity.Nature,
                       PayeeAccountID = entity.PayeeAccountID,
                       StartDate = entity.StartDate,
                       Status = (MoneyOrderStatus)entity.Status,
                       Type = (MoneyOrderType)entity.Type,
                       IsMoney = entity.IsMoney,
                   };
        }
    }
}