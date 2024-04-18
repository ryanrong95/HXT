using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    /// <summary>
    /// 产品归类预处理一
    /// </summary>
    [Obsolete]
    public class ClassifyProductsStep1 : QueryRoll<Models.OrderItem, Models.OrderItem, PvWsOrderReponsitory>
    {
        public ClassifyProductsStep1()
        {
        }

        protected override IQueryable<OrderItem> GetIQueryable(Expression<Func<OrderItem, bool>> expression, params LambdaExpression[] expressions)
        {
            var productsView = new ProductsAll(this.Reponsitory);
            var locks_ClassifyView = new Locks_ClassifyAll(this.Reponsitory);
            var chcdsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>();
            var ordersView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>();

            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                       join product in productsView on entity.ProductID equals product.ID
                       join chcd in chcdsView on entity.ID equals chcd.ID into chcds
                       from chcd in chcds.DefaultIfEmpty()
                       join lock_Classify in locks_ClassifyView on entity.ID equals lock_Classify.MainID into locks_Classify
                       from lock_Classify in locks_Classify.DefaultIfEmpty()
                       join order in ordersView on entity.OrderID equals order.ID
                       where entity.Status == (int)GeneralStatus.Normal && chcd == null
                       orderby entity.CreateDate
                       select new OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderedDate = order.CreateDate,
                           WsClientID = order.ClientID,

                           ProductID = product.ID,
                           Product = product,
                           CustomName = entity.CustomName,
                           //Origin = entity.Origin,
                           Origin = (Origin)Enum.Parse(typeof(Origin), entity.Origin),
                           UnitPrice = entity.UnitPrice,
                           Quantity = entity.Quantity,
                           Currency = (Underly.Currency)entity.Currency,
                           TotalPrice = entity.TotalPrice,
                           Unit = (Underly.LegalUnit)entity.Unit,
                           CreateDate = entity.CreateDate,

                           IsLocked = lock_Classify == null ? false : true,
                           Locker = lock_Classify == null ? null : lock_Classify.Locker,
                           LockDate = lock_Classify == null ? null : (DateTime?)lock_Classify.LockDate
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<OrderItem, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<OrderItem> OnReadShips(OrderItem[] results)
        {
            var clientIds = results.Select(r => r.WsClientID).Distinct().ToArray();
            var clients = new WsClientsAlls(this.Reponsitory).Where(c => clientIds.Contains(c.ID)).ToArray();

            return from result in results
                   join wsClient in clients on result.WsClientID equals wsClient.ID
                   select new OrderItem
                   {
                       ID = result.ID,
                       OrderID = result.OrderID,
                       OrderedDate = result.OrderedDate,
                       WsClientID = result.WsClientID,
                       WsClient = wsClient,

                       Product = result.Product,
                       CustomName = result.CustomName,
                       Origin = result.Origin,
                       UnitPrice = result.UnitPrice,
                       Quantity = result.Quantity,
                       Currency = result.Currency,
                       TotalPrice = result.TotalPrice,
                       Unit = result.Unit,
                       CreateDate = result.CreateDate,

                       IsLocked = result.IsLocked,
                       Locker = result.Locker,
                       LockDate = result.LockDate
                   };
        }
    }

    /// <summary>
    /// 产品归类预处理二
    /// </summary>
    [Obsolete]
    public class ClassifyProductsStep2 : QueryRoll<Models.OrderItem, Models.OrderItem, PvWsOrderReponsitory>
    {
        public ClassifyProductsStep2()
        {
        }

        protected override IQueryable<OrderItem> GetIQueryable(Expression<Func<OrderItem, bool>> expression, params LambdaExpression[] expressions)
        {
            var productsView = new ProductsAll(this.Reponsitory);
            var cpnsView = new ClassifiedPartNumbersOrigin(this.Reponsitory);
            var locks_ClassifyView = new Locks_ClassifyAll(this.Reponsitory);
            var chcdsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>()
                            .Where(chcd => chcd.SecondHSCodeID == null)
                            .Select(chcd => new OrderItemsChcd
                            {
                                ID = chcd.ID,
                                FirstHSCodeID = chcd.FirstHSCodeID != null ? chcd.FirstHSCodeID : chcd.AutoHSCodeID
                            });
            var ordersView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>();

            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                       join product in productsView on entity.ProductID equals product.ID
                       join chcd in chcdsView on entity.ID equals chcd.ID
                       join cpn in cpnsView on chcd.FirstHSCodeID equals cpn.ID
                       join lock_Classify in locks_ClassifyView on entity.ID equals lock_Classify.MainID into locks_Classify
                       from lock_Classify in locks_Classify.DefaultIfEmpty()
                       join order in ordersView on entity.OrderID equals order.ID
                       where entity.Status == (int)GeneralStatus.Normal
                       orderby entity.CreateDate
                       select new OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderedDate = order.CreateDate,
                           WsClientID = order.ClientID,

                           ProductID = product.ID,
                           Product = product,
                           CustomName = entity.CustomName,
                           //Origin = entity.Origin,
                           Origin = (Origin)Enum.Parse(typeof(Origin), entity.Origin),
                           UnitPrice = entity.UnitPrice,
                           Quantity = entity.Quantity,
                           Currency = (Underly.Currency)entity.Currency,
                           TotalPrice = entity.TotalPrice,
                           Unit = (Underly.LegalUnit)entity.Unit,
                           CreateDate = entity.CreateDate,
                           ClassifiedPartNumber = cpn,
                           OrderItemsChcd = chcd,

                           IsLocked = lock_Classify == null ? false : true,
                           Locker = lock_Classify == null ? null : lock_Classify.Locker,
                           LockDate = lock_Classify == null ? null : (DateTime?)lock_Classify.LockDate
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<OrderItem, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<OrderItem> OnReadShips(OrderItem[] results)
        {
            var ids = results.Select(r => r.ID).ToArray();
            var terms = new Origins.OrderItemTermsOrigin(this.Reponsitory).Where(oit => ids.Contains(oit.ID)).ToArray();

            var clientIds = results.Select(r => r.WsClientID).Distinct().ToArray();
            var clients = new WsClientsAlls(this.Reponsitory).Where(c => clientIds.Contains(c.ID)).ToArray();

            return from result in results
                   join term in terms on result.ID equals term.ID
                   join wsClient in clients on result.WsClientID equals wsClient.ID
                   select new OrderItem
                   {
                       ID = result.ID,
                       OrderID = result.OrderID,
                       OrderedDate = result.OrderedDate,
                       WsClientID = result.WsClientID,
                       WsClient = wsClient,

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
                       OrderItemsTerm = term,

                       IsLocked = result.IsLocked,
                       Locker = result.Locker,
                       LockDate = result.LockDate
                   };
        }
    }

    /// <summary>
    /// 产品归类已完成
    /// </summary>
    [Obsolete]
    public class ClassifyProductsDone : QueryRoll<Models.OrderItem, Models.OrderItem, PvWsOrderReponsitory>
    {
        public ClassifyProductsDone()
        {
        }

        protected override IQueryable<OrderItem> GetIQueryable(Expression<Func<OrderItem, bool>> expression, params LambdaExpression[] expressions)
        {
            var productsView = new ProductsAll(this.Reponsitory);
            var cpnsView = new ClassifiedPartNumbersOrigin(this.Reponsitory);
            var locks_ClassifyView = new Locks_ClassifyAll(this.Reponsitory);
            var chcdsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().Where(chcd => chcd.SecondHSCodeID != null);
            var ordersView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>();

            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                       join product in productsView on entity.ProductID equals product.ID
                       join chcd in chcdsView on entity.ID equals chcd.ID
                       join cpn in cpnsView on chcd.SecondHSCodeID equals cpn.ID
                       join lock_Classify in locks_ClassifyView on entity.ID equals lock_Classify.MainID into locks_Classify
                       from lock_Classify in locks_Classify.DefaultIfEmpty()
                       join order in ordersView on entity.OrderID equals order.ID
                       where entity.Status == (int)GeneralStatus.Normal
                       orderby entity.CreateDate descending
                       select new OrderItem
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderedDate = order.CreateDate,
                           WsClientID = order.ClientID,

                           ProductID = product.ID,
                           Product = product,
                           CustomName = entity.CustomName,
                           //Origin = entity.Origin,
                           Origin = (Origin)Enum.Parse(typeof(Origin), entity.Origin),
                           UnitPrice = entity.UnitPrice,
                           Quantity = entity.Quantity,
                           Currency = (Underly.Currency)entity.Currency,
                           TotalPrice = entity.TotalPrice,
                           Unit = (Underly.LegalUnit)entity.Unit,
                           CreateDate = entity.CreateDate,
                           ClassifiedPartNumber = cpn,

                           IsLocked = lock_Classify == null ? false : true,
                           Locker = lock_Classify == null ? null : lock_Classify.Locker,
                           LockDate = lock_Classify == null ? null : (DateTime?)lock_Classify.LockDate
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<OrderItem, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<OrderItem> OnReadShips(OrderItem[] results)
        {
            var ids = results.Select(r => r.ID).ToArray();
            var terms = new Origins.OrderItemTermsOrigin(this.Reponsitory).Where(oit => ids.Contains(oit.ID)).ToArray();

            var clientIds = results.Select(r => r.WsClientID).Distinct().ToArray();
            var clients = new WsClientsAlls(this.Reponsitory).Where(c => clientIds.Contains(c.ID)).ToArray();

            return from result in results
                   join term in terms on result.ID equals term.ID
                   join wsClient in clients on result.WsClientID equals wsClient.ID
                   select new OrderItem
                   {
                       ID = result.ID,
                       OrderID = result.OrderID,
                       OrderedDate = result.OrderedDate,
                       WsClientID = result.WsClientID,
                       WsClient = wsClient,

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
                       OrderItemsTerm = term,

                       IsLocked = result.IsLocked,
                       Locker = result.Locker,
                       LockDate = result.LockDate
                   };
        }
    }
}
