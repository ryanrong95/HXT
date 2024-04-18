using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AdvanceMoneyApplyLogView : UniqueView<Models.AdvanceMoneyApplyLogModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyApplyLogView()
        {
        }
        internal AdvanceMoneyApplyLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.AdvanceMoneyApplyLogModel> GetIQueryable()
        {
            var result = from applyLogs in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplyLogs>()
                         select new Models.AdvanceMoneyApplyLogModel
                         {
                             ID = applyLogs.ID,
                             ApplyID = applyLogs.ApplyID,
                             AdminID = applyLogs.AdminID,
                             Summary = applyLogs.Summary,
                             CreateDate = applyLogs.CreateDate
                         };

            return result;
        }
    }
}
