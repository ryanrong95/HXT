using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 出库交货信息扩展方法
    /// </summary>
    public static class ExitDeliverExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ExitDelivers ToLinq(this Models.ExitDeliver entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ExitDelivers
            {
                ID = entity.ID,
                ExitNoticeID = entity.ExitNoticeID,
                Code = entity.Code,
                Name = entity.Name,
                ConsigneeID = entity.Consignee?.ID,
                DeliverID = entity.Deliver?.ID,
                ExpressageID = entity.Expressage?.ID,
                PackNo = entity.PackNo,
                DeliverDate = entity.DeliverDate,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
