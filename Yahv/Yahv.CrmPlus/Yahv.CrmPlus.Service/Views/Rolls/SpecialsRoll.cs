using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class SpecialsRoll : Origins.SpecialsOrigin
    {
        string enterpriseid;
        public SpecialsRoll()
        {

        }
        public SpecialsRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }
        protected override IQueryable<Models.Origins.Special> GetIQueryable()
        {
            if (string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable();
            }
            return from entity in base.GetIQueryable()
                   where entity.EnterpriseID == this.enterpriseid
                   select entity;
        }
    }
}
