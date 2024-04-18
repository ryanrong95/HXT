//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.Linq.Extends;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.PvWsOrder.Services.ClientModels;
//using Yahv.PvWsOrder.Services.ClientViews;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsOrder.Services.Extends;
//using Yahv.PvWsOrder.Services.Views;
//using Yahv.Services.Models.LsOrder;
//using Yahv.Services.Views;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;


//namespace Yahv.PvWsClient.WebApp.Controllers
//{
//    public class OrdersController : UserController
//    {
//        #region 代收货
//        /// <summary>
//        /// 新增代仓储订单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult StorageAdd(string id)
//        {
//            StorageAddModel model = new StorageAddModel();
//            model.Origin = (Origin.HKG).ToString(); //默认中国香港
//            model.OrderItems = new Models.OrderItem[0]; //订单项初始化
//            model.TakingFiles = new FileModel[0];
//            model.PIFiles = new FileModel[0];
//            model.WaybillType = ((int)WaybillType.PickUp).ToString(); //默认自提
//            model.PartsTotal = 1; //总件数默认为1
//            model.IsPrePaid = false;
//            if (!string.IsNullOrWhiteSpace(id)) //编辑
//            {
//                var order = Client.Current.MyRecievedOrder.GetOrderDetail(id);
//                if (order != null)
//                {
//                    //驳回才能编辑
//                    if (order.MainStatus != CgOrderStatus.暂存)
//                    {
//                        return View("Error");
//                    }
//                    var orderConditions = JsonConvert.DeserializeObject<Yahv.PvWsOrder.Services.Models.OrderCondition>(order.Input.Conditions);
//                    model.IsTesting = orderConditions.IsDetection;//是否代检测

//                    var waybillConditions = JsonConvert.DeserializeObject<Yahv.Services.Models.WayCondition>(order.InWaybill.Condition);

//                    model.IsUnpacking = waybillConditions.UnBoxed; //拆箱验货
//                    model.IsPayFreight = waybillConditions.PayForFreight; //是否垫付运费

//                    model.IsPrePaid = Convert.ToBoolean(order.Input.IsPayCharge);
//                    model.PayCurrency = ((int)order.Input.Currency).ToString();
//                    model.SettlementCurrency = ((int)order.SettlementCurrency).ToString(); //添加结算币种
//                    model.ID = order.ID;
//                    model.Origin = order.InWaybill.Consignor.Place.ToString();
//                    model.IsPrePaid = Convert.ToBoolean(order.Input.IsPayCharge);

//                    model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
//                    {
//                        ID = item.ID,
//                        PartNumber = item.Product.PartNumber,
//                        Manufacturer = item.Product.Manufacturer,
//                        Origin = item.Origin,
//                        OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                        DateCode = item.DateCode,
//                        Quantity = item.Quantity,
//                        Unit = (int)item.Unit,
//                        UnitLabel = item.Unit.GetDescription(),
//                        TotalPrice = item.TotalPrice,
//                        GrossWeight = item.GrossWeight,
//                        Volume = item.Volume,
//                    }).ToArray();
//                    model.SupplierID = order.SupplierID;
//                    model.WaybillID = order.InWaybill.ID;
//                    model.LoadingID = order.InWaybill.WayLoading?.ID;
//                    model.WaybillType = ((int)order.InWaybill.Type).ToString();
//                    model.CarrierID = order.InWaybill.CarrierID;
//                    model.TakingDate = order.InWaybill.WayLoading?.TakingDate;
//                    model.TakingAddress = order.InWaybill.WayLoading?.TakingAddress.ToAddress();
//                    model.TakingDetailAddress = order.InWaybill.WayLoading?.TakingAddress.ToDetailAddress();
//                    model.TakingContact = order.InWaybill.WayLoading?.TakingContact;
//                    model.TakingPhone = order.InWaybill.WayLoading?.TakingPhone;
//                    model.Code = order.InWaybill.Code;
//                    model.Subcodes = order.InWaybill.Subcodes;
//                    model.PartsTotal = order.InWaybill.TotalParts.Value;
//                    model.IsPayFreight = model.IsPayFreight;
//                    model.VoyageNumber = order.InWaybill.VoyageNumber;
//                    //提货文件
//                    model.TakingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).Select(item => new FileModel
//                    {
//                        name = item.CustomName,
//                        URL = item.Url
//                    }).ToArray();
//                    //PI文件
//                    model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new FileModel
//                    {
//                        name = item.CustomName,
//                        URL = item.Url
//                    }).ToArray();
//                }
//                else
//                {
//                    return View("Error");
//                }
//            }
//            var my = Yahv.Client.Current;
//            var data = new
//            {
//                UnitOptions = UnitEnumHelper.ToUnitDictionary().Select(item => new { value = item.Value, text = item.Code + " " + item.Name }).ToArray(),
//                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "Unknown").Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
//                SupplierOptions = my.MySupplier.Select(item => new { value = item.ID, text = item.ChineseName }).ToArray(),
//                CarrierOptions = Yahv.Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
//                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View(model);
//        }

//        /// <summary>
//        /// 获取供应商受益人
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetSupplierBeneficiaries(string supplier)
//        {
//            var beneficiaries = "";
//            supplier = supplier.InputText();
//            var entity = new MySupplierBankView(Client.Current.EnterpriseID, supplier);
//            if (entity == null)
//            {
//                return base.JsonResult(VueMsgType.error, "供应商不存在！");
//            }
//            else
//            {
//                beneficiaries = entity.Select(item => new
//                {
//                    value = item.ID,
//                    text = item.Account,
//                }).Json();
//            }

//            return base.JsonResult(VueMsgType.success, "", beneficiaries);
//        }

//        /// <summary>
//        /// 新增代仓储保存数据
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        [HttpPost]
//        public JsonResult StorageSubmit(string data)
//        {
//            try
//            {
//                var model = JsonConvert.DeserializeObject<StorageAddModel>(data);
//                //入库条件
//                PvWsOrder.Services.Models.OrderCondition orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsDetection = model.IsTesting;

//                Services.Models.WayCondition wayCondition = new Services.Models.WayCondition();
//                wayCondition.PayForFreight = model.IsPayFreight;
//                wayCondition.UnBoxed = model.IsUnpacking;

//                PvWsOrder.Services.Models.OrderItemCondition orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition();
//                orderItemCondition.IsDetection = model.IsTesting;

//                var current = Client.Current;

//                //订单
//                RecievedOrderExtends order = new RecievedOrderExtends();
//                order.ID = model.ID;
//                order.ClientID = current.EnterpriseID;
//                order.CreatorID = current.ID;
//                order.Type = OrderType.Recieved;
//                order.PayeeID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
//                order.BeneficiaryID = Yahv.Alls.Current.CompanyBeneficary.Account;  //公司的受益人
//                order.MainStatus = CgOrderStatus.暂存;
//                order.SupplierID = model.SupplierID;
//                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
//                order.EnterCode = current.MyClients.EnterCode;
//                //结算币种添加
//                order.SettlementCurrency = (Currency)int.Parse(model.SettlementCurrency);

//                //订单附件
//                List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
//                //PI文件
//                foreach (var item in model.PIFiles)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Invoice;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                //提货文件
//                foreach (var item in model.TakingFiles)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Delivery;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                order.OrderFiles = files.ToArray();
//                //入库
//                var orderInputs = new OrderInput();
//                //orderInputs.BeneficiaryID = model.IsPrePaid ? model.SupplierBeneficiaryID : null;
//                orderInputs.IsPayCharge = model.IsPrePaid;
//                orderInputs.Conditions = orderCondition.Json();
//                orderInputs.Currency = (Currency)int.Parse(model.PayCurrency);
//                order.Input = orderInputs;
//                //订单项
//                order.OrderItems = model.OrderItems.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
//                {
//                    Product = new Yahv.Services.Models.CenterProduct
//                    {
//                        PartNumber = item.PartNumber,
//                        Manufacturer = item.Manufacturer,
//                    },
//                    ID = item.ID,
//                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    Currency = (Currency)int.Parse(model.PayCurrency),
//                    UnitPrice = item.TotalPrice / item.Quantity,
//                    Unit = (LegalUnit)item.Unit,
//                    TotalPrice = item.TotalPrice,
//                    GrossWeight = item.GrossWeight,
//                    Volume = item.Volume,
//                    Conditions = orderItemCondition.Json()
//                }).ToArray();
//                var supplier = current.MySupplier[model.SupplierID];
//                //运单
//                var company = Alls.Current.Company[PvWsOrder.Services.PvClientConfig.ThirdCompanyID];
//                var contact1 = company?.Contacts.FirstOrDefault();
//                order.InWaybill = new Waybill()
//                {
//                    Consignee = new Yahv.Services.Models.WayParter
//                    {
//                        Company = company.Name,
//                        Address = company.RegAddress,
//                        Contact = contact1?.Name,
//                        Phone = contact1?.Tel ?? contact1?.Mobile,
//                        Email = contact1?.Email,
//                        Place = Origin.HKG.ToString(),
//                    },
//                    Consignor = new Yahv.Services.Models.WayParter
//                    {
//                        Company = supplier.Name,
//                        Place = model.Origin,
//                        Address = supplier.RegAddress,
//                    },
//                    WayLoading = (WaybillType)int.Parse(model.WaybillType) != WaybillType.PickUp ? null : new Services.Models.WayLoading
//                    {
//                        ID = model.LoadingID,
//                        TakingDate = DateTime.Parse(model.TakingDateStr),
//                        TakingAddress = string.Join(" ", model.TakingAddress.Concat(new string[] { model.TakingDetailAddress })),
//                        TakingContact = model.TakingContact,
//                        TakingPhone = model.TakingPhone,
//                        CreatorID = Client.Current.ID,
//                        ModifierID = Client.Current.ID,
//                    },
//                    ID = model.WaybillID,
//                    Code = model.Code,
//                    Subcodes = model.Subcodes,
//                    Type = (WaybillType)int.Parse(model.WaybillType),
//                    CarrierID = model.CarrierID,
//                    FreightPayer = model.IsPayFreight ? WaybillPayer.Consignee : WaybillPayer.Consignor,
//                    TotalParts = model.PartsTotal,
//                    TotalWeight = model.OrderItems.Sum(item => item.GrossWeight),
//                    CreatorID = Client.Current.ID,
//                    ModifierID = Client.Current.ID,
//                    VoyageNumber = model.VoyageNumber,
//                    EnterCode = Client.Current.MyClients.EnterCode,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (WaybillType)int.Parse(model.WaybillType) == WaybillType.PickUp ? (int)SortingExcuteStatus.WaitTake : (int)SortingExcuteStatus.PendingStorage,
//                    Supplier = new WsSupplierAlls()[model.SupplierID]?.Name,
//                    NoticeType = Services.Enums.CgNoticeType.Enter,
//                    Source = Services.Enums.CgNoticeSource.AgentEnter,
//                };
//                order.Enter();
//                OperationLog(order.ID, "代收货订单保存成功");

//                return base.JsonResult(VueMsgType.success, "新增成功", order.ID);
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("代收货订单保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 上传产品明细
//        /// </summary>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult StorageFileUpload(HttpPostedFileBase file)
//        {
//            try
//            {
//                //拿到上传来的文件
//                file = Request.Files["file"];
//                string ext = Path.GetExtension(file.FileName).ToLower();
//                string[] exts = { ".xls", ".xlsx" };
//                if (exts.Contains(ext) == false)
//                {
//                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传.xls或.xlsx文件！");
//                }
//                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

//                List<Models.OrderItem> list = new List<Models.OrderItem>();

//                if (dt.Rows.Count == 0)
//                {
//                    return base.JsonResult(VueMsgType.error, "导入的数据不能为空");
//                }

//                for (var i = 0; i < dt.Rows.Count; i++)
//                {
//                    var array = dt.Rows[i].ItemArray;
//                    var origin = array[3].ToString();
//                    var unit = array[5].ToString();
//                    var aaa = array[6].GetType().Name;
//                    Models.OrderItem preData = new Models.OrderItem
//                    {
//                        DateCode = array[0].ToString(),
//                        Manufacturer = array[1].ToString(),
//                        PartNumber = array[2].ToString(),
//                        Quantity = array[4].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[4].ToString()) ? 0 : Convert.ToInt32(array[4]),
//                        TotalPrice = array[6].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[6].ToString()) ? 0 : Convert.ToDecimal(array[6]),
//                    };
//                    if (array[7].GetType().Name != "DBNull" && !string.IsNullOrWhiteSpace(array[7].ToString()))
//                    {
//                        preData.GrossWeight = Convert.ToDecimal(array[7]);
//                    }
//                    if (array[8].GetType().Name != "DBNull" && !string.IsNullOrWhiteSpace(array[8].ToString()))
//                    {
//                        preData.Volume = Convert.ToDecimal(array[8]);
//                    }
//                    if (!string.IsNullOrWhiteSpace(origin))
//                    {
//                        var originList = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Name.ToUpper() == origin.ToUpper() || item.Value.ToUpper() == origin.ToUpper()).ToArray();
//                        if (originList.Count() > 0)
//                        {
//                            preData.Origin = originList[0].Value;
//                            preData.OriginLabel = originList[0].Value + " " + originList[0].Name;
//                        }
//                    }
//                    if (!string.IsNullOrWhiteSpace(unit))
//                    {
//                        var unitList = UnitEnumHelper.ToUnitDictionary().Where(item => item.Code.ToUpper() == unit.ToUpper() || item.Value.ToString().ToUpper() == unit.ToUpper() || item.Name.ToUpper() == unit.ToUpper()).FirstOrDefault();
//                        if (unitList != null)
//                        {
//                            preData.Unit = unitList.Value;
//                            preData.UnitLabel = unitList.Code + " " + unitList.Name;
//                        }
//                    }
//                    list.Add(preData);
//                }
//                return base.JsonResult(VueMsgType.success, "导入成功", list.Json());
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }

