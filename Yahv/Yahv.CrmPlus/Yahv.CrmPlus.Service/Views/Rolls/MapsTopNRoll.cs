using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class MapsTopNRoll : MapsTopNOrigin
    {
        IErpAdmin admin;

        public MapsTopNRoll(IErpAdmin Admin)
        {

            this.admin = Admin;
        }
        protected override IQueryable<Models.Origins.MapsTopN> GetIQueryable()
        {
            return base.GetIQueryable().Where(item=>item.OwnerID == this.admin.ID); ; 
        }
    }
}
