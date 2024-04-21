using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    /// <summary>
    /// 待审批客户
    /// </summary>
    public class WaitingClientsRoll : WaitingClientsOrigin
    {

        public WaitingClientsRoll() { }
        protected override IQueryable<Models.Origins.WatingClient> GetIQueryable()
        {
            return base.GetIQueryable(); ;
        }

    }

    public class _ConductsRoll : ConductsOrigin
    {
        string enterpriseid;
        public _ConductsRoll()
        {


        }

        public _ConductsRoll(string enterpriseid)
        {
            this.enterpriseid = enterpriseid;
        }
        protected override IQueryable<Models.Origins.Conduct> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
            }
            return base.GetIQueryable();
        }
    }
}
