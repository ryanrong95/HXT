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
    public class PayersTopView<TReponsitory> : QueryView<Payer, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public PayersTopView()
        {

        }

        public PayersTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Payer> GetIQueryable()
        {
            var enterpriseView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.EnterprisesTopView>();

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.PayersTopView>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join realets in enterpriseView on entity.RealID equals realets.ID into real
                   where entity.Status == (int)GeneralStatus.Normal
                   from realeterprise in real.DefaultIfEmpty()
                   select new Payer
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       EnterpriseName= enterprise.Name,
                       RealID = entity.RealID,
                       RealName = realeterprise.Name,
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
                       Status=(GeneralStatus)entity.Status
                   };
        }
    }
}
