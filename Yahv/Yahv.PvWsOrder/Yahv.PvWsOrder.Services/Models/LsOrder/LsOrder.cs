using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 租赁订单扩展
    /// </summary>
    public class LsOrder : Yahv.Services.Models.LsOrder.LsOrder
    {
        #region 扩展属性
        public string OperatorID { get; set; }

        /// <summary>
        /// 代仓储客户
        /// </summary>
        public Yahv.Services.Models.WsClient wsClient { get; set; }

        /// <summary>
        /// 内部受益公司
        /// </summary>
        public Company Payee
        {
            get
            {
                return new Views.CompanysAll().Where(item => item.ID == this.PayeeID).FirstOrDefault();
            }
        }

        /// <summary>
        /// 管理员
        /// </summary>
        public Admin Admin
        {
            get
            {
                return new Yahv.Services.Views.AdminsAll<PvWsOrderReponsitory>().Where(item => item.ID == this.Creator).FirstOrDefault();
            }
        }

        /// <summary>
        /// 租赁订单合同文件
        /// </summary>
        public CenterFileDescription ContractFile
        {
            get
            {
                return new Yahv.Services.Views.CenterFilesTopView().Where(f => f.LsOrderID == this.ID && f.Type == (int)FileType.Contract && f.Status == FileDescriptionStatus.Normal)
                    .OrderByDescending(f => f.CreateDate).FirstOrDefault();
            }
        }
        
        /// <summary>
        /// 订单合同协议
        /// </summary>
        public Contract Contract
        {
            get
            {
                return new Yahv.Services.Views.ContractsTopView<PvbCrmReponsitory>().Where(c => c.EnterpriseID == this.wsClient.ID&& c.Status==GeneralStatus.Normal).FirstOrDefault();
            }
        }

        /// <summary>
        /// 租赁订单发票
        /// </summary>
        public CenterFileDescription InvoiceFile
        {
            get
            {
                return new Yahv.Services.Views.CenterFilesTopView().Where(f => f.LsOrderID == this.ID && f.Type == (int)FileType.Invoice && f.Status == FileDescriptionStatus.Normal)
                    .OrderByDescending(f => f.CreateDate).FirstOrDefault();
            }
        }

        /// <summary>
        /// 发票数据源
        /// </summary>
        public IEnumerable<Invoice> InvoiceData
        {
            get
            {
                var invoice = new Yahv.Services.Views.WsInvoicesTopView<PvWsOrderReponsitory>()
                    .Where(item => item.ID == this.InvoiceID).AsEnumerable();
                return invoice;
            }
        }

        /// <summary>
        /// 订单项
        /// </summary>
        IEnumerable<Yahv.Services.Models.LsOrder.LsOrderItem> orderItems;
        public IEnumerable<Yahv.Services.Models.LsOrder.LsOrderItem> OrderItems
        {
            get
            {
                if (this.orderItems == null)
                {
                    this.orderItems = new Views.LsOrderItemRoll(this.ID);
                }
                return this.orderItems;
            }
            set
            {
                this.orderItems = value;
            }
        }

        /// <summary>
        /// 租赁订单的应收
        /// </summary>
        public VoucherStatistic VouchersStatistic
        {
            get
            {
                return new Views.Alls.VouchersStatisticsRoll(this.ID).FirstOrDefault();
            }
        }

        #endregion

        public LsOrder()
        {
            this.Status = LsOrderStatus.Unpaid;
            this.InvoiceStatus = OrderInvoiceStatus.UnInvoiced;
            this.CreateDate = this.ModifyDate = DateTime.Now;

            this.EnterSuccess += Order_EnterSuccess;
            this.AbandonSuccess += Order_AbandonSuccess;
        }

        #region 事件

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        public void OnAbandonSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
        private void Order_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var order = (LsOrder)e.Object;
            order.StatusLogUpdate();
            //添加租赁费应收款项
            order.LsOrderFee();
        }
        private void Order_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var order = (LsOrder)e.Object;
            order.StatusLogUpdate();
            //重记费用
        }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory(false))
            {
                //保存订单和订单项
                this.SaveOrderData(Reponsitory);
                if (string.IsNullOrEmpty(this.FatherID))
                {
                    //扣库位剩余数量
                    ReduceLeaseQty(Reponsitory);
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                    {
                        InheritStatus = true,
                    }, t => t.ID == this.FatherID);
                }
                Reponsitory.Submit();
            }
            this.OnEnterSuccess();
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        public void Abandon()
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                //修改订单
                Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = LsOrderStatus.Closed,
                }, item => item.ID == this.ID);

                if (string.IsNullOrEmpty(this.FatherID))
                {
                    //非续租订单
                    //返还可租赁库位数量
                    ReturnLeaseQty(Reponsitory);
                }
                else
                {
                    //修改父订单状态
                    Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                    {
                        ModifyDate = DateTime.Now,
                        InheritStatus = false,
                    }, item => item.ID == this.FatherID);
                }
            }
            this.Status = LsOrderStatus.Closed;
            this.LsOrderFee();//重记账
            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 订单到期
        /// </summary>
        public void Expired()
        {
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                //修改订单
                Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = LsOrderStatus.Expired,
                }, item => item.ID == this.ID);

                if (string.IsNullOrEmpty(this.FatherID))
                {
                    //非续租订单
                    //返还可租赁库位数量
                    ReturnLeaseQty(Reponsitory);
                }
            }
            this.Status = LsOrderStatus.Expired;
            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 管理员修改租赁单价
        /// </summary>
        public void UpdatePrice()
        {
            using (PvLsOrderReponsitory repository = new PvLsOrderReponsitory(false))
            {
                foreach (var item in this.OrderItems)
                {
                    repository.Update<Layers.Data.Sqls.PvLsOrder.OrderItems>(new
                    {
                        UnitPrice = item.UnitPrice
                    }, t => t.ID == item.ID);
                }
                repository.Submit();
            }
            this.orderItems = null;
            this.LsOrderFee();//重记账
        }

        /// <summary>
        /// 保存订单和订单项
        /// </summary>
        private void SaveOrderData(PvLsOrderReponsitory Reponsitory)
        {
            #region 保存订单
            this.ID = Layers.Data.PKeySigner.Pick(PKeyType.LsOrder);
            Reponsitory.Insert(new Layers.Data.Sqls.PvLsOrder.Orders
            {
                ID = this.ID,
                Type = (int)this.Type,
                Source = (int)this.Source,
                ClientID = this.ClientID,
                PayeeID = this.PayeeID,
                BeneficiaryID = this.BeneficiaryID,
                Currency = (int)this.Currency,
                InvoiceID = this.InvoiceID,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Status = (int)this.Status,
                Creator = this.Creator,
                CreateDate = this.CreateDate,
                ModifyDate = this.ModifyDate,
                Summary = this.Summary,
                FatherID = this.FatherID,
            });
            #endregion

            #region 保存订单项
            foreach (var item in this.OrderItems)
            {
                item.ID = Layers.Data.PKeySigner.Pick(PKeyType.LsOrderItem);
                item.OrderID = this.ID;
                item.Status = GeneralStatus.Normal;
                CreateDate = DateTime.Now;
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
            Reponsitory.Insert(linq.ToArray());
            //租赁期限
            Reponsitory.Insert(this.OrderItems.Where(item => item.Lease != null).Select(item => new Layers.Data.Sqls.PvLsOrder.OrderItemsLease
            {
                ID = item.ID,
                StartDate = (DateTime)item.Lease.StartDate,
                EndDate = (DateTime)item.Lease.EndDate,
                Status = (int)item.Lease.Status,
                CreateDate = (DateTime)item.Lease.CreateDate,
                ModifyDate = (DateTime)item.Lease.ModifyDate,
                Summary = item.Lease.Summary,
            }).ToArray());
            #endregion
        }

        /// <summary>
        /// 扣除可租赁库位数量
        /// </summary>
        /// <param name="Reponsitory"></param>
        private void ReduceLeaseQty(PvLsOrderReponsitory Reponsitory)
        {
            foreach (var item in this.OrderItems)
            {
                Reponsitory.Command($"update LsProducts set Quantity-={item.Quantity} where id='{item.ProductID}'");
            }
        }

        /// <summary>
        /// 返还可租赁库位数量
        /// </summary>
        /// <param name="Reponsitory"></param>
        private void ReturnLeaseQty(PvLsOrderReponsitory Reponsitory)
        {
            foreach (var item in this.OrderItems)
            {
                Reponsitory.Command($"update LsProducts set Quantity+={item.Quantity} where id='{item.ProductID}'");
            }
        }
        #endregion
    }
}
