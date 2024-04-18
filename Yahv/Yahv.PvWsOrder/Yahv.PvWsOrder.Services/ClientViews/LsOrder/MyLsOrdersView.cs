using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Layers.Data.Sqls;
using Yahv.Underly;
using Yahv.Services.Models.LsOrder;
using Yahv.Services.Models;
using Layers.Linq;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyLsOrdersView : UniqueView<LsOrderList, PvLsOrderReponsitory>
    {
        IUser User;

        public MyLsOrdersView(IUser user)
        {
            this.User = user;
        }

        protected override IQueryable<LsOrderList> GetIQueryable()
        {
            var LsOrdersView = new Yahv.Services.Views.LsOrderTopView<PvLsOrderReponsitory>(this.Reponsitory);
            var LsOrderItemsView = new Yahv.Services.Views.LsOrderItemTopView<PvLsOrderReponsitory>(this.Reponsitory);

            var linq = from lsorder in LsOrdersView
                       join lsorderitem in LsOrderItemsView on lsorder.ID equals lsorderitem.OrderID
                       where lsorder.Type == LsOrderType.Lease && lsorder.ClientID == this.User.EnterpriseID
                       select new LsOrderList
                       {
                           ID = lsorder.ID,
                           FatherID = lsorder.FatherID,
                           ItemID = lsorderitem.ID,
                           StartDate = lsorderitem.Lease.StartDate,
                           EndDate = lsorderitem.Lease.EndDate,
                           CreateDate = lsorderitem.CreateDate,
                           SpecID = lsorderitem.Product.SpecID,
                           ProductID = lsorderitem.ProductID,
                           Quantity = lsorderitem.Quantity,
                           UnitPrice = lsorderitem.UnitPrice,
                           Status = lsorder.Status,
                           Currency = lsorder.Currency,
                           InheritStatus = lsorder.InheritStatus,
                           CreatorID = lsorder.Creator,
                       };

            if (!this.User.IsMain)
            {
                linq = linq.Where(item => item.CreatorID == this.User.ID);
            }

            return linq;
        }

        /// <summary>
        /// 获取租赁订单详情
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public LsOrderItem[] GetLsOrderDetail(string OrderID)
        {
            var linq = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsOrderItemsTopView>()
                       join product in new LsProductView(this.Reponsitory) on item.ProductID equals product.ID
                       where item.OrderID == OrderID && item.Status == (int)GeneralStatus.Normal
                       select new LsOrderItem
                       {
                           ID = item.ID,
                           OrderID = item.OrderID,
                           Quantity = item.Quantity,
                           Currency = (Currency)item.Currency,
                           UnitPrice = item.UnitPrice,
                           ProductID = item.ProductID,
                           Description = item.Description,
                           Supplier = item.Supplier,
                           Status = (GeneralStatus)item.Status,
                           CreateDate = item.CreateDate,
                           Lease = new OrderItemsLease
                           {
                               ID = item.ID,
                               StartDate = (DateTime)item.StartDate,
                               EndDate = (DateTime)item.EndDate,
                               CreateDate = item.LeaseCreateDate,
                               Status = (LsStatus)item.LeaseStatus,
                               ModifyDate = item.ModifyDate,
                               Summary = item.Summary
                           },
                           Product = product
                       };
            return linq.ToArray();
        }

        /// <summary>
        /// 订单取消
        /// </summary>
        /// <param name="ItemID">租赁项ID</param>
        public void Cancel(string ItemID)
        {
            var order = this.SingleOrDefault(item => item.ItemID == ItemID);

            //删除该租赁项数据
            this.Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.OrderItems>(new
            {
                Status = (int)GeneralStatus.Closed,
            }, item => item.ID == order.ItemID);

            //加库存
            Reponsitory.Command($"update LsProducts set Quantity+={order.Quantity} where id='{order.ProductID}'");

            //新增订单需要加库存
            if (!string.IsNullOrWhiteSpace(order.FatherID))
            {
                //更改主订单的续借状态
                Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                {
                    InheritStatus = false,
                }, item => item.ID == order.FatherID);
            }

            //修改订单的开始时间、结束时间
            var orderItemTimes = from orderItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.OrderItems>()
                                 join orderItemsLease in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.OrderItemsLease>()
                                 on orderItem.ID equals orderItemsLease.ID
                                 where orderItem.Status == (int)GeneralStatus.Normal
                                    && orderItem.OrderID == order.ID
                                 select new
                                 {
                                     OrderItemID = orderItem.ID,
                                     StartDate = orderItemsLease.StartDate,
                                     EndDate = orderItemsLease.EndDate,
                                 };
            if (orderItemTimes != null && orderItemTimes.Count() > 0)
            {
                DateTime orderStartDate = orderItemTimes.Select(t => t.StartDate).Min();
                DateTime orderEndDate = orderItemTimes.Select(t => t.EndDate).Max();

                this.Reponsitory.Update<Layers.Data.Sqls.PvLsOrder.Orders>(new
                {
                    StartDate = orderStartDate,
                    EndDate = orderEndDate,
                }, item => item.ID == order.ID);
            }
        }

        /// <summary>
        /// 保存合同
        /// </summary>
        /// <param name="OrderID"></param>
        public void SaveContractFile(string OrderID)
        {

        }
    }

    /// <summary>
    /// 页面展示对象
    /// </summary>
    public class LsOrderList : Yahv.Linq.IUnique
    {
        public string ID { get; set; }

        public string FatherID { get; set; }

        public string ItemID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string SpecID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string ProductID { get; set; }

        public string CreatorID { get; set; }

        /// <summary>
        /// 总租期
        /// </summary>
        public int Months
        {
            get
            {
                return (this.EndDate.Year - this.StartDate.Year) * 12 + (this.EndDate.Month - this.StartDate.Month);
            }
        }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice
        {
            get
            {
                return this.UnitPrice * this.Months * this.Quantity;
            }
        }

        public LsOrderStatus Status { get; set; }

        public Currency Currency { get; set; }

        /// <summary>
        /// 是否被续租
        /// </summary>
        public bool InheritStatus { get; set; }

        /// <summary>
        /// 订单附件
        /// </summary>
        public CenterFileDescription[] OrderFiles { get; set; }
    }
}
