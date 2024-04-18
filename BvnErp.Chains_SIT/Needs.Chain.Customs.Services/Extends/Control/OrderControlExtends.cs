using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class OrderControlExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderControls ToLinq(this Models.OrderControlData entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderControls
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                OrderItemID = entity.OrderItemID,
                ControlType = (int)entity.ControlType,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}