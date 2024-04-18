using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Generic;
using System.Linq.Expressions;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    /// <summary>
    /// 产品归类视图
    /// </summary>
    public class ClassifyProductsAll : QueryRoll<OrderItem, OrderItem, PvWsOrderReponsitory>
    {
        private ClassifyStep Step { get; set; }

        public ClassifyProductsAll(ClassifyStep step)
        {
            this.Step = step;
        }

        protected override IQueryable<OrderItem> GetIQueryable(Expression<Func<OrderItem, bool>> expression, params LambdaExpression[] expressions)
        {
            IQueryable<OrderItem> itemsExtend = null;
            var cpnsView = new ClassifiedPartNumbersOrigin(this.Reponsitory);
            var adminsView = new AdminsAll(this.Reponsitory);
            var chcdsView = from chcd in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>()
                            join firstAdmin in adminsView on chcd.FirstAdminID equals firstAdmin.ID into firstAdmins
                            from firstAdmin in firstAdmins.DefaultIfEmpty()
                            join secondAdmin in adminsView on chcd.SecondAdminID equals secondAdmin.ID into secondAdmins
                            from secondAdmin in secondAdmins.DefaultIfEmpty()
                            select new OrderItemsChcd
                            {
                                ID = chcd.ID,
                                FirstAdminID = chcd.FirstAdminID,
                                FirstAdmin = firstAdmin,
                                FirstHSCodeID = chcd.FirstHSCodeID,
                                FirstDate = chcd.FirstDate,
                                SecondAdminID = chcd.SecondAdminID,
                                SecondAdmin = secondAdmin,
                                SecondHSCodeID = chcd.SecondHSCodeID,
                                SecondDate = chcd.SecondDate
                            };

            switch (Step)
            {
                case ClassifyStep.Step1:
                    itemsExtend = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                                  join chcd in chcdsView on entity.ID equals chcd.ID into chcds
                                  from chcd in chcds.DefaultIfEmpty()
                                  where chcd == null
                                  select new OrderItem { ID = entity.ID, OrderItemsChcd = null, ClassifiedPartNumber = null };
                    break;
                case ClassifyStep.Step2:
                    itemsExtend = from chcd in chcdsView
                                  join cpn in cpnsView on chcd.FirstHSCodeID equals cpn.ID
                                  where chcd.SecondHSCodeID == null
                                  select new OrderItem { ID = chcd.ID, OrderItemsChcd = chcd, ClassifiedPartNumber = cpn };
                    break;
                case ClassifyStep.Done:
                    itemsExtend = from chcd in chcdsView
                                  join cpn in cpnsView on chcd.SecondHSCodeID equals cpn.ID
                                  where chcd.SecondHSCodeID != null
                                  select new OrderItem { ID = chcd.ID, OrderItemsChcd = chcd, ClassifiedPartNumber = cpn };
                    break;

                default:
                    break;
            }

            var productsView = new ProductsAll(this.Reponsitory);
            var locks_ClassifyView = new Locks_ClassifyAll(this.Reponsitory);
            var ordersView = new Origins.OrderOrigin(this.Reponsitory)
                .Where(o => (o.Type == OrderType.TransferDeclare || o.Type == OrderType.Declare) && o.MainStatus >= CgOrderStatus.待审核);
            var orderItemsView = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                                 group entity by entity.InputID into entities
                                 select entities.OrderByDescending(e => e.Type).First();

            var linq = from entity in orderItemsView
                       join product in productsView on entity.ProductID equals product.ID
                       join itemExtend in itemsExtend on entity.ID equals itemExtend.ID
                       join lock_Classify in locks_ClassifyView on entity.InputID equals lock_Classify.MainID into locks_Classify
                       from lock_Classify in locks_Classify.DefaultIfEmpty()
                       join order in ordersView on entity.OrderID equals order.ID
                       where entity.Status == (int)GeneralStatus.Normal
                       select new OrderItem
                       {
                           ID = entity.ID,
                           InputID = entity.InputID,

                           OrderID = entity.OrderID,
                           OrderedDate = order.CreateDate,
                           OrderStatus = order.MainStatus,
                           WsClientID = order.ClientID,

                           ProductID = product.ID,
                           Product = product,
                           CustomName = entity.CustomName,
                           Origin = (Origin)Enum.Parse(typeof(Origin), entity.Origin),
                           UnitPrice = entity.UnitPrice,
                           Quantity = entity.Quantity,
                           Currency = (Underly.Currency)entity.Currency,
                           TotalPrice = entity.TotalPrice,
                           Unit = (Underly.LegalUnit)entity.Unit,
                           CreateDate = entity.CreateDate,
                           ClassifiedPartNumber = itemExtend.ClassifiedPartNumber,
                           OrderItemsChcd = itemExtend.OrderItemsChcd,

                           IsLocked = lock_Classify == null ? false : true,
                           Locker = lock_Classify == null ? null : lock_Classify.Locker,
                           LockDate = lock_Classify == null ? null : (DateTime?)lock_Classify.LockDate
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<OrderItem, bool>>);
            }

            if (this.Step == ClassifyStep.Done)
            {
                return linq.Where(expression).OrderByDescending(item => item.CreateDate);
            }
            else
            {
                return linq.Where(expression).OrderBy(item => item.CreateDate);
            }
        }

        protected override IEnumerable<OrderItem> OnReadShips(OrderItem[] results)
        {
            var ids = results.Select(r => r.ID).ToArray();
            var termsArr = new Origins.OrderItemTermsOrigin(this.Reponsitory).Where(oit => ids.Contains(oit.ID)).ToArray();

            var clientIds = results.Select(r => r.WsClientID).Distinct().ToArray();
            var clientsArr = new WsClientsAlls(this.Reponsitory).Where(c => clientIds.Contains(c.ID)).ToArray();

            return from result in results
                   join term in termsArr on result.ID equals term.ID into terms
                   from term in terms.DefaultIfEmpty()
                   join wsClient in clientsArr on result.WsClientID equals wsClient.ID
                   select new OrderItem
                   {
                       ID = result.ID,
                       InputID = result.InputID,
                       OrderID = result.OrderID,
                       OrderedDate = result.OrderedDate,
                       OrderStatus = result.OrderStatus,
                       WsClientID = result.WsClientID,
                       WsClient = wsClient,

                       ProductID = result.ProductID,
                       Product = result.Product,
                       CustomName = result.CustomName,
                       Origin = result.Origin,
                       UnitPrice = result.UnitPrice,
                       Quantity = result.Quantity,
                       Currency = result.Currency,
                       TotalPrice = result.TotalPrice,
                       Unit = result.Unit,
                       CreateDate = result.CreateDate,
                       ClassifiedPartNumber = result.ClassifiedPartNumber,
                       OrderItemsChcd = result.OrderItemsChcd,
                       OrderItemsTerm = term,

                       IsLocked = result.IsLocked,
                       Locker = result.Locker,
                       LockDate = result.LockDate
                   };
        }
    }
}
