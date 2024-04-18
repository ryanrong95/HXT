using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 受益人视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class BeneficiariesTopView<TReponsitory> : UniqueView<Beneficiary, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public BeneficiariesTopView()
        { 
        }

        public BeneficiariesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Beneficiary> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.BeneficiariesTopView>()
                   select new Beneficiary
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Account = entity.Account,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Currency = (Currency)entity.Currency,
                       //District = (District)entity.District,
                       Email = entity.Email,
                       EnterpriseID = entity.EnterpriseID,
                       //Methord = (Methord)entity.Methord,
                       Mobile = entity.Mobile,
                       RealName = entity.RealName,
                       SwiftCode = entity.SwiftCode,
                       Tel = entity.Tel,
                       //InvoiceType = (InvoiceType)entity.InvoiceType
                   };
        }
    }
}
