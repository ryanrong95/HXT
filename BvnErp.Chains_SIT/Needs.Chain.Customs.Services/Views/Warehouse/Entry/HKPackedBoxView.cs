using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 香港库房入库 已装箱视图
    /// </summary>
    public class HKPackedBoxView
    {
        private ScCustomsReponsitory _reponsitory;

        public HKPackedBoxView()
        {
            _reponsitory = new ScCustomsReponsitory();
        }

        public HKPackedBoxView(ScCustomsReponsitory reponsitory)
        {
            _reponsitory = reponsitory;
        }

        #region 香港库房查看装箱单 - 列表

        public class BoxListModel
        {
            /// <summary>
            /// 装箱日期
            /// </summary>
            public string PackingID { get; set; } = string.Empty;

            /// <summary>
            /// 箱号
            /// </summary>
            public DateTime PackingDate { get; set; }

            /// <summary>
            /// 库位号
            /// </summary>
            public string BoxIndex { get; set; } = string.Empty;

            /// <summary>
            /// 库位号
            /// </summary>
            public string StockCode { get; set; } = string.Empty;

            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// 封箱人
            /// </summary>
            public string SealedName { get; set; } = string.Empty;

            /// <summary>
            /// 封箱时间
            /// </summary>
            public DateTime? SealedDate { get; set; }

            /// <summary>
            /// 客户编号
            /// </summary>
            public string ClientCode { get; set; } = string.Empty;

            /// <summary>
            /// 客户名称
            /// </summary>
            public string ClientName { get; set; } = string.Empty;

            public DateTime? OrderTraceCreateDate { get; set; }
        }

        private IEnumerable<BoxListModel> GetBoxListModelBase(params LambdaExpression[] expressions)
        {
            var packings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var packingItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>();
            var storeStorages = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>();

            var packingInfos = from packing in packings
                               join packingItem in packingItems
                                    on new
                                    {
                                        PackingID = packing.ID,
                                        PackingStatus = packing.Status,
                                        PackingItemStatus = (int)Enums.Status.Normal,
                                    }
                                    equals new
                                    {
                                        PackingID = packingItem.PackingID,
                                        PackingStatus = (int)Enums.Status.Normal,
                                        PackingItemStatus = packingItem.Status,
                                    }
                               join storeStorage in storeStorages
                                   on new
                                   {
                                       SortingID = packingItem.SortingID,
                                       StoreStorageStatus = (int)Enums.Status.Normal,
                                       Purpose = (int)Enums.StockPurpose.Declared,
                                   }
                                   equals new
                                   {
                                       SortingID = storeStorage.SortingID,
                                       StoreStorageStatus = storeStorage.Status,
                                       Purpose = storeStorage.Purpose,
                                   }
                               select new BoxListModel
                               {
                                   PackingID = packing.ID,
                                   PackingDate = packing.PackingDate,
                                   BoxIndex = packing.BoxIndex,
                                   StockCode = storeStorage.StockCode,
                                   OrderID = packing.OrderID,
                               };

            foreach (var expression in expressions)
            {
                packingInfos = packingInfos.Where(expression as Expression<Func<BoxListModel, bool>>);
            }

            packingInfos = from packingInfo in packingInfos
                           group packingInfo by new
                           {
                               packingInfo.PackingID,
                               packingInfo.PackingDate,
                               packingInfo.BoxIndex,
                               packingInfo.StockCode,
                               packingInfo.OrderID,
                           } into g
                           select new BoxListModel
                           {
                               PackingID = g.Key.PackingID,
                               PackingDate = g.Key.PackingDate,
                               BoxIndex = g.Key.BoxIndex,
                               StockCode = g.Key.StockCode,
                               OrderID = g.Key.OrderID,
                           };

            packingInfos = packingInfos.OrderByDescending(t => t.PackingDate).ThenByDescending(t => t.BoxIndex);

            return packingInfos;
        }

        public IEnumerable<BoxListModel> GetBoxListModelResult(out int total, int page, int rows, params LambdaExpression[] expressions)
        {
            var orderTraces = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>();
            var orders = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            var baseView = GetBoxListModelBase(expressions);

            total = baseView.Count();

            var thePackingInfos = baseView.Skip(rows * (page - 1)).Take(rows);

            thePackingInfos = from thePackingInfo in thePackingInfos

                              join order in orders on new { OrderID = thePackingInfo.OrderID, OrderDataStatus = (int)Enums.Status.Normal, }
                                    equals new { OrderID = order.ID, OrderDataStatus = order.Status, }
                                    into orders2
                              from order in orders2.DefaultIfEmpty()

                              join client in clients on new { ClientID = order.ClientID, ClientStatus = (int)Enums.Status.Normal, }
                                    equals new { ClientID = client.ID, ClientStatus = client.Status, }
                                    into clients2
                              from client in clients2.DefaultIfEmpty()

                              join company in companies on new { CompanyID = client.CompanyID, CompanyStatus = (int)Enums.Status.Normal, }
                                    equals new { CompanyID = company.ID, CompanyStatus = company.Status, }
                                    into companies2
                              from company in companies2.DefaultIfEmpty()
                              select new BoxListModel
                              {
                                  PackingID = thePackingInfo.PackingID,
                                  PackingDate = thePackingInfo.PackingDate,
                                  BoxIndex = thePackingInfo.BoxIndex,
                                  StockCode = thePackingInfo.StockCode,
                                  OrderID = thePackingInfo.OrderID,
                                  ClientCode = client.ClientCode,
                                  ClientName = company.Name,
                              };


            var listThePackingInfos = thePackingInfos.ToList();

            var listAllOrderTraceSealedInfos = (from thePackingInfo in thePackingInfos
                                                join orderTrace in orderTraces on new { OrderID = thePackingInfo.OrderID, Step = (int)Enums.OrderTraceStep.HKProcessing, }
                                                     equals new { OrderID = orderTrace.OrderID, Step = orderTrace.Step, }
                                                join admin in adminsTopView on new { AdminID = orderTrace.AdminID, AdminStatus = (int)Enums.Status.Normal, }
                                                     equals new { AdminID = admin.ID, AdminStatus = admin.Status, }
                                                     into adminsTopView2
                                                from admin in adminsTopView2.DefaultIfEmpty()
                                                orderby orderTrace.CreateDate descending
                                                select new BoxListModel
                                                {
                                                    OrderID = thePackingInfo.OrderID,
                                                    SealedName = admin?.RealName,
                                                    OrderTraceCreateDate = orderTrace.CreateDate,
                                                }).ToList();

            foreach (var thePackingInfo in listThePackingInfos)
            {
                var targetOrderTraceSealedInfo = listAllOrderTraceSealedInfos.Where(t => t.OrderID == thePackingInfo.OrderID).FirstOrDefault();

                if (targetOrderTraceSealedInfo != null)
                {
                    thePackingInfo.SealedName = targetOrderTraceSealedInfo.SealedName;
                    thePackingInfo.SealedDate = targetOrderTraceSealedInfo.OrderTraceCreateDate;
                }
            }

            return listThePackingInfos.AsEnumerable();
        }

        #endregion

        #region 鸿图库房查看装箱单 - 型号明细

        public class BoxDetailModel
        {
            /// <summary>
            /// 型号
            /// </summary>
            public string Model { get; set; } = string.Empty;

            /// <summary>
            /// 品名
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// 品牌
            /// </summary>
            public string Manufacturer { get; set; } = string.Empty;

            /// <summary>
            /// 数量
            /// </summary>
            public decimal Quantity { get; set; }

            /// <summary>
            /// 产地
            /// </summary>
            public string Origin { get; set; } = string.Empty;

            /// <summary>
            /// 毛重
            /// </summary>
            public decimal GrossWeight { get; set; }
        }

        public IEnumerable<BoxDetailModel> GetBoxDetailModel(string packingID)
        {
            var packings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>();
            var packingItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>();
            var sortings = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();
            var orderItems = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            //var products = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Products>();
            var orderItemCategories = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>();

            var results = from packing in packings
                          join packingItem in packingItems on new { PackingID = packing.ID, ThePackingID = packing.ID, }
                                equals new { PackingID = packingItem.PackingID, ThePackingID = packingID, }

                          join sorting in sortings on new { SortingID = packingItem.SortingID, SortingStatus = (int)Enums.Status.Normal, }
                                equals new { SortingID = sorting.ID, SortingStatus = sorting.Status, }
                                into sortings2
                          from sorting in sortings2.DefaultIfEmpty()

                          join orderItem in orderItems on new { OrderItemID = sorting.OrderItemID, OrderItemStatus = (int)Enums.Status.Normal, }
                                equals new { OrderItemID = orderItem.ID, OrderItemStatus = orderItem.Status, }
                                into orderItems2
                          from orderItem in orderItems2.DefaultIfEmpty()

                          //join product in products on new { ProductID = orderItem.ProductID, }
                          //      equals new { ProductID = product.ID, }
                          //      into products2
                          //from product in products2.DefaultIfEmpty()

                          join orderItemCategory in orderItemCategories on new { OrderItemID = orderItem.ID, OrderItemCategoryStatus = (int)Enums.Status.Normal, }
                                equals new { OrderItemID = orderItemCategory.OrderItemID, OrderItemCategoryStatus = orderItemCategory.Status, }
                                into orderItemCategories2
                          from orderItemCategory in orderItemCategories2.DefaultIfEmpty()
                          select new BoxDetailModel
                          {
                              Model = orderItem.Model,
                              Name = orderItemCategory.Name,
                              Manufacturer = orderItem.Manufacturer,
                              Quantity = sorting.Quantity,
                              Origin = orderItem.Origin,
                              GrossWeight = sorting.GrossWeight,
                          };

            results = from result in results
                      group result by new { result.Model, result.Name, result.Manufacturer, result.Quantity, result.Origin, result.GrossWeight, } into g
                      select new BoxDetailModel
                      {
                          Model = g.Key.Model,
                          Name = g.Key.Name,
                          Manufacturer = g.Key.Manufacturer,
                          Quantity = g.Key.Quantity,
                          Origin = g.Key.Origin,
                          GrossWeight = g.Key.GrossWeight,
                      };

            return results;
        }

        #endregion

    }
}
