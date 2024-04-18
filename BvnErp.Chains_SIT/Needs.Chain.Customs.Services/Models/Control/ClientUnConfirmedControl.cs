using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientUnConfirmedControl
    {
        private string OrderID { get; set; } = string.Empty;

        private int OrderStatusInt { get; set; }

        private User User { get; set; }

        public ClientUnConfirmedControl(string orderID, int orderStatusInt, User user)
        {
            this.OrderID = orderID;
            this.OrderStatusInt = orderStatusInt;
            this.User = user;
        }

        /// <summary>
        /// 客户取消挂起
        /// </summary>
        public void CancelHangUp()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //将当前管控步骤置为“通过”
                var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                    .Where(t => t.OrderID == this.OrderID
                            && t.Status == (int)Enums.Status.Normal
                            && (t.ControlType == (int)Enums.OrderControlType.DeleteModel
                             || t.ControlType == (int)Enums.OrderControlType.ChangeQuantity)).ToList();

                if (orderControls != null && orderControls.Any())
                {
                    foreach (var orderControl in orderControls)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                        {
                            ControlStatus = Enums.OrderControlStatus.Approved,
                            //AdminID = this.Admin.ID,
                            UserID = this.User.ID,
                            UpdateDate = DateTime.Now
                        }, controlStep => controlStep.OrderControlID == orderControl.ID && controlStep.Step == (int)Enums.OrderControlStep.Client);
                    }
                }
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int unAuditedCount = (from orderControl in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                                      join orderControlStep in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>()
                                          on new
                                          {
                                              OrderControlID = orderControl.ID,
                                              OrderControlDataStatus = orderControl.Status,
                                              OrderControlStepDataStatus = (int)Enums.Status.Normal,
                                              OrderID = orderControl.OrderID,
                                              ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                          }
                                          equals new
                                          {
                                              OrderControlID = orderControlStep.OrderControlID,
                                              OrderControlDataStatus = (int)Enums.Status.Normal,
                                              OrderControlStepDataStatus = orderControlStep.Status,
                                              OrderID = this.OrderID,
                                              ControlStatus = orderControlStep.ControlStatus,
                                          }
                                      select new
                                      {
                                          orderControl,
                                          orderControlStep
                                      }).Count();

                if (unAuditedCount <= 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                    {
                        IsHangUp = false,
                    }, item => item.ID == this.OrderID);


                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderLogs
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderLog),
                        OrderID = this.OrderID,
                        //OrderItemID = entity.OrderItemID,
                        //AdminID = entity.Admin?.ID,
                        UserID = this.User.ID,
                        OrderStatus = this.OrderStatusInt,
                        CreateDate = DateTime.Now,
                        Summary = "用户[" + this.User.RealName + "]取消了订单挂起",
                    });
                }
                
            }

        }


    }
}
