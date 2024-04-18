using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ApplyStepAlls : QueryView<ApplyStep, BvCrmReponsitory>
    {
        public ApplyStepAlls()
        {

        }

        protected override IQueryable<ApplyStep> GetIQueryable()
        {
            return from applystep in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ApplySteps>()
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>()
                   on applystep.AdminID equals admin.ID
                   select new ApplyStep
                   {
                       ApplyID = applystep.ApplyID,
                       Step = applystep.Step,
                       Status = (Enums.ApplyStep)applystep.Status,
                       Comment = applystep.Comment,
                       AdminID = applystep.AdminID,
                       AdminName = admin.RealName,
                       AprDate = applystep.Aprdate,
                   };
        }
    }
}
