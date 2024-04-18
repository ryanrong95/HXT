using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 申请中客户的收款人视图
    /// </summary>
    public class ApplicationPayeesOrigin : UniqueView<ApplicationPayee, PvWsOrderReponsitory>
    {
        protected ApplicationPayeesOrigin()
        {

        }

        public ApplicationPayeesOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ApplicationPayee> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationPayees>()
                       select new ApplicationPayee()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           PayeeID = entity.PayeeID,
                           EnterpriseID = entity.EnterpriseID,
                           EnterpriseName = entity.EnterpriseName,
                           BankName = entity.BankName,
                           BankAccount = entity.BankAccount,
                           Method = (Methord)entity.Method,
                           Currency = (Currency)entity.Currency,
                           Amount = entity.Amount
                       };
            return linq;
        }
    }
}
