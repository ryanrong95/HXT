using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 外单删除型号 Do
    /// </summary>
    public class ArrivalOutsideDeleteModel : DeleteModelDo
    {
        private Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; set; }

        private string OrderItemID { get; set; } = string.Empty;

        private string SortingID { get; set; } = string.Empty;

        private string OrderID { get; set; } = string.Empty;

        private Enums.OrderStatus OrderStatus { get; set; }

        private Models.OrderItem OrderItemInfo { get; set; }

        private Layer.Data.Sqls.ScCustoms.OrderControls[] OldOrderControls { get; set; }

        private Layer.Data.Sqls.ScCustoms.OrderControlSteps[] OldOrderControlSteps { get; set; }

        public ArrivalOutsideDeleteModel(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string orderItemID, string sortingID, string orderID, Enums.OrderStatus orderStatus,
            Models.OrderItem orderItemInfo,
            Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls,
            Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps)
        {
            this.Reponsitory = reponsitory;
            this.OrderItemID = orderItemID;
            this.SortingID = sortingID;
            this.OrderID = orderID;
            this.OrderStatus = orderStatus;
            this.OrderItemInfo = orderItemInfo;
            this.OldOrderControls = oldOrderControls;
            this.OldOrderControlSteps = oldOrderControlSteps;
        }

        public override void Do()
        {
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPremiums>(item => item.OrderItemID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(item => item.OrderItemID == this.OrderItemID);
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(item => item.OrderItemID == this.OrderItemID);           
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItems>(item => item.ID == this.OrderItemID);           

            //删除与该型号有关的 OrderControl 、OrderControlStep
            this.AbandonThisOrderItemControl(this.Reponsitory, this.OrderItemID, this.OrderID, this.OldOrderControls, this.OldOrderControlSteps);

            if (this.OrderStatus >= Enums.OrderStatus.QuoteConfirmed)
            {
                //外单需要挂起订单
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    IsHangUp = true,
                }, item => item.ID == this.OrderID);

                //如果 orderControl 有状态为 200 的 900 的，则不插入数据
                //如果没有，要插入数据，注意这里不要插入 OrderItemID ，因为这个型号都被删了
                if (this.OldOrderControls != null && this.OldOrderControls.Any())
                {
                    string[] thisOrderItemOrderControlIDs = this.OldOrderControls.Where(t => t.OrderItemID == this.OrderItemID).Select(t => t.ID).ToArray();

                    var stepNoThisItem = this.OldOrderControlSteps.Where(t => !thisOrderItemOrderControlIDs.Contains(t.OrderControlID)).ToArray();
                    var controlNoThisItem = this.OldOrderControls.Where(t => !thisOrderItemOrderControlIDs.Contains(t.ID)).ToArray();

                    var unHandledControl = (from control in controlNoThisItem
                                            join step in stepNoThisItem on control.ID equals step.OrderControlID
                                            where control.Status == (int)Enums.Status.Normal
                                               && step.Status == (int)Enums.Status.Normal
                                               && step.ControlStatus == (int)Enums.OrderControlStatus.Auditing  //如果是拒绝通过，会取消挂起订单吗？
                                            select control).ToArray();

                    var theOld = unHandledControl.Where(t => t.ControlType == (int)Enums.OrderControlType.DeleteModel).FirstOrDefault();
                    if (theOld == null || string.IsNullOrEmpty(theOld.ID))
                    {
                        var controlInsert = new OrderControlData();
                        controlInsert.OrderID = this.OrderID;
                        controlInsert.ControlType = Enums.OrderControlType.DeleteModel;
                        controlInsert.Summary = "已删除型号【" + this.OrderItemInfo.Model + "】（品牌：" + this.OrderItemInfo.Manufacturer + "）";
                        this.Reponsitory.Insert(controlInsert.ToLinq());

                        var controlStepInsert = new OrderControlStep();
                        controlStepInsert.OrderControlID = controlInsert.ID;
                        controlStepInsert.Step = Enums.OrderControlStep.Client;
                        this.Reponsitory.Insert(controlStepInsert.ToLinq());
                    }
                    else
                    {
                        this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControls>(new
                        {
                            Summary = theOld.Summary + (!string.IsNullOrEmpty(theOld.Summary) ? "、" : "") 
                                        + "已删除型号【" + this.OrderItemInfo.Model + "】（品牌：" + this.OrderItemInfo.Manufacturer + "）",
                        }, t => t.ID == theOld.ID);
                    }

                }
                else
                {
                    var controlInsert = new OrderControlData();
                    controlInsert.OrderID = this.OrderID;
                    controlInsert.ControlType = Enums.OrderControlType.DeleteModel;
                    controlInsert.Summary = "已删除型号【" + this.OrderItemInfo.Model + "】（品牌：" + this.OrderItemInfo.Manufacturer + "）";
                    this.Reponsitory.Insert(controlInsert.ToLinq());

                    var controlStepInsert = new OrderControlStep();
                    controlStepInsert.OrderControlID = controlInsert.ID;
                    controlStepInsert.Step = Enums.OrderControlStep.Client;
                    this.Reponsitory.Insert(controlStepInsert.ToLinq());
                }
            }




        }

    }
}