//        }

//        /// <summary>
//        /// 代仓储订单列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult StorageList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取代仓储数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetStorageList()
//        {
//            var order = Client.Current.MyRecievedOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                MainStatus = item.MainStatus.GetDescription(),
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Input.Currency?.GetDescription(),
//                SettlementCurrency = item.SettlementCurrency?.GetDescription(), //结算币种
//                Origin = EnumHelper.GetEnum<Origin>(item.InWaybill.Consignor.Place).GetDescription(),
//                IsEdit = item.MainStatus == CgOrderStatus.暂存,
//                IsConfirm = item.PaymentStatus == OrderPaymentStatus.Confirm,
//                IsShowBill =item.MainStatus != CgOrderStatus.取消,
//                IsShowApply = item.Input.IsPayCharge.Value ? item.TotalPrice > item.ApplicationPaymentPrice : false,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 代仓储订单详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult StorageDetail()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            StorageDetailModel model = new StorageDetailModel();
//            var order = Client.Current.MyRecievedOrder.GetOrderDetail(id);
//            if (order != null)
//            {
//                model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
//                {
//                    PartNumber = item.Product.PartNumber,
//                    Manufacturer = item.Product.Manufacturer,
//                    OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    Unit = (int)item.Unit,
//                    UnitLabel = item.Unit.GetDescription(),
//                    TotalPrice = item.TotalPrice,
//                    GrossWeight = item.GrossWeight,
//                    Volume = item.Volume,
//                }).ToArray();
//                model.TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice));
//                model.ID = order.ID;
//                model.MainStatus = order.MainStatus.GetDescription();
//                model.CreateDate = order.CreateDate.ToString("yyyy-MM-dd");
//                model.SupplierName = order.SupplierName;
//                var wayCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.InWaybill.Condition);
//                var orderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Input.Conditions);

//                model.IsUnpacking = wayCondition.UnBoxed ? "是" : "否";
//                model.IsTesting = orderCondition.IsDetection ? "是" : "否";
//                model.IsPrePaid = Convert.ToBoolean(order.Input.IsPayCharge) ? "是" : "否";

//                model.PayCurrency = order.Input.Currency.GetDescription();
//                model.SettlementCurrency = order.SettlementCurrency?.GetDescription(); //结算币种
//                model.Origin = EnumHelper.GetEnum<Origin>(order.InWaybill.Consignor.Place).GetDescription();
//                model.DeliveryTypeName = order.InWaybill.Type.GetDescription();
//                model.DeliveryType = ((int)order.InWaybill.Type).ToString();
//                model.TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd");
//                model.TakingContact = order.InWaybill.WayLoading?.TakingContact;
//                model.TakingDetailAddress = order.InWaybill.WayLoading?.TakingAddress;
//                model.TakingPhone = order.InWaybill.WayLoading?.TakingPhone;
//                model.IsPayFreight = wayCondition.PayForFreight ? "是" : "否";
//                model.Code = order.InWaybill.Code;
//                model.Subcodes = order.InWaybill.Subcodes;
//                model.CarrierID = order.InWaybill.CarrierName;
//                model.PartsTotal = order.InWaybill.TotalParts;
//                model.VoyageNumber = order.InWaybill.VoyageNumber;
//                model.IsPrePay = Convert.ToBoolean(order.Input.IsPayCharge);
//                //文件
//                var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//                var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).FirstOrDefault();
//                if (deliveryFile != null)
//                {
//                    model.DeliveryFiles = fileurl + deliveryFile.Url.ToUrl();
//                }
//                model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
//                {
//                    Name = item.CustomName,
//                    Url = fileurl + item.Url,
//                }).ToArray();
//            }
//            return View(model);
//        }

//        /// <summary>
//        /// 提货文件
//        /// </summary>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult UploadPickUpFile(HttpPostedFileBase file)
//        {
//            try
//            {
//                //拿到上传来的文件
//                file = Request.Files["file"];

//                string fileFormat = Path.GetExtension(file.FileName).ToLower();
//                string[] Files = { ".pdf", ".doc", ".docx" };
//                //后台也要校验
//                var fileSize = file.ContentLength / 1024;
//                if (fileSize > 1024 * 3 && Files.Contains(fileFormat))
//                {
//                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
//                }

//                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".doc", ".docx", ".jpeg" };
//                if (allowFiles.Contains(fileFormat) == false)
//                {
//                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片、Word文件或者pdf文件！");
//                }

//                var result = Yahv.Alls.Current.centerFiles.fileSave(file);
//                //var fullUrl = AppDomain.CurrentDomain.BaseDirectory + result;
//                var fullUrl = PvWsOrder.Services.PvClientConfig.DomainUrl + @"/" + result;
//                var contentType = file.ContentType;
//                if (fileFormat == ".docx")
//                {
//                    contentType = "application/msword";
//                }
//                return base.JsonResult(VueMsgType.success, "", new { name = file.FileName, URL = result, fullURL = fullUrl, fileFormat = contentType }.Json());
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }

//        /// <summary>
//        /// 文件上传
//        /// </summary>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult UploadOrderFile(HttpPostedFileBase file)
//        {
//            try
//            {
//                //拿到上传来的文件
//                file = Request.Files["file"];

//                //后台也要校验
//                string fileFormat = Path.GetExtension(file.FileName).ToLower();
//                var fileSize = file.ContentLength / 1024;
//                if (fileSize > 1024 * 3 && fileFormat == ".pdf")
//                {
//                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
//                }
//                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
//                if (allowFiles.Contains(fileFormat) == false)
//                {
//                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片或者pdf文件！");
//                }

//                var result = Yahv.Alls.Current.centerFiles.fileSave(file);
//                // var fullUrl = AppDomain.CurrentDomain.BaseDirectory + result;
//                var fullUrl = PvWsOrder.Services.PvClientConfig.DomainUrl + @"/" + result;
//                return base.JsonResult(VueMsgType.success, "", new FileModel { name = file.FileName, URL = result, fullURL = fullUrl, fileFormat = file.ContentType }.Json());
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }
//        #endregion

//        #region 代发货
//        /// <summary>
//        /// 代发货新增页面
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeliveryAdd()
//        {
//            DeliveryAddModel model = new DeliveryAddModel();
//            var products = Request.Form["products"];
//            if (string.IsNullOrWhiteSpace(products))
//            {
//                return View("Error");
//            }
//            var productList = JsonConvert.DeserializeObject<StorageListViewModel[]>(products).Select(item => new
//            {
//                item.ID,
//                item.SaleQuantity,
//            });
//            var storageList = from storage in Client.Current.MyStorages.ToList()
//                              join product in productList on storage.ID equals product.ID
//                              select new DeliveryItem
//                              {
//                                  StorageID = storage.ID,
//                                  StockNum = storage.Quantity,
//                                  Quantity = product.SaleQuantity,
//                                  Manufacturer = storage.Product.Manufacturer,
//                                  PartNumber = storage.Product.PartNumber,
//                                  DateCode = storage.DateCode,
//                                  Origin = storage.Input.Origin,
//                                  OriginName = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(item => item.Value == storage.Input.Origin)?.Name,
//                                  Unit = ((int)storage.Unit).ToString(),
//                                  InputID = storage.InputID,
//                                  ProductID = storage.ProductID,
//                                  WareHouseID = storage.WareHouseID,
//                              };
//            model.DeliveryItems = storageList.ToArray();
//            model.DeliveryType = ((int)DeliveryType.HKShipment).ToString();
//            model.ShippingMethod = ((int)WaybillType.PickUp).ToString();
//            model.ExpressPaymentType = ((int)ExpressPayType.Deposit).ToString();
//            model.EntrustFile = new FileModel[0];
//            model.PIFile = new FileModel[0];
//            model.AccompanyingFile = new FileModel[0];
//            //绑定数据
//            var data = new
//            {
//                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "Unknown").Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
//                DeliveryTypeOptions = ExtendsEnum.ToDictionary<DeliveryType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PayTypeOptions = ExtendsEnum.ToDictionary<Methord>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                ShippingMethodOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                ExpressPayTypeOptions = ExtendsEnum.ToDictionary<ExpressPayType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                SupplierOptions = Client.Current.MySupplier.Select(item => new { value = item.ID, text = item.ChineseName }).ToArray(),
//                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                CarrierOptions = Yahv.Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View(model);
//        }

//        /// <summary>
//        /// 代发货订单列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeliveryList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取代发货数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetDeliveryList()
//        {
//            var order = Client.Current.MyDeliveryOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                MainStatus = item.MainStatus.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                Currency = item.Output.Currency?.GetDescription(),
//                SettlementCurrency = item.SettlementCurrency?.GetDescription(), //结算币种
//                Contact = item.OutWaybill.Consignee.Contact,
//                IsEdit = item.MainStatus == CgOrderStatus.暂存,
//                IsConfirm = item.PaymentStatus == OrderPaymentStatus.Confirm,
//                IsShowReceiveApply = item.Output.IsReciveCharge.Value ? item.TotalPrice > item.ApplicationReceivalPrice : false,
//                 IsShowBill = item.MainStatus!= CgOrderStatus.取消,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);

//        }

//        /// <summary>
//        /// 保存发货数据
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        [HttpPost]
//        public JsonResult DeliverySubmit(string data)
//        {
//            try
//            {
//                var model = JsonConvert.DeserializeObject<DeliveryAddModel>(data);
//                var current = Client.Current;
//                //库存校验
//                foreach (var item in model.DeliveryItems)
//                {
//                    var storage = current.MyStorages.Where(c => c.ID == item.ID && c.Quantity < item.Quantity).FirstOrDefault();
//                    if (storage != null)
//                    {
//                        return base.JsonResult(VueMsgType.error, "提交失败！型号【" + item.PartNumber + "】的发货数量不超过：" + item.Quantity);
//                    }
//                }
//                //条件
//                PvWsOrder.Services.Models.OrderCondition orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsCustomLabel = model.IsCustomLabel;
//                orderCondition.IsDetection = model.IsDetection;
//                orderCondition.IsRepackaging = model.IsRepackaging;
//                orderCondition.IsVacuumPackaging = model.IsVacuumPackaging;
//                orderCondition.IsWaterproofPackaging = model.IsWaterproofPackaging;

//                Services.Models.WayCondition wayCondition = new Services.Models.WayCondition();
//                wayCondition.PayForFreight = (ExpressPayType)int.Parse(model.ExpressPaymentType) == ExpressPayType.Deposit;
//                wayCondition.UnBoxed = model.IsUnBoxed;

//                PvWsOrder.Services.Models.OrderItemCondition orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition();
//                orderItemCondition.IsCustomLabel = model.IsCustomLabel;
//                orderItemCondition.IsDetection = model.IsDetection;
//                orderItemCondition.IsRepackaging = model.IsRepackaging;
//                orderItemCondition.IsVacuumPackaging = model.IsVacuumPackaging;
//                orderItemCondition.IsWaterproofPackaging = model.IsWaterproofPackaging;

//                //订单
//                PvWsOrder.Services.ClientModels.DeliveryOrderExtends order = new PvWsOrder.Services.ClientModels.DeliveryOrderExtends();
//                order.ID = model.ID;
//                order.ClientID = current.EnterpriseID;
//                order.CreatorID = current.ID;
//                order.Type = OrderType.Delivery;
//                order.PayeeID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
//                order.BeneficiaryID = Yahv.Alls.Current.CompanyBeneficary.Account;  //公司的受益人
//                order.MainStatus = CgOrderStatus.已提交;//发货订单直接提交
//                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
//               // order.ExecutionStatus = ExcuteStatus.香港待装箱;
//                order.EnterCode = current.MyClients.EnterCode;
//                order.SettlementCurrency = (Currency)int.Parse(model.SettlementCurrency);  //结算币种

//                //订单项    
//                order.OrderItems = model.DeliveryItems.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
//                {
//                    Product = new Yahv.Services.Models.CenterProduct
//                    {
//                        ID = item.ProductID,
//                    },
//                    ID = item.ID,
//                    InputID = item.InputID,
//                    OutputID = item.OutputID,
//                    StorageID = item.StorageID,
//                    Origin = item.Origin,
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    Currency = (Currency)int.Parse(model.Currency),
//                    UnitPrice = item.SalePrice ?? 0,
//                    Unit = (LegalUnit)int.Parse(item.Unit),
//                    TotalPrice = item.SaleTotalPrice ?? 0,
//                    Conditions = orderItemCondition.Json(),
//                    WareHouseID = item.WareHouseID,
//                }).ToArray();

