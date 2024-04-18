using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class OrderControlStepExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderControlSteps ToLinq(this Models.OrderControlStep entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderControlSteps
            {
                ID = entity.ID,
                OrderControlID = entity.OrderControlID,
                Step = (int)entity.Step,
                ControlStatus = (int)entity.ControlStatus,
                AdminID = entity.AdminID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
