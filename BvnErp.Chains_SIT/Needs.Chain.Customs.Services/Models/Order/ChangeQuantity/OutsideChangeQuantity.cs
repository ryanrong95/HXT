using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 外单修改数量 Do
    /// </summary>
    public class OutsideChangeQuantity : IChangeQuantityDo
    {
        private Layer.Data.Sqls.ScCustomsReponsitory Reponsitory { get; set; }

        private string OrderItemID { get; set; } = string.Empty;

        private decimal NewQuantity { get; set; }

        private decimal UnitPrice { get; set; }

        private string OrderID { get; set; } = string.Empty;

        private Enums.OrderStatus OrderStatus;

        private Models.OrderItem OrderItemInfo;

        private Layer.Data.Sqls.ScCustoms.OrderControls[] OldOrderControls { get; set; }

        private Layer.Data.Sqls.ScCustoms.OrderControlSteps[] OldOrderControlSteps { get; set; }

        public OutsideChangeQuantity(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string orderItemID, decimal newQuantity, decimal unitPrice, string orderID,
            Enums.OrderStatus orderStatus,
            Models.OrderItem orderItemInfo,
            Layer.Data.Sqls.ScCustoms.OrderControls[] oldOrderControls,
            Layer.Data.Sqls.ScCustoms.OrderControlSteps[] oldOrderControlSteps)
        {
            this.Reponsitory = reponsitory;
            this.OrderItemID = orderItemID;
            this.NewQuantity = newQuantity;
            this.UnitPrice = unitPrice;
            this.OrderID = orderID;
            this.OrderStatus = orderStatus;
            this.OrderItemInfo = orderItemInfo;
            this.OldOrderControls = oldOrderControls;
            this.OldOrderControlSteps = oldOrderControlSteps;
        }

        public void Do()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
            {
                Quantity = this.NewQuantity,
                TotalPrice = this.NewQuantity * this.UnitPrice,
            }, t => t.ID == this.OrderItemID);


            if (this.OrderStatus >= Enums.OrderStatus.QuoteConfirmed)
            {
                //外单需要挂起订单
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    IsHangUp = true,
                }, item => item.ID == this.OrderID);

                //如果 orderControl 有状态为 200 的 800 的，则不插入数据
                //如果没有，要插入数据，注意这里不要插入 OrderItemID
                if (this.OldOrderControls != null && this.OldOrderControls.Any())
                {
                    var unHandledControl = (from control in this.OldOrderControls
                                            join step in this.OldOrderControlSteps on control.ID equals step.OrderControlID
                                            where control.Status == (int)Enums.Status.Normal
                                               && step.Status == (int)Enums.Status.Normal
                                               && step.ControlStatus == (int)Enums.OrderControlStatus.Auditing  //如果是拒绝通过，会取消挂起订单吗？
                                            select control).ToArray();

                    var theOld = unHandledControl.Where(t => t.ControlType == (int)Enums.OrderControlType.ChangeQuantity).FirstOrDefault();
                    if (theOld == null || string.IsNullOrEmpty(theOld.ID))
                    {
                        var controlInsert = new OrderControlData();
                        controlInsert.ID = Guid.NewGuid().ToString("N");
                        controlInsert.OrderID = this.OrderID;
                        controlInsert.ControlType = Enums.OrderControlType.ChangeQuantity;
                        controlInsert.Summary = "型号【" + this.OrderItemInfo.Model + "】（品牌：" + this.OrderItemInfo.Manufacturer + "）数量已由 " 
                            + this.OrderItemInfo.Quantity.ToString("#0.######") + " 修改为 " + this.NewQuantity.ToString("#0.######");
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
                                        + "型号【" + this.OrderItemInfo.Model + "】（品牌：" + this.OrderItemInfo.Manufacturer + "）数量已由 "
                                        + this.OrderItemInfo.Quantity.ToString("#0.######") + " 修改为 " + this.NewQuantity.ToString("#0.######"),
                        }, t => t.ID == theOld.ID);
                    }
                }
                else
                {
                    var controlInsert = new OrderControlData();
                    controlInsert.ID = Guid.NewGuid().ToString("N");
                    controlInsert.OrderID = this.OrderID;
                    controlInsert.ControlType = Enums.OrderControlType.ChangeQuantity;
                    controlInsert.Summary = "型号【" + this.OrderItemInfo.Model + "】（品牌：" + this.OrderItemInfo.Manufacturer + "）数量已由 "
                            + this.OrderItemInfo.Quantity.ToString("#0.######") + " 修改为 " + this.NewQuantity.ToString("#0.######");
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
