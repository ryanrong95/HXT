using Layers.Data.Sqls;
using System;
using System.Linq;
using Layers.Data.Sqls.PvCenter;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Models;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;
using Yahv.Usually;
using Logs_PvLsOrder = Yahv.Services.Models.Logs_PvLsOrder;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    /// <summary>
    /// 租赁订单扩展类
    /// </summary>
    public class LsOrderExtends : LsOrder
    {
        public LsOrderExtends()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = LsOrderStatus.Unpaid;
            this.InvoiceStatus = OrderInvoiceStatus.UnInvoiced;
            this.EnterSuccess += Order_EnterSuccess;
        }

        #region 扩展属性

        /// <summary>
        /// 订单项
        /// </summary>
        public LsOrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 订单附件
        /// </summary>
        public CenterFileDescription[] OrderFiles { get; set; }

        /// <summary>
        /// 发票对象
        /// </summary>
        public Invoice Invoice { get; set; }

        /// <summary>
        /// 租赁的月数
        /// </summary>
        public int LsMonth
        {
            get
            {
                return this.EndDate.Value.Year * 12 + this.EndDate.Value.Month - this.StartDate.Value.Year * 12 - this.StartDate.Value.Month;
            }
        }

        #endregion

        #region 订单保存成功事件
        public event SuccessHanlder EnterSuccess;

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 订单生成成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_EnterSuccess(object sender, SuccessEventArgs e)
        {
        }

        #endregion

        #region 订单保存逻辑
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (PvLsOrderReponsitory reponsitory = new PvLsOrderReponsitory(false))
            {
                //保存订单，订单项
                this.SaveOrderData(reponsitory);
                //扣库位剩余数量
                UpdataStock(reponsitory);

                //统一提交
                reponsitory.Submit();
            }
            this.OnEnterSuccess();
        }
        #endregion

        #region 生成通知
        ///// <summary>
        ///// 生成通知
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void PaySuccessEvent()
        //{
        //    //生成通知
        //    this.LsNotice();
        //    //更改订单状态
        //    this.Status = LsOrderStatus.UnAllocate;
        //    using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
        //    {
        //        Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
        //        {
        //            Status = (int)this.Status,
        //        }, item => item.ID == this.ID);
        //    }
        //    //日志
        //    StatusLogUpdate(this);
        //}
        #endregion

        #region 扣库位剩余数量
        /// <summary>
        /// 扣库位剩余数量
        /// </summary>
        /// <param name="reponsitory"></param>
        private void UpdataStock(PvLsOrderReponsitory reponsitory)
        {
            foreach (var item in this.OrderItems)
            {
                reponsitory.Command($"update LsProducts set Quantity-={item.Quantity} where id='{item.ProductID}'");
            }
        }
        #endregion

        #region 订单 订单项保存
        /// <summary>
        /// 保存订单订单项
        /// </summary>
        private void SaveOrderData(PvLsOrderReponsitory reponsitory)
        {
            #region 保存订单
            this.ID = Layers.Data.PKeySigner.Pick(PKeyType.LsOrder);
            reponsitory.Insert(new Layers.Data.Sqls.PvLsOrder.Orders
            {
                ID = this.ID,
                FatherID = this.FatherID,
                Type = (int)this.Type,
                Source = (int)this.Source,
                ClientID = this.ClientID,
                PayeeID = this.PayeeID,
                BeneficiaryID = this.BeneficiaryID,
                Currency = (int)this.Currency,
                InvoiceID = this.InvoiceID,
                IsInvoiced = this.IsInvoiced,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Status = (int)this.Status,
                Creator = this.Creator,
                CreateDate = this.CreateDate,
                ModifyDate = this.ModifyDate,
                Summary = this.Summary,
            });
            ////续借时更新主订单的续借标志
            //if (!string.IsNullOrWhiteSpace(this.FatherID))
            //{
            //    Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
            //    {
            //        InheritStatus = true,
            //    }, item => item.ID == this.FatherID);
            //}
            //日志
            StatusLogUpdate(this);
            #endregion


            #region 保存订单项
            foreach (var item in this.OrderItems)
            {
                item.ID = Layers.Data.PKeySigner.Pick(PKeyType.LsOrderItem);
                item.OrderID = this.ID;
                item.Status = GeneralStatus.Normal;
                item.CreateDate = DateTime.Now;
            }
            var linq = this.OrderItems.Select(item => new Layers.Data.Sqls.PvLsOrder.OrderItems
            {
                ID = item.ID,
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Currency = (int)item.Currency,
                Description = item.Description,
                Supplier = item.Supplier,
                Status = (int)item.Status,
                CreateDate = item.CreateDate,
            });
            reponsitory.Insert(linq.ToArray());
            //租赁期限
            reponsitory.Insert(this.OrderItems.Where(item => item.Lease != null).Select(item => new Layers.Data.Sqls.PvLsOrder.OrderItemsLease
            {
                ID = item.ID,
                StartDate = item.Lease.StartDate,
                EndDate = item.Lease.EndDate,
                Status = (int)item.Lease.Status,
                CreateDate = (DateTime)item.Lease.CreateDate,
                ModifyDate = (DateTime)item.Lease.ModifyDate,
                Summary = item.Lease.Summary,
            }).ToArray());
            #endregion
        }
        #endregion

        #region 订单取消
        /// <summary>
        /// 订单取消
        /// </summary>
        /// <param name="order"></param>
        public static void Cancle(LsOrderExtends order)
        {
            using (PvLsOrderReponsitory reponsitory = new PvLsOrderReponsitory(false))
            {
                //更改订单状态
                order.Status = LsOrderStatus.Closed;
                reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                {
                    Status = (int)order.Status,
                }, item => item.ID == order.ID);
                //新增订单需要加库存
                if (string.IsNullOrWhiteSpace(order.FatherID))
                {
                    //加库存
                    foreach (var item in order.OrderItems)
                    {
                        reponsitory.Command($"update LsProducts set Quantity+={item.Quantity} where id='{item.ProductID}'");
                    }
                }
                else
                {
                    //更改主订单的续借状态
                    reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                    {
                        InheritStatus = false,
                    }, item => item.ID == order.FatherID);
                }
                reponsitory.Submit();
            }
        }
        #endregion

        #region 状态日志
        /// <summary>
        /// 租赁订单的状态日志更新
        /// </summary>
        private static void StatusLogUpdate(LsOrderExtends order)
        {
            using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
            {
                //查询订单所有状态的当前状态值
                var current = reponsitory
                    .ReadTable<Logs_PvLsOrderCurrentTopView>().FirstOrDefault(item => item.MainID == order.ID);
                if (current == null)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = order.ID,
                        Type = (int)LsOrderStatusType.MainStatus,
                        Status = (int)order.Status,
                        CreateDate = DateTime.Now,
                        CreatorID = order.Creator,
                        IsCurrent = true,
                    });
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = order.ID,
                        Type = (int)LsOrderStatusType.InvoiceStatus,
                        Status = (int)order.InvoiceStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = order.Creator,
                        IsCurrent = true,
                    });
                }
                else
                {
                    if (current.MainStatus != (int)order.Status)
                    {
                        Logs_PvLsOrder log = new Logs_PvLsOrder
                        {
                            MainID = order.ID,
                            Type = LsOrderStatusType.MainStatus,
                            Status = (int) order.Status,
                            CreatorID = order.Creator
                        };
                        log.Enter();
                    }
                    if (current.InvoiceStatus != (int)order.InvoiceStatus)
                    {
                        Logs_PvLsOrder log = new Logs_PvLsOrder
                        {
                            MainID = order.ID,
                            Type = LsOrderStatusType.InvoiceStatus,
                            Status = (int) order.InvoiceStatus,
                            CreatorID = order.Creator
                        };
                        log.Enter();
                    }
                }
            }
        }
        #endregion

        #region 保存文件
        //public void SaveFiles()
        //{
        //    var file = new CenterFilesView().Upload(this.OrderFiles.Select(item => new CenterFileDescription
        //    {
        //        CustomName = item.CustomName,
        //        Url = item.Url,
        //        AdminID = item.AdminID,
        //        LsOrderID = this.ID,
        //        Type = item.Type,
        //    }).ToArray());
        //}
        #endregion
    }
}
