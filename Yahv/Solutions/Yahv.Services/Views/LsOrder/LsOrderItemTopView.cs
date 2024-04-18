using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 租赁订单项通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class LsOrderItemTopView<TReponsitory> : UniqueView<LsOrderItem, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public LsOrderItemTopView()
        {

        }

        public LsOrderItemTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        public LsOrderItemTopView(TReponsitory reponsitory, IQueryable<LsOrderItem> iquery) : base(reponsitory, iquery)
        {

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<LsOrderItem> GetIQueryable()
        {
            var productView = new LsProductTopView<TReponsitory>(this.Reponsitory);
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsOrderItemsTopView>()
                   join product in productView on entity.ProductID equals product.ID
                   where entity.Status == (int)GeneralStatus.Normal
                   select new LsOrderItem
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       Quantity = entity.Quantity,
                       Currency = (Currency)entity.Currency,
                       UnitPrice = entity.UnitPrice,
                       ProductID = entity.ProductID,
                       Description = entity.Description,
                       Supplier = entity.Supplier,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       Lease = new OrderItemsLease
                       {
                           ID = entity.ID,
                           StartDate = (DateTime)entity.StartDate,
                           EndDate = (DateTime)entity.EndDate,
                           CreateDate = entity.LeaseCreateDate,
                           Status = (LsStatus)entity.LeaseStatus,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary
                       },
                       Product = product
                   };
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="LsOrders"></param>
        static public void Enter(params LsOrderItem[] LsOrderItems)
        {
            if (LsOrderItems == null)
            {
                return;
            }
            using (PvLsOrderReponsitory Reponsitory = new PvLsOrderReponsitory())
            {
                foreach (var item in LsOrderItems)
                {
                    item.ID = Layers.Data.PKeySigner.Pick(PKeyType.LsOrderItem);
                }
                var linq = LsOrderItems.Select(item => new Layers.Data.Sqls.PvLsOrder.OrderItems
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
                Reponsitory.Insert(LsOrderItems.Where(item => item.Lease != null).Select(item => new Layers.Data.Sqls.PvLsOrder.OrderItemsLease
                {
                    ID = item.ID,
                    StartDate = (DateTime)item.Lease.StartDate,
                    EndDate = (DateTime)item.Lease.EndDate,
                    Status = (int)item.Lease.Status,
                    CreateDate = (DateTime)item.Lease.CreateDate,
                    ModifyDate = (DateTime)item.Lease.ModifyDate,
                    Summary = item.Lease.Summary,
                }).ToArray());
            }
        }
    }
}
