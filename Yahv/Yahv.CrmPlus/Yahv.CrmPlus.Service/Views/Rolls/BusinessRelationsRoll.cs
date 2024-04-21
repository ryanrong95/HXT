using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class BusinessRelationsRoll : Origins.BusinessRelationsOrigin
    {
        public BusinessRelationsRoll()
        {

        }
        string enterpriseid;
        public BusinessRelationsRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }

        protected override IQueryable<BusinessRelation> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.MainID == this.enterpriseid || item.SubID == this.enterpriseid);
            }
            return base.GetIQueryable();
        }
        /// <summary>
        /// 审批任务中的关联关系，区分主体是客户还是供应商
        /// </summary>
        /// <returns></returns>
        public IQueryable<BusinessRelation> this[ApplyTaskType task]
        {
            get
            {
                var ids = (from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.ApplyTasks>()
                          where entity.ApplyTaskType == (int)task &&entity.Status==(int)AuditStatus.Waiting
                          select entity.MainID).ToArray();
                return base.GetIQueryable().Where(item => ids.Contains(item.ID));
               
            }
        }
      
    }

}
