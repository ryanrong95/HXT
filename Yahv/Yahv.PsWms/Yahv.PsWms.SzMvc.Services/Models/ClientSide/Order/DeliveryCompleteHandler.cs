using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Notice;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    /// <summary>
    /// 到货完成处理
    /// </summary>
    public class DeliveryCompleteHandler
    {
        private string _OrderID { get; set; }

        public DeliveryCompleteHandler(string orderID)
        {
            this._OrderID = orderID;
        }

        /// <summary>
        /// 检查是否有这个订单
        /// </summary>
        /// <returns></returns>
        public bool CheckIsHasOrder()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                int count = repository.ReadTable<Layers.Data.Sqls.PsOrder.Orders>().Where(t => t.ID == this._OrderID).Count();
                return count > 0;
            }
        }

        /*
        
        OrderItemID ==> FormItemID
        OrderID ==> FormID
        CustomCode ==> CustomCode
        StocktakingType ==> mpq =1，按个则该值为1; 否则 mpq >1 就是按最小包装量，该值为2
        Mpq ==> Mpq
        PackageNumber ==> StoragePackageNumber
        Total ==> StorageTotal

        Partnumber ==> Partnumber
        Brand ==> Brand
        Package ==> Package
        DateCode ==> DateCode
        Mpq ==> Mpq

         */

        /// <summary>
        /// 同步 OrderItem 信息
        /// </summary>
        public void SyncOrderItem()
        {
            string BatchID = DateTime.Now.ToString("yyyyMMddHHmmssffff") + Guid.NewGuid().ToString("N").Substring(0, 6);

            new Log()
            {
                ID = Guid.NewGuid().ToString("N"),
                ActionName = LogAction.DeliveryCompleteHandle.GetDescription(),
                MainID = BatchID,
                CreateDate = DateTime.Now,
            }.Insert();

            //1. 查询 topView 视图, 并将 topView 视图中需要的字段，数据保存下来
            DeliveryTopViewModel[] deliveryTopViewModels;

            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                deliveryTopViewModels = repository.ReadTable<Layers.Data.Sqls.PsOrder.DeliveryTopView>().Where(t => t.FormID == this._OrderID)
                                        .Select(item => new DeliveryTopViewModel
                                        {
                                            InputID = item.InputID,
                                            StoragePackageNumber = item.StoragePackageNumber,
                                            StorageTotal = item.StorageTotal,
                                            FormID = item.FormID,
                                            FormItemID = item.FormItemID,
                                            Partnumber = item.Partnumber,
                                            Brand = item.Brand,
                                            Package = item.Package,
                                            DateCode = item.DateCode,
                                            Mpq = item.Mpq,
                                            //CustomCode = item.CustomCode,
                                            NoticeID = item.NoticeID,
                                            NoticeItemID = item.NoticeItemID,
                                        }).ToArray();
            }

            deliveryTopViewModels = (from delivery in deliveryTopViewModels
                                     group delivery by new { delivery.InputID, } into g
                                     select new DeliveryTopViewModel
                                     {
                                         InputID = g.Key.InputID,
                                         StoragePackageNumber = g.Sum(t => t.StoragePackageNumber),
                                         StorageTotal = g.Sum(t => t.StorageTotal),
                                         FormID = g.FirstOrDefault()?.FormID,
                                         FormItemID = g.FirstOrDefault()?.FormItemID,
                                         Partnumber = g.FirstOrDefault()?.Partnumber,
                                         Brand = g.FirstOrDefault()?.Brand,
                                         Package = g.FirstOrDefault()?.Package,
                                         DateCode = g.FirstOrDefault()?.DateCode,
                                         Mpq = g.FirstOrDefault()?.Mpq,
                                         //CustomCode
                                         NoticeID = g.FirstOrDefault()?.NoticeID,
                                         NoticeItemID = g.FirstOrDefault()?.NoticeItemID,
                                     }).ToArray();

            OrderItemOrigin handler = new OrderItemOrigin();

            var abc = deliveryTopViewModels.Select(item => new OrderItemOriginModel
            {
                ID = string.Join(@"_", item.FormItemID, BatchID, BakOrderItem.DeliveryTopView.GetDescription()),
                OrderID = item.FormID,
                ProductID = "",
                Supplier = null,
                Origin = null,
                CustomCode = item.CustomCode,
                StocktakingType = CheckStocktakingTypeByMpq(item.Mpq),
                Mpq = item.Mpq ?? 1,
                PackageNumber = item.StoragePackageNumber,
                Total = item.StorageTotal,
                Currency = (int)Currency.CNY,
                UnitPrice = 0,
                StorageID = null,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = GeneralStatus.Closed,
                BakPartnumber = item.Partnumber,
                BakBrand = item.Brand,
                BakPackage = item.Package,
                BakDateCode = item.DateCode,
                NoticeID = item.NoticeID,
                NoticeItemID = item.NoticeItemID,
                InputID = item.InputID,
            }).ToArray();
            for (int i = 0; i < abc.Length; i++)
            {
                if (abc[i].ID.StartsWith("_"))
                {
                    abc[i].ID = Guid.NewGuid().ToString("N").Substring(0, 5) + abc[i].ID;
                }
            }
            handler.Insert(abc);

            //2. 将这个订单中原有的数据保存下来
            var orginOrderItems = handler.Query(this._OrderID);

            handler.Insert(orginOrderItems.Select(item => new OrderItemOriginModel
            {
                ID = string.Join(@"_", item.ID, BatchID, BakOrderItem.OriginOrderItem.GetDescription()),
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                Supplier = item.Supplier,
                Origin = item.Origin,
                CustomCode = item.CustomCode,
                StocktakingType = item.StocktakingType,
                Mpq = item.Mpq,
                PackageNumber = item.PackageNumber,
                Total = item.Total,
                Currency = item.Currency,
                UnitPrice = item.UnitPrice,
                StorageID = item.StorageID,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = GeneralStatus.Closed,
                BakPartnumber = item.BakPartnumber,
                BakBrand = item.BakBrand,
                BakPackage = item.BakPackage,
                BakDateCode = item.BakDateCode,
                NoticeID = item.NoticeID,
                NoticeItemID = item.NoticeItemID,
                InputID = item.InputID,
            }).ToArray());

            #region 做新增、删除（假删）、更新操作

            string[] deliveryTopViewModelIDs = deliveryTopViewModels.Select(item => item.FormItemID).ToArray();
            string[] orginOrderItemIDs = orginOrderItems.Select(item => item.ID).ToArray();
            var intersectOrderItemIDs = deliveryTopViewModelIDs.Intersect(orginOrderItemIDs).ToArray(); //交集

            //3. 找出新增的 OrderItem，插入
            //var preInsertOrderItemIDs = deliveryTopViewModelIDs.Where(item => !intersectOrderItemIDs.Contains(item)).ToArray();
            var newOrderItemList = new List<OrderItemOriginModel>();
            var newProductList = new List<ProductModel>();
            var nullFormItemIdDeliveryTopViewModels = deliveryTopViewModels.Where(t => t.FormItemID == null).ToArray();
            foreach (var item in nullFormItemIdDeliveryTopViewModels)
            {
                string newOrderItemID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.OrderItem);
                string newProductID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Product);

                newProductList.Add(new ProductModel
                {
                    ID = newProductID,
                    Partnumber = item.Partnumber,
                    Brand = item.Brand,
                    Package = item.Package,
                    DateCode = item.DateCode,
                    Mpq = item.Mpq ?? 1,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                });

                newOrderItemList.Add(new OrderItemOriginModel
                {
                    ID = newOrderItemID,
                    OrderID = item.FormID,
                    ProductID = newProductID,
                    Supplier = null,
                    Origin = null,
                    CustomCode = item.CustomCode,
                    StocktakingType = CheckStocktakingTypeByMpq(item.Mpq),
                    Mpq = item.Mpq ?? 1,
                    PackageNumber = item.StoragePackageNumber,
                    Total = item.StorageTotal,
                    Currency = (int)Currency.CNY,
                    UnitPrice = 0,
                    StorageID = null,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = GeneralStatus.Normal,
                    BakPartnumber = item.Partnumber,
                    BakBrand = item.Brand,
                    BakPackage = item.Package,
                    BakDateCode = item.DateCode,
                    NoticeID = item.NoticeID,
                    NoticeItemID = item.NoticeItemID,
                    InputID = item.InputID,
                });
            }

            handler.InsertProducts(newProductList.ToArray());
            handler.Insert(newOrderItemList.ToArray());


            //4. 找出已经不存在的 OrderItem，删除
            var preDeleteOrderItemIDs = orginOrderItemIDs.Where(item => !intersectOrderItemIDs.Contains(item)).ToArray();
            handler.Delete(preDeleteOrderItemIDs);

            //5. 找出 仍然在的 OrderItem，逐个更新
            var preUpdateOrderItems = orginOrderItems.Where(t => intersectOrderItemIDs.Contains(t.ID)).ToArray();
            for (int i = 0; i < preUpdateOrderItems.Length; i++)
            {
                var theDeliveryTopViewModel = deliveryTopViewModels.Where(t => t.FormItemID == preUpdateOrderItems[i].ID).FirstOrDefault();
                if (theDeliveryTopViewModel != null)
                {
                    preUpdateOrderItems[i].CustomCode = theDeliveryTopViewModel.CustomCode;
                    preUpdateOrderItems[i].StocktakingType = CheckStocktakingTypeByMpq(theDeliveryTopViewModel.Mpq);
                    preUpdateOrderItems[i].Mpq = theDeliveryTopViewModel.Mpq ?? 1;
                    preUpdateOrderItems[i].PackageNumber = theDeliveryTopViewModel.StoragePackageNumber;
                    preUpdateOrderItems[i].Total = theDeliveryTopViewModel.StorageTotal;
                    preUpdateOrderItems[i].ModifyDate = DateTime.Now;
                    preUpdateOrderItems[i].BakPartnumber = theDeliveryTopViewModel.Partnumber;
                    preUpdateOrderItems[i].BakBrand = theDeliveryTopViewModel.Brand;
                    preUpdateOrderItems[i].BakPackage = theDeliveryTopViewModel.Package;
                    preUpdateOrderItems[i].BakDateCode = theDeliveryTopViewModel.DateCode;
                    preUpdateOrderItems[i].NoticeID = theDeliveryTopViewModel.NoticeID;
                    preUpdateOrderItems[i].NoticeItemID = theDeliveryTopViewModel.NoticeItemID;
                    preUpdateOrderItems[i].InputID = theDeliveryTopViewModel.InputID;
                    handler.Update(preUpdateOrderItems[i]);
                }
            }

            #endregion

            //6. 将 OrderItem 的结果数据再做一个备份
            var newOrderItems = handler.Query(this._OrderID);

            handler.Insert(newOrderItems.Select(item => new OrderItemOriginModel
            {
                ID = string.Join(@"_", item.ID, BatchID, BakOrderItem.NewOrderItem.GetDescription()),
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                Supplier = item.Supplier,
                Origin = item.Origin,
                CustomCode = item.CustomCode,
                StocktakingType = item.StocktakingType,
                Mpq = item.Mpq,
                PackageNumber = item.PackageNumber,
                Total = item.Total,
                Currency = item.Currency,
                UnitPrice = item.UnitPrice,
                StorageID = item.StorageID,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = GeneralStatus.Closed,
                BakPartnumber = item.BakPartnumber,
                BakBrand = item.BakBrand,
                BakPackage = item.BakPackage,
                BakDateCode = item.BakDateCode,
                NoticeID = item.NoticeID,
                NoticeItemID = item.NoticeItemID,
                InputID = item.InputID,
            }).ToArray());

            ////根据 NoticeTransportsTopView 同步货运信息 Begin

            //NoticeTransportHandler noticeTransportHandler = new NoticeTransportHandler(this._OrderID);
            //noticeTransportHandler.Sync();

            ////根据 NoticeTransportsTopView 同步货运信息 End

            Task.Run(() =>
            {
                try
                {
                    using (PsOrderRepository repository = new PsOrderRepository())
                    {
                        var order = repository.ReadTable<Layers.Data.Sqls.PsOrder.Orders>().Where(t => t.ID == this._OrderID).FirstOrDefault();
                        var orderItems = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>()
                            .Where(t => t.OrderID == this._OrderID && t.Status == (int)GeneralStatus.Normal).ToArray();
                        var productIDs = orderItems.Select(t => t.ProductID).ToArray();
                        var products = repository.ReadTable<Layers.Data.Sqls.PsOrder.Products>().Where(t => productIDs.Contains(t.ID)).ToArray();
                        var orderTransport = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>().Where(t => t.OrderID == this._OrderID).FirstOrDefault();
                        var requires = repository.ReadTable<Layers.Data.Sqls.PsOrder.Requires>().Where(t => t.OrderID == this._OrderID).ToArray();
                        var files = repository.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>().Where(t => t.MainID == this._OrderID).ToArray();

                        var client = repository.ReadTable<Layers.Data.Sqls.PsOrder.Clients>().Where(t => t.ID == order.ClientID).FirstOrDefault();

                        if (order.Type == (int)Enums.OrderType.Inbound)
                        {
                            StorageInNoticeService noticeService = new StorageInNoticeService();
                            noticeService.Order = order;
                            noticeService.OrderItems = orderItems;
                            noticeService.Products = products;
                            noticeService.OrderTransport = orderTransport;
                            noticeService.Requires = requires;
                            noticeService.Files = files;
                            //noticeService.GenerateJson(client.TrackerID);
                            noticeService.GenerateJsonNew(client.TrackerID);
                            noticeService.SendNotice(this._OrderID);
                            noticeService.GenerateJsonFile();
                            noticeService.SendFileInfo(this._OrderID);
                        }
                        else if (order.Type == (int)Enums.OrderType.Outbound)
                        {
                            //var picker = repository.ReadTable<Layers.Data.Sqls.PsOrder.Pickers>().Where(t => t.ID == orderTransport.PickerID).FirstOrDefault();

                            StorageOutNoticeService noticeService = new StorageOutNoticeService();
                            noticeService.Order = order;
                            noticeService.OrderItems = orderItems;
                            noticeService.Products = products;
                            noticeService.OrderTransport = orderTransport;
                            noticeService.Requires = requires;
                            //noticeService.Picker = picker;
                            noticeService.FileInfos = files;
                            noticeService.GenerateJson(client.TrackerID);
                            noticeService.SendNotice(this._OrderID);
                            noticeService.GenerateJsonFile();
                            noticeService.SendFileInfo(this._OrderID);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (PsOrderRepository repository = new PsOrderRepository())
                    {
                        repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            ActionName = LogAction.SendNoticeAfterDeliveryComplete.GetDescription(),
                            MainID = this._OrderID,
                            Content = ex.Message,
                            CreateDate = DateTime.Now,
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 根据 mpq 判断 StocktakingType
        /// </summary>
        /// <param name="mpq"></param>
        /// <returns></returns>
        private StocktakingType CheckStocktakingTypeByMpq(int? mpq)
        {
            if (mpq == null)
            {
                return StocktakingType.Single;
            }

            if (mpq == 1)
            {
                return StocktakingType.Single;
            }

            if (mpq > 1)
            {
                return StocktakingType.MinPackage;
            }

            return StocktakingType.Single;
        }

        public class DeliveryTopViewModel
        {
            /// <summary>
            /// 
            /// </summary>
            public string InputID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int StoragePackageNumber { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int StorageTotal { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string FormID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string FormItemID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Partnumber { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Brand { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Package { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DateCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int? Mpq { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string CustomCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string NoticeID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string NoticeItemID { get; set; }
        }
    }
}
