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
using Yahv.PsWms.SzMvc.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class OrderController : BaseController
    {
        #region 页面

        /// <summary>
        /// 新增入库订单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewInStorage() { return View(); }

        /// <summary>
        /// 新增入库订单-订单基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NewInStorageBaseInfo() { return PartialView(); }

        /// <summary>
        /// 新增入库订单-特殊要求
        /// </summary>
        /// <returns></returns>
        public ActionResult NewInStorageSpecialRequire() { return PartialView(); }

        /// <summary>
        /// 新增入库订单-货运信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NewInStorageTransport() { return PartialView(); }

        /// <summary>
        /// 新增入库订单-附件信息
        /// </summary>
        /// <returns></returns>
        public ActionResult NewInStorageAttachment() { return PartialView(); }

        /// <summary>
        /// 新增入库订单-批量导入窗口
        /// </summary>
        /// <returns></returns>
        public ActionResult NewInStorageBatchAddOrderItem() { return PartialView(); }

        #endregion


        /// <summary>
        /// 批量导入文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public JsonResult InStorageBatchFileUpload()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string ext = Path.GetExtension(file.FileName).ToLower();
                string[] exts = { ".xls", ".xlsx" };
                if (exts.Contains(ext) == false)
                {
                    return Json(new { type = "error", msg = "文件格式错误，请上传.xls或.xlsx文件！" }, JsonRequestBehavior.AllowGet);
                }
                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

                List<InStorageBatchFileUploadReturnModel.OrderItem> list = new List<InStorageBatchFileUploadReturnModel.OrderItem>();

                if (dt.Rows.Count == 0)
                {
                    return Json(new { type = "error", msg = "导入的数据不能为空" }, JsonRequestBehavior.AllowGet);
                }

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var array = dt.Rows[i].ItemArray;

                    //1 - 自定义编号 CustomCode (string)
                    //2 - 型号 PartNumber  (string)
                    //3 - 品牌 Brand (string)
                    //4 - 封装 Package (string)
                    //5 - 批次 DateCode (string)
                    //6 - 包装类型 StocktakingTypeInt (int, enum)
                    //7 - 最小包装量 Mpq (int)
                    //8 - 数量 PackageNumber (int)

                    int mpq = array[7].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[7].ToString()) ? 0 : Convert.ToInt32(array[7]);
                    int packageNumber = array[8].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[8].ToString()) ? 0 : Convert.ToInt32(array[8]);

                    if (array[1].ToString().Trim() == "" && array[2].ToString().Trim() == "" && array[3].ToString().Trim() == "" &&
                        array[4].ToString().Trim() == "" && array[5].ToString().Trim() == "" && mpq == 0 && packageNumber == 0)
                    {
                        continue;
                    }

                    InStorageBatchFileUploadReturnModel.OrderItem preData = new InStorageBatchFileUploadReturnModel.OrderItem
                    {
                        CustomCode = array[1].ToString().Trim(),
                        PartNumber = array[2].ToString().Trim(),
                        Brand = array[3].ToString().Trim(),
                        Package = array[4].ToString().Trim(),
                        DateCode = array[5].ToString().Trim(),
                        //StocktakingTypeInt = array[6].ToString(),
                        Mpq = mpq,
                        PackageNumber = packageNumber,
                    };

                    preData.StocktakingTypeInt = Convert.ToString((int)StocktakingType.Single);
                    string stocktakingTypeName = array[6].ToString();
                    if (!string.IsNullOrWhiteSpace(stocktakingTypeName))
                    {
                        var stocktakingTypeList = ExtendsEnum.ToNameDictionary<StocktakingType>()
                                                    .Where(item => item.Name.ToUpper() == stocktakingTypeName.ToUpper()
                                                                || item.Value.ToUpper() == stocktakingTypeName.ToUpper()).ToArray();
                        if (stocktakingTypeList != null && stocktakingTypeList.Count() > 0)
                        {
                            preData.StocktakingTypeInt = Convert.ToString((int)((StocktakingType)(Enum.Parse(typeof(StocktakingType), stocktakingTypeList[0].Value))));
                        }
                    }

                    //如果包装类型是个（1）, 将 Mpq 置为 1
                    if (preData.StocktakingTypeInt == Convert.ToString((int)StocktakingType.Single))
                    {
                        preData.Mpq = 1;
                    }

                    list.Add(preData);
                }
                var data = new
                {
                    list,
                    file.FileName
                };

                return Json(new { type = "success", msg = "导入成功", data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上传提货单文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadTakingFiles()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];

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

                NewFile newFile = new NewFile(file.FileName, PsOrderFileType.Taking);
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
        /// 上传装箱单文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public JsonResult UploadPackingFiles()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];

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

                NewFile newFile = new NewFile(file.FileName, PsOrderFileType.InDelivery);
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
        /// 新增入库订单-提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult NewInStorageSubmit(NewInStorageSubmitModel model)
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
                InStorage inStorage = new InStorage();
                inStorage.Order = new Layers.Data.Sqls.PsOrder.Orders
                {
                    ID = newOrderID,
                    Type = (int)Services.Enums.OrderType.Inbound,
                    ClientID = theClientID,
                    SiteuserID = theSiteuserID,
                    CompanyID = "CompanyID1",
                    ConsignorID = newOrderTransportID, //入库订单填
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

                inStorage.OrderItems = orderItemList.ToArray();
                inStorage.Products = productList.ToArray();

                //货运信息
                string consigneeManName = null;
                string consigneeManTel = null;
                string consigneeManAddress = null;
                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    var address = repository.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>().Where(t => t.ID == model.ConsigneeManValue)
                        .Select(item => new
                        {
                            ConsigneeManID = item.ID,
                            ConsigneeManName = item.Contact,
                            ConsigneeManTel = item.Phone,
                            ConsigneeManAddress = item.Address,
                        }).FirstOrDefault();

                    if (address != null)
                    {
                        consigneeManName = address.ConsigneeManName;
                        consigneeManTel = address.ConsigneeManTel;
                        consigneeManAddress = address.ConsigneeManAddress;
                    }
                }

                inStorage.OrderTransport = new Layers.Data.Sqls.PsOrder.OrderTransports
                {
                    ID = newOrderTransportID,
                    OrderID = newOrderID,
                    TransportMode = model.TransportModeInt,
                    Carrier = model.ExpressCompanyValue,
                    WaybillCode = model.ExpressNumber,
                    TakingTime = !string.IsNullOrEmpty(model.TakingDate) ? (DateTime?)DateTime.Parse(model.TakingDate) : null,
                    Address = consigneeManAddress,
                    Contact = consigneeManName,
                    Phone = consigneeManTel,
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
                            Name = ((StorageInSpecialRequire)item).GetDescription(),
                            Content = item == (int)StorageInSpecialRequire.QTYQ ? model.OtherRequire : null,
                            CreateDate = DateTime.Now,
                        };

                        requireList.Add(newRequire);
                    }
                }
                inStorage.Requires = requireList.ToArray();

                //提货单文件、装箱单文件
                List<Layers.Data.Sqls.PsOrder.PcFiles> fileList = new List<Layers.Data.Sqls.PsOrder.PcFiles>();
                if (model.TakingFiles != null && model.TakingFiles.Length > 0)
                {
                    foreach (var item in model.TakingFiles)
                    {
                        var newTakingFile = new Layers.Data.Sqls.PsOrder.PcFiles
                        {
                            ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.PcFile),
                            MainID = newOrderID,
                            Type = (int)PsOrderFileType.Taking,
                            CustomName = item.name,
                            Url = item.URL,
                            CreateDate = DateTime.Now,
                            SiteuserID = theSiteuserID,
                        };
                        fileList.Add(newTakingFile);
                    }
                }
                if (model.PackingFiles != null && model.PackingFiles.Length > 0)
                {
                    foreach (var item in model.PackingFiles)
                    {
                        var newPackingFile = new Layers.Data.Sqls.PsOrder.PcFiles
                        {
                            ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.PcFile),
                            MainID = newOrderID,
                            Type = (int)PsOrderFileType.InDelivery,
                            CustomName = item.name,
                            Url = item.URL,
                            CreateDate = DateTime.Now,
                            SiteuserID = theSiteuserID,
                        };
                        fileList.Add(newPackingFile);
                    }
                }
                inStorage.Files = fileList.ToArray();

                //新增
                inStorage.Insert(theTrackerID);

                return Json(new { type = "success", msg = "保存成功", data = newOrderID }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = "保存失败：" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}