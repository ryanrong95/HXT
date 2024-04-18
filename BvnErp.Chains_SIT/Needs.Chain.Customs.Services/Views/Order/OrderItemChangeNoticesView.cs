using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views
{
    public class OrderItemChangeNoticesView : Needs.Linq.Generic.Unique1Classics<Models.OrderItemChangeNotice, ScCustomsReponsitory>
    {
        public OrderItemChangeNoticesView()
        {

        }

        internal OrderItemChangeNoticesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderItemChangeNotice> GetIQueryable(Expression<Func<OrderItemChangeNotice, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq =
                from changeNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemChangeNotices>()
                join orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on changeNotice.OrderItemID equals orderItem.ID
                join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        .Where(t => t.Status == (int)Enums.Status.Normal && t.OrderStatus != (int)Enums.OrderStatus.Returned)
                    on orderItem.OrderID equals order.ID
                orderby orderItem.OrderID
                select new OrderItemChangeNotice()
                {
                    ID = changeNotice.ID,
                    OrderItemID = changeNotice.OrderItemID,
                    OrderID = orderItem.OrderID,
                    //ProductID = orderItem.ProductID,
                    Type = (OrderItemChangeType)changeNotice.Type,
                    OldValue = changeNotice.OldValue,
                    NewValue = changeNotice.NewValue,
                    IsSplited = changeNotice.IsSplited,
                    ProcessState = (ProcessState)changeNotice.ProcessStatus,
                    Status = (Status)changeNotice.Status,
                    CreateDate = changeNotice.CreateDate,
                    UpdateDate = changeNotice.UpdateDate,
                    TriggerSource = (TriggerSource)changeNotice.TriggerSource,
                };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.OrderItemChangeNotice, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<OrderItemChangeNotice> OnReadShips(OrderItemChangeNotice[] results)
        {
            var orderids = results.Select(item => item.OrderID);
            var companys = (from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                            join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals
                                client.ID
                            join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID
                                equals company.ID
                            where orderids.Contains(order.ID)
                            select new
                            {
                                orderid = order.ID,
                                companyname = company.Name,
                                clientcode = client.ClientCode,
                            }).ToArray();

            var orderItemIds = results.Select(t => t.OrderItemID);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var categories =(from categorie in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                             where orderItemIds.Contains(categorie.OrderItemID)
                             select new
                             {categorie.OrderItemID,
                                 categorie.Name
                             }).ToArray();
            var productLocksView = (from productLock in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>()
                                    join admin in adminsView on productLock.AdminID equals admin.ID
                                    where orderItemIds.Contains(productLock.ID)
                                    select new
                                    {
                                        productLock.ID,
                                        productLock.IsLocked,
                                        productLock.LockDate,
                                        Locker = admin
                                    }).ToArray();
            var orderItemsView = (from orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                  where orderItemIds.Contains(orderitem.ID)
                                  select orderitem).ToArray();

            return from changeNotice in results
                   join company in companys on changeNotice.OrderID equals company.orderid
                   join orderItem in orderItemsView on changeNotice.OrderItemID equals orderItem.ID
                   join ca  in  categories on changeNotice.OrderItemID equals ca.OrderItemID
                   join productLock in productLocksView on changeNotice.OrderItemID equals productLock.ID into productLocks
                   from productLock in productLocks.DefaultIfEmpty() 
                   
                   select new OrderItemChangeNotice()
                   {
                       OrderID = changeNotice.OrderID,
                       OrderItemID = changeNotice.OrderItemID,
                       ClientCode = company.clientcode,
                       CompanyName = company.companyname,
                       //归类后的品名
                       ProductName = ca.Name,
                       ProductModel = orderItem.Model,
                       Type = (OrderItemChangeType)changeNotice.Type,
                       OldValue = changeNotice.OldValue,
                       NewValue = changeNotice.NewValue,
                       IsSplited = changeNotice.IsSplited,
                       CreateDate = changeNotice.CreateDate,
                       UpdateDate = changeNotice.UpdateDate,
                       TriggerSource = changeNotice.TriggerSource,
                       ProcessState = (ProcessState)changeNotice.ProcessState,
                       IsLocked = productLock == null ? false : productLock.IsLocked,
                       Locker = productLock == null ? null : productLock.Locker,
                       LockDate = productLock == null ? null : (DateTime?)productLock.LockDate
                   };
        }
    }
}