//                //订单附件
//                List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
//                //PI文件
//                foreach (var item in model.PIFile)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Invoice;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                //随货文件
//                foreach (var item in model.AccompanyingFile)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.FollowGoods;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                //代收货款委托书文件
//                foreach (var item in model.EntrustFile)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.ReceiveEntrust;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                order.OrderFiles = files.ToArray();

//                //出库
//                var orderOutputs = new OrderOutput();
//                //TODO:存公司账户
//                orderOutputs.BeneficiaryID = model.IsBringPay ? Yahv.Alls.Current.CompanyBeneficary.Account : "";
//                orderOutputs.IsReciveCharge = model.IsBringPay;
//                orderOutputs.Currency = (Currency)int.Parse(model.Currency);
//                orderOutputs.Conditions = orderCondition.Json();
//                order.Output = orderOutputs;

//                //运单
//                var address = "";
//                var contact = "";
//                var phone = "";
//                IDType? IDType = null;
//                var IDNumber = "";
//                WaybillPayer freightPayer = (ExpressPayType)int.Parse(model.ExpressPaymentType) == ExpressPayType.Deposit ? WaybillPayer.Consignor : WaybillPayer.Consignee;
//                WaybillType waybillType = (DeliveryType)int.Parse(model.DeliveryType) == DeliveryType.HKShipment ? (WaybillType)int.Parse(model.ShippingMethod) : WaybillType.InternationalExpress;
//                if (waybillType == WaybillType.PickUp)
//                {
//                    contact = model.TakingContact;
//                    phone = model.TakingPhone;
//                    IDType = string.IsNullOrWhiteSpace(model.CertificateType) ? null : (IDType?)int.Parse(model.CertificateType);
//                    IDNumber = model.Certificate;
//                    freightPayer = WaybillPayer.Consignee;
//                }
//                else
//                {
//                    contact = model.ReceivedContact;
//                    phone = model.ReceivedPhone;

//                    if ((DeliveryType)int.Parse(model.DeliveryType) == DeliveryType.InternationalShipment)
//                    {
//                        address = EnumHelper.GetEnum<Origin>(model.InternationalAddress).GetDescription() + " " + model.ReceivedDetailAddress;
//                        waybillType = WaybillType.InternationalExpress;
//                    }
//                    else
//                    {
//                        address = string.Join(" ", model.ReceivedAddress.Concat(new string[] { model.ReceivedDetailAddress }));
//                    }
//                    if (waybillType == WaybillType.DeliveryToWarehouse)
//                    {
//                        freightPayer = WaybillPayer.Consignor;
//                    }
//                }

