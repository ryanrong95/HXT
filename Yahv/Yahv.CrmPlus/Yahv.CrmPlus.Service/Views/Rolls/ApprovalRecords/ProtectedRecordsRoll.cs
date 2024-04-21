using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;

namespace Yahv.CrmPlus.Service.Views.Rolls.ApprovalRecords
{
    public class ProtectedRecordsRoll : Origins.ProtectApplysOrigin
    {
        public ProtectedRecordsRoll()
        {
        }

        public ProtectedRecordsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProtectApply> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }
}

