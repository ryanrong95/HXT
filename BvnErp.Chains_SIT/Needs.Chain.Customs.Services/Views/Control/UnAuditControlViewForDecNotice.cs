using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 为处理的管控视图
    /// </summary>
    public class UnAuditControlViewForDecNotice : UniqueView<UnAuditControlViewForDecNoticeModel, ScCustomsReponsitory>
    {
        public UnAuditControlViewForDecNotice()
        {
            
        }

        protected override IQueryable<UnAuditControlViewForDecNoticeModel> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        public List<UnAuditControlViewForDecNoticeModel> GetUnAuditControlInfos(string orderID)
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
                                OrderID = orderControl.OrderID,
                            }
                            equals new
                            {
                                OrderControlID = orderControlStep.OrderControlID,
                                OrderControlDataStatus = (int)Enums.Status.Normal,
                                OrderControlStepDataStatus = orderControlStep.Status,
                                ControlStatus = orderControlStep.ControlStatus,
                                OrderID = orderID,
                            }
                       select new UnAuditControlViewForDecNoticeModel
                       {
                           OrderControlType = (Enums.OrderControlType)orderControl.ControlType,
                           OrderControlStep = (Enums.OrderControlStep)orderControlStep.Step,
                       };

            return linq.ToList();
        }

        public bool GetOrderIsHangUp(string orderID)
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var order = orders.Where(t => t.ID == orderID && t.Status == (int)Enums.Status.Normal).FirstOrDefault();

            if (order == null)
            {
                throw new Exception("不存在订单号为：" + orderID + " 的状态正常的订单！");
            }

            return order.IsHangUp;
        }

        public bool CancelOrderHangUp(string orderID)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { IsHangUp = false }, item => item.ID == orderID);
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }       
    }

    public class UnAuditControlViewForDecNoticeModel : IUnique
    {
        public string ID { get; set; } = string.Empty;

        public Enums.OrderControlType OrderControlType { get; set; }

        public Enums.OrderControlStep OrderControlStep { get; set; }
    }
}
