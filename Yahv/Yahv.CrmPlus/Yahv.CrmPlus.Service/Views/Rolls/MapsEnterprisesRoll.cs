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
using Yahv.Underly.CrmPlus;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class MapsEnterprisesRoll : MapsEnterprisesOrigin
    {
        public MapsEnterprisesRoll()
        {


        }
        protected override IQueryable<Models.Origins.MapsEnterprise> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }

    public class MapsEnterpriseExtendRoll : MapsEnterpriseExtendOrigin
    {
        string enterpriseid;
        public MapsEnterpriseExtendRoll()
        { }
        public MapsEnterpriseExtendRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        protected override IQueryable<MapsEnterprise> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.MainID == this.enterpriseid || item.SubID == this.enterpriseid);
            }
            return base.GetIQueryable();
        }

        public  IQueryable<MapsEnterprise> this[ApplyTaskType task]
        {
            get {
                var ids = (from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                           where entity.ApplyTaskType == (int)task && entity.Status == (int)AuditStatus.Waiting
                           select entity.MainID).ToArray();

                return base.GetIQueryable().Where(x =>ids.Contains(x.ID));
            }
        }
    }

}
