using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls.Samples
{
    public class SampleRoll : Origins.SampleOrigin
    {
        public SampleRoll()
        {

        }

        protected override IQueryable<Sample> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    public class MySampleRoll : Origins.SampleOrigin
    {

        IErpAdmin Admin;
        public MySampleRoll() { }
        public MySampleRoll(IErpAdmin admin)
        {

            this.Admin = admin;
        }
        protected override IQueryable<Sample> GetIQueryable()
        {
            if (Admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else
            {

                return base.GetIQueryable().Where(item => item.ApplierID == Admin.ID);
            }
        }

    }
}