//                var company = Alls.Current.Company[PvWsOrder.Services.PvClientConfig.ThirdCompanyID];
//                var contact1 = company?.Contacts.FirstOrDefault();
//                order.OutWaybill = new Waybill()
//                {
//                    Consignee = new Yahv.Services.Models.WayParter
//                    {
//                        Address = address,
//                        Contact = contact,
//                        Phone = phone,
//                        IDType = IDType,
//                        IDNumber = IDNumber,
//                        Zipcode = model.PostCode,
//                        Place = (DeliveryType)int.Parse(model.DeliveryType) == DeliveryType.HKShipment ? Origin.HKG.ToString() : model.InternationalAddress,
//                        Company = contact,
//                    },
//                    Consignor = new Yahv.Services.Models.WayParter
//                    {
//                        Company = company.Name,
//                        Address = company.RegAddress,
//                        Contact = contact1?.Name,
//                        Phone = contact1?.Tel ?? contact1?.Mobile,
//                        Email = contact1?.Email,
//                        Place = Origin.HKG.GetOrigin().Code,
//                    },
//                    WayLoading = waybillType != WaybillType.PickUp ? null : new Services.Models.WayLoading
//                    {
//                        ID = model.LoadingID,
//                        TakingDate = DateTime.Parse(model.TakingDateStr),
//                        TakingAddress = address,
//                        TakingContact = contact,
//                        TakingPhone = phone,
//                        CreatorID = current.ID,
//                        ModifierID = current.ID,
//                    },
//                    ID = model.WaybillID,
//                    Type = waybillType,
//                    CarrierID = model.CarrierID,
//                    FreightPayer = freightPayer,
//                    //TotalParts = model.DeliveryItems.Sum(item => item.Quantity),
//                    CreatorID = current.ID,
//                    ModifierID = current.ID,
//                    EnterCode = current.MyClients.EnterCode,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (int)PickingExcuteStatus.Waiting,
//                    NoticeType = Services.Enums.CgNoticeType.Out,
//                    Source = Services.Enums.CgNoticeSource.AgentSend,
//                };
//                order.Enter();
//                var message = order.NoticeMessage;
//                if (message.code == 400)
//                {
//                    return base.JsonResult(VueMsgType.warning, "库房实际库存不足,当前订单自动关闭,请重新从库存页面下单！");
//                }
//                OperationLog(order.ID, "代发货订单保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", order.ID);
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("代发货订单保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 代仓储订单详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeliveryDetail()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            StorageDetailModel model = new StorageDetailModel();
//            var order = Client.Current.MyDeliveryOrder.GetOrderDetail(id);
//            if (order != null)
//            {
//                model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
//                {
//                    PartNumber = item.Product.PartNumber,
//                    Manufacturer = item.Product.Manufacturer,
//                    OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    UnitPrice = item.UnitPrice,
//                    Unit = (int)item.Unit,
//                    UnitLabel = item.Unit.GetDescription(),
//                    TotalPrice = item.TotalPrice,
//                    GrossWeight = item.GrossWeight,
//                    Volume = item.Volume,
//                }).ToArray();
//                model.TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice));
//                model.TotalSalePrice = model.OrderItems.Sum(item => item.TotalPrice).ToString("0.00");
//                model.PayCurrency = order.Output.Currency.GetDescription();
//                model.SettlementCurrency = order.SettlementCurrency?.GetDescription(); //结算币种
//                model.ID = order.ID;
//                model.CreateDate = order.CreateDate.ToString("yyyy-MM-dd");
//                model.MainStatus = order.MainStatus.GetDescription();
//                model.DeliveryGoodType = order.OutWaybill.Type == WaybillType.InternationalExpress ? "国际发货" : "本港发货";
//                model.IsBringPay = order.Output.IsReciveCharge == true ? "是" : "否";
//                var wayCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.OutWaybill.Condition);
//                var orderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Output.Conditions);
//                model.IsCustomLabel = orderCondition.IsCustomLabel ? "是" : "否";
//                model.IsRepackaging = orderCondition.IsRepackaging ? "是" : "否";
//                model.IsVacuumPackaging = orderCondition.IsVacuumPackaging ? "是" : "否";
//                model.IsWaterproofPackaging = orderCondition.IsWaterproofPackaging ? "是" : "否";
//                model.IsDetection = orderCondition.IsDetection ? "是" : "否";
//                model.IsUnBoxed = wayCondition.UnBoxed ? "是" : "否";
//                model.DeliveryType = ((int)order.OutWaybill.Type).ToString();
//                model.DeliveryTypeName = order.OutWaybill.Type.GetDescription();
//                model.TakingDate = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd");
//                model.TakingContact = order.OutWaybill.WayLoading?.TakingContact;
//                model.TakingDetailAddress = order.OutWaybill.WayLoading?.TakingAddress;
//                model.TakingPhone = order.OutWaybill.WayLoading?.TakingPhone;
//                model.ReceivedContact = order.OutWaybill.Consignee.Contact;
//                model.ReceivedPhone = order.OutWaybill.Consignee.Phone;
//                model.ReceivedAddress = order.OutWaybill.Consignee.Address;
//                model.Zipcode = order.OutWaybill.Consignee.Zipcode;
//                model.CarrierID = order.OutWaybill.CarrierName;
//                model.IsPayFreight = wayCondition.PayForFreight ? "寄付" : "到付";
//                model.CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription();
//                model.Certificate = order.OutWaybill.Consignee?.IDNumber;
//                var invoice = order.Invoice;
//                //开票信息
//                model.Invoice = new InvoiceViewModel
//                {
//                    ID = invoice.ID,
//                    Type = invoice.Type.GetDescription(),
//                    DeliveryType = invoice.DeliveryType.GetDescription(),
//                    CompanyName = invoice.Name,
//                    TaxperNumber = invoice.TaxperNumber,
//                    RegAddress = invoice.RegAddress,
//                    CompanyTel = invoice.CompanyTel,
//                    Bank = invoice.Bank,
//                    Account = invoice.Account
//                };
//                //文件
//                //合同发票
//                var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//                model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
//                {
//                    Name = item.CustomName,
//                    Url = fileurl + item.Url,
//                }).ToArray();
//                //随货文件
//                model.AccompanyFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
//                {
//                    Name = item.CustomName,
//                    Url = fileurl + item.Url,
//                }).ToArray();
//                var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.ReceiveEntrust).FirstOrDefault();
//                if (deliveryFile != null)
//                {
//                    model.ReceiveEntrust = fileurl + deliveryFile.Url.ToUrl();
//                }
//            }
//            return View(model);
//        }

//        #endregion

//        #region 代收代发
//        /// <summary>
//        /// 代收代发新增页面
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TransportAdd(string id)
//        {
//            TransportAddModel model = new TransportAddModel();
//            model.Origin = Origin.HKG.ToString(); //默认中国香港
//            model.OrderItems = new Models.TransportOrderItem[0]; //订单项初始化
//            model.TakingFiles = new FileModel[0];
//            model.PIFiles = new FileModel[0];
//            model.EntrustFile = new FileModel[0];
//            model.AccompanyingFile = new FileModel[0];
//            model.WaybillType = ((int)WaybillType.PickUp).ToString(); //默认自提
//            model.PartsTotal = 1; //总件数默认为1
//            model.IsPrePaid = false;
//            model.DeliveryType = ((int)DeliveryType.HKShipment).ToString();
//            model.ShippingMethod = ((int)WaybillType.PickUp).ToString();
//            model.ExpressPaymentType = ((int)ExpressPayType.Deposit).ToString();
//            if (!string.IsNullOrWhiteSpace(id)) //编辑
//            {
//                var order = Client.Current.MyTransOrder.GetOrderDetail(id);
//                if (order != null)
//                {
//                    // 驳回才能编辑
//                    if (order.MainStatus != CgOrderStatus.暂存)
//                    {
//                        return View("Error");
//                    }

//                    #region 代收
//                    //运单
//                    model.InWaybillID = order.InWaybill.ID;
//                    model.Code = order.InWaybill.Code;
//                    model.Subcodes = order.InWaybill.Subcodes;
//                    model.WaybillType = ((int)order.InWaybill.Type).ToString();
//                    model.CarrierID = order.InWaybill.CarrierID;
//                    model.VoyageNumber = order.InWaybill.VoyageNumber;
//                    var inwaybillConditions = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.InWaybill.Condition);
//                    model.IsPayFreight = inwaybillConditions.PayForFreight;
//                    model.IsUnpacking = inwaybillConditions.UnBoxed;
//                    model.IsPrePaid = Convert.ToBoolean(order.Input.IsPayCharge);

//                    //提货单
//                    model.InLoadingID = order.InWaybill.WayLoading?.ID;
//                    model.TakingDate = order.InWaybill.WayLoading?.TakingDate;
//                    model.TakingAddress = order.InWaybill.WayLoading?.TakingAddress.ToAddress();
//                    model.TakingDetailAddress = order.InWaybill.WayLoading?.TakingAddress.ToDetailAddress();
//                    model.TakingContact = order.InWaybill.WayLoading?.TakingContact;
//                    model.TakingPhone = order.InWaybill.WayLoading?.TakingPhone;

//                    //货物条款
//                    model.PayCurrency = ((int?)order.Input.Currency).ToString();
//                    model.SettlementCurrency = ((int?)order.SettlementCurrency).ToString();  //结算币种
//                    #endregion

//                    #region 代发
//                    //运单
//                    model.OutWaybillID = order.OutWaybill.ID;
//                    var outwaybillCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.OutWaybill.Condition);
//                    model.ExpressPaymentType = outwaybillCondition.PayForFreight ? ((int)ExpressPayType.Deposit).ToString() : ((int)ExpressPayType.Collected).ToString();
//                    model.IsUnBoxed = outwaybillCondition.UnBoxed;
//                    var orderConditions = JsonConvert.DeserializeObject<Yahv.PvWsOrder.Services.Models.OrderCondition>(order.Output.Conditions);
//                    model.IsTesting = orderConditions.IsDetection;//是否代检测
//                    model.IsCustomLabel = orderConditions.IsCustomLabel;
//                    model.IsRepackaging = orderConditions.IsRepackaging;
//                    model.IsVacuumPackaging = orderConditions.IsVacuumPackaging;
//                    model.IsWaterproofPackaging = orderConditions.IsWaterproofPackaging;
//                    model.IsCollectMoney = order.Output.IsReciveCharge == true ? true : false;
//                    model.DeliveryType = order.OutWaybill.Type == WaybillType.InternationalExpress ? ((int)DeliveryType.InternationalShipment).ToString() : ((int)DeliveryType.HKShipment).ToString();
//                    model.ReceivedContact = order.OutWaybill.Consignee.Contact;
//                    model.ReceivedPhone = order.OutWaybill.Consignee.Phone;
//                    model.ReceivedDetailAddress = order.OutWaybill.Consignee.Address.ToDetailAddress();
//                    model.ReceiveTakingDate = order.OutWaybill.WayLoading?.TakingDate;
//                    if (order.OutWaybill.Type == WaybillType.InternationalExpress)
//                    {
//                        model.InternationalAddress = EnumHelper.GetEnumValue<Origin>(order.OutWaybill.Consignee.Place).ToString();
//                        model.ReceivedDetailAddress = order.OutWaybill.Consignee.Address.Split(' ').Last();
//                        model.ShippingMethod = ((int)WaybillType.LocalExpress).ToString();
//                    }
//                    else if (order.OutWaybill.Type == WaybillType.PickUp)
//                    {
//                        //提货单
//                        model.OutLoadingID = order.OutWaybill.WayLoading?.ID;
//                        model.ReceiveTakingDate = order.OutWaybill.WayLoading?.TakingDate;
//                        model.ReceivedAddress = order.OutWaybill.WayLoading?.TakingAddress.ToAddress();
//                        model.ReceivedDetailAddress = order.OutWaybill.WayLoading?.TakingAddress.ToDetailAddress();
//                        model.ReceiveTakingContact = order.OutWaybill.WayLoading?.TakingContact;
//                        model.ReceiveTakingPhone = order.OutWaybill.WayLoading?.TakingPhone;
//                        model.CertificateType = ((int?)order.OutWaybill.Consignee?.IDType).ToString();
//                        model.Certificate = order.OutWaybill.Consignee.IDNumber;
//                    }
//                    else
//                    {
//                        model.ReceivedAddress = order.OutWaybill.Consignee.Address.ToAddress();
//                        model.ShippingMethod = ((int)order.OutWaybill.Type).ToString();
//                    }
//                    model.PostCode = order.OutWaybill.Consignee.Zipcode;
//                    model.ReceiveCarrierID = order.OutWaybill.CarrierID;
//                    #endregion

//                    model.SupplierID = order.SupplierID;
//                    model.ID = order.ID;
//                    model.Origin = EnumHelper.GetEnumValue<Origin>(order.InWaybill.Consignor.Place).ToString();

//                    model.OrderItems = order.OrderItems.Select(item => new Models.TransportOrderItem
//                    {
//                        ID = item.ID,
//                        PartNumber = item.Product.PartNumber,
//                        Manufacturer = item.Product.Manufacturer,
//                        Origin = item.Origin,
//                        OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                        DateCode = item.DateCode,
//                        Quantity = item.Quantity,
//                        Unit = (int)item.Unit,
//                        UnitLabel = item.Unit.GetDescription(),
//                        TotalPrice = item.TotalPrice,
//                        UnitPrice = item.UnitPrice,
//                        GrossWeight = item.GrossWeight,
//                        Volume = item.Volume,
//                    }).ToArray();
//                }
//                else
//                {
//                    return View("Error");
//                }
//            }
//            var my = Yahv.Client.Current;
//            var data = new
//            {
//                UnitOptions = UnitEnumHelper.ToUnitDictionary().Select(item => new { value = item.Value, text = item.Code + " " + item.Name }).ToArray(),
//                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "Unknown").Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
//                SupplierOptions = my.MySupplier.Select(item => new { value = item.ID, text = item.ChineseName }).ToArray(),
//                BeneficiariesOptions = my.MyBeneficiaries.Select(item => new { value = item.ID, text = item.Account, supplierID = item.EnterpriseID }).ToArray(),
//                CarrierOptions = Yahv.Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
//                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                ShippingMethodOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                ExpressPayTypeOptions = ExtendsEnum.ToDictionary<ExpressPayType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View(model);
//        }

//        /// <summary>
//        /// 代收代发提交
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult TransportSubmit(string data)
//        {
//            try
//            {
//                var model = JsonConvert.DeserializeObject<TransportAddModel>(data);
//                var current = Client.Current;

//                PvWsOrder.Services.ClientModels.TransportOrderExtends order = new PvWsOrder.Services.ClientModels.TransportOrderExtends();

//                //订单
//                order.ID = model.ID;
//                order.ClientID = current.EnterpriseID;
//                order.CreatorID = current.ID;
//                order.Type = OrderType.Transport;
//                order.PayeeID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
//                order.BeneficiaryID = Yahv.Alls.Current.CompanyBeneficary.Account;  //公司的受益人
//                order.MainStatus = CgOrderStatus.暂存;//发货订单直接提交
//                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
//                //order.ExecutionStatus = ExcuteStatus.香港待入库;
//                order.SupplierID = model.SupplierID;
//                order.EnterCode = current.MyClients.EnterCode;
//                order.SettlementCurrency = (Currency)int.Parse(model.SettlementCurrency); //结算币种

//                #region 代收
//                //入库条件
//                PvWsOrder.Services.Models.OrderCondition orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsDetection = model.IsTesting;

//                Services.Models.WayCondition wayCondition = new Services.Models.WayCondition();
//                wayCondition.PayForFreight = model.IsPayFreight;
//                wayCondition.UnBoxed = model.IsUnpacking;
//                //入库
//                var orderInputs = new OrderInput();
//                orderInputs.IsPayCharge = model.IsPrePaid;
//                orderInputs.Conditions = orderCondition.Json();
//                orderInputs.Currency = (Currency)int.Parse(model.PayCurrency);
//                order.Input = orderInputs;

//                var supplier = Client.Current.MySupplier[model.SupplierID];
//                //运单
//                var company = Alls.Current.Company[PvWsOrder.Services.PvClientConfig.ThirdCompanyID];
//                var contact1 = company?.Contacts.FirstOrDefault();
//                order.InWaybill = new Waybill()
//                {
//                    Consignee = new Yahv.Services.Models.WayParter
//                    {
//                        Company = company.Name,
//                        Address = company.RegAddress,
//                        Contact = contact1?.Name,
//                        Phone = contact1?.Tel ?? contact1?.Mobile,
//                        Email = contact1?.Email,
//                        Place = Origin.HKG.ToString(),
//                    },
//                    Consignor = new Yahv.Services.Models.WayParter
//                    {
//                        Company = supplier.Name,
//                        Place = model.Origin,
//                        Address = supplier.RegAddress,
//                    },
//                    WayLoading = (WaybillType)int.Parse(model.WaybillType) != WaybillType.PickUp ? null : new Services.Models.WayLoading
//                    {
//                        ID = model.InLoadingID,
//                        TakingDate = DateTime.Parse(model.TakingDateStr),
//                        TakingAddress = string.Join(" ", model.TakingAddress.Concat(new string[] { model.TakingDetailAddress })),
//                        TakingContact = model.TakingContact,
//                        TakingPhone = model.TakingPhone,
//                        CreatorID = Client.Current.ID,
//                        ModifierID = Client.Current.ID,
//                    },
//                    ID = model.InWaybillID,
//                    Code = model.Code,
//                    Subcodes = model.Subcodes,
//                    Type = (WaybillType)int.Parse(model.WaybillType),
//                    CarrierID = model.CarrierID,
//                    FreightPayer = model.IsPayFreight ? WaybillPayer.Consignee : WaybillPayer.Consignor,
//                    EnterCode = current.MyClients.EnterCode,
//                    TotalParts = model.PartsTotal,
//                    TotalWeight = model.OrderItems.Sum(item => item.GrossWeight),
//                    CreatorID = Client.Current.ID,
//                    ModifierID = Client.Current.ID,
//                    VoyageNumber = model.VoyageNumber,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (WaybillType)int.Parse(model.WaybillType) == WaybillType.PickUp ? (int)SortingExcuteStatus.WaitTake : (int)SortingExcuteStatus.PendingStorage,
//                    Supplier = new WsSupplierAlls()[model.SupplierID]?.Name,
//                    NoticeType = Services.Enums.CgNoticeType.Enter,
//                    Source = Services.Enums.CgNoticeSource.Transfer,
//                };
//                #endregion

//                #region 代发
//                //条件
//                orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsCustomLabel = model.IsCustomLabel;
//                orderCondition.IsDetection = model.IsTesting;
//                orderCondition.IsRepackaging = model.IsRepackaging;
//                orderCondition.IsVacuumPackaging = model.IsVacuumPackaging;
//                orderCondition.IsWaterproofPackaging = model.IsWaterproofPackaging;

//                wayCondition = new Services.Models.WayCondition();
//                wayCondition.PayForFreight = (ExpressPayType)int.Parse(model.ExpressPaymentType) == ExpressPayType.Deposit;
//                wayCondition.UnBoxed = model.IsUnBoxed;
//                wayCondition.LableServices = model.IsCustomLabel;
//                wayCondition.Repackaging = model.IsRepackaging;
//                wayCondition.VacuumPackaging = model.IsVacuumPackaging;
//                wayCondition.WaterproofPackaging = model.IsWaterproofPackaging;


//                var orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition();
//                orderItemCondition.IsCustomLabel = model.IsCustomLabel;
//                orderItemCondition.IsDetection = model.IsTesting;
//                orderItemCondition.IsRepackaging = model.IsRepackaging;
//                orderItemCondition.IsVacuumPackaging = model.IsVacuumPackaging;
//                orderItemCondition.IsWaterproofPackaging = model.IsWaterproofPackaging;

//                //出库
//                var orderOutputs = new OrderOutput();

//                orderOutputs.BeneficiaryID = model.IsCollectMoney ? Yahv.Alls.Current.CompanyBeneficary.Account : "";
//                orderOutputs.IsReciveCharge = model.IsCollectMoney;
//                orderOutputs.Currency = (Currency)int.Parse(model.PayCurrency);
//                orderOutputs.Conditions = orderCondition.Json();
//                order.Output = orderOutputs;

//                //运单
//                var address = "";
//                var contact = "";
//                var phone = "";
//                IDType? IDType = null;
//                var IDNumber = "";
//                WaybillPayer freightPayer = (ExpressPayType)int.Parse(model.ExpressPaymentType) == ExpressPayType.Deposit ? WaybillPayer.Consignor : WaybillPayer.Consignee;
//                WaybillType waybillType = (DeliveryType)int.Parse(model.DeliveryType) == DeliveryType.HKShipment ? (WaybillType)int.Parse(model.ShippingMethod) : WaybillType.InternationalExpress;
//                if (waybillType == WaybillType.PickUp)
//                {
//                    contact = model.ReceiveTakingContact;
//                    phone = model.ReceiveTakingPhone;
//                    IDType = string.IsNullOrWhiteSpace(model.CertificateType) ? null : (IDType?)int.Parse(model.CertificateType);
//                    IDNumber = model.Certificate;
//                    freightPayer = WaybillPayer.Consignee;
//                }
//                else
//                {
//                    contact = model.ReceivedContact;
//                    phone = model.ReceivedPhone;

//                    if ((DeliveryType)int.Parse(model.DeliveryType) == DeliveryType.InternationalShipment)
//                    {
//                        address = EnumHelper.GetEnum<Origin>(model.InternationalAddress).GetDescription() + " " + model.ReceivedDetailAddress;
//                        waybillType = WaybillType.InternationalExpress;
//                    }
//                    else
//                    {
//                        address = string.Join(" ", model.ReceivedAddress.Concat(new string[] { model.ReceivedDetailAddress }));
//                    }
//                    if (waybillType == WaybillType.DeliveryToWarehouse)
//                    {
//                        freightPayer = WaybillPayer.Consignor;
//                    }
//                }
//                order.OutWaybill = new Waybill()
//                {
//                    Consignee = new Yahv.Services.Models.WayParter
//                    {
//                        Address = address,
//                        Contact = contact,
//                        Phone = phone,
//                        IDType = IDType,
//                        IDNumber = IDNumber,
//                        Zipcode = model.PostCode,
//                        Place = (DeliveryType)int.Parse(model.DeliveryType) == DeliveryType.HKShipment ? Origin.HKG.ToString() : model.InternationalAddress,
//                        Company = contact,
//                    },
//                    Consignor = new Yahv.Services.Models.WayParter
//                    {
//                        Company = company.Name,
//                        Address = company.RegAddress,
//                        Contact = contact1?.Name,
//                        Phone = contact1?.Tel ?? contact1?.Mobile,
//                        Email = contact1?.Email,
//                        Place = Origin.HKG.ToString(),
//                    },
//                    WayLoading = waybillType != WaybillType.PickUp ? null : new Services.Models.WayLoading
//                    {
//                        ID = model.OutLoadingID,
//                        TakingDate = DateTime.Parse(model.ReceiveTakingDateStr),
//                        TakingAddress = address,
//                        TakingContact = contact,
//                        TakingPhone = phone,
//                        CreatorID = current.ID,
//                        ModifierID = current.ID,
//                    },
//                    ID = model.OutWaybillID,
//                    Type = waybillType,
//                    CarrierID = model.ReceiveCarrierID,
//                    FreightPayer = freightPayer,
//                    CreatorID = current.ID,
//                    ModifierID = current.ID,
//                    EnterCode = current.MyClients.EnterCode,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (int)PickingExcuteStatus.Waiting,
//                    Status = GeneralStatus.Closed,
//                    NoticeType = Services.Enums.CgNoticeType.Out,
//                    Source = Services.Enums.CgNoticeSource.Transfer,
//                };
//                #endregion

//                //订单项    
//                order.OrderItems = model.OrderItems.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
//                {
//                    Product = new Yahv.Services.Models.CenterProduct
//                    {
//                        PartNumber = item.PartNumber,
//                        Manufacturer = item.Manufacturer,
//                    },
//                    ID = item.ID,
//                    InputID = item.InputID,
//                    OutputID = item.OutputID,
//                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    Currency = (Currency)int.Parse(model.PayCurrency),
//                    UnitPrice = item.UnitPrice,
//                    Unit = (LegalUnit)item.Unit,
//                    TotalPrice = item.TotalPrice,
//                    Conditions = orderItemCondition.Json(),
//                    GrossWeight = item.GrossWeight,
//                    Volume = item.Volume,
//                }).ToArray();

//                //订单附件
//                List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
//                //PI文件
//                foreach (var item in model.PIFiles)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Invoice;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                //随货文件
//                foreach (var item in model.AccompanyingFile)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.FollowGoods;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                //代收货款委托书文件
//                foreach (var item in model.EntrustFile)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.ReceiveEntrust;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                //提货文件
//                foreach (var item in model.TakingFiles)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Delivery;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                order.OrderFiles = files.ToArray();
//                order.Enter();
//                OperationLog(order.ID, "代收代发订单保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", order.ID);
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("代收代发订单保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 代收代发订单详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TransportDetail()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            var order = Client.Current.MyTransOrder.GetOrderDetail(id);
//            if (order != null)
//            {
//                //代收自提文件
//                var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//                var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).FirstOrDefault();
//                var _deliveryFile = "";
//                if (deliveryFile != null)
//                {
//                    _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
//                }
//                //代收合同发票
//                var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
//                {
//                    Name = item.CustomName,
//                    Url = fileurl + item.Url,
//                }).ToArray();
//                //随货文件
//                var accompanyFile = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
//                {
//                    Name = item.CustomName,
//                    Url = fileurl + item.Url,
//                }).ToArray();
//                var orderItems = order.OrderItems.Select(item => new Models.OrderItem
//                {
//                    PartNumber = item.Product.PartNumber,
//                    Manufacturer = item.Product.Manufacturer,
//                    OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    UnitPrice = item.UnitPrice,
//                    Unit = (int)item.Unit,
//                    UnitLabel = item.Unit.GetDescription(),
//                    TotalPrice = item.TotalPrice,
//                    GrossWeight = item.GrossWeight,
//                    Volume = item.Volume,
//                }).ToArray();
//                var inWayCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.InWaybill.Condition);
//                var outWayCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.OutWaybill.Condition);
//                var inOrderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Input.Conditions);
//                var outOrderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Output.Conditions);
//                var model = new
//                {
//                    ID = order.ID,
//                    MainStatus = order.MainStatus.GetDescription(),
//                    CreateDate = order.CreateDate.ToString("yyyy-MM-dd"),
//                    Origin = EnumHelper.GetEnum<Origin>(order.InWaybill.Consignor.Place).GetDescription(),
//                    SupplierName = order.SupplierName,
//                    Currency = order.Input.Currency.GetDescription(),
//                    SettlementCurrency = order.SettlementCurrency?.GetDescription(),  //结算币种
//                    IsPayCharge = Convert.ToBoolean(order.Input.IsPayCharge) ? "是" : "否",
//                    BeneficiaryName = order.Input.BeneficiaryName,
//                    InWaybillType = order.InWaybill.Type.GetDescription(),
//                    InDeliveryType = ((int)order.InWaybill.Type).ToString(),
//                    TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
//                    TakingContact = order.InWaybill.WayLoading?.TakingContact,
//                    TakingPhone = order.InWaybill.WayLoading?.TakingPhone,
//                    TakingDetailAddress = order.InWaybill.WayLoading?.TakingAddress,
//                    DeliveryFile = _deliveryFile,
//                    PIFiles,
//                    IsPayFreight = inWayCondition.PayForFreight ? "是" : "否",
//                    Code = order.InWaybill.Code,
//                    Subcodes = order.InWaybill.Subcodes,
//                    InCarrierName = order.InWaybill.CarrierName,
//                    PartsTotal = order.InWaybill.TotalParts,
//                    VoyageNumber = order.InWaybill.VoyageNumber,
//                    IsUnBoxed = inWayCondition.UnBoxed ? "是" : "否",
//                    IsDetection = inOrderCondition.IsDetection ? "是" : "否",
//                    OutWaybillType = order.OutWaybill.Type.GetDescription(),
//                    OutDeliveryType = ((int)order.OutWaybill.Type).ToString(),
//                    IsReciveCharge = Convert.ToBoolean(order.Output.IsReciveCharge) ? "是" : "否",
//                    OutTakingDate = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
//                    OutTakingContact = order.OutWaybill.WayLoading?.TakingContact,
//                    OutTakingPhone = order.OutWaybill.WayLoading?.TakingPhone,
//                    CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription(),
//                    Certificate = order.OutWaybill.Consignee?.IDNumber,
//                    ReceivedContact = order.OutWaybill.Consignee.Contact,
//                    ReceivedPhone = order.OutWaybill.Consignee.Phone,
//                    ReceivedAddress = order.OutWaybill.Consignee.Address,
//                    OutCarrierName = order.OutWaybill.CarrierName,
//                    OutIsPayFreight = outWayCondition.PayForFreight ? "寄付" : "到付",
//                    Zipcode = order.OutWaybill.Consignee.Zipcode,
//                    AccompanyFiles = accompanyFile,
//                    IsCustomLabel = outOrderCondition.IsCustomLabel ? "是" : "否",
//                    IsRepackaging = outOrderCondition.IsRepackaging ? "是" : "否",
//                    IsVacuumPackaging = outOrderCondition.IsVacuumPackaging ? "是" : "否",
//                    IsWaterproofPackaging = outOrderCondition.IsWaterproofPackaging ? "是" : "否",
//                    OrderItems = orderItems,
//                    TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice))
//                };
//                return View(model);
//            }
//            return View("Error");

//        }

//        /// <summary>
//        /// 代收代发订单列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TransportList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取代收代发数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetTransportList()
//        {
//            var order = Client.Current.MyTransOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                MainStatus = item.MainStatus.GetDescription(),
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Input.Currency.GetDescription(),
//                SettlementCurrency = item.SettlementCurrency?.GetDescription(), //结算币种
//                Consignee = item.OutWaybill.Consignee.Contact,
//                IsEdit = item.MainStatus == CgOrderStatus.暂存,
//                IsConfirm = item.PaymentStatus == OrderPaymentStatus.Confirm,
//                IsShowBill= item.MainStatus!= CgOrderStatus.取消,
//                IsShowApply = item.Input.IsPayCharge.Value ? item.TotalPrice > item.ApplicationPaymentPrice : false,
//                IsShowReceiveApply = item.Output.IsReciveCharge.Value ? item.TotalPrice > item.ApplicationReceivalPrice : false,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 上传产品明细
//        /// </summary>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult TransportFileUpload(HttpPostedFileBase file)
//        {
//            try
//            {
//                //拿到上传来的文件
//                file = Request.Files["file"];
//                string ext = Path.GetExtension(file.FileName).ToLower();
//                string[] exts = { ".xls", ".xlsx" };
//                if (exts.Contains(ext) == false)
//                {
//                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传.xls或.xlsx文件！");
//                }
//                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

//                List<Models.TransportOrderItem> list = new List<Models.TransportOrderItem>();

//                if (dt.Rows.Count == 0)
//                {
//                    return base.JsonResult(VueMsgType.error, "导入的数据不能为空");
//                }

//                for (var i = 0; i < dt.Rows.Count; i++)
//                {
//                    var array = dt.Rows[i].ItemArray;
//                    var origin = array[3].ToString();
//                    var unit = array[5].ToString();
//                    Models.TransportOrderItem preData = new Models.TransportOrderItem
//                    {
//                        DateCode = array[0].ToString(),
//                        Manufacturer = array[1].ToString(),
//                        PartNumber = array[2].ToString(),
//                        Quantity = array[4].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[4].ToString()) ? 0 : Convert.ToInt32(array[4]),
//                        GrossWeight = array[7].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[7].ToString()) ? 0 : Convert.ToDecimal(array[7]),
//                        Volume = array[8].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[8].ToString()) ? 0 : Convert.ToDecimal(array[8]),
//                        UnitPrice = Math.Round((array[6].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[6].ToString()) ? 0 : Convert.ToDecimal(array[6])), 2),
//                        TotalPrice = Math.Round((array[6].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[6].ToString()) ? 0 : Convert.ToDecimal(array[6])) * (array[4].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[4].ToString()) ? 0 : Convert.ToInt32(array[4])), 2),
//                    };

//                    if (!string.IsNullOrWhiteSpace(origin))
//                    {
//                        var originList = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Name.ToUpper() == origin.ToUpper() || item.Value.ToUpper() == origin.ToUpper()).ToArray();
//                        if (originList.Count() > 0)
//                        {
//                            preData.Origin = originList[0].Value;
//                            preData.OriginLabel = originList[0].Value + " " + originList[0].Name;
//                        }
//                    }
//                    if (!string.IsNullOrWhiteSpace(unit))
//                    {
//                        var unitList = UnitEnumHelper.ToUnitDictionary().Where(item => item.Code.ToUpper() == unit.ToUpper() || item.Value.ToString().ToUpper() == unit.ToUpper() || item.Name.ToUpper() == unit.ToUpper()).FirstOrDefault();
//                        if (unitList != null)
//                        {
//                            preData.Unit = unitList.Value;
//                            preData.UnitLabel = unitList.Code + " " + unitList.Name;
//                        }
//                    }
//                    list.Add(preData);
//                }
//                return base.JsonResult(VueMsgType.success, "导入成功", list.Json());
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }

//        }
//        #endregion

//        #region 转报关
//        /// <summary>
//        /// 转报关新增页面
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TransferDeclarationAdd()
//        {
//            TransferDeclareAddModel model = new TransferDeclareAddModel();
//            var current = Client.Current;
//            var products = Request.Form["products"];
//            if (string.IsNullOrWhiteSpace(products))
//            {
//                return View("Error");
//            }
//            var productList = JsonConvert.DeserializeObject<StorageListViewModel[]>(products).Select(item => new
//            {
//                item.ID,
//                item.SaleQuantity,
//            });
//            var storageList = from storage in Client.Current.MyStorages.ToList()
//                              join product in productList on storage.ID equals product.ID
//                              select new TransferDeclare
//                              {
//                                  StorageID = storage.ID,
//                                  StockNum = storage.Quantity,
//                                  Quantity = product.SaleQuantity,
//                                  Manufacturer = storage.Product.Manufacturer,
//                                  PartNumber = storage.Product.PartNumber,
//                                  DateCode = storage.DateCode,
//                                  Origin = storage.Input.Origin,
//                                  OriginName = string.IsNullOrWhiteSpace(storage.Input.Origin) ? "" : EnumHelper.GetEnum<Origin>(storage.Input.Origin).GetDescription(),
//                                  Unit = ((int)storage.Unit).ToString(),
//                                  UnitLabel = storage.Unit.GetDescription(),
//                              };
//            model.OrderItem = storageList.ToArray();

//            model.PIFiles = new FileModel[0];
//            model.Currency = ((int)Currency.USD).ToString(); //默认美元
//            model.EnterCode = current.MyClients?.EnterCode;
//            model.SZDeliveryType = ((int)WaybillType.PickUp).ToString();
//            model.PayExchangeSupplier = new string[0];  //付汇供应商实例化
//            model.WrapType = ((int)Package.纸制或纤维板制桶).ToString();
//            //收货信息
//            var receiveOptions = current.MyConsignees.Select(item => new { value = item.ID, text = item.Enterprise.Name, address = item.Address, name = item.Name, mobile = item.Tel }).ToArray();
//            model.ClientConsignee = receiveOptions.FirstOrDefault()?.value;
//            model.ClientConsigneeName = receiveOptions.FirstOrDefault()?.name;
//            model.ClientConsigneeAddress = receiveOptions.FirstOrDefault()?.address;
//            model.ClientContactMobile = receiveOptions.FirstOrDefault()?.mobile;
//            //绑定数据
//            var data = new
//            {
//                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.ChineseName }).ToArray(),
//                ReceiveOptions = current.MyConsignees.Select(item => new { value = item.ID, text = item.Enterprise.Name, address = item.Address, name = item.Name, mobile = item.Tel }).ToArray(),
//                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PackTypeOptions = ExtendsEnum.ToDictionary<Package>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                SZDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View(model);
//        }

//        /// <summary>
//        /// 新增转报关保存数据
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        [HttpPost]
//        public JsonResult TransferDeclareSubmit(string data)
//        {
//            try
//            {
//                var model = JsonConvert.DeserializeObject<TransferDeclareAddModel>(data);
//                var current = Client.Current;
//                //库存校验
//                foreach (var item in model.OrderItem)
//                {
//                    var storage = current.MyStorages.Where(c => c.ID == item.ID && c.Quantity < item.Quantity).FirstOrDefault();
//                    if (storage != null)
//                    {
//                        return base.JsonResult(VueMsgType.error, "提交失败！型号【" + item.PartNumber + "】的数量不超过：" + item.Quantity);
//                    }
//                }

//                TransferEntryOrderExtends order = new TransferEntryOrderExtends();

//                //订单
//                order.ID = model.ID;
//                order.ClientID = current.EnterpriseID;
//                order.CreatorID = current.ID;
//                order.Type = OrderType.TransferDeclare;
//                order.PayeeID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
//                order.BeneficiaryID = Yahv.Alls.Current.CompanyBeneficary.Account;  //公司的受益人
//                order.MainStatus = CgOrderStatus.已提交;
//                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
//                //order.ExecutionStatus = ExcuteStatus.待归类;
//                order.SupplierID = model.SupplierID;
//                order.EnterCode = current.MyClients.EnterCode;
//                order.Summary = model.Summary;
//                order.PayExchangeSuppliers = model.PayExchangeSupplier;
//                order.SettlementCurrency = Currency.CNY;

//                //条件
//                var orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsCharterBus = model.IsFullVehicle;

//                var wayCondition = new Services.Models.WayCondition();
//                wayCondition.IsCharterBus = model.IsFullVehicle;
//                wayCondition.AgencyPayment = model.IsLoan;

//                var orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition();
//                orderItemCondition.IsCharterBus = model.IsFullVehicle;

//                //出库
//                var orderOutputs = new Yahv.PvWsOrder.Services.ClientModels.OrderOutput();
//                orderOutputs.Currency = (Currency)int.Parse(model.Currency);
//                orderOutputs.Conditions = orderCondition.Json();
//                order.Output = orderOutputs;

//                //运单
//                var address = "";
//                var contact = "";
//                var phone = "";
//                IDType? IDType = null;
//                var IDNumber = "";
//                WaybillPayer freightPayer = WaybillPayer.Consignee;

//                if ((WaybillType)int.Parse(model.SZDeliveryType) == WaybillType.PickUp)
//                {
//                    contact = model.ClientPicker;
//                    phone = model.ClientPickerMobile;
//                    IDType = string.IsNullOrWhiteSpace(model.IDType) ? null : (IDType?)int.Parse(model.IDType);
//                    IDNumber = model.IDNumber;
//                    freightPayer = WaybillPayer.Consignee;
//                }
//                else
//                {
//                    contact = model.ClientConsigneeName;
//                    phone = model.ClientContactMobile;
//                    address = model.ClientConsigneeAddress;
//                    freightPayer = WaybillPayer.Consignor;
//                }

//                order.OutWaybill = new PvWsOrder.Services.ClientModels.Waybill()
//                {
//                    Consignee = new Yahv.Services.Models.WayParter
//                    {
//                        Address = address,
//                        Contact = contact,
//                        Phone = phone,
//                        IDType = IDType,
//                        IDNumber = IDNumber,
//                        Place = Origin.CHN.ToString(),
//                        Company = contact,
//                    },
//                    Consignor = new Yahv.Services.Models.WayParter
//                    {
//                        Address = address,
//                        Contact = contact,
//                        Phone = phone,
//                        Place = Origin.CHN.ToString(),
//                        Company = contact,
//                    },
//                    WayLoading = (WaybillType)int.Parse(model.SZDeliveryType) != WaybillType.PickUp ? null : new Services.Models.WayLoading
//                    {
//                        TakingDate = DateTime.Now,
//                        TakingAddress = address,
//                        TakingContact = contact,
//                        TakingPhone = phone,
//                        CreatorID = current.ID,
//                        ModifierID = current.ID,
//                    },
//                    Type = (WaybillType)int.Parse(model.SZDeliveryType),
//                    FreightPayer = freightPayer,
//                    CreatorID = current.ID,
//                    ModifierID = current.ID,
//                    EnterCode = current.MyClients.EnterCode,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (int)PickingExcuteStatus.Waiting,
//                    Status = GeneralStatus.Closed,
//                    Packaging = model.WrapType,
//                    TotalParts = string.IsNullOrWhiteSpace(model.PackNo) ? null : (int?)int.Parse(model.PackNo),
//                    NoticeType = Services.Enums.CgNoticeType.Out,
//                    Source = Services.Enums.CgNoticeSource.AgentCustomsFromStorage,
//                };

//                //订单项    
//                order.OrderItems = model.OrderItem.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
//                {
//                    Product = new Yahv.Services.Models.CenterProduct
//                    {
//                        PartNumber = item.PartNumber,
//                        Manufacturer = item.Manufacturer,

//                    },
//                    ID = item.ID,
//                    Name = item.Name,
//                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    Currency = (Currency)int.Parse(model.Currency),
//                    UnitPrice = item.UnitPrice.Value,
//                    Unit = (LegalUnit)int.Parse(item.Unit),
//                    TotalPrice = item.TotalPrice.Value,
//                    Conditions = orderItemCondition.Json(),
//                    GrossWeight = item.GrossWeight,
//                    Volume = item.Volume,
//                }).ToArray();

//                //订单附件
//                List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
//                //PI文件
//                foreach (var item in model.PIFiles)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Invoice;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                order.OrderFiles = files.ToArray();
//                order.XDTClientName = current.XDTClientName;
//                order.Enter();
//                OperationLog(order.ID, "转报关订单保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", order.ID);
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("转报关订单保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 转报关列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TransferDeclarationList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取转报关数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetTDeclareList()
//        {
//            var order = Client.Current.MyTransDeclareOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                MainStatus = item.MainStatus.GetDescription(),
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Output.Currency?.GetDescription(),
//                Origin = EnumHelper.GetEnum<Origin>(item.OutWaybill.Consignor.Place).GetDescription(),
//                IsConfirm = item.PaymentStatus == OrderPaymentStatus.Confirm,
//                item.OutWaybill.Consignee.Contact,
//                IsBill = item.ConfirmStatus != OrderConfirmStatus.Waiting && item.MainStatus != CgOrderStatus.取消,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 转报关详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TransDecDetail()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];

//            var order = Client.Current.MyTransDeclareOrder.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            //代收合同发票
//            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
//            {
//                Name = item.CustomName,
//                Url = fileurl + item.Url,
//            }).ToArray();
//            //对账单
//            var orderBillURL = "";
//            var orderBillName = "";
//            var orderBillStatus = false;
//            var orderBill = order.OrderFiles.Where(item => item.Type == (int)FileType.OrderBill).OrderByDescending(item => item.CreateDate).FirstOrDefault();
//            var IsUploadOrderBill = order.ConfirmStatus == OrderConfirmStatus.Confirmed; //客户已确认才可上传对账单
//            if (orderBill != null)
//            {
//                orderBillURL = fileurl + orderBill.Url;
//                orderBillName = orderBill.CustomName;
//                orderBillStatus = orderBill.Status == Services.Models.FileDescriptionStatus.Approved;
//            }
//            //报关委托书
//            var orderAgentURL = "";
//            var orderAgentName = "";
//            var orderAgentStatus = false;
//            var orderAgent = order.OrderFiles.Where(item => item.Type == (int)FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
//            var IsUploadOrderAgent = order.ConfirmStatus == OrderConfirmStatus.Confirmed; //客户已确认才可上传报关委托书
//            if (orderAgent != null)
//            {
//                orderAgentURL = fileurl + orderAgent.Url;
//                orderAgentName = orderAgent.CustomName;
//                orderAgentStatus = orderAgent.Status == Services.Models.FileDescriptionStatus.Approved;
//            }
//            var invoice = order.Invoice;
//            //开票信息
//            var _invoice = new InvoiceViewModel
//            {
//                ID = invoice.ID,
//                Type = invoice.Type.GetDescription(),
//                DeliveryType = invoice.DeliveryType.GetDescription(),
//                CompanyName = invoice.Name,
//                TaxperNumber = invoice.TaxperNumber,
//                RegAddress = invoice.RegAddress,
//                CompanyTel = invoice.CompanyTel,
//                Bank = invoice.Bank,
//                Account = invoice.Account
//            };
//            var orderItems = order.OrderItems.Select(item => new Models.OrderItem
//            {
//                PartNumber = item.Product.PartNumber,
//                Name = item.Name,
//                Manufacturer = item.Product.Manufacturer,
//                OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                DateCode = item.DateCode,
//                Quantity = item.Quantity,
//                UnitPrice = item.UnitPrice,
//                Unit = (int)item.Unit,
//                UnitLabel = item.Unit.GetDescription(),
//                TotalPrice = item.TotalPrice,
//                GrossWeight = item.GrossWeight,
//                Volume = item.Volume,
//            }).ToArray();
//            var outOrderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Output.Conditions);
//            var model = new
//            {
//                ID = order.ID,
//                MainStatus = order.MainStatus.GetDescription(),
//                CreateDate = order.CreateDate.ToString("yyyy-MM-dd"),
//                Currency = order.Output.Currency.GetDescription(),
//                PIFiles,
//                SZDeliveryTypeName = order.OutWaybill.Type.GetDescription(),
//                SZDeliveryType = order.OutWaybill.Type,
//                OutTakingContact = order.OutWaybill.WayLoading?.TakingContact,
//                OutTakingPhone = order.OutWaybill.WayLoading?.TakingPhone,
//                CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription(),
//                Certificate = order.OutWaybill.Consignee?.IDNumber,
//                ReceivedContact = order.OutWaybill.Consignee.Contact,
//                ReceivedPhone = order.OutWaybill.Consignee.Phone,
//                ReceivedAddress = order.OutWaybill.Consignee.Address,
//                IsFullVehicle = outOrderCondition.IsCharterBus ? "是" : "否",
//                order.OutWaybill.TotalParts,
//                order.Summary,
//                Packaging = ((Package)int.Parse(order.OutWaybill.Packaging)).GetDescription(),
//                TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice)),
//                OrderItems = orderItems,
//                Invoice = _invoice,
//                PayExchangeSupplier = order.PayExchangeSupplier.Select(c => c.Name).ToArray(),
//                OrderBillURL = orderBillURL,
//                OrderBillName = orderBillName,
//                OrderBillStatus = orderBillStatus,
//                IsUploadOrderBill,
//                OrderAgentURL = orderAgentURL,
//                OrderAgentName = orderAgentName,
//                OrderAgentStatus = orderAgentStatus,
//                IsUploadOrderAgent,
//            };
//            return View(model);
//        }
//        #endregion

//        #region 我的库存
//        /// <summary>
//        /// 我的库存列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyStockList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取库存列表数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetMyStockList()
//        {
//            var storage = Client.Current.MyStorages.AsEnumerable();
//            var query = Request.Form["query"];
//            if (!string.IsNullOrWhiteSpace(query))
//            {
//                storage = storage.Where(item => item.Product.PartNumber.Contains(query) | item.Product.Manufacturer.Contains(query));
//            }
//            #region 页面数据
//            Func<StorageExtend, object> convert = item => new StorageListViewModel
//            {
//                ID = item.ID,
//                ProductID = item.ProductID,
//                PartNumber = item.Product.PartNumber,
//                Quantity = item.Quantity,
//                Unit = item.Unit.GetDescription(),
//                Manufacturer = item.Product.Manufacturer,
//                DateCode = item.DateCode,
//            };
//            #endregion
//            return this.Paging(storage, convert);
//        }
//        #endregion

//        #region 我的租赁
//        /// <summary>
//        /// 我的租赁列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyLeaseList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 新增租赁
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult LeaseAdd()
//        {
//            var product = Yahv.Alls.Current.LsProducts.ToList().Select(item => new
//            {
//                ID = item.ID,
//                SpecID = item.SpecID,
//                item.Load,
//                item.Volume,
//                item.Quantity,
//                item.Summary,
//                isVisiable = true,
//                Price = item.LsProductPrice.OrderBy(c => c.Month).Select(c => new
//                {
//                    UnitPrice = c.Price,
//                    c.Month
//                }).ToArray(),
//            });
//            ViewBag.Product = product;
//            LeaseModel model = new LeaseModel();
//            model.ContractFile = new FileModel[0];
//            model.MonthNum = 1;
//            return View(model);
//        }

//        /// <summary>
//        /// 获取库位租赁数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetLsOrderList()
//        {
//            var order = Client.Current.MyLsOrders;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            Func<PvWsOrder.Services.ClientModels.LsOrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                Status = item.Status.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
//                StartDate = item.StartDate.Value.ToString("yyyy-MM-dd"),
//                EndDate = item.EndDate.Value.ToString("yyyy-MM-dd"),
//                TotalMoney = item.OrderItems.Sum(c => c.Quantity * c.UnitPrice)*item.LsMonth,
//                OrderItem = item.OrderItems.Select(c => new
//                {
//                    c.Product.SpecID,
//                    c.Product.Load,
//                    c.Product.Volume,
//                    c.UnitPrice,
//                    c.Quantity,
//                }).ToArray(),
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                IsApplyInvoice = item.Status != LsOrderStatus.Unpaid && item.Status != LsOrderStatus.Closed && item.InvoiceStatus == OrderInvoiceStatus.UnInvoiced,
//                isShow = false,
//                IsClosed = item.Status == LsOrderStatus.Closed,
//                IsUpload = ((item.Status != LsOrderStatus.Unpaid) && (item.Status != LsOrderStatus.Closed) && item.OrderFiles.Where(c => c.Type == (int)FileType.Contract).Count() == 0) || (item.Status == LsOrderStatus.Unpaid),
//                IsRenewal = (item.Status == LsOrderStatus.Allocated) && (!item.InheritStatus),//是否可续租
//                IsCancel = item.Status == LsOrderStatus.Unpaid, //是否可以取消
//                FileUrl = item.OrderFiles.Where(c => c.Type == (int)FileType.Contract).Count() == 0 ? "" : fileurl + item.OrderFiles.Where(c => c.Type == (int)FileType.Contract).OrderByDescending(c => c.CreateDate).FirstOrDefault()?.Url,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 新增库位保存数据
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        [HttpPost]
//        public JsonResult LsOrderSubmit(string data)
//        {
//            try
//            {
//                var model = JsonConvert.DeserializeObject<LeaseModel>(data);
//                //新增的申请单需要校验库存，扣库存
//                if (string.IsNullOrWhiteSpace(model.FatherID))
//                {
//                    //库存校验
//                    foreach (var item in model.LeaseData)
//                    {
//                        var storage = Yahv.Alls.Current.LsProducts.Where(c => c.ID == item.ProductID && c.Quantity < item.Num).FirstOrDefault();
//                        if (storage != null)
//                        {
//                            return base.JsonResult(VueMsgType.error, "提交失败！库位【" + storage .SpecID+ "】的可申请数量不够");
//                        }
//                    }
//                }
//                if (!string.IsNullOrWhiteSpace(model.FatherID))
//                {
//                    var f_order = Client.Current.MyLsOrders[model.FatherID];
//                    if (f_order == null)
//                    {
//                        return base.JsonResult(VueMsgType.error, "续借失败，该订单无法提供续借");
//                    }
//                    else
//                    {
//                        var count = Client.Current.MyLsOrders.Where(c => c.FatherID == model.FatherID && c.Status != LsOrderStatus.Closed).Count();
//                        if (count > 0)
//                        {
//                            return base.JsonResult(VueMsgType.error, "续借失败，一个订单只能续借一次");
//                        }
//                    }
//                }
//                var current = Client.Current;
//                PvWsOrder.Services.ClientModels.LsOrderExtends order = new PvWsOrder.Services.ClientModels.LsOrderExtends();
//                //订单
//                order.Type = LsOrderType.Lease;
//                order.FatherID = model.FatherID;
//                order.Source = LsOrderSource.WarehouseServicing;
//                order.ClientID = current.MyClients.ID;
//                order.PayeeID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
//                order.Currency = Currency.CNY;
//                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
//                order.Creator = current.ID;
//                order.StartDate = model.StartDate;
//                order.EndDate = model.StartDate?.AddMonths(model.MonthNum);
//                //订单项
//                order.OrderItems = model.LeaseData.Select(item => new LsOrderItem
//                {
//                    Quantity = item.Num,
//                    Currency = Currency.CNY,
//                    UnitPrice = item.UnitPrice,
//                    ProductID = item.ProductID,
//                    CreateDate = DateTime.Now,
//                    Lease = new OrderItemsLease
//                    {
//                        StartDate = (DateTime)model.StartDate,
//                        EndDate = ((DateTime)model.StartDate).AddMonths(model.MonthNum),
//                        CreateDate = DateTime.Now,
//                        ModifyDate = DateTime.Now,
//                        Status = LsStatus.Subsist,
//                    }
//                }).ToArray();
//                //订单附件
//                List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
//                //PI文件
//                foreach (var item in model.ContractFile)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Contract;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = Client.Current.ID;
//                    files.Add(file);
//                }
//                order.OrderFiles = files.ToArray();
//                order.Enter();
//                OperationLog(order.ID, "租赁订单保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", order.ID);
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("租赁订单保存失败" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 库位续租
//        /// </summary>
//        /// <param name="id">订单ID</param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult LeaseRenewal(string id)
//        {
//            var order = Client.Current.MyLsOrders.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            else if (order.Status != LsOrderStatus.Allocated)
//            {
//                return View("Error"); //只有已分配才能续租
//            }
//            var model = new LeaseModel();
//            model.ContractFile = new FileModel[0];
//            model.MonthNum = 1;
//            model.FatherID = id;
//            model.StartDate = order.EndDate.Value.AddDays(1);
//            model.StartDateStr = model.StartDate.Value.ToString("yyyy-MM-dd");
//            model.MonthNum = (order.EndDate.Value.Year - order.StartDate.Value.Year) * 12 + order.EndDate.Value.Month - order.StartDate.Value.Month;
//            ViewBag.Product = order.OrderItems.Select(item => new
//            {
//                item.Quantity,
//                item.ProductID,
//                UnitPrice = 0,
//                item.Product.Volume,
//                item.Product.Load,
//                item.Product.LsProductPrice,
//            }).ToArray();
//            return View(model);
//        }

//        /// <summary>
//        /// 取消租赁申请
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CancelLsOrder(string id)
//        {
//            try
//            {
//                var order = Client.Current.MyLsOrders.GetOrderDetail(id);
//                if (order == null)
//                {
//                    return base.JsonResult(VueMsgType.error, "取消失败！");
//                }
//                if (order.Status != LsOrderStatus.Unpaid)
//                {
//                    return base.JsonResult(VueMsgType.error, "取消失败！");
//                }
//                PvWsOrder.Services.ClientModels.LsOrderExtends.Cancle(order);
//                OperationLog(id, "租赁订单取消成功");
//                return base.JsonResult(VueMsgType.success, "取消成功！");
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("租赁订单取消失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.success, "取消失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 导出租赁合同
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ExportLeasingContract()
//        {
//            try
//            {
//                //创建文件夹
//                var fileName = DateTime.Now.Ticks + "租赁合同.docx";
//                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload");
//                if (!System.IO.Directory.Exists(dirPath))
//                {
//                    System.IO.Directory.CreateDirectory(dirPath);
//                }
//                var path = System.IO.Path.Combine(dirPath, fileName);
//                string fileurl = $"../../Files/Dowload/{fileName}";
//                //var order = Client.Current.MyLsOrders.GetOrderDetail(id);
//                var order = new PvWsOrder.Services.ClientModels.LsOrderExtends();
//                order.Export(path);
//                return base.JsonResult(VueMsgType.success, "", fileurl);
//            }
//            catch (Exception e)
//            {
//                return base.JsonResult(VueMsgType.error, e.Message);
//            }
//        }

//        /// <summary>
//        /// 保存合同文件
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult UploadContract()
//        {
//            var id = Request.Form["id"];
//            var fileName = Request.Form["FileName"];
//            var url = Request.Form["URL"];
//            var order = Client.Current.MyLsOrders.GetOrderDetail(id);
//            if (order == null)
//            {
//                return base.JsonResult(VueMsgType.error, "上传失败！");
//            }
//            List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
//            var file = new Yahv.Services.Models.CenterFileDescription();
//            file.Type = (int)FileType.Contract;
//            file.CustomName = fileName;
//            file.Url = url;
//            file.AdminID = Client.Current.ID;
//            files.Add(file);
//            order.OrderFiles = files.ToArray();
//            order.SaveFiles();
//            return base.JsonResult(VueMsgType.success, "上传成功！");
//        }

//        /// <summary>
//        /// 开票申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult LsInvoice(string id)
//        {
//            var order = Client.Current.MyLsOrders.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            //合同发票
//            var invoiceUrl = "";
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            var invoiceFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).FirstOrDefault();
//            if (invoiceFile != null)
//            {
//                invoiceUrl = fileurl + invoiceFile.Url.ToUrl();
//            }
//            var data = new
//            {
//                OrderID = id,
//                Type = order.Invoice.Type.GetDescription(),
//                order.Invoice.CompanyName,
//                TaxperNumber = order.Invoice.TaxperNumber,
//                order.Invoice.RegAddress,
//                order.Invoice.CompanyTel,
//                order.Invoice.Bank,
//                order.Invoice.Account,
//                order.Invoice.Name,
//                order.Invoice.Mobile,
//                order.Invoice.Address,
//                FileUrl = invoiceUrl,
//            };
//            return View(data);
//        }
//        #endregion

//        #region 全部订单
//        /// <summary>
//        /// 全部订单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyOrdersList()
//        {
//            var data = new
//            {
//                OrderStatusOptions = ExtendsEnum.ToDictionary<CgOrderStatus>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                InvoiceOptions = ExtendsEnum.ToDictionary<OrderInvoiceStatus>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View();
//        }

//        /// <summary>
//        /// 获取全部订单数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetMyOrdersList()
//        {
//            //var orderPaymentStatus = Request.Form["orderPaymentStatus"].ToString();  //支付状态
//            var orderMainStatus1 = Request.Form["orderMainStatus1"].ToString();  //订单主状态
//            var orderMainStatus2 = Request.Form["orderMainStatus2"].ToString();  //订单主状态
//            var orderInvoiceStatus1 = Request.Form["orderInvoiceStatus1"].ToString();  //开票状态
//            var orderInvoiceStatus2 = Request.Form["orderInvoiceStatus2"].ToString();  //开票状态
//            //var orderRemittanceStatus = Request.Form["orderRemittanceStatus"].ToString();  //付汇状态
//            var excuteStatus = Request.Form["excuteStatus"].ToString();  //执行状态
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var orderID = Request.Form["orderID"];  //订单ID

//            var order = Client.Current.MyOrder;

//            #region 筛选数据
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<Services.Models.WsOrder, bool>> expression = item => true;
//            if (!string.IsNullOrWhiteSpace(orderID))
//            {
//                Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.ID == orderID;
//                expression = expression.And(exp);
//            }
//            if (!string.IsNullOrWhiteSpace(orderMainStatus2))
//            {
//                Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.MainStatus == (CgOrderStatus)(int.Parse(orderMainStatus2));
//                expression = expression.And(exp);
//            }
//            if (!string.IsNullOrWhiteSpace(orderInvoiceStatus2))
//            {
//                Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.InvoiceStatus == (OrderInvoiceStatus)(int.Parse(orderInvoiceStatus2));
//                expression = expression.And(exp);
//            }
//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.CreateDate >= dStart && item.CreateDate <= dEnd;
//                expression = expression.And(exp);
//            }
//            lambdas.Add(expression);

//            Expression<Func<Services.Models.WsOrder, bool>> orexpression = null;
//            //if (!string.IsNullOrWhiteSpace(orderPaymentStatus))
//            //{
//            //    Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.PaymentStatus == (OrderPaymentStatus)(int.Parse(orderPaymentStatus));
//            //    orexpression = orexpression.Or(exp);
//            //}
//            if (!string.IsNullOrWhiteSpace(orderMainStatus1))
//            {
//                Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.MainStatus == (CgOrderStatus)(int.Parse(orderMainStatus1));
//                orexpression = orexpression.Or(exp);
//            }
//            if (!string.IsNullOrWhiteSpace(orderInvoiceStatus1))
//            {
//                Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.InvoiceStatus == (OrderInvoiceStatus)(int.Parse(orderInvoiceStatus1));
//                orexpression = orexpression.Or(exp);
//            }
//            //if (!string.IsNullOrWhiteSpace(orderRemittanceStatus))
//            //{
//            //    Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.RemittanceStatus != OrderRemittanceStatus.Remittanced && item.ConfirmStatus== OrderConfirmStatus.Confirmed ;  //待付货款：未付汇 部分付汇
//            //    orexpression = orexpression.Or(exp);
//            //}
//            if (!string.IsNullOrWhiteSpace(excuteStatus))
//            {
//                //Expression<Func<Services.Models.WsOrder, bool>> exp = item => item.ExecutionStatus == ExcuteStatus.深圳已出库 || item.ExecutionStatus == ExcuteStatus.香港已出库;  //待收货： 香港已出库 深圳已出库
//                //orexpression = orexpression.Or(exp);
//            }
//            if (orexpression != null)
//            {
//                lambdas.Add(orexpression);
//            }
//            #endregion

//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                MainStatus = item.MainStatus.GetDescription(),
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                Type = item.Type,
//                TypeName = item.Type.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Input.Currency == null ? item.Output.Currency?.GetDescription() : item.Input.Currency?.GetDescription(),
//                IsBill = item.Type == OrderType.Delivery || item.Type == OrderType.Recieved || item.Type == OrderType.Transport ? true : item.ConfirmStatus != OrderConfirmStatus.Waiting,
//            };
//            #endregion

//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 发票信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Invoice()
//        {
//            var a = Alls.Current.XDTInvoice.GetDetailByOrderID("").Select(item => new
//            {
//                item.InvoiceType, //类型
//                item.TaxName,//税务名称
//                item.Amount,//金额
//                item.InvoiceNoticeStatus,//开票状态
//            });

//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            var order = Client.Current.MyOrder.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            var data = new
//            {
//                OrderID = id,
//                Type = order.Invoice.Type.GetDescription(),
//                order.Invoice.CompanyName,
//                TaxperNumber = order.Invoice.TaxperNumber,
//                order.Invoice.RegAddress,
//                order.Invoice.CompanyTel,
//                order.Invoice.Bank,
//                order.Invoice.Account,
//                order.Invoice.Name,
//                order.Invoice.Mobile,
//                order.Invoice.Address,
//            };
//            return View(data);
//        }
//        #endregion

//        #region 代付/收货款
//        /// <summary>
//        /// 代付货款页面
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult PrePayApplyList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取代付货款申请列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetPrePayApplyList()
//        {
//            //var order = Client.Current.MyApplictions.ToList().Where(item => item.Type == ApplicationType.Payment).OrderByDescending(c => c.CreateDate);

//            //#region 页面数据
//            //Func<Yahv.PvWsOrder.Services.Models.Application, object> convert = item => new
//            //{
//            //    ID = item.ID,
//            //    Status = item.ExcuteStatus.GetDescription(),
//            //    ApplyMoney = item.Price,
//            //    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//            //    Currency = item.Beneficiary.Currency.GetDescription(),
//            //    PayType = item.Beneficiary.Methord.GetDescription(),
//            //    Bank = item.Beneficiary.Bank,
//            //    Type = item.Type.GetDescription(),
//            //};
//            //#endregion
//            //return this.Paging(order, convert);
//            return null;
//        }

//        /// <summary>
//        /// 代收货款页面
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult ReceivableList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取代收货款申请列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetReceivableList()
//        {
//            //var order = Client.Current.MyApplictions.ToList().Where(item => item.Type == ApplicationType.Receival).OrderByDescending(c => c.CreateDate);

//            //#region 页面数据
//            //Func<Yahv.PvWsOrder.Services.Models.Application, object> convert = item => new
//            //{
//            //    ID = item.ID,
//            //    Status = item.ExcuteStatus.GetDescription(),
//            //    ApplyMoney = item.Price,
//            //    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//            //    Currency = item.Beneficiary.Currency.GetDescription(),
//            //    PayType = item.Beneficiary.Methord.GetDescription(),
//            //    Bank = item.Beneficiary.Bank,
//            //    Type = item.Type.GetDescription(),
//            //};
//            //#endregion
//            //return this.Paging(order, convert);
//            return null;
//        }

//        /// <summary>
//        /// 申请详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult ApplicationDetail()
//        {
//            //var para = Request.Form["para"];
//            //if (string.IsNullOrWhiteSpace(para))
//            //{
//            //    return View("Error");
//            //}
//            //var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            //ViewBag.navid = paraArr[1];
//            //var id = paraArr[0];
//            //var apply = Client.Current.MyApplictions.GetDetail(id);
//            //if (apply == null)
//            //{
//            //    return View("Error");
//            //}
//            //var supplier = Client.Current.MySupplier[apply.Beneficiary.EnterpriseID];
//            //var order = Client.Current.MyOrder.GetOrderDetail(apply.OrderID);
//            //var orderItems = order.OrderItems.Select(item => new Models.OrderItem
//            //{
//            //    PartNumber = item.Product.PartNumber,
//            //    Manufacturer = item.Product.Manufacturer,
//            //    OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//            //    DateCode = item.DateCode,
//            //    Quantity = item.Quantity,
//            //    Unit = (int)item.Unit,
//            //    UnitLabel = item.Unit.GetDescription(),
//            //    TotalPrice = item.TotalPrice,
//            //    GrossWeight = item.GrossWeight,
//            //    Volume = item.Volume,
//            //}).ToArray();
//            //var file = apply.FileItems.FirstOrDefault();
//            //var data = new
//            //{
//            //    apply.ID,
//            //    Status = apply.ExcuteStatus.GetDescription(),
//            //    CreateDate = apply.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//            //    SupplierName = supplier?.Name,
//            //    SupplierEnglishName = supplier?.EnglishName,
//            //    apply.Beneficiary.Bank,
//            //    apply.Beneficiary.BankAddress,
//            //    apply.Beneficiary.Account,
//            //    apply.Beneficiary.SwiftCode,
//            //    Price = apply.Price.ToRound(2),
//            //    Type = apply.Type.GetDescription(),
//            //    TypeCode = apply.Type,
//            //    Methord = apply.Beneficiary.Methord.GetDescription(),
//            //    Currency = apply.Beneficiary.Currency.GetDescription(),
//            //    OrderItems = orderItems,
//            //    File = new
//            //    {
//            //        Name = file?.CustomName,
//            //        URL = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + file?.Url.ToUrl(),
//            //        Type = ((FileType?)file?.Type).GetDescription()
//            //    },
//            //    TotalMoney = orderItems.Sum(c => c.TotalPrice).ToRound(2),
//            //    PayerName = apply.Payer?.Name,
//            //    PayerBank = apply.Payer?.Bank,
//            //    PayerAccount = apply.Payer?.Account,
//            //};
//            //return View(data);
//            return View();
//        }

//        /// <summary>
//        /// 代付货款申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult PrePayApply()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            PrePayApplyModel model = new PrePayApplyModel();

//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            var order = Client.Current.MyOrder.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            model.AppliedMoney = order.ApplicationPaymentPrice.ToRound(2);
//            model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
//            {
//                PartNumber = item.Product.PartNumber,
//                Manufacturer = item.Product.Manufacturer,
//                OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                DateCode = item.DateCode,
//                Quantity = item.Quantity,
//                Unit = (int)item.Unit,
//                UnitLabel = item.Unit.GetDescription(),
//                TotalPrice = item.TotalPrice,
//                GrossWeight = item.GrossWeight,
//                Volume = item.Volume,
//            }).ToArray();
//            model.TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice));
//            model.OrderID = order.ID;
//            model.PrePayFile = new FileModel[0];
//            model.Currency = order.OrderItems[0].Currency.GetDescription();
//            //供应商
//            var supplier = Client.Current.MySupplier[order.SupplierID];
//            model.SupplierName = supplier.ChineseName;
//            model.SupplierEnglishName = supplier.EnglishName;
//            //银行账号
//            var bank = new MySupplierBankView(Client.Current.EnterpriseID, supplier.ID).Where(item => item.Currency == order.OrderItems[0].Currency).ToList().Select(item => new
//            {
//                ID = item.ID,
//                item.Bank,
//                item.BankAddress,
//                item.Account,
//                Methord = item.Methord.GetDescription(),
//                item.SwiftCode,
//            }).ToArray();
//            ViewBag.BankOptions = bank;
//            return View(model);
//        }

//        /// <summary>
//        /// 检查代付货款金额
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckPrePayMoney(string orderID, decimal applyMoney)
//        {
//            var order = Client.Current.MyOrder.GetOrderDetail(orderID);
//            if (order == null)
//            {
//                return base.JsonResult(VueMsgType.error, "该订单异常请返回主页面");
//            }
//            var leftPrice = order.TotalPrice - order.ApplicationPaymentPrice;
//            if (leftPrice < applyMoney)
//            {
//                return base.JsonResult(VueMsgType.error, "本次申请金额最多为" + leftPrice.ToRound(2) + "(" + order.OrderItems[0].Currency.GetDescription() + ")");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 检查代收货款金额
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckReceiveMoney(string orderID, decimal applyMoney)
//        {
//            var order = Client.Current.MyOrder.GetOrderDetail(orderID);
//            if (order == null)
//            {
//                return base.JsonResult(VueMsgType.error, "该订单异常请返回主页面");
//            }
//            var leftPrice = order.TotalPrice - order.ApplicationReceivalPrice;
//            if (leftPrice < applyMoney)
//            {
//                return base.JsonResult(VueMsgType.error, "本次申请金额最多为" + leftPrice.ToRound(2) + "(" + order.OrderItems[0].Currency.GetDescription() + ")");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 提交代付货款申请
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult PrePaySubmit(string data)
//        {
//            var current = Client.Current;
//            try
//            {
//                var model = JsonConvert.DeserializeObject<PrePayApplyModel>(data);
//                Application apply = new Application();
//                apply.Type = ApplicationType.Payment;
//                apply.AdminID = current.ID;
//                apply.ClientID = current.MyClients.ID;
//                apply.OrderID = model.OrderID;
//                apply.Price = model.ApplyMoney;
//                apply.BeneficiaryID = model.BankID;
//                apply.Files = model.PrePayFile.Select(item => new Services.Models.CenterFileDescription
//                {
//                    CustomName = item.name,
//                    Type = (int)FileType.PaymentEntrust,
//                    Url = item.URL,
//                }).ToArray();
//                apply.Enter();
//                OperationLog(model.OrderID, "代付货款申请保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", apply.ID);
//            }
//            catch (Exception ex)
//            {
//                current.Errorlog.Log("代付货款申请保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 提交代收货款申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ReceiveSubmit(string data)
//        {
//            var current = Client.Current;
//            try
//            {
//                var model = JsonConvert.DeserializeObject<ReceiveApplyModel>(data);
//                Application apply = new Application();
//                apply.Type = ApplicationType.Receival;
//                apply.AdminID = current.ID;
//                apply.ClientID = current.MyClients.ID;
//                apply.OrderID = model.OrderID;
//                apply.Price = model.ApplyMoney;
//                apply.BeneficiaryID = model.BankID;
//                apply.PayerID = model.PayerID;
//                apply.Files = model.ReceiveFile.Select(item => new Services.Models.CenterFileDescription
//                {
//                    CustomName = item.name,
//                    Type = (int)FileType.ReceiveEntrust,
//                    Url = item.URL,
//                }).ToArray();
//                apply.Enter();
//                OperationLog(model.OrderID, "代收货款申请保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", apply.ID);
//            }
//            catch (Exception ex)
//            {
//                current.Errorlog.Log("代收货款申请保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }


//        /// <summary>
//        /// 代收货款申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult ReceiveApply()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            ReceiveApplyModel model = new ReceiveApplyModel();

//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            var order = Client.Current.MyOrder.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            model.AppliedMoney = order.ApplicationReceivalPrice.ToRound(2);
//            model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
//            {
//                PartNumber = item.Product.PartNumber,
//                Manufacturer = item.Product.Manufacturer,
//                OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                DateCode = item.DateCode,
//                Quantity = item.Quantity,
//                Unit = (int)item.Unit,
//                UnitLabel = item.Unit.GetDescription(),
//                TotalPrice = item.TotalPrice,
//                GrossWeight = item.GrossWeight,
//                Volume = item.Volume,
//            }).ToArray();
//            model.TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice));
//            model.OrderID = order.ID;
//            model.ReceiveFile = new FileModel[0];
//            model.Currency = order.OrderItems[0].Currency.GetDescription();
//            model.CurrencyID = ((int)order.OrderItems[0].Currency).ToString();

//            //银行账号
//            var bank = Yahv.Alls.Current.BeneficiariesAll.Where(c => c.EnterpriseID == PvWsOrder.Services.PvClientConfig.ThirdCompanyID && c.Currency == order.OrderItems[0].Currency).Select(item => new
//            {
//                ID = item.ID,
//                item.Account,
//            }).ToArray();
//            //var payers = Client.Current.MyPayers.Where(c => c.Currency == order.OrderItems[0].Currency).ToList().Select(c => new
//            //{
//            //    ID = c.ID,
//            //    c.Bank,
//            //    c.Account,
//            //    Methord = c.Methord.GetDescription(),
//            //    c.Name
//            //}).ToArray();


//            var data = new
//            {
//                BankOptions = bank,
//               // PayerOptions = payers,
//            };
//            ViewBag.Options = data;
//            return View(model);
//        }

//        /// <summary>
//        /// 付款人新增分部视图
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialPayer()
//        {
//            PayerModel model = new PayerModel();
//            var data = new
//            {
//                MethordOptions = ExtendsEnum.ToDictionary<Methord>().Where(c => c.Key != "1").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                CurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                DistrictOptions = ExtendsEnum.ToDictionary<District>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return PartialView(model);
//        }

//        /// <summary>
//        /// 付款人新增分部视图
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult SubmitPayer(PayerModel model)
//        {
//            var current = Client.Current;
//            try
//            {
//                Payer apply = new Payer();
//                apply.Account = model.Account;
//                apply.AdminID = current.ID;
//                apply.Bank = model.Bank;
//                apply.BankAddress = model.BankAddress;
//                apply.ClientID = current.MyClients.ID;
//                apply.Currency = (Currency)int.Parse(model.Currency);
//                apply.District = (District)int.Parse(model.District);
//                apply.Email = model.Email;
//                apply.Methord = (Methord)int.Parse(model.Methord);
//                apply.Mobile = model.Mobile;
//                apply.Name = model.Name;
//                apply.SwiftCode = model.SwiftCode;
//                apply.Tel = model.Tel;
//                apply.Enter();
//                return base.JsonResult(VueMsgType.success, "新增成功", apply.ID);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 获取付款人列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetPayerOptions(string currency)
//        {
//            //var payers = Client.Current.MyPayers.Where(c => c.Currency == (Currency)int.Parse(currency)).ToList().Select(c => new
//            //{
//            //    ID = c.ID,
//            //    c.Bank,
//            //    c.Account,
//            //    Methord = c.Methord.GetDescription(),
//            //    c.Name
//            //}).Json();
//            var payers =new
//            {
//            }.Json();
//            return base.JsonResult(VueMsgType.success, "", payers);
//        }
//        #endregion
//    }
//}