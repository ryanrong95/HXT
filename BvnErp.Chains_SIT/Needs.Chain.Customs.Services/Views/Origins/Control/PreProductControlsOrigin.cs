using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    /// <summary>
    /// 预归类产品管控审批视图
    /// </summary>
    public class PreProductControlsOrigin : Needs.Linq.UniqueView<Models.PreProductControl, ScCustomsReponsitory>
    {
        public PreProductControlsOrigin()
        {
        }

        internal PreProductControlsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PreProductControl> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductControls>()
                   select new Models.PreProductControl
                   {
                       ID = entity.ID,
                       PreProductID = entity.PreProductID,
                       Type = (Enums.ItemCategoryType)entity.Type,
                       Status = (Enums.PreProductControlStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       ApproveDate = entity.ApproveDate,
                       ApproverID = entity.ApproverID,
                       Summary = entity.Summary
                   };
        }
    }
}
