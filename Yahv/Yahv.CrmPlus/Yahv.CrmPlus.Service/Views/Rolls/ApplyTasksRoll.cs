using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views.Rolls
{

    public class ApplyTasksRoll : Origins.ApplyTasksOrigin
    {
        public ApplyTasksRoll()
        {
        }

        public ApplyTasksRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ApplyTask> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }
}
