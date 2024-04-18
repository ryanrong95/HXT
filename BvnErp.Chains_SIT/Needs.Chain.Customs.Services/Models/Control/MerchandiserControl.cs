using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 跟单员管控
    /// </summary>
    public sealed class MerchandiserControl : OrderControlBase
    {
        /// <summary>
        /// 当前订单管控类型是否处于总部审核阶段
        /// </summary>
        public bool IsHQAuditing { get; set; }

        /// <summary>
        /// 订单退回原因
        /// </summary>
        public string ReturnedSummary { get; set; }

        public string DepartmentCode { get; set; }

        /// <summary>
        /// 订单管控项
        /// </summary>
        private OrderControlItems items;
        public override OrderControlItems Items
        {
            get
            {
                if (this.items == null)
                {
                    using (var view = new Views.MerchandiserControlItemsView())
                    {
                        var query = view.Where(item => item.OrderID == this.Order.ID && item.ControlType == this.ControlType);
                        this.Items = new OrderControlItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = value;
            }
        }

        /// <summary>
        /// 3C认证或原产地证明文件
        /// </summary>
        OrderFiles files;
        public OrderFiles Files
        {
            get
            {
                if (files == null)
                {
                    using (var view = new Views.OrderFilesView())
                    {
                        var query = view.Where(item => item.OrderID == this.Order.ID &&
                        (item.FileType == Enums.FileType.CCC || item.FileType == Enums.FileType.OriginCertificate));
                        this.Files = new OrderFiles(query);
                    }
                }
                return files;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                else
                {
                    this.files = value;
                }
            }
        }

        /// <summary>
        /// 当管控类型为“3C认证”、“原产地证明”，确认文件上传完成，审批通过时发生
        /// </summary>
        public event OrderControledHanlder Confirmed;

        /// <summary>
        /// 当管控类型为“超出垫款上限”、“产地变更”，跟单员取消订单挂起，审批通过时发生
        /// </summary>
        public event OrderControledHanlder HangUpCanceled;

        /// <summary>
        /// 当管控类型为“归类异常”、“禁运”、“分拣异常”、“抽检异常”，审批不通过，需要将订单退回时发生
        /// </summary>
        public event OrderControledHanlder Returned;

        public MerchandiserControl()
        {
            this.Confirmed += Control_Confirmeded;
            this.HangUpCanceled += Control_HangUpCanceled;
            this.Returned += Control_Returned;
        }

        private void Control_Confirmeded(object sender, OrderControledEventArgs e)
        {
            base.CancelOrderHangUp();
        }

        private void Control_HangUpCanceled(object sender, OrderControledEventArgs e)
        {
            var order = e.OrderControl.Order;
            order.SetAdmin(Admin);

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();


                //通过当前 control、controlStep 信息判断 订单的 IsHangUp 应该是 True/False
                int unAuditedControlsCount = (from orderControl in orderControls
                                              join orderControlStep in orderControlSteps
                                                    on new
                                                    {
                                                        OrderControlID = orderControl.ID,
                                                        OrderControlStatus = orderControl.Status,
                                                        OrderControlStepStatus = (int)Enums.Status.Normal,
                                                        OrderID = orderControl.OrderID,

                                                        ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                                    }
                                                    equals new
                                                    {
                                                        OrderControlID = orderControlStep.OrderControlID,
                                                        OrderControlStatus = (int)Enums.Status.Normal,
                                                        OrderControlStepStatus = orderControlStep.Status,
                                                        OrderID = order.ID,

                                                        ControlStatus = orderControlStep.ControlStatus,
                                                    }
                                              select new OrderControlData
                                              {
                                                  ID = orderControl.ID,
                                              }).Count();

                if (unAuditedControlsCount <= 0)
                {
                    order.CancelHangUp();
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { IsHangUp = true }, item => item.ID == order.ID);
                }

            }



        }

        private void Control_Returned(object sender, OrderControledEventArgs e)
        {
            var order = e.OrderControl.Order;
            order.SetAdmin(Admin);
            order.ReturnedSummary = this.ReturnedSummary;
            order.Return();
        }

        void OnConfirmed()
        {
            if (this != null && this.Confirmed != null)
            {
                this.Confirmed(this, new OrderControledEventArgs(this));
            }
        }

        void OnHangUpCanceled()
        {
            if (this != null && this.HangUpCanceled != null)
            {
                this.HangUpCanceled(this, new OrderControledEventArgs(this));
            }
        }

        void OnReturned()
        {
            if (this != null && this.Returned != null)
            {
                this.Returned(this, new OrderControledEventArgs(this));
            }
        }

        /// <summary>
        /// 上传“3C认证”或“原产地证明”完成，审批通过
        /// </summary>
        public void Confirm()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in this.Items)
                {
                    //将当前管控步骤置为“通过”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Approved,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == item.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Merchandiser);
                }
            }

            this.OnConfirmed();
        }

        /// <summary>
        /// 当管控类型为“超出垫款上限”、“产地变更”时，跟单员可以取消订单挂起
        /// </summary>
        public void CancelHangUp()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (this.ControlType == Enums.OrderControlType.ExceedLimit)
                {
                    //将当前管控步骤置为“通过”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Approved,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == this.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Headquarters);
                }
                else if (this.ControlType == Enums.OrderControlType.OriginChange)
                {
                    foreach (var item in this.Items)
                    {
                        //将当前管控步骤置为“通过”
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                        {
                            ControlStatus = Enums.OrderControlStatus.Approved,
                            AdminID = this.Admin.ID,
                            UpdateDate = DateTime.Now
                        }, controlStep => controlStep.OrderControlID == item.ID &&
                                        (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Merchandiser);
                    }
                }
            }

            this.OnHangUpCanceled();
        }
        public void CancelOverduePaymentHangUp(string bufferDays, string approveSummary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (this.ControlType == Enums.OrderControlType.OverdueAdvancePayment)
                {
                    //将当前管控步骤置为“通过”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Approved,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == this.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Headquarters);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControls>(new
                    {
                        BufferDays = Convert.ToDateTime(bufferDays),
                        ApproveSummary = approveSummary
                    }, controls => controls.OrderID == this.Order.ID &&
                                           controls.ControlType == (int)Enums.OrderControlType.OverdueAdvancePayment);
                }
            }

            this.OnHangUpCanceled();
        }
        /// <summary>
        /// 当管控类型为“归类异常”、“禁运”、“分拣异常”、“抽检异常”时，订单退回
        /// </summary>
        public void Return()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //分拣异常为整单管控，不具体管控某个产品
                if (this.ControlType == Enums.OrderControlType.SortingAbnomaly)
                {
                    //将当前管控步骤置为“拒绝”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Rejected,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == this.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Merchandiser);
                }
                else
                {
                    foreach (var item in this.Items)
                    {
                        //将当前管控步骤置为“拒绝”
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                        {
                            ControlStatus = Enums.OrderControlStatus.Rejected,
                            AdminID = this.Admin.ID,
                            UpdateDate = DateTime.Now
                        }, controlStep => controlStep.OrderControlID == item.ID &&
                                        (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Merchandiser);
                    }
                }
            }

            this.OnReturned();
        }
    }
}
