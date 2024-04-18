using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class BeneficiariesAlls : UniqueView<Beneficiaries, BvCrmReponsitory>, Needs.Underly.IFkoView<Beneficiaries>
    {
       public  BeneficiariesAlls()
        {

        }
        internal BeneficiariesAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<Beneficiaries> GetIQueryable()
        {
            var companyview = new CompanyAlls(this.Reponsitory);
            return from benifit in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Beneficiaries>()
                   join company in companyview on benifit.CompanyID equals company.ID
                   select new Beneficiaries
                   {
                       ID = benifit.ID,
                       Bank = benifit.Bank,
                       BankCode = benifit.BankCode,
                       CompanyID = benifit.CompanyID,
                       Company = company,
                       Address = benifit.Address,
                       Status = (Enums.Status)benifit.Status,
                       CreateDate = benifit.CreateDate,
                       UpdateDate = benifit.UpdateDate
                   };
        }
    }
}
