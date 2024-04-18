using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class OrderController : BaseController
    {
        #region 页面

        /// <summary>
        /// 入库订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailIn() { return View(); }

        /// <summary>
        /// 入库订单详情-订单基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailInBaseInfo() { return PartialView(); }

        /// <summary>
        /// 入库订单详情-特殊要求
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailInSpecialRequire() { return PartialView(); }

        /// <summary>
        /// 入库订单详情-货运信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailInTransport() { return PartialView(); }

        /// <summary>
        /// 入库订单详情-附件信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailInAttachment() { return PartialView(); }

        /// <summary>
        /// 入库订单详情-费用信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailInCostInfo() { return PartialView(); }

        #endregion


        /// <summary>
        /// 获取入库订单信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageDetailIn(string OrderID)
        {
            GetStorageDetailInModel returnModel = new GetStorageDetailInModel();

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var orderItems = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>();
                var products = repository.ReadTable<Layers.Data.Sqls.PsOrder.Products>();
                var requires = repository.ReadTable<Layers.Data.Sqls.PsOrder.Requires>();
                var orderTransports = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>();
                var pcFiles = repository.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>();

                var orderItemsDB = (from orderItem in orderItems
                                    join product in products on orderItem.ProductID equals product.ID
                                    where orderItem.OrderID == OrderID && orderItem.Status == (int)GeneralStatus.Normal
                                    select new
                                    {
                                        CustomCode = orderItem.CustomCode,
                                        PartNumber = product.Partnumber,
                                        Brand = product.Brand,
                                        Package = product.Package,
                                        DateCode = product.DateCode,
                                        StocktakingTypeInt = orderItem.StocktakingType,
                                        Mpq = orderItem.Mpq,
                                        PackageNumber = orderItem.PackageNumber,
                                        ItemTotal = orderItem.Mpq * orderItem.PackageNumber,
                                    }).ToArray();

                int allPackageNumber = orderItemsDB.Sum(t => t.PackageNumber);
                int allItemTotal = orderItemsDB.Sum(t => t.ItemTotal);

                var requiresDB = (from require in requires
                                  where require.OrderID == OrderID
                                  select new
                                  {
                                      Name = require.Name,
                                      Content = require.Content,
                                  }).ToArray();

                var orderTransportDB = (from orderTransport in orderTransports
                                        where orderTransport.OrderID == OrderID
                                        select new
                                        {
                                            TransportModeInt = orderTransport.TransportMode,
                                            Carrier = orderTransport.Carrier,
                                            ExpressNumber = orderTransport.WaybillCode,
                                            TakingDate = orderTransport.TakingTime,
                                            ConsigneeManName = orderTransport.Contact,
                                            ConsigneeManTel = orderTransport.Phone,
                                            ConsigneeManAddress = orderTransport.Address,
                                        }).FirstOrDefault();

                var takingFilesDB = (from pcFile in pcFiles
                                     where pcFile.MainID == OrderID && pcFile.Type == (int)PsOrderFileType.Taking
                                     select new
                                     {
                                         name = pcFile.CustomName,
                                         Url = pcFile.Url,
                                     }).ToArray();

                var packingFilesDB = (from pcFile in pcFiles
                                      where pcFile.MainID == OrderID && pcFile.Type == (int)PsOrderFileType.InDelivery
                                      select new
                                      {
                                          name = pcFile.CustomName,
                                          Url = pcFile.Url,
                                      }).ToArray();

                //组装 returnModel
                returnModel.OrderID = OrderID;

                List<GetStorageDetailInModel.OrderItem> orderItemList = new List<GetStorageDetailInModel.OrderItem>();
                if (orderItemsDB != null && orderItemsDB.Length > 0)
                {
                    foreach (var item in orderItemsDB)
                    {
                        orderItemList.Add(new GetStorageDetailInModel.OrderItem
                        {
                            CustomCode = item.CustomCode,
                            PartNumber = item.PartNumber,
                            Brand = item.Brand,
                            Package = item.Package,
                            DateCode = item.DateCode,
                            StocktakingTypeDes = ((StocktakingType)item.StocktakingTypeInt).GetDescription(),
                            Mpq = item.Mpq,
                            PackageNumber = item.PackageNumber,
                            ItemTotal = item.Mpq * item.PackageNumber,
                        });
                    }
                }
                returnModel.OrderItems = orderItemList.ToArray();

                returnModel.AllPackageNumber = allPackageNumber;
                returnModel.AllItemTotal = allItemTotal;

                List<GetStorageDetailInModel.SpecialRequire> specialRequireList = new List<GetStorageDetailInModel.SpecialRequire>();
                if (requiresDB != null && requiresDB.Length > 0)
                {
                    foreach (var item in requiresDB)
                    {
                        specialRequireList.Add(new GetStorageDetailInModel.SpecialRequire
                        {
                            Name = item.Name,
                            IsOtherRequire = !string.IsNullOrEmpty(item.Content),
                            OtherRequire = item.Content,
                        });
                    }
                }
                returnModel.SpecialRequires = specialRequireList.ToArray();

                returnModel.TransportModeInt = orderTransportDB.TransportModeInt;
                returnModel.TransportModeName = ((TransportMode)orderTransportDB.TransportModeInt).GetDescription();
                returnModel.ExpressCompanyName = orderTransportDB.Carrier != null ?
                                                    ((Express)(Enum.Parse(typeof(Express), orderTransportDB.Carrier))).GetDescription() : string.Empty;
                returnModel.ExpressNumber = orderTransportDB.ExpressNumber;
                returnModel.TakingDate = orderTransportDB.TakingDate?.ToString("yyyy-MM-dd");
                returnModel.ConsigneeManName = orderTransportDB.ConsigneeManName;
                returnModel.ConsigneeManTel = orderTransportDB.ConsigneeManTel;
                returnModel.ConsigneeManAddress = orderTransportDB.ConsigneeManAddress;

                string fileUrlPrefix = ConfigurationManager.AppSettings["FileUrlPrefix"];

                List<GetStorageDetailInModel.File> takingFileList = new List<GetStorageDetailInModel.File>();
                if (takingFilesDB != null && takingFilesDB.Length > 0)
                {
                    foreach (var item in takingFilesDB)
                    {
                        takingFileList.Add(new GetStorageDetailInModel.File
                        {
                            name = item.name,
                            fullURL = string.Join(@"/", fileUrlPrefix, item.Url),
                        });
                    }
                }
                returnModel.TakingFiles = takingFileList.ToArray();

                List<GetStorageDetailInModel.File> packingFileList = new List<GetStorageDetailInModel.File>();
                if (packingFilesDB != null && packingFilesDB.Length > 0)
                {
                    foreach (var item in packingFilesDB)
                    {
                        packingFileList.Add(new GetStorageDetailInModel.File
                        {
                            name = item.name,
                            fullURL = string.Join(@"/", fileUrlPrefix, item.Url),
                        });
                    }
                }
                returnModel.PackingFiles = packingFileList.ToArray();
            }

            //库房修改的货运信息
            var storageTransportInfo = new Yahv.PsWms.SzMvc.Services.Views.TransportInfoFromStorageView().Where(t => t.FormID == OrderID).FirstOrDefault();
            if (storageTransportInfo != null)
            {
                returnModel.StorageTransportInfo = new GetStorageDetailInModel.TransportInfoFromStorage
                {
                    TransportModeInt = storageTransportInfo.TransportMode,
                    CarrierName = storageTransportInfo.Carrier != null ?
                                    ((Express)(Enum.Parse(typeof(Express), storageTransportInfo.Carrier))).GetDescription() : string.Empty,
                    WaybillCode = storageTransportInfo.WaybillCode,
                };
            }

            return Json(new { success = 200, result = returnModel }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取入库订单费用列表
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult GetInStorageFeeList(string OrderID)
        {
            using (var query = new FeeListInOrderView(OrderID))
            {
                var view = query;

                Func<FeeListInOrderViewModel, GetInStorageFeeListReturnModel> convert = item => new GetInStorageFeeListReturnModel
                {
                    CutDateIndex = Convert.ToString(item.CutDateIndex),
                    Subject = item.Subject,
                    TotalDes = item.Total.ToString("0.00"),
                };

                var viewData = view.ToMyPage(convert);

                return Json(new { type = "success", msg = "", data = new { list = viewData.Item1, } }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}