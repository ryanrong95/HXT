using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Yahv.Linq;
using Yahv.Payments;
using Yahv.Payments.Models.Origins;
using Yahv.Payments.Views.Rolls;
using Yahv.Underly;

namespace Yahv.Payments.Views.Origins
{
    /// <summary>
    /// 流水账视图
    /// </summary>
    public class FlowAccountsOrigin : UniqueView<FlowAccount, PvbCrmReponsitory>
    {
        internal FlowAccountsOrigin()
        {

        }

        internal FlowAccountsOrigin(PvbCrmReponsitory repository) : base(repository)
        {

        }

        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            var admins = new AdminsAllRoll(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<FlowAccounts>()
                   join admin in admins on entity.AdminID equals admin.ID into joinAdmin
                   from admin in joinAdmin.DefaultIfEmpty()
                   select new FlowAccount()
                   {
                       Subject = entity.Subject,
                       ID = entity.ID,
                       Type = (AccountType)entity.Type,
                       Currency = (Currency)entity.Currency,
                       Business = entity.Business,
                       Price = entity.Price,
                       Price1 = entity.Price1,
                       ERate1 = entity.ERate1,
                       OrderID = entity.OrderID,
                       Admin = admin,
                       Currency1 = (Currency)entity.Currency1,
                       Bank = entity.Bank,
                       FormCode = entity.FormCode,
                       Catalog = entity.Catalog,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       CreateDate = entity.CreateDate,
                       ChangeDate = entity.ChangeDate,
                       WaybillID = entity.WaybillID,
                       OriginalDate = entity.OriginalDate,
                       ChangeIndex = entity.ChangeIndex,
                       OriginIndex = entity.OriginIndex,
                       DateIndex = entity.DateIndex,
                       Account = entity.Account,
                   };
        }
    }
}
