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
    public class MyBeneficiariesView : BeneficiariesAlls
    {
        IGenericAdmin Admin;

        MyBeneficiariesView()
        {

        }

        internal MyBeneficiariesView(BvCrmReponsitory bv) : base(bv)
        {

        }

        public MyBeneficiariesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }
        protected override IQueryable<Beneficiaries> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Status != Enums.Status.Delete
                   select entity;
        }
    }
}
