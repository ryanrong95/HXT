using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class OrderController : BaseController
    {
        #region 页面

        /// <summary>
        /// 新增出库订单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorage(string products)
        {
            StorageListModel[] appointedDataObjs = new StorageListModel[0];

            #region 是否由我的库存跳转

            if (!string.IsNullOrWhiteSpace(products))
            {
                var productList = JsonConvert.DeserializeObject<StorageListModel[]>(products).Select(item => new
                {
                    StorageID = item.StorageID,
                    NeedPackageNumber = item.NeedPackageNumber,
                }).ToArray();
                var productListIDs = productList.Select(item => item.StorageID).ToArray();

                using (var query = new StorageListView())
                {
                    var view = query;
                    view = view.SearchByStorageIDs(productListIDs);

                    Func<StorageListViewModel, StorageListModel> convert = item => new StorageListModel
                    {
                        StorageID = item.StorageID,
                        CustomCode = item.CustomCode,
                        PartNumber = item.Partnumber,
                        Brand = item.Brand,
                        Package = item.Package,
                        DateCode = item.DateCode,
                        StocktakingTypeInt = (int)item.StocktakingType,
                        StocktakingTypeName = item.StocktakingType.GetDescription(),
                        Mpq = item.Mpq,
                        StoragePackageNumber = item.PackageNumber - (item.Ex_PackageNumber ?? 0),
                        PackageNumber = 0, //然后要用 NeedPackageNumber 赋值

                        ItemTotal = 0, //然后要用 (Mpq * PackageNumber) 计算赋值
                    };

                    var dataList = view.ToMyPage(convert).Item1;

                    for (int i = 0; i < dataList.Length; i++)
                    {
                        if (productListIDs.Contains(dataList[i].StorageID))
                        {
                            var theProduct = productList.Where(t => t.StorageID == dataList[i].StorageID).FirstOrDefault();
                            dataList[i].PackageNumber = theProduct.NeedPackageNumber;
                            dataList[i].ItemTotal = dataList[i].Mpq * dataList[i].PackageNumber;
                        }
                    }

                    appointedDataObjs = dataList;
                }
            }

            #endregion

            NewOutStorageReturnModel returnModel = new NewOutStorageReturnModel()
            {
                OrderItemsFromMyStorage = appointedDataObjs,
            };

            return View(returnModel);
        }

        /// <summary>
        /// 新增出库订单-订单基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorageBaseInfo() { return PartialView(); }

        /// <summary>
        /// 新增出库订单-特殊要求
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorageSpecialRequire() { return PartialView(); }

        /// <summary>
        /// 新增出库订单-货运信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorageTransport() { return PartialView(); }

        /// <summary>
        /// 新增出库订单-库存导入窗口
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorageStockImport() { return PartialView(); }

        /// <summary>
        /// 新增出库订单-编辑送货地址窗口
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorageEditDeliveryInfo() { return PartialView(); }

        /// <summary>
        /// 新增出库订单-编辑提货信息窗口
        /// </summary>
        /// <returns></returns>
        public ActionResult NewOutStorageEditTakingInfo() { return PartialView(); }

        #endregion


        /// <summary>
        /// 获取我的库存数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMyStorage(GetMyStorageListSearchModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (var query = new StorageListView())
            {
                var view = query;

                view = view.SearchByClientID(theClientID);

                if (!string.IsNullOrEmpty(searchModel.PartNumber))
                {
                    view = view.SearchByPartnumber(searchModel.PartNumber);
                }

                if (!string.IsNullOrEmpty(searchModel.Brand))
                {
                    view = view.SearchByBrand(searchModel.Brand);
                }

                view = view.SearchByPackageNumberIsNotZero();

                Func<StorageListViewModel, StorageListModel> convert = item => new StorageListModel
                {
                    StorageID = item.StorageID,
                    CustomCode = item.CustomCode,
                    PartNumber = item.Partnumber,
                    Brand = item.Brand,
                    Package = item.Package,
                    DateCode = item.DateCode,
                    StocktakingTypeInt = (int)item.StocktakingType,
                    StocktakingTypeName = item.StocktakingType.GetDescription(),
                    Mpq = item.Mpq,
                    PackageNumber = item.PackageNumber - (item.Ex_PackageNumber ?? 0),
                    LocationNo = item.Code,

                    ItemTotal = item.Mpq * (item.PackageNumber - (item.Ex_PackageNumber ?? 0)),
                    IsCheck = false,
                    NeedPackageNumber = item.PackageNumber - (item.Ex_PackageNumber ?? 0),
                };

                var viewData = view.ToMyPage(convert, searchModel.page, searchModel.rows);

                return Json(new { type = "success", msg = "", data = new { list = viewData.Item1, total = viewData.Item2 } }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取指定的库存信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAppointedStorage()
        {
            var addedInfosStr = Request.Form["AddedInfos"];
            var addedInfos = JsonConvert.DeserializeObject<StorageListModel[]>(addedInfosStr);
            var addedStorageIDs = addedInfos.Select(t => t.StorageID).ToArray();

            StorageListModel[] appointedDataObjs = new StorageListModel[0];

            using (var query = new StorageListView())
            {
                var view = query;
                view = view.SearchByStorageIDs(addedStorageIDs.ToArray());

                Func<StorageListViewModel, StorageListModel> convert = item => new StorageListModel
                {
                    StorageID = item.StorageID,
                    CustomCode = item.CustomCode,
                    PartNumber = item.Partnumber,
                    Brand = item.Brand,
                    Package = item.Package,
                    DateCode = item.DateCode,
                    StocktakingTypeInt = (int)item.StocktakingType,
                    StocktakingTypeName = item.StocktakingType.GetDescription(),
                    Mpq = item.Mpq,
                    StoragePackageNumber = item.PackageNumber - (item.Ex_PackageNumber ?? 0),
                    PackageNumber = 0, //然后要用 NeedPackageNumber 赋值

                    ItemTotal = 0, //然后要用 (Mpq * PackageNumber) 计算赋值
                };

                var dataList = view.ToMyPage(convert).Item1;

                for (int i = 0; i < dataList.Length; i++)
                {
                    if (addedStorageIDs.Contains(dataList[i].StorageID))
                    {
                        var theProduct = addedInfos.Where(t => t.StorageID == dataList[i].StorageID).FirstOrDefault();
                        dataList[i].PackageNumber = theProduct.PackageNumber;
                        dataList[i].ItemTotal = dataList[i].Mpq * dataList[i].PackageNumber;
                    }
                }

                appointedDataObjs = dataList;
            }

            return Json(new { type = "success", msg = "", data = new { list = appointedDataObjs, } }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增收货地址提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult NewDeliveryInfoSubmit(NewDeliveryInfoSubmitModel model)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            try
            {
                if (!string.IsNullOrEmpty(model.Contact))
                {
                    model.Contact = model.Contact.Trim();
                }
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    model.Phone = model.Phone.Trim();
                }

                if (model.Address != null && model.Address.Length > 0 && model.Address[0].Contains(","))
                {
                    string[] addressArray = model.Address[0].Split(',');
                    model.Address = addressArray;
                }

                string addressValue = string.Join(" ", model.Address?.Concat(new string[] { model.AddressDetail.Trim() }) ?? Array.Empty<string>());

                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Insert(new Layers.Data.Sqls.PsOrder.Addresses
                    {
                        ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Address),
                        Type = (int)Services.Enums.AddressType.Consignee,
                        ClientID = theClientID,
                        Title = model.Title,
                        Contact = model.Contact,
                        Address = addressValue,
                        Phone = model.Phone,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                    });
                }

                return Json(new { type = "success", msg = "保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = "保存失败：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增提货人信息提交
        /// </summary>
        /// <returns></returns>
        public JsonResult NewTakingInfoSubmit(NewTakingInfoSubmitModel model)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            try
            {
                if (!string.IsNullOrEmpty(model.TakingMan))
                {
                    model.TakingMan = model.TakingMan.Trim();
                }
                if (!string.IsNullOrEmpty(model.TakingTel))
                {
                    model.TakingTel = model.TakingTel.Trim();
                }
                if (!string.IsNullOrEmpty(model.ProofNumber))
                {
                    model.ProofNumber = model.ProofNumber.Trim();
                }

                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Insert(new Layers.Data.Sqls.PsOrder.Pickers
                    {
                        ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Picker),
                        ClientID = theClientID,
                        IDType = Convert.ToInt32(model.ProofTypeValue),
                        IDCode = model.ProofNumber,
                        Contact = model.TakingMan,
                        Phone = model.TakingTel,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)GeneralStatus.Normal,
                    });
                }

                return Json(new { type = "success", msg = "保存成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = "保存失败：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除提货人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult DeleteTakingInfo(DeleteTakingInfoModel model)
        {
            try
            {
                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Update<Layers.Data.Sqls.PsOrder.Pickers>(new
                    {
                        Status = (int)GeneralStatus.Deleted,
                    }, item => item.ID == model.PickingID);
                }

                return Json(new { type = "success", msg = "删除成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = "删除失败：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上传文件发货单格式
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFileFaHuoDan(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];

                //后台也要校验
                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && fileFormat == ".pdf")
                {
                    return Json(new { type = "error", msg = "上传的文件大小不超过3M！" }, JsonRequestBehavior.AllowGet);
                }
                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return Json(new { type = "error", msg = "文件格式错误，请上传图片或者pdf文件！" }, JsonRequestBehavior.AllowGet);
                }

                NewFile newFile = new NewFile(file.FileName, PsOrderFileType.OutDelivery);
                file.SaveAs(newFile.FullName);

                var fileModel = new
                {
                    name = file.FileName,
                    URL = newFile.URL,
                    fullURL = newFile.FullURL,
                    fileFormat = file.ContentType
                };

                return Json(new { type = "success", data = fileModel, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上传文件客户标签格式
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public JsonResult UploadFileKeHuBiaoQian(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];

                //后台也要校验
                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && fileFormat == ".pdf")
                {
                    return Json(new { type = "error", msg = "上传的文件大小不超过3M！" }, JsonRequestBehavior.AllowGet);
                }
                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return Json(new { type = "error", msg = "文件格式错误，请上传图片或者pdf文件！" }, JsonRequestBehavior.AllowGet);
                }

                NewFile newFile = new NewFile(file.FileName, PsOrderFileType.Label);
                file.SaveAs(newFile.FullName);

                var fileModel = new
                {
                    name = file.FileName,
                    URL = newFile.URL,
                    fullURL = newFile.FullURL,
                    fileFormat = file.ContentType
                };

                return Json(new { type = "success", data = fileModel, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增出库订单-提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult NewOutStorageSubmit(NewOutStorageSubmitModel model)
        {
            try
            {
                var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
                string theSiteuserID = siteuser.SiteuserID;
                string theClientID = siteuser.TheClientID;
                string theTrackerID = siteuser.TheTrackerID;

                string newOrderID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Order, "SZODR");
                string newOrderTransportID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.OrderTransport);

                //订单
                OutStorage outStorage = new OutStorage();
                outStorage.Order = new Layers.Data.Sqls.PsOrder.Orders
                {
                    ID = newOrderID,
                    Type = (int)Services.Enums.OrderType.Outbound,
                    ClientID = theClientID,
                    SiteuserID = theSiteuserID,
                    CompanyID = "CompanyID1",
                    ConsigneeID = newOrderTransportID, //出库订单填
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = (int)OrderStatus.Waiting,
                };

                //订单项、产品信息
                List<Layers.Data.Sqls.PsOrder.OrderItems> orderItemList = new List<Layers.Data.Sqls.PsOrder.OrderItems>();
                List<Layers.Data.Sqls.PsOrder.Products> productList = new List<Layers.Data.Sqls.PsOrder.Products>();

                string[] newProductIDs = Layers.Data.PKeySigner.Series(Services.Enums.PKeyType.Product, model.OrderItems.Length);
                string[] newOrderItemIDs = Layers.Data.PKeySigner.Series(Services.Enums.PKeyType.OrderItem, model.OrderItems.Length);

                //foreach (var item in model.OrderItems)
                for (int i = 0; i < model.OrderItems.Length; i++)
                {
                    var item = model.OrderItems[i];

                    var newOrderItem = new Layers.Data.Sqls.PsOrder.OrderItems
                    {
                        ID = newOrderItemIDs[i],
                        OrderID = newOrderID,
                        ProductID = newProductIDs[i],
                        Supplier = theClientID,
                        CustomCode = item.CustomCode,
                        StocktakingType = item.StocktakingTypeInt,
                        Mpq = item.Mpq,
                        PackageNumber = item.PackageNumber,
                        Total = item.ItemTotal,
                        Currency = (int)Yahv.Underly.Currency.CNY,
                        UnitPrice = 0,
                        StorageID = item.StorageID,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = (int)Yahv.Underly.GeneralStatus.Normal,
                        BakPartnumber = item.PartNumber,
                        BakBrand = item.Brand,
                        BakPackage = item.Package,
                        BakDateCode = item.DateCode,
                    };
                    orderItemList.Add(newOrderItem);

                    var newProduct = new Layers.Data.Sqls.PsOrder.Products
                    {
                        ID = newProductIDs[i],
                        Partnumber = item.PartNumber,
                        Brand = item.Brand,
                        Package = item.Package,
                        DateCode = item.DateCode,
                        Mpq = item.Mpq,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    };
                    productList.Add(newProduct);
                }

                outStorage.OrderItems = orderItemList.ToArray();
                outStorage.Products = productList.ToArray();

                //货运信息
                //string newPickerID = null;
                string transportAddress = null;
                string transportContact = null;
                string transportPhone = null;

                if (model.TransportModeInt == (int)TransportMode.Express || model.TransportModeInt == (int)TransportMode.Dtd)
                {
                    string usedAddressID = string.Empty;
                    if (model.TransportModeInt == (int)TransportMode.Express)
                    {
                        usedAddressID = model.DeliverTargetValue;
                    }
                    else if (model.TransportModeInt == (int)TransportMode.Dtd)
                    {
                        usedAddressID = model.DeliverTargetValue2;
                    }

                    using (PsOrderRepository repository = new PsOrderRepository())
                    {
                        var address = repository.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>().Where(t => t.ID == usedAddressID)
                            .Select(item => new
                            {
                                DeliverTargetID = item.ID,
                                DeliverTargetMan = item.Contact,
                                DeliverTargetTel = item.Phone,
                                DeliverTargetAddress = item.Address,
                            }).FirstOrDefault();

                        transportAddress = address.DeliverTargetAddress;
                        transportContact = address.DeliverTargetMan;
                        transportPhone = address.DeliverTargetTel;
                    }
                }
                else if (model.TransportModeInt == (int)TransportMode.PickUp)
                {
                    transportAddress = null;
                    transportContact = model.TakingMan;
                    transportPhone = model.TakingTel;

                    //newPickerID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Picker);
                    //outStorage.Picker = new Layers.Data.Sqls.PsOrder.Pickers
                    //{
                    //    ID = newPickerID,
                    //    ClientID = theClientID,
                    //    IDType = Convert.ToInt32(model.ProofTypeValue),
                    //    IDCode = model.ProofNumber,
                    //    Contact = transportContact,
                    //    Phone = transportPhone,
                    //    CreateDate = DateTime.Now,
                    //    ModifyDate = DateTime.Now,
                    //    Status = (int)GeneralStatus.Normal,
                    //};
                }

                outStorage.OrderTransport = new Layers.Data.Sqls.PsOrder.OrderTransports
                {
                    ID = newOrderTransportID,
                    OrderID = newOrderID,
                    TransportMode = model.TransportModeInt,
                    Carrier = model.ExpressCompanyValue,
                    ExpressPayer = Convert.ToInt32(model.FreightPayValue),
                    ExpressTransport = Convert.ToString(model.ExpressMethodValue),
                    ExpressEscrow = model.ThirdParty,
                    TakingTime = !string.IsNullOrEmpty(model.TakingDate) ? (DateTime?)DateTime.Parse(model.TakingDate) : null,
                    PickerID = model.TakingID,
                    Address = transportAddress,
                    Contact = transportContact,
                    Phone = transportPhone,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                };

                //特殊要求
                List<Layers.Data.Sqls.PsOrder.Requires> requireList = new List<Layers.Data.Sqls.PsOrder.Requires>();
                if (model.SpecialRequireValues != null && model.SpecialRequireValues.Length > 0)
                {
                    foreach (var item in model.SpecialRequireValues)
                    {
                        var newRequire = new Layers.Data.Sqls.PsOrder.Requires
                        {
                            ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Requires),
                            OrderID = newOrderID,
                            OrderTransportID = newOrderTransportID,
                            Name = ((StorageOutSpecialRequire)item).GetDescription(),
                            Content = item == (int)StorageOutSpecialRequire.QTYQ ? model.OtherRequire : null,
                            CreateDate = DateTime.Now,
                        };

                        requireList.Add(newRequire);
                    }
                }
                outStorage.Requires = requireList.ToArray();

                //一些文件信息(发货单文件、客户标签文件)
                List<Layers.Data.Sqls.PsOrder.PcFiles> fileInfoList = new List<Layers.Data.Sqls.PsOrder.PcFiles>();
                if (model.FileInfoFaHuoDan.IsUploaded)
                {
                    fileInfoList.Add(new Layers.Data.Sqls.PsOrder.PcFiles
                    {
                        ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.PcFile),
                        MainID = newOrderID,
                        Type = (int)PsOrderFileType.OutDelivery,
                        CustomName = model.FileInfoFaHuoDan.FileName,
                        Url = model.FileInfoFaHuoDan.URL,
                        CreateDate = DateTime.Now,
                        SiteuserID = theSiteuserID,
                    });
                }
                if (model.FileInfoKeHuBiaoQian.IsUploaded)
                {
                    fileInfoList.Add(new Layers.Data.Sqls.PsOrder.PcFiles
                    {
                        ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.PcFile),
                        MainID = newOrderID,
                        Type = (int)PsOrderFileType.Label,
                        CustomName = model.FileInfoKeHuBiaoQian.FileName,
                        Url = model.FileInfoKeHuBiaoQian.URL,
                        CreateDate = DateTime.Now,
                        SiteuserID = theSiteuserID,
                    });
                }
                outStorage.FileInfos = fileInfoList.ToArray();

                //新增
                outStorage.Insert(theTrackerID);

                return Json(new { type = "success", msg = "保存成功", data = newOrderID }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = "保存失败：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}