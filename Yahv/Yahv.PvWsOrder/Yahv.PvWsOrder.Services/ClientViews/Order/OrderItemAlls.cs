using Layers.Data.Sqls;
using System.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Views;
using OrderItem = Yahv.PvWsOrder.Services.ClientModels.OrderItem;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class OrderItemAlls : OrderItemOrigin
    {
        internal protected OrderItemAlls()
        {

        }

        public OrderItemAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        public OrderItemAlls(PvWsOrderReponsitory reponsitory, IQueryable<OrderItem> items) : base(reponsitory, items)
        {

        }

        protected override IQueryable<OrderItem> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Status != OrderItemStatus.Deleted);
        }

        /// <summary>
        /// 根据订单ID获取订单数据
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public OrderItemAlls SearchByOrderID(string OrderID)
        {
            var iQuery = this.IQueryable.Where(item => item.OrderID == OrderID);

            if (iQuery.Count(item => item.Type == OrderItemType.Modified) > 0)
            {
                iQuery = iQuery.Where(item => item.Type == OrderItemType.Modified);
            }

            return new OrderItemAlls(this.Reponsitory, iQuery);
        }

        /// <summary>
        /// 获取订单项账单
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        internal OrderItem[] GetItemBillByOrderID(string OrderID)
        {
            var orderitems = this.SearchByOrderID(OrderID);

            var linq = from item in orderitems
                       join term in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals term.ID
                       join chcd in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>() on item.ID equals chcd.ID
                       join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                       join declarePrice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderDeclareStatisticsView>()
                       on new { ItemID = item.ID, item.OrderID } equals new { declarePrice.ItemID, declarePrice.OrderID }
                       join classfied in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClassifiedPartNumbersTopView>()
                       on chcd.SecondHSCodeID equals classfied.ID
                       select new OrderItem
                       {
                           ID = item.ID,
                           Name = item.Name,
                           OrderID = item.OrderID,
                           InputID = item.InputID,
                           ProductID = item.ProductID,
                           TinyOrderID = item.TinyOrderID,
                           Origin = item.Origin,
                           DateCode = item.DateCode,
                           Quantity = item.Quantity,
                           Currency = item.Currency,
                           UnitPrice = item.UnitPrice,
                           Unit = item.Unit,
                           TotalPrice = item.TotalPrice,
                           CreateDate = item.CreateDate,
                           ModifyDate = item.ModifyDate,
                           GrossWeight = item.GrossWeight,
                           Volume = item.Volume,
                           Conditions = item.Conditions,
                           Status = item.Status,
                           Product = product,
                           StorageID = item.StorageID,
                           //RealExchangeRate = RealRate,
                           //CustomsExchangeRate = customsRate,
                           AddTaxRate = classfied.VATRate,
                           TraiffRate = classfied.ImportPreferentialTaxRate + term.OriginRate,
                           ExciseTaxRate = classfied.ExciseTaxRate,
                           //DeclareTotalPrice = item.TotalPrice * RealRate,
                           Traiff = declarePrice.CutomsPrice.GetValueOrDefault(),
                           AddTax = declarePrice.DutyPrice.GetValueOrDefault(),
                           AgencyFee = declarePrice.AgentPrice.GetValueOrDefault(),
                           InspectionFee = declarePrice.otherPrice.GetValueOrDefault(),
                           ExcisePrice = declarePrice.ExcisePrice.GetValueOrDefault(),
                           ClassfiedName = classfied.Name,
                           ItemsTerm = new OrderItemsTerm()
                           {
                               Ccc = term.Ccc,
                               Embargo = term.Embargo,
                               HkControl = term.Embargo,
                               Coo = term.Coo,
                               CIQ = term.CIQ,
                               IsHighPrice = term.IsHighPrice,
                           }
                       };

            return linq.ToArray();
        }

        public class TinyOrderRate
        {
            public string TinyOrderID { get; set; }

            public decimal RealRate { get; set; }

            public decimal CustomsRate { get; set; }
        }

        /// <summary>
        /// 获取一个大订单中，各个小订单的实时汇率、海关汇率
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public TinyOrderRate[] GetTinyOrderRates(string OrderID)
        {
            TinyOrderRate[] tinyOrderRates = new TinyOrderRate[0];

            using (ScCustomReponsitory res = new ScCustomReponsitory())
            {
                //var xdtorder = res.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>().FirstOrDefault(item => item.MainOrderId == OrderID);
                //RealRate = xdtorder.RealExchangeRate.GetValueOrDefault();
                //customsRate = xdtorder.CustomsExchangeRate.GetValueOrDefault();

                tinyOrderRates = res.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>()
                    .Where(item => item.MainOrderId == OrderID)
                    .Select(item => new TinyOrderRate
                    {
                        TinyOrderID = item.ID,
                        RealRate = item.RealExchangeRate ?? 0,
                        CustomsRate = item.CustomsExchangeRate ?? 0,
                    }).ToArray();
            }

            return tinyOrderRates;
        }

        /// <summary>
        /// 获取订单项归类信息
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public OrderItem[] GetClassfiedItemByOrderID(string OrderID)
        {
            var orderitems = this.SearchByOrderID(OrderID);

            var linq = from item in orderitems
                       join term in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals term.ID
                       join chcd in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>() on item.ID equals chcd.ID
                       join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                       join classfied in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClassifiedPartNumbersTopView>()
                       on chcd.SecondHSCodeID equals classfied.ID
                       select new OrderItem
                       {
                           ID = item.ID,
                           Name = item.Name,
                           OrderID = item.OrderID,
                           InputID = item.InputID,
                           ProductID = item.ProductID,
                           TinyOrderID = item.TinyOrderID,
                           Origin = item.Origin,
                           DateCode = item.DateCode,
                           Quantity = item.Quantity,
                           Currency = item.Currency,
                           UnitPrice = item.UnitPrice,
                           Unit = item.Unit,
                           StorageID = item.StorageID,
                           TotalPrice = item.TotalPrice,
                           CreateDate = item.CreateDate,
                           ModifyDate = item.ModifyDate,
                           GrossWeight = item.GrossWeight,
                           Volume = item.Volume,
                           Conditions = item.Conditions,
                           Status = item.Status,
                           Product = product,
                           TraiffRate = classfied == null ? 0 : classfied.ImportPreferentialTaxRate + term.OriginRate,
                           ClassfiedName = classfied == null ? null : classfied.Name,
                       };

            return linq.ToArray();
        }

        /// <summary>
        /// 获取订单详情信息
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        internal OrderItem[] GetItemDetailByOrderID(string OrderID)
        {
            var orderitems = this.SearchByOrderID(OrderID);
            var linq = from item in orderitems
                       join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                       select new OrderItem
                       {
                           ID = item.ID,
                           Name = item.Name,
                           OrderID = item.OrderID,
                           InputID = item.InputID,
                           ProductID = item.ProductID,
                           TinyOrderID = item.TinyOrderID,
                           Origin = item.Origin,
                           DateCode = item.DateCode,
                           Quantity = item.Quantity,
                           Currency = item.Currency,
                           UnitPrice = item.UnitPrice,
                           Unit = item.Unit,
                           TotalPrice = item.TotalPrice,
                           CreateDate = item.CreateDate,
                           ModifyDate = item.ModifyDate,
                           GrossWeight = item.GrossWeight,
                           Volume = item.Volume,
                           Conditions = item.Conditions,
                           Status = item.Status,
                           Product = product,
                           StorageID = item.StorageID,
                       };

            return linq.ToArray();
        }

        internal OrderItem[] GetDeclareItemDetailByOrderID(string OrderID)
        {
            var orderitems = this.SearchByOrderID(OrderID);

            var linq = from chcd in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>()
                       join classfied in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClassifiedPartNumbersTopView>()
                       on chcd.SecondHSCodeID equals classfied.ID
                       select new
                       {
                           chcd.ID,
                           classfied.Name
                       };

            var result = from item in orderitems
                         join product in new ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                         join classfied in linq on item.ID equals classfied.ID into classfieds
                         from _clasfied in classfieds.DefaultIfEmpty()

                         join xdtOrderItemsOriginView in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtOrderItemsOriginView>()
                         on item.ID equals xdtOrderItemsOriginView.ID into XdtOrderItemsOriginView2
                         from xdtOrderItemsOriginView in XdtOrderItemsOriginView2.DefaultIfEmpty()
                         select new OrderItem
                         {
                             ID = item.ID,
                             Name = item.Name,
                             OrderID = item.OrderID,
                             InputID = item.InputID,
                             ProductID = item.ProductID,
                             TinyOrderID = item.TinyOrderID,
                             Origin = item.Origin,
                             DateCode = item.DateCode,
                             Quantity = item.Quantity,
                             Currency = item.Currency,
                             UnitPrice = item.UnitPrice,
                             Unit = item.Unit,
                             TotalPrice = item.TotalPrice,
                             CreateDate = item.CreateDate,
                             ModifyDate = item.ModifyDate,
                             GrossWeight = item.GrossWeight,
                             Volume = item.Volume,
                             Conditions = item.Conditions,
                             Status = item.Status,
                             Product = product,
                             ClassfiedName = _clasfied == null ? null : _clasfied.Name,
                             StorageID = item.StorageID,
                             ProductUnicode = xdtOrderItemsOriginView != null ? xdtOrderItemsOriginView.ProductUniqueCode : null,
                         };
            return result.ToArray();
        }
    }
}
