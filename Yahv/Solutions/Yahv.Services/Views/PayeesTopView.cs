using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class PayeesTopView<TReponsitory> : QueryView<Payee, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public PayeesTopView()
        {

        }

        public PayeesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Payee> GetIQueryable()
        {
            var enterpriseView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.EnterprisesTopView>();
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.PayeesTopView>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join realenterprises in enterpriseView on entity.RealID equals realenterprises.ID into et
                   where entity.Status == (int)GeneralStatus.Normal
                   from realenterprise in et.DefaultIfEmpty()
                   select new Payee
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       EnterpriseName = enterprise.Name,
                       RealID = entity.RealID,
                       RealName = realenterprise.Name,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       Currency = (Currency)entity.Currency,
                       Methord = (Methord)entity.Methord,
                       Name = entity.Name,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       SwiftCode = entity.SwiftCode,
                       Tel = entity.Tel,
                       Status = (GeneralStatus)entity.Status
                   };
        }
    }
}
