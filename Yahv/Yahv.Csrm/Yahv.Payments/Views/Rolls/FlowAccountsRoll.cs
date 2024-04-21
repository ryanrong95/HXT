using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Payments.Models.Origins;
using Yahv.Payments.Views.Origins;
using Yahv.Underly;

namespace Yahv.Payments.Views.Rolls
{
    /// <summary>
    /// 流水账视图
    /// </summary>
    public class FlowAccountsRoll : FlowAccountsOrigin
    {
        public FlowAccountsRoll()
        {

        }

        public FlowAccountsRoll(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            return base.GetIQueryable();
        }


        public override FlowAccount this[string id]
        {
            get { return this.SingleOrDefault(item => item.ID == id); }
        }

        public void Add(FlowAccount entity)
        {
            var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];

            Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
            {
                ID = PKeySigner.Pick(PKeyType.FlowAccount),
                Currency = (int)entity.Currency,
                Price = entity.Price,
                Business = entity.Business,
                Catalog = entity.Catalog,
                Payer = entity.Payer,
                Payee = entity.Payee,
                CreateDate = DateTime.Now,
                Subject = entity.Subject,
                AdminID = entity.Admin.ID,
                Type = (int)entity.Type,
                OrderID = entity.OrderID,
                FormCode = entity.FormCode,
                Currency1 = (int)Currency.CNY,
                ERate1 = rate,
                Price1 = entity.Price * rate,
                OriginIndex = entity.OriginIndex,
                OriginalDate = entity.OriginalDate,
                ChangeIndex = entity.ChangeIndex,
                ChangeDate = entity.ChangeDate,
                DateIndex = entity.DateIndex,
            });
        }
    }
}
