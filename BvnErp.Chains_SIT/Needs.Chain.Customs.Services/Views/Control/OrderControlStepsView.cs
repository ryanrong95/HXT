using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    public class OrderControlStepsView : UniqueView<Models.OrderControlStep, ScCustomsReponsitory>
    {
        public OrderControlStepsView()
        {
        }

        internal OrderControlStepsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderControlStep> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>()
                   select new Models.OrderControlStep
                   {
                       ID = entity.ID,
                       OrderControlID = entity.OrderControlID,
                       Step = (Enums.OrderControlStep)entity.Step,
                       ControlStatus = (Enums.OrderControlStatus)entity.ControlStatus,
                       AdminID = entity.AdminID,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
