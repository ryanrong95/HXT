using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 排除用户的、排除非用户的已处理的 剩下的未处理的管控
    /// </summary>
    public class ExceptUserUnAuditControlView : UniqueView<Models.OrderControlData, ScCustomsReponsitory>
    {
        public ExceptUserUnAuditControlView()
        {
        }

        internal ExceptUserUnAuditControlView(ScCustomsReponsitory reponsitory)
        {
        }

        protected override IQueryable<OrderControlData> GetIQueryable()
        {
            var orderControls = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var orderControlSteps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

            var linq = from orderControl in orderControls
                       join orderControlStep in orderControlSteps
                            on new
                            {
                                OrderControlID = orderControl.ID,
                                OrderControlDataStatus = orderControl.Status,
                                OrderControlStepDataStatus = (int)Enums.Status.Normal,
                                ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                            }
                            equals new
                            {
                                OrderControlID = orderControlStep.OrderControlID,
                                OrderControlDataStatus = (int)Enums.Status.Normal,
                                OrderControlStepDataStatus = orderControlStep.Status,
                                ControlStatus = orderControlStep.ControlStatus,
                            }
                       where (orderControl.ControlType != (int)Enums.OrderControlType.DeleteModel && orderControl.ControlType != (int)Enums.OrderControlType.ChangeQuantity)
                       select new Models.OrderControlData
                       {
                           ID = orderControl.ID,
                           OrderID = orderControl.OrderID,
                           OrderItemID = orderControl.OrderItemID,
                           ControlType = (Enums.OrderControlType)orderControl.ControlType,
                           Status = (Enums.Status)orderControl.Status,
                           CreateDate = orderControl.CreateDate,
                           UpdateDate = orderControl.UpdateDate,
                           Summary = orderControl.Summary
                       };

            return linq;
        }
    }
}
