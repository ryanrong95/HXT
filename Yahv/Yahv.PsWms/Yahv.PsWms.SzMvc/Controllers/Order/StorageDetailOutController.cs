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
        /// 出库订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailOut() { return View(); }

        /// <summary>
        /// 出库订单详情-订单基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailOutBaseInfo() { return PartialView(); }

        /// <summary>
        /// 出库订单详情-特殊要求
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailOutSpecialRequire() { return PartialView(); }

        /// <summary>
        /// 出库订单详情-货运信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailOutTransport() { return PartialView(); }

        /// <summary>
        /// 出库订单详情-费用信息
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageDetailOutCostInfo() { return PartialView(); }

        #endregion


        /// <summary>
        /// 获取出库订单信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageDetailOut(string OrderID)
        {
            GetStorageDetailOutModel returnModel = new GetStorageDetailOutModel();

            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var orderItems = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderItems>();
                var products = repository.ReadTable<Layers.Data.Sqls.PsOrder.Products>();
                var requires = repository.ReadTable<Layers.Data.Sqls.PsOrder.Requires>();
                var orderTransports = repository.ReadTable<Layers.Data.Sqls.PsOrder.OrderTransports>();
                var pcFiles = repository.ReadTable<Layers.Data.Sqls.PsOrder.PcFiles>();
                var pickers = repository.ReadTable<Layers.Data.Sqls.PsOrder.Pickers>();

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
                                            Address = orderTransport.Address,
                                            Contact = orderTransport.Contact,
                                            Phone = orderTransport.Phone,

                                            Carrier = orderTransport.Carrier,
                                            ExpressTransport = orderTransport.ExpressTransport,
                                            ExpressPayer = orderTransport.ExpressPayer,
                                            ExpressEscrow = orderTransport.ExpressEscrow,
                                            TakingTime = orderTransport.TakingTime,

                                            PickerID = orderTransport.PickerID,
                                        }).FirstOrDefault();

                var faHuoDanFileDB = (from pcFile in pcFiles
                                      where pcFile.MainID == OrderID && pcFile.Type == (int)PsOrderFileType.OutDelivery
                                      select new
                                      {
                                          filename = pcFile.CustomName,
                                          URL = pcFile.Url,
                                      }).FirstOrDefault();

                var keHuBiaoQianDB = (from pcFile in pcFiles
                                      where pcFile.MainID == OrderID && pcFile.Type == (int)PsOrderFileType.Label
                                      select new
                                      {
                                          filename = pcFile.CustomName,
                                          URL = pcFile.Url,
                                      }).FirstOrDefault();

                //组装 returnModel
                returnModel.OrderID = OrderID;

                List<GetStorageDetailOutModel.OrderItem> orderItemList = new List<GetStorageDetailOutModel.OrderItem>();
                if (orderItemsDB != null && orderItemsDB.Length > 0)
                {
                    foreach (var item in orderItemsDB)
                    {
                        orderItemList.Add(new GetStorageDetailOutModel.OrderItem
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

                List<GetStorageDetailOutModel.SpecialRequire> specialRequireList = new List<GetStorageDetailOutModel.SpecialRequire>();
                if (requiresDB != null && requiresDB.Length > 0)
                {
                    string fileUrlPrefix = ConfigurationManager.AppSettings["FileUrlPrefix"];

                    foreach (var item in requiresDB)
                    {
                        bool isHasFile = false;
                        string filename = string.Empty;
                        string fullURL = string.Empty;

                        if (item.Name == StorageOutSpecialRequire.FHD.GetDescription() && faHuoDanFileDB != null)
                        {
                            isHasFile = true;
                            filename = faHuoDanFileDB.filename;
                            fullURL = string.Join(@"/", fileUrlPrefix, faHuoDanFileDB.URL);
                        }
                        else if (item.Name == StorageOutSpecialRequire.KHBQ.GetDescription() && keHuBiaoQianDB != null)
                        {
                            isHasFile = true;
                            filename = keHuBiaoQianDB.filename;
                            fullURL = string.Join(@"/", fileUrlPrefix, keHuBiaoQianDB.URL);
                        }

                        specialRequireList.Add(new GetStorageDetailOutModel.SpecialRequire
                        {
                            Name = item.Name,
                            IsOtherRequire = !string.IsNullOrEmpty(item.Content),
                            OtherRequire = item.Content,
                            IsHasFile = isHasFile,
                            filename = filename,
                            fullURL = fullURL,
                        });
                    }
                }
                returnModel.SpecialRequires = specialRequireList.ToArray();

                returnModel.TransportModeInt = orderTransportDB.TransportModeInt;
                returnModel.TransportModeName = ((TransportMode)orderTransportDB.TransportModeInt).GetDescription();

                if (orderTransportDB.TransportModeInt == (int)TransportMode.Express)
                {
                    returnModel.DeliverTargetManShow = orderTransportDB.Contact;
                    returnModel.DeliverTargetTelShow = orderTransportDB.Phone;
                    returnModel.DeliverTargetAddressShow = orderTransportDB.Address;

                    var expressEnum = (Express)(Enum.Parse(typeof(Express), orderTransportDB.Carrier));
                    returnModel.ExpressCompanyShow = expressEnum.GetDescription();
                    switch (expressEnum)
                    {
                        case Express.SF:
                            returnModel.ExpressMethodShow = ((ExpressMethodSF)Convert.ToInt32(orderTransportDB.ExpressTransport)).GetDescription();
                            break;
                        case Express.KY:
                            returnModel.ExpressMethodShow = ((ExpressMethodKY)Convert.ToInt32(orderTransportDB.ExpressTransport)).GetDescription();
                            break;
                        case Express.DB:
                            returnModel.ExpressMethodShow = ((ExpressMethodDB)Convert.ToInt32(orderTransportDB.ExpressTransport)).GetDescription();
                            break;
                        default:
                            returnModel.ExpressMethodShow = string.Empty;
                            break;
                    }

                    returnModel.FreightPayInt = Convert.ToString(orderTransportDB.ExpressPayer);
                    returnModel.FreightPayShow = ((Services.Enums.FreightPayer)(Convert.ToInt32(orderTransportDB.ExpressPayer))).GetDescription();
                    returnModel.ThirdParty = orderTransportDB.ExpressEscrow;
                }
                else if (orderTransportDB.TransportModeInt == (int)TransportMode.Dtd)
                {
                    returnModel.DeliverTargetMan2Show = orderTransportDB.Contact;
                    returnModel.DeliverTargetTel2Show = orderTransportDB.Phone;
                    returnModel.DeliverTargetAddress2Show = orderTransportDB.Address;
                }
                else if (orderTransportDB.TransportModeInt == (int)TransportMode.PickUp)
                {
                    returnModel.TakingMan = orderTransportDB.Contact;
                    returnModel.TakingTel = orderTransportDB.Phone;

                    returnModel.TakingDate = orderTransportDB.TakingTime?.ToString("yyyy-MM-dd");

                    if (!string.IsNullOrEmpty(orderTransportDB.PickerID))
                    {
                        var pickerDB = pickers.Where(t => t.ID == orderTransportDB.PickerID).FirstOrDefault();
                        if (pickerDB != null)
                        {
                            returnModel.ProofTypeShow = ((Services.Enums.IDType)pickerDB.IDType).GetDescription();
                            returnModel.ProofNumber = pickerDB.IDCode;
                        }
                    }
                }
            }

            //库房修改的货运信息
            var storageTransportInfo = new Yahv.PsWms.SzMvc.Services.Views.TransportInfoFromStorageView().Where(t => t.FormID == OrderID).FirstOrDefault();
            if (storageTransportInfo != null)
            {
                string expressTransportStorageShow = string.Empty;

                if (storageTransportInfo.TransportMode == (int)TransportMode.Express)
                {
                    var expressEnumStorage = (Express)(Enum.Parse(typeof(Express), storageTransportInfo.Carrier));
                    switch (expressEnumStorage)
                    {
                        case Express.SF:
                            expressTransportStorageShow = ((ExpressMethodSF)Convert.ToInt32(storageTransportInfo.ExpressTransport)).GetDescription();
                            break;
                        case Express.KY:
                            expressTransportStorageShow = ((ExpressMethodKY)Convert.ToInt32(storageTransportInfo.ExpressTransport)).GetDescription();
                            break;
                        case Express.DB:
                            expressTransportStorageShow = ((ExpressMethodDB)Convert.ToInt32(storageTransportInfo.ExpressTransport)).GetDescription();
                            break;
                        default:
                            expressTransportStorageShow = string.Empty;
                            break;
                    }
                }

                returnModel.StorageTransportInfo = new GetStorageDetailOutModel.TransportInfoFromStorage
                {
                    TransportModeInt = storageTransportInfo.TransportMode,
                    CarrierName = storageTransportInfo.Carrier != null ?
                                    ((Express)(Enum.Parse(typeof(Express), storageTransportInfo.Carrier))).GetDescription() : string.Empty,
                    WaybillCode = storageTransportInfo.WaybillCode,
                    ExpressPayerInt = storageTransportInfo.ExpressPayer != null ? Convert.ToString(storageTransportInfo.ExpressPayer) : string.Empty,
                    ExpressPayerShow = storageTransportInfo.ExpressPayer != null ?
                                        ((Services.Enums.FreightPayer)(Convert.ToInt32(storageTransportInfo.ExpressPayer))).GetDescription()
                                      : string.Empty,
                    ExpressTransportShow = expressTransportStorageShow,
                };
            }

            return Json(new { success = 200, result = returnModel }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取出库订单费用列表
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public JsonResult GetOutStorageFeeList(string OrderID)
        {
            using (var query = new FeeListInOrderView(OrderID))
            {
                var view = query;

                Func<FeeListInOrderViewModel, GetOutStorageFeeListReturnModel> convert = item => new GetOutStorageFeeListReturnModel
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