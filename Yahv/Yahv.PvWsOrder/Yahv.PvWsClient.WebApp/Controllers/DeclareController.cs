//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Net;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using Newtonsoft.Json;
//using Yahv.PvWsOrder.Services.ConstConfig;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsOrder.Services.XDTClientView;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.Linq.Generic;
//using NPOI.SS.UserModel;
//using Yahv.Utils.Npoi;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsOrder.Services.ClientModels;
//using System.Collections;
//using Yahv.PvWsOrder.Services.Extends;
//using Yahv.PvWsOrder.Services;
//using Yahv.Payments;

//namespace Yahv.PvWsClient.WebApp.Controllers
//{
//    public class DeclareController : UserController
//    {
//        #region 报关单
//        /// <summary>
//        /// 报关单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareOrder()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取报关单数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetDeclareOrders()
//        {
//            var orderID = Request.Form["orderID"].ToString().Trim();  //订单编号
//            var contrNo = Request.Form["contrNo"].ToString().Trim();  //合同编号
//            var decID = Request.Form["decID"].ToString().Trim();  //报关单号
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var dec = Client.Current.MyDecHeads;
//            #region 条件筛选
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<DeclareOrderViewModel, bool>> lambda = item => true;
//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                lambda = item => item.DDate >= dStart && item.DDate <= dEnd;
//                lambdas.Add(lambda);
//            }
//            if (!string.IsNullOrWhiteSpace(contrNo))
//            {
//                lambda = item => item.ContrNo == contrNo;
//                lambdas.Add(lambda);
//            }
//            if (!string.IsNullOrWhiteSpace(orderID))
//            {
//                lambda = item => item.OrderID == orderID;
//                lambdas.Add(lambda);
//            }
//            if (!string.IsNullOrWhiteSpace(decID))
//            {
//                lambda = item => item.EntryId == decID;
//                lambdas.Add(lambda);
//            }
//            #endregion
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = dec.GetPageList(lambdas.ToArray(), rows, page);
//            Func<DeclareOrderViewModel, object> convert = item => new
//            {
//                item.OrderID,
//                item.ID,
//                item.EntryId,
//                DDate = item.DDate?.ToString("yyyy-MM-dd"),
//                item.ContrNo,
//                item.TotalDeclarePrice,
//                item.Currency
//            };
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 下载报关单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DownloadDeclare(string id)
//        {
//            var ids = JsonConvert.DeserializeObject<string[]>(id);
//            Encoding encoding = Encoding.UTF8;
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["DownLoadDecheadUrl"]);
//            request.Method = "POST";
//            request.Accept = "text/html, application/xhtml+xml, */*";
//            request.ContentType = "application/json";

//            byte[] buffer = encoding.GetBytes(ids.Json());
//            request.ContentLength = buffer.Length;
//            request.GetRequestStream().Write(buffer, 0, buffer.Length);
//            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//            Stream s = response.GetResponseStream();
//            //读取服务器端返回的消息  
//            StreamReader sr = new StreamReader(s);
//            string sReturnString = sr.ReadLine();
//            var responseData = JsonConvert.DeserializeObject<ResponseData>(sReturnString);
//            if (responseData.success == "true")
//            {
//                return base.JsonResult(VueMsgType.success, "", responseData.url);
//            }
//            else
//            {
//                return base.JsonResult(VueMsgType.error, responseData.message);
//            }
//        }

//        #endregion

//        #region 报关单数据
//        /// <summary>
//        /// 报关数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareData()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取报关单数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetDeclareData()
//        {
//            var result = this.GetPageData();
//            Func<ClientOrderDataViewModel, object> convert = item => new
//            {
//                OrderID = item.OrderID,
//                ID = item.DecListID,
//                CodeTS = item.CodeTS,
//                GName = item.GName,
//                GModel = item.GModel,
//                GoodsBrand = item.GoodsBrand,
//                GoodsModel = item.GoodsModel,
//                OriginCountry = item.OriginCountryName,
//                GQty = item.GQty,
//                NetWt = item.NetWt,
//                DeclPrice = item.DeclPrice,
//                DeclTotal = item.DeclTotal,
//                TradeCurr = item.TradeCurr,
//                ContrNo = item.ContrNo,
//                CustomsRate = item.CustomsExchangeRate.ToString("0.0000"),
//                TariffRate = item.TariffRate.ToString("0.0000"),
//                DeclTotalRMB = item.DeclTotalRMB.ToString("0.00"),
//                TariffPay = (item.DeclTotalRMB * item.TariffRate).ToString("0.00"),
//                TariffPayed = (item.DeclTotalRMB * item.ReceiptRate).ToString("0.00"),
//                ValueVat = item.TotalValueVat >= 50 ? ((item.DeclTotalRMB + (item.DeclTotalRMB * item.TariffRate)) * item.Vat).ToString("0.00") : "0",
//                CustomsValue = item.DeclTotalRMB + (item.DeclTotalRMB * item.TariffRate),
//                CustomsValueVat = (item.DeclTotalRMB + (item.DeclTotalRMB * item.TariffRate)) * ConstConfig.ValueAddedTaxRate,
//                InvoiceCompany = item.InvoiceCompany,
//                EntryId = item.EntryId,
//                CreateDate = item.DDate?.ToString("yyyy-MM-dd"),
//                TaxName = item.TaxName,
//                TaxCode = item.TaxCode,
//                ProductUniqueCode = item.ProductUniqueCode,
//                ImportTaxCode = item.ImportTaxCode,
//                AddValueTaxCode = item.AddValueTaxCode
//            };
//            return this.Paging(result, result.Total, convert);

//        }

//        /// <summary>
//        /// 根据条件获取数据
//        /// </summary>
//        /// <returns></returns>
//        protected PageList<ClientOrderDataViewModel> GetPageData()
//        {
//            var orderID = Request.Form["orderID"].ToString().Trim();  //订单编号
//            var contrNo = Request.Form["contrNo"].ToString().Trim();  //合同编号
//            var decHeadID = Request.Form["decHeadID"].ToString().Trim();  //报关单号
//            var models = Request.Form["models"].ToString().Trim(); //型号
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var exportDateType = Request.Form["exportDateType"];
//            var month = Request.Form["month"];

//            var dec = Client.Current.MyDecHeadData;
//            #region 条件筛选
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<ClientOrderDataViewModel, bool>> lambda = item => true;

//            if (!string.IsNullOrWhiteSpace(orderID))
//            {
//                lambda = item => item.OrderID == orderID;
//                lambdas.Add(lambda);
//            }
//            if (!string.IsNullOrWhiteSpace(contrNo))
//            {
//                lambda = item => item.ContrNo == contrNo;
//                lambdas.Add(lambda);
//            }
//            if (!string.IsNullOrWhiteSpace(decHeadID))
//            {
//                lambda = item => item.DecHeadID == decHeadID;
//                lambdas.Add(lambda);
//            }
//            if (!string.IsNullOrWhiteSpace(models))
//            {
//                lambda = item => item.GoodsModel == models;
//                lambdas.Add(lambda);
//            }
//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                lambda = item => item.DDate >= dStart && item.DDate <= dEnd;
//                lambdas.Add(lambda);
//            }
//            if (exportDateType == "day")
//            {
//                if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//                {
//                    var dStart = DateTime.Parse(startDate);
//                    dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                    var dEnd = DateTime.Parse(endDate);
//                    dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                    lambda = item => item.DDate >= dStart && item.DDate <= dEnd;
//                    lambdas.Add(lambda);
//                }
//            }
//            else if (exportDateType == "month")
//            {
//                if (!string.IsNullOrEmpty(month))
//                {
//                    string y = month.Split('-')[0];
//                    int yint = int.Parse(y);
//                    string m = month.Split('-')[1];
//                    int mint = int.Parse(m);

//                    if (mint != 12)
//                    {
//                        var dStart = new DateTime(yint, mint, 1);
//                        var dEnd = new DateTime(yint, mint + 1, 1);
//                        lambda = item => item.DDate >= dStart && item.DDate < dEnd;
//                        lambdas.Add(lambda);
//                    }
//                    else
//                    {
//                        var dStart = new DateTime(yint, mint, 1);
//                        var dEnd = new DateTime(yint + 1, 1, 1);
//                        lambda = item => item.DDate >= dStart && item.DDate < dEnd;
//                        lambdas.Add(lambda);
//                    }
//                }
//            }
//            #endregion

//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = dec.GetPageList(lambdas.ToArray(), rows, page);

//            return list;
//        }

//        /// <summary>
//        /// 报关数据导出Excel
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ExportDecDataList()
//        {
//            var orderData = this.GetPageData();

//            var linq = orderData.ToList().Select((t, i) => new
//            {
//                序号 = i + 1,
//                报关日期 = t.DDate?.ToString("yyyy-MM-dd"),
//                合同号 = t.ContrNo,
//                物料号 = t.ProductUniqueCode,
//                商品编码 = t.CodeTS,
//                规格型号 = t.GoodsModel,
//                成交数量 = t.GQty,
//                单价 = t.DeclPrice,
//                总价 = t.DeclTotal,
//                币制 = t.TradeCurr,
//                品名 = t.GName,
//                海关汇率 = t.CustomsExchangeRate.ToString("0.0000"),
//                关税率 = t.TariffRate.ToString("0.0000"),
//                报关总价 = t.DeclTotalRMB.ToString("0.00"),
//                应交关税 = (t.DeclTotalRMB * t.TariffRate).ToString("0.00"),
//                实交增值税 = t.TotalValueVat >= 50 ? ((t.DeclTotalRMB + (t.DeclTotalRMB * t.TariffRate)) * t.Vat).ToString("0.00") : "0",
//                实交关税 = (t.DeclTotalRMB * t.ReceiptRate).ToString("0.00"),
//                完税价格 = t.DeclTotalRMB + (t.DeclTotalRMB * t.TariffRate).ToString("0.00"),
//                完税价格增值税 = (t.DeclTotalRMB + (t.DeclTotalRMB * t.TariffRate)) * ConstConfig.ValueAddedTaxRate,
//                开票公司 = t.InvoiceCompany,
//                订单编号 = t.OrderID,
//                报关单号 = t.EntryId,
//                税务名称 = t.TaxName,
//                税务编码 = t.TaxCode,
//                关税发票号 = t.ImportTaxCode,
//                增值税发票号 = t.AddValueTaxCode,
//            });

//            IWorkbook workbook = ExcelFactory.Create();
//            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
//            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
//            npoi.EnumrableToExcel(linq, 0, columnsWidth);
//            //创建文件夹
//            var fileName = DateTime.Now.Ticks + ".xlsx";
//            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
//            npoi.SaveAs(filepath);
//            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
//            return base.JsonResult(VueMsgType.success, "", localpath);
//        }
//        #endregion

//        #region 缴税记录
//        /// <summary>
//        /// 缴税记录
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult TaxRecord()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取缴税记录的数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetMyCustomsTaxReports()
//        {
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var orderID = Request.Form["orderID"];  //订单ID
//            var contrNo = Request.Form["contrNo"];  //合同号

//            var view = Client.Current.MyTaxReports.AsQueryable();

//            #region 条件筛选
//            if (!string.IsNullOrWhiteSpace(orderID))
//            {
//                view = view.Where(item => item.OrderID == orderID);
//            }
//            if (!string.IsNullOrWhiteSpace(contrNo))
//            {
//                view = view.Where(item => item.ContractNo == contrNo);
//            }

//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                view = view.Where(item => item.CreateDate >= dStart && item.CreateDate <= dEnd);
//            }

//            #endregion
//            Func<CustomsTaxReport, object> convert = item => new
//            {
//                ID = item.ID,
//                TaxNumber = item.TaxNumber,
//                OrderID = item.OrderID,
//                ContrNo = item.ContractNo,
//                DecAmount = item.Amount,
//                TaxType = item.TaxType.GetDescription(),
//                CreateDate = item.PayDate?.ToString("yyyy-MM-dd"),
//                isLoading = true,
//                isCheck = false,
//            };
//            return this.Paging(view, convert);
//        }

//        /// <summary>
//        /// 下载税单
//        /// </summary>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DownloadTaxList(string id)
//        {
//            var ids = JsonConvert.DeserializeObject<string[]>(id);

//            var orders = Client.Current.MytaxRecords;
//            var orderList = orders.Where(item => ids.Contains(item.ID)).ToArray();

//            Encoding encoding = Encoding.UTF8;
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["DownLoadInvoiceUrl"]);
//            request.Method = "POST";
//            request.Accept = "text/html, application/xhtml+xml, */*";
//            request.ContentType = "application/json";

//            byte[] buffer = encoding.GetBytes(orderList.Json());
//            request.ContentLength = buffer.Length;
//            request.GetRequestStream().Write(buffer, 0, buffer.Length);
//            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//            Stream s = response.GetResponseStream();
//            //读取服务器端返回的消息  
//            StreamReader sr = new StreamReader(s);
//            string sReturnString = sr.ReadLine();
//            var responseData = JsonConvert.DeserializeObject<ResponseData>(sReturnString);
//            if (responseData.success == "true")
//            {
//                return base.JsonResult(VueMsgType.success, "", responseData.url);
//            }
//            else
//            {
//                return base.JsonResult(VueMsgType.error, responseData.message);
//            }
//        }


//        /// <summary>
//        /// 缴费流水导出Excel
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ExportTaxList(string id)
//        {
//            var ids = JsonConvert.DeserializeObject<string[]>(id);
//            var view = Client.Current.MyTaxReports.AsQueryable().Where(item => ids.Contains(item.ID));

//            var linq = view.ToList().Select((t, i) => new
//            {
//                序号 = i + 1,
//                订单号 = t.OrderID,
//                税费单号 = t.TaxNumber,
//                合同号 = t.ContractNo,
//                金额 = t.Amount,
//                类型 = t.TaxType.GetDescription(),
//                缴税日期 = t.PayDate?.ToString("yyyy-MM-dd"),
//            });

//            IWorkbook workbook = ExcelFactory.Create();
//            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
//            int[] columnsWidth = { 5, 30, 30, 30, 30, 30, 30 };
//            npoi.EnumrableToExcel(linq, 0, columnsWidth);
//            //创建文件夹
//            var fileName = DateTime.Now.Ticks + ".xlsx";
//            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
//            npoi.SaveAs(filepath);
//            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
//            return base.JsonResult(VueMsgType.success, "", localpath);
//        }
//        #endregion

//        #region 报关
//        /// <summary>
//        /// 报关列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取报关单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetDeclareList()
//        {
//            var order = Client.Current.MyDeclareOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                //MainStatus = item.MainStatus.GetDescription(),
//                //InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                //CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                //Currency = item.Input?.Currency?.GetDescription(),
//                //IsEdit = item.MainStatus == CgOrderStatus.暂存,
//                //IsBill = item.ConfirmStatus != OrderConfirmStatus.Waiting&& item.MainStatus!CgOrderStatus.取消ed,
//                //item.OutWaybill.Consignee.Contact,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 新增报关单
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareAdd()
//        {
//            var para = Request.Form["para"];
//            DeclareAddViewModel model = new DeclareAddViewModel();
//            var current = Client.Current;

//            //收货地址
//            var receiveOptions = current.MyConsignees.Select(item => new { value = item.ID, text = item.Enterprise.Name, address = item.Address, name = item.Name, mobile = item.Tel }).ToArray();
//            model.Currency = ((int)Currency.USD).ToString(); //默认美元
//            model.EnterCode = current.MyClients.EnterCode;
//            model.OrderItems = new Models.OrderItem[0];
//            model.HKFiles = new FileModel[0];
//            model.PIFiles = new FileModel[0];
//            //香港交货方式 默认送货上门
//            model.HKDeliveryType = ((int)WaybillType.PickUp).ToString();
//            model.SZDeliveryType = ((int)WaybillType.PickUp).ToString();
//            model.PayExchangeSupplier = new string[0];  //付汇供应商实例化
//                                                        //其他信息
//            model.WrapType = ((int)Package.纸制或纤维板制盒).ToString();
//            //收货信息
//            model.ClientConsignee = receiveOptions.FirstOrDefault()?.value;
//            model.ClientConsigneeName = receiveOptions.FirstOrDefault()?.name;
//            model.ClientConsigneeAddress = receiveOptions.FirstOrDefault()?.address;
//            model.ClientContactMobile = receiveOptions.FirstOrDefault()?.mobile;


//            #region 快捷下单
//            if (!string.IsNullOrWhiteSpace(para))
//            {
//                var ids = para.Split(',');
//                if (ids.Count() == 0)
//                {
//                    return View("Error");  //id错误
//                }
//                var product = current.MyClassifiedPreProducts;
//                model.OrderItems = product.Where(item => ids.Contains(item.ID)).Select(item => new Models.OrderItem
//                {
//                    PreProcuctID = item.ID,
//                    Manufacturer = item.Manufacturer,
//                    Name = item.ProductName,
//                    PartNumber = item.Model,
//                }).ToArray();

//                model.IsClssified = true;
//                model.Currency = EnumHelper.GetEnumValue<Currency>(product.First().Currency).ToString();

//            }
//            #endregion

//            //基础数据
//            var data = new
//            {
//                UnitOptions = UnitEnumHelper.ToUnitDictionary().Select(item => new { value = item.Value, text = item.Code + " " + item.Name }).ToArray(),
//                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item=>item.value).ToArray(),
//                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.ChineseName }).ToArray(),
//                ReceiveOptions = receiveOptions,
//                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PackTypeOptions = ExtendsEnum.ToDictionary<Package>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                HKDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                SZDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View(model);
//        }


//        /// <summary>
//        /// 修改报关单
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareEdit(string id)
//        {
//            DeclareAddViewModel model = new DeclareAddViewModel();
//            var current = Client.Current;
//            model.EnterCode = current.MyClients.EnterCode;
//            //收货地址
//            var receiveOptions = current.MyConsignees.Select(item => new { value = item.ID, text = item.Enterprise.Name, address = item.Address, name = item.Name, mobile = item.Tel }).ToArray();
//            #region 修改
//            if (string.IsNullOrWhiteSpace(id))
//            {
//                return View("Error");
//            }
//            var order = current.MyDeclareOrder.GetOrderDetail(id);
//            if (order.MainStatus != CgOrderStatus.暂存)
//            {
//                return View("Error"); //待提交订单方可修改
//            }
//            model.ID = order.ID;
//            //订单项
//            model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
//            {
//                ID = item.ID,
//                DateCode = item.DateCode,
//                GrossWeight = item.GrossWeight,
//                InputID = item.InputID,
//                Manufacturer = item.Product.Manufacturer,
//                Name = item.Name,
//                Origin = item.Origin,
//                OriginLabel = EnumHelper.GetEnum<Origin>(item.Origin).GetDescription(),
//                OutputID = item.OutputID,
//                PartNumber = item.Product.PartNumber,
//                Quantity = item.Quantity,
//                TotalPrice = item.TotalPrice,
//                Unit = (int)item.Unit,
//                UnitLabel = item.Unit.GetDescription(),
//                UnitPrice = item.UnitPrice,
//                Volume = item.Volume,
//            }).ToArray();

//            //香港交货方式
//            model.HKDeliveryType = ((int)order.InWaybill.Type).ToString();
//            model.WayBillNo = order.InWaybill.Code;
//            model.SupplierID = order.SupplierID;
//            if (order.InWaybill.Type == WaybillType.PickUp)
//            {
//                var wayLoading = order.InWaybill.WayLoading;
//                var supplier = Client.Current.MySupplier[model.SupplierID]?.MySupplierAddress;
//                model.SupplierAddress = supplier.Where(item => item.Name == wayLoading.TakingContact && item.Mobile == wayLoading.TakingPhone && item.Address == wayLoading.TakingAddress).FirstOrDefault()?.ID;
//                model.SupplierAddressOptions = supplier.Select(item => new
//                {
//                    value = item.ID,
//                    text = "联系人:" + item.Name + " 电话:" + item.Mobile + " 地址:" + item.Address,
//                    isDefault = item.IsDefault
//                }).Json();
//                model.PickupTime = wayLoading.TakingDate;
//            }

//            //国内交货方式
//            var outWayLoading = order.OutWaybill.WayLoading;
//            model.SZDeliveryType = ((int)order.OutWaybill.Type).ToString();
//            model.ClientPicker = outWayLoading?.TakingContact;
//            model.ClientPickerMobile = outWayLoading?.TakingPhone;
//            model.IDType = ((int?)order.OutWaybill.Consignee.IDType).ToString();
//            model.IDNumber = order.OutWaybill.Consignee.IDNumber;
//            var consignee = order.OutWaybill.Consignee;
//            var address = receiveOptions.Where(item => item.name == consignee.Contact && item.address == consignee.Address && item.mobile == consignee.Phone).FirstOrDefault();
//            model.ClientConsignee = address?.value;
//            model.ClientConsigneeName = address?.name;
//            model.ClientContactMobile = address?.mobile;
//            model.ClientConsigneeAddress = address?.address;

//            //付汇供应商
//            model.PayExchangeSupplier = order.PayExchangeSupplier.Select(item => item.ID).ToArray();

//            //其他信息
//            var orderConditions = JsonConvert.DeserializeObject<Yahv.PvWsOrder.Services.Models.OrderCondition>(order.Input.Conditions);
//            var wayCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.InWaybill.Condition);
//            model.IsFullVehicle = orderConditions.IsCharterBus;
//            model.IsLoan = wayCondition.AgencyPayment;
//            model.WrapType = order.InWaybill.Packaging;
//            model.PackNo = order.InWaybill.TotalParts?.ToString();
//            model.Summary = order.Summary;

//            //提货文件
//            model.HKFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).Select(item => new FileModel
//            {
//                name = item.CustomName,
//                URL = item.Url,
//                fileFormat = Path.GetExtension(item.CustomName).ToLower()
//            }).ToArray();
//            //PI文件
//            model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new FileModel
//            {
//                name = item.CustomName,
//                URL = item.Url,
//                fileFormat = Path.GetExtension(item.CustomName).ToLower()
//            }).ToArray();
//            model.Currency = ((int)order.Input.Currency).ToString();
//            #endregion

//            //基础数据
//            var data = new
//            {
//                UnitOptions = UnitEnumHelper.ToUnitDictionary().Select(item => new { value = item.Value, text = item.Code + " " + item.Name }).ToArray(),
//                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
//                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.ChineseName }).ToArray(),
//                ReceiveOptions = receiveOptions,
//                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PackTypeOptions = ExtendsEnum.ToDictionary<Package>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                HKDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                SZDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key == "1" || item.Key == "2").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return View("DeclareAdd", model);
//        }

//        /// <summary>
//        /// 报关单订单详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareDetail()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            var order = Client.Current.MyDeclareOrder.GetOrderDetail(id);
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
//                var invoice = order.Invoice;
//                //开票信息
//                //var _invoice = new InvoiceViewModel
//                //{
//                //    ID = invoice.ID,
//                //    Type = invoice.Type.GetDescription(),
//                //    DeliveryType = invoice.DeliveryType.GetDescription(),
//                //    CompanyName = invoice.Name,
//                //    TaxperNumber = invoice.TaxperNumber,
//                //    RegAddress = invoice.RegAddress,
//                //    CompanyTel = invoice.CompanyTel,
//                //    Bank = invoice.Bank,
//                //    Account = invoice.Account
//                //};
//                //对账单
//                var orderBillURL = "";
//                var orderBillName = "";
//                var orderBillStatus = false;
//                var orderBill = order.OrderFiles.Where(item => item.Type == (int)FileType.OrderBill).OrderByDescending(item => item.CreateDate).FirstOrDefault();
//                var IsUploadOrderBill = order.ConfirmStatus == OrderConfirmStatus.Confirmed; //客户已确认才可上传对账单
//                if (orderBill != null)
//                {
//                    orderBillURL = fileurl + orderBill.Url;
//                    orderBillName = orderBill.CustomName;
//                    orderBillStatus = orderBill.Status == Services.Models.FileDescriptionStatus.Approved;
//                }
//                //报关委托书
//                var orderAgentURL = "";
//                var orderAgentName = "";
//                var orderAgentStatus = false;
//                var orderAgent = order.OrderFiles.Where(item => item.Type == (int)FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
//                var IsUploadOrderAgent = order.ConfirmStatus == OrderConfirmStatus.Confirmed; //客户已确认才可上传报关委托书
//                if (orderAgent != null)
//                {
//                    orderAgentURL = fileurl + orderAgent.Url;
//                    orderAgentName = orderAgent.CustomName;
//                    orderAgentStatus = orderAgent.Status == Services.Models.FileDescriptionStatus.Approved;
//                }
//                var orderItems = order.OrderItems.Select(item => new Models.OrderItem
//                {
//                    PartNumber = item.Product.PartNumber,
//                    Name = item.ClassfiedName ?? item.Name,
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
//                var inOrderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Input.Conditions);
//                var model = new
//                {
//                    ID = order.ID,
//                    MainStatus = order.MainStatus.GetDescription(),
//                    CreateDate = order.CreateDate.ToString("yyyy-MM-dd"),
//                    Currency = order.Input.Currency.GetDescription(),
//                    IsPayCharge = Convert.ToBoolean(order.Input.IsPayCharge) ? "是" : "否",
//                    HKDeliveryTypeName = order.InWaybill.Type.GetDescription(),
//                    HKDeliveryType = order.InWaybill.Type,
//                    SupplierName = order.SupplierName,
//                    TakingDetailAddress = order.InWaybill.WayLoading?.TakingAddress,
//                    TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
//                    TakingContact = order.InWaybill.WayLoading?.TakingContact,
//                    TakingPhone = order.InWaybill.WayLoading?.TakingPhone,
//                    DeliveryFile = _deliveryFile,
//                    PIFiles,
//                    SZDeliveryTypeName = order.OutWaybill.Type.GetDescription(),
//                    SZDeliveryType = order.OutWaybill.Type,
//                    OutTakingContact = order.OutWaybill.WayLoading?.TakingContact,
//                    OutTakingPhone = order.OutWaybill.WayLoading?.TakingPhone,
//                    CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription(),
//                    Certificate = order.OutWaybill.Consignee?.IDNumber,
//                    ReceivedContact = order.OutWaybill.Consignee.Contact,
//                    ReceivedPhone = order.OutWaybill.Consignee.Phone,
//                    ReceivedAddress = order.OutWaybill.Consignee.Address,
//                    IsFullVehicle = inOrderCondition.IsCharterBus ? "是" : "否",
//                    order.InWaybill.TotalParts,
//                    order.Summary,
//                    WaybillCode = order.InWaybill.Code,
//                    Packaging = ((Package)int.Parse(order.InWaybill.Packaging)).GetDescription(),
//                    TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice)),
//                    OrderItems = orderItems,
//                    //Invoice = _invoice,
//                    PayExchangeSupplier = order.PayExchangeSupplier.Select(c => c.Name).ToArray(),
//                    OrderBillURL = orderBillURL,
//                    OrderBillName = orderBillName,
//                    OrderBillStatus = orderBillStatus,
//                    IsUploadOrderBill,
//                    OrderAgentURL = orderAgentURL,
//                    OrderAgentName = orderAgentName,
//                    OrderAgentStatus = orderAgentStatus,
//                    IsUploadOrderAgent=true,
//                };
//                return View(model);
//            }
//            return View("Error");
//        }

//        /// <summary>
//        /// 获取供应商收货地址
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetSupplierAddress(string supplier)
//        {
//            var address = "";
//            string supplierID = supplier.InputText();
//            var entity = Client.Current.MySupplier[supplier];
//            if (entity == null)
//            {
//                return base.JsonResult(VueMsgType.error, "供应商不存在！");
//            }
//            else
//            {
//                address = entity.MySupplierAddress.OrderByDescending(item => item.IsDefault).Select(item => new
//                {
//                    value = item.ID,
//                    text = "联系人:" + item.Name + " 电话:" + item.Mobile + " 地址:" + item.Address,
//                    isDefault = item.IsDefault
//                }).Json();
//            }

//            return base.JsonResult(VueMsgType.success, "", address);
//        }

//        /// <summary>
//        /// 报关单产品导入
//        /// </summary>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareFileUpload(HttpPostedFileBase file)
//        {
//            var result = new List<object>();
//            try
//            {
//                //拿到上传来的文件
//                file = Request.Files["file"];
//                string fileExtension = Path.GetExtension(file.FileName).ToLower();//文件拓展名
//                string[] allowFiles = { ".xls", ".xlsx" };
//                if (allowFiles.Contains(fileExtension) == false)
//                {
//                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传.xls或.xlsx格式的文件。");
//                }
//                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(fileExtension, file.InputStream, true);
//                foreach (System.Data.DataRow row in dt.Rows)
//                {
//                    var array = row.ItemArray;
//                    int no = 0;
//                    if (!int.TryParse(array[0].ToString(), out no))
//                    {
//                        continue;
//                    }

//                    //无效行
//                    if (array[6].ToString() == string.Empty && array[9].ToString() == string.Empty)
//                    {
//                        continue;
//                    }

//                    var orignvalue = "";  //原产地值
//                    var orignlable = "";  //原产地名称
//                    if (!string.IsNullOrWhiteSpace(array[7].ToString()))
//                    {
//                        var origin = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Name.ToUpper() == array[7].ToString().ToUpper() || item.Value.ToUpper() == array[7].ToString().ToUpper()).ToArray();
//                        if (origin.Count() > 0)
//                        {
//                            orignvalue = origin[0].Value;
//                            orignlable = origin[0].Name;
//                        }
//                    }
//                    var quantity = array[9].GetType().Name == "DBNull" ? 0 : Convert.ToDecimal(array[9]);
//                    var totalPrice = array[11].GetType().Name == "DBNull" ? 0 : Math.Round(Convert.ToDecimal(array[11]), 2, MidpointRounding.AwayFromZero);
//                    var model = new
//                    {
//                        Batch = array[2].ToString(),
//                        Name = array[4].ToString(),
//                        Manufacturer = array[5].ToString(),
//                        PartNumber = array[6].ToString(),
//                        Origin = orignvalue,
//                        OriginLabel = orignlable,
//                        Quantity = quantity,
//                        Unit = (int)LegalUnit.个, //单位默认个
//                        UnitLabel = LegalUnit.个.GetUnit().Code + " " + LegalUnit.个.GetDescription(),
//                        TotalPrice = totalPrice,
//                        UnitPrice = Math.Round(totalPrice / quantity, 4),
//                    };
//                    result.Add(model);
//                }
//            }
//            catch (Exception ex)
//            {

//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//            return base.JsonResult(VueMsgType.success, "", result.Json());
//        }

//        /// <summary>
//        /// 报关单提交
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DeclareSubmit(string data)
//        {
//            try
//            {
//                var model = JsonConvert.DeserializeObject<DeclareAddViewModel>(data);
//                var current = Client.Current;

//                DeclareOrderExtends order = string.IsNullOrWhiteSpace(model.ID) ? new DeclareOrderExtends() : current.MyDeclareOrder.GetOrderDetail(model.ID);

//                //订单
//                order.ID = model.ID;
//                order.ClientID = current.EnterpriseID;
//                order.CreatorID = current.ID;
//                order.Type = OrderType.Declare;
//                order.PayeeID = Yahv.PvWsOrder.Services.PvClientConfig.CompanyID;
//                order.BeneficiaryID = Yahv.Alls.Current.CompanyBeneficary.Account;  //公司的受益人
//                order.MainStatus = model.IsSubmit ? CgOrderStatus.已提交 : CgOrderStatus.暂存;
//                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
//                //order.ExecutionStatus = (WaybillType)int.Parse(model.HKDeliveryType) == WaybillType.PickUp ? ExcuteStatus.香港待提货 : ExcuteStatus.香港待入库;
//                order.SupplierID = model.SupplierID;
//                order.EnterCode = current.MyClients.EnterCode;
//                order.Summary = model.Summary;
//                order.PayExchangeSuppliers = model.PayExchangeSupplier;
//                order.SettlementCurrency = Currency.CNY;

//                #region 代收
//                //入库条件
//                PvWsOrder.Services.Models.OrderCondition orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsCharterBus = model.IsFullVehicle;
//                Services.Models.WayCondition wayCondition = new Services.Models.WayCondition();
//                wayCondition.AgencyPayment = model.IsLoan;
//                //入库
//                var orderInputs = new Yahv.PvWsOrder.Services.ClientModels.OrderInput();
//                orderInputs.IsPayCharge = model.IsLoan;
//                orderInputs.Conditions = orderCondition.Json();
//                orderInputs.Currency = (Currency)int.Parse(model.Currency);

//                order.Input = orderInputs;

//                //运单
//                var company = Alls.Current.Company[PvWsOrder.Services.PvClientConfig.ThirdCompanyID];
//                var contact1 = company?.Contacts.FirstOrDefault();
//                var supplier = current.MySupplier[model.SupplierID];
//                var pickAddress = supplier?.MySupplierAddress[model.SupplierAddress]; //提货地址
//                var packNO = string.IsNullOrWhiteSpace(model.PackNo) ? null : (int?)int.Parse(model.PackNo);
//                order.InWaybill = new Yahv.PvWsOrder.Services.ClientModels.Waybill()
//                {
//                    Consignee = new Yahv.Services.Models.WayParter
//                    {
//                        Company = company.Name,
//                        Address = company.RegAddress,
//                        Contact = contact1?.Name,
//                        Phone = contact1?.Tel ?? contact1?.Mobile,
//                        Email = contact1?.Email,
//                        Place = Origin.HKG.GetOrigin().Code,
//                    },
//                    Consignor = new Yahv.Services.Models.WayParter
//                    {
//                        Company = supplier.Name,
//                        Address = supplier.RegAddress,
//                        Contact = supplier.Contact?.Name,
//                        Phone = supplier.Contact?.Mobile,
//                        Email = supplier.Contact?.Email,
//                    },
//                    WayLoading = (WaybillType)int.Parse(model.HKDeliveryType) != WaybillType.PickUp ? null : new Services.Models.WayLoading
//                    {
//                        //ID = model.InLoadingID,
//                        TakingDate = DateTime.Parse(model.PickupTimeStr),
//                        TakingAddress = pickAddress?.Address,
//                        TakingContact = pickAddress?.Name,
//                        TakingPhone = pickAddress?.Mobile,
//                        CreatorID = current.ID,
//                        ModifierID = current.ID,
//                    },
//                    Code = model.WayBillNo,
//                    Type = (WaybillType)int.Parse(model.HKDeliveryType),
//                    FreightPayer = model.IsLoan ? WaybillPayer.Consignee : WaybillPayer.Consignor,
//                    TotalParts = packNO,
//                    TotalWeight = model.OrderItems.Sum(item => item.GrossWeight),
//                    CreatorID = current.ID,
//                    ModifierID = current.ID,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (WaybillType)int.Parse(model.HKDeliveryType) == WaybillType.PickUp ? (int)SortingExcuteStatus.WaitTake : (int)SortingExcuteStatus.PendingStorage,
//                    Supplier = supplier?.Name,
//                    Packaging = model.WrapType,
//                    EnterCode = current.MyClients.EnterCode,
//                    NoticeType = Services.Enums.CgNoticeType.Enter,
//                    Source = Services.Enums.CgNoticeSource.AgentBreakCustoms,

//                };
//                #endregion

//                #region 代发
//                //条件
//                orderCondition = new PvWsOrder.Services.Models.OrderCondition();
//                orderCondition.IsCharterBus = model.IsFullVehicle;

//                wayCondition = new Services.Models.WayCondition();
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
//                        //ID = model.OutLoadingID,
//                        TakingDate = DateTime.Now,
//                        TakingAddress = address,
//                        TakingContact = contact,
//                        TakingPhone = phone,
//                        CreatorID = current.ID,
//                        ModifierID = current.ID,
//                    },
//                    //ID = model.OutWaybillID,
//                    Type = (WaybillType)int.Parse(model.SZDeliveryType),
//                    FreightPayer = freightPayer,
//                    CreatorID = current.ID,
//                    ModifierID = current.ID,
//                    EnterCode = current.MyClients.EnterCode,
//                    Condition = wayCondition.Json(),
//                    ExcuteStatus = (int)PickingExcuteStatus.Waiting,
//                    Status = GeneralStatus.Closed,
//                    Packaging = model.WrapType,
//                    NoticeType = Services.Enums.CgNoticeType.Out,
//                    Source = Services.Enums.CgNoticeSource.AgentBreakCustoms,
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
//                    PreProductID = item.PreProcuctID,
//                    ID = item.ID,
//                    Name = item.Name,
//                    InputID = item.InputID,
//                    OutputID = item.OutputID,
//                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
//                    DateCode = item.DateCode,
//                    Quantity = item.Quantity,
//                    Currency = (Currency)int.Parse(model.Currency),
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
//                //提货文件
//                foreach (var item in model.HKFiles)
//                {
//                    var file = new Yahv.Services.Models.CenterFileDescription();
//                    file.Type = (int)FileType.Delivery;
//                    file.CustomName = item.name;
//                    file.Url = item.URL;
//                    file.AdminID = current.ID;
//                    files.Add(file);
//                }
//                order.OrderFiles = files.ToArray();
//                order.XDTClientName = current.XDTClientName;
//                order.Enter();
//                OperationLog(order.ID, "报关订单保存成功");
//                return base.JsonResult(VueMsgType.success, "新增成功", order.ID);
//            }
//            catch (Exception ex)
//            {
//                Client.Current.Errorlog.Log("报关订单保存失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 对账单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareBill()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            ViewBag.navid = paraArr[1];
//            var id = paraArr[0];
//            var current = Client.Current;
//            var order = current.MyXDTDecOrders.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
           
//            var orderItem = order.OrderItems.Select(item => new
//            {
//                item.ID,
//                item.Type,
//                ItemCategoryTypes = new string[0], //特殊类型
//                Model = item.Product.PartNumber,  //型号
//                Name = item.ClassfiedName,
//                item.Product.Manufacturer,
//                item.Quantity,
//                item.UnitPrice,
//                item.TotalPrice,
//                item.TraiffRate,
//                DeclareValue = item.DeclareTotalPrice,  //报关货值
//                Traiff = item.Traiff,     //关税
//                AddTax = item.AddTax,    //增值税
//                AgencyFee = item.AgencyFee,  //代理费
//                InspectionFee = item.InspectionFee,
//                TotalTaxFee = item.Traiff + item.AddTax + item.AgencyFee + item.InspectionFee,  //税费合计
//                TotalDeclareValue = item.Traiff + item.AddTax + item.AgencyFee + item.InspectionFee + item.DeclareTotalPrice, //报关总金额
//                CustomsExchangeRate=item.CustomsExchangeRate,
//                item.RealExchangeRate,
//            });
//            var bill = order.OrderFiles.Where(item => item.Type == (int)FileType.OrderBill).FirstOrDefault();
//            var bill_name = "";
//            var bill_url = "";
//            var isChecked = false;
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            if (bill != null)
//            {
//                bill_url = fileurl + bill.Url.ToUrl();
//                bill_name = bill.CustomName;
//                isChecked = bill.Status == Services.Models.FileDescriptionStatus.Approved; //是否审批通过
//            }
//            var purchaser = PurchaserContext.Current;
//            var vendor = VendorContext.Current;
//            var wayCondition = JsonConvert.DeserializeObject<Services.Models.WayCondition>(order.InWaybill.Condition);

//            //费用总价计算
//            var totalentity = new
//            {
//                Quantity = orderItem.Sum(c => c.Quantity),
//                TotalPrice = orderItem.Sum(c => c.TotalPrice),
//                DeclareValue = orderItem.Sum(c => c.DeclareValue),
//                Traiff = orderItem.Sum(c => c.Traiff),
//                AddTax = orderItem.Sum(c => c.AddTax),
//                AgencyFee = orderItem.Sum(c => c.AgencyFee).Round(),
//                InspectionFee = orderItem.Sum(c => c.InspectionFee).Round(),
//            };
//            var billlist = current.MyVouchers.Where(item => item.OrderID == order.ID && item.Catalog != "货款");
//            var totalRight = billlist.Sum(item => (decimal?)item.RightPrice.Value).GetValueOrDefault();//已付金额
//            var totalleft = billlist.Sum(item => (decimal)item.LeftPrice);//未付金额
//            var unPay = totalleft - totalRight;
//            #region 获取可用信用额度
//            //查看账单是否逾期
//            bool IsOverDue = Payments.PaymentManager.Npc[PvClientConfig.CompanyID, current.EnterpriseID][Payments.ConductConsts.代报关]
//                .DebtTerm[DateTime.Now].IsOverdue;
//            var isPay = false; //是否可以信用支付
//            if (!IsOverDue)
//            {
//                //查询当前币种可用额度列表
//                var creditList = current.MyCredits.Where(item => item.Currency ==  Currency.CNY).Select(item => new
//                {
//                    item.Catalog,
//                    Left = item.Total - item.Cost,
//                });
//                var credit = new CreditAvailable
//                {
//                    TaxCredit = creditList.Where(item => item.Catalog == "税款").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                    AgentCredit = creditList.Where(item => item.Catalog == "代理费").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                    OtherCredit = creditList.Where(item => item.Catalog == "杂费").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                };
//                if((totalentity.Traiff + totalentity.AddTax)<= credit.TaxCredit && totalentity.AgencyFee <= credit.AgentCredit && totalentity.InspectionFee <= credit.OtherCredit)
//                {
//                    isPay = true; //满足信用支付额度
//                }
//            }
           
//            #endregion
//            var data = new
//            {
//                OrderItem = orderItem,
//                Products_Num = totalentity.Quantity,
//                Products_TotalPrice = totalentity.TotalPrice,
//                Products_DeclareValue = totalentity.DeclareValue,
//                Products_Traiff = totalentity.Traiff,
//                Products_AddTax = totalentity.AddTax,
//                Products_AgencyFee = totalentity.AgencyFee,
//                Products_InspectionFee = totalentity.InspectionFee,
//                Products_TotalTaxFee = totalentity.Traiff + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee,
//                Products_TotalDeclareValue = totalentity.Traiff + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee + totalentity.DeclareValue,
//                Bill_URL = bill_url,
//                Bill_Name = bill_name,
//                IsChecked = isChecked,
//                order.Type,
//                order.ID,
//                AgentName = purchaser.CompanyName,
//                AgentAddress = purchaser.Address,
//                AgentTel = purchaser.Tel,
//                AgentFax = purchaser.UseOrgPersonTel,
//                Purchaser = purchaser.CompanyName,
//                Bank = purchaser.BankName,
//                Account = purchaser.AccountName,
//                AccountId = purchaser.AccountId,
//                CurrencyCode = order.Output?.Currency.ToString(),
//                IsLoan = wayCondition.AgencyPayment,
//                DueDate = Client.Current.MyAgreement.GetDueDate().ToString("yyyy年MM月dd日"),
//                CustomsExchangeRate= orderItem.First().CustomsExchangeRate,
//                RealExchangeRate=orderItem.First().RealExchangeRate,
//                TotalRight= totalRight,
//                IsPay= (unPay > 0)&&isPay,
//            };
//            return View(data);
//        }

//        /// <summary>
//        /// 信用支付
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult PayBill(string id)
//        {
//            try
//            {
//                var user = Client.Current;
//                var bills = user.MyVouchers.Where(item => item.OrderID == id && item.Catalog != "货款").ToArray();

//                var receiveids = bills.Select(item => item.ReceivableID).ToArray();
//                PaymentManager.Site(Client.Current.ID).Credit.For(receiveids).Pay(Currency.CNY, bills.Sum(item => item.LeftPrice - item.RightPrice).Value);

//                OperationLog(id, "信用支付成功");
//                return base.JsonResult(VueMsgType.success, "信用支付成功");
//            }
//            catch (Exception ex)
//            {
//                ErrorOperationLog(id, "信用支付失败" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "信用支付失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 订单跟踪
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetTrace(string id)
//        {
//            id = id.InputText();

//            var order = Client.Current.MyXDTOrder.Where(c => c.MainOrderID == id).FirstOrDefault();
//            if (order == null)
//            {
//                return base.JsonResult(VueMsgType.error, "订单状态异常");
//            }

//            var log = Yahv.Alls.Current.XDTOrderTraces.Where(c => c.OrderID == id).Select(item => new
//            {
//                Step = item.Step.GetDescription(),
//                Date = item.CreateDate.ToString("yyyy-MM-dd") + "/" + item.CreateDate.GetWeekName(),
//                Time = item.CreateDate.ToString("HH:mm:ss"),
//                Summary = item.Summary,
//                isCompleted = order.OrderStatus == OrderStatus.Completed,
//                isDot = false
//            });
//            return base.JsonResult(VueMsgType.success, "", log.Json());
//        }
//        #endregion

//        #region 待确认
//        /// <summary>
//        /// 待确认
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult PreConfirmList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 获取待确认订单列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetPreConfirmList()
//        {
//            var order = Client.Current.MyConfirmOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                TotalMoney = string.Format("{0:N}", item.OrderItems.Sum(c => c.UnitPrice * c.Quantity)),
//                MainStatus = item.MainStatus.GetDescription(),
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Output.Currency?.GetDescription(),
//                Addressee = item.OutWaybill.Consignee.Contact,
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                Type = item.Type,
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 客户确认页面
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareConfirm(string id)
//        {
//            id = id.InputText();
//            var order = Client.Current.MyConfirmOrder.GetOrderDetail(id);
//            if (order == null)
//            {
//                return View("Error");
//            }
//            //代收自提文件
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).FirstOrDefault();
//            var _deliveryFile = "";
//            if (deliveryFile != null)
//            {
//                _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
//            }
//            //代收合同发票
//            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
//            {
//                Name = item.CustomName,
//                Url = fileurl + item.Url,
//            }).ToArray();
//            //代理报关委托书

//            //var AgentFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.AgentTrustInstrument).Select(item => new
//            //{
//            //    Name = item.CustomName,
//            //    Url = fileurl + item.Url,
//            //    Status=item.Status
//            //}).ToArray();
//            //bool IsUploadAgentFile = AgentFiles.FirstOrDefault()==null?false: AgentFiles.FirstOrDefault().Status== Services.Models.FileDescriptionStatus.Approved; //是否审批通过代理报关委托书
//            //
//            //报关委托书
//            var orderAgentURL = "";
//            var orderAgentName = "";
//            var orderAgentStatus = false;
//            var orderAgent = order.OrderFiles.Where(item => item.Type == (int)FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
//            if (orderAgent != null)
//            {
//                orderAgentURL = fileurl + orderAgent.Url;
//                orderAgentName = orderAgent.CustomName;
//                orderAgentStatus = orderAgent.Status == Services.Models.FileDescriptionStatus.Approved;
//            }
//            //开票信息
//            //var invoice = order.Invoice;
//            //var _invoice = new InvoiceViewModel
//            //{
//            //    ID = invoice.ID,
//            //    Type = invoice.Type.GetDescription(),
//            //    DeliveryType = invoice.DeliveryType.GetDescription(),
//            //    CompanyName = invoice.Name,
//            //    TaxperNumber = invoice.TaxperNumber,
//            //    RegAddress = invoice.RegAddress,
//            //    CompanyTel = invoice.CompanyTel,
//            //    Bank = invoice.Bank,
//            //    Account = invoice.Account
//            //};
//            var orderItem = order.OrderItems.Select(item => new
//            {
//                item.ID,
//                item.Type,
//                ItemCategoryTypes = new string[0], //特殊类型
//                Model = item.Product.PartNumber,  //型号
//                Name = item.ClassfiedName,
//                item.Product.Manufacturer,
//                item.Quantity,
//                item.UnitPrice,
//                item.TotalPrice,
//                DeclareValue = item.DeclareTotalPrice,  //报关货值
//                TraiffRate = item.TraiffRate,    //关税率
//                Traiff = item.Traiff,     //关税
//                AddTax = item.AddTax,    //增值税
//                item.AddTaxRate,
//                AgencyFee = item.AgencyFee,  //代理费
//                InspectionFee = item.InspectionFee,//杂费
//                TotalTaxFee = item.Traiff + item.AddTax + item.AgencyFee + item.InspectionFee,  //税费合计
//                TotalDeclareValue = item.Traiff + item.AddTax + item.AgencyFee + item.InspectionFee + item.DeclareTotalPrice, //报关总金额
//            });
//            var inOrderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Output.Conditions);

//            //费用总价计算
//            var totalentity = new
//            {
//                Quantity = orderItem.Sum(c => c.Quantity),
//                TotalPrice = orderItem.Sum(c => c.TotalPrice),
//                DeclareValue = orderItem.Sum(c => c.DeclareValue),
//                Traiff = orderItem.Sum(c => c.Traiff),
//                AddTax = orderItem.Sum(c => c.AddTax),
//                AgencyFee = orderItem.Sum(c => c.AgencyFee).Round(),
//                InspectionFee = orderItem.Sum(c => c.InspectionFee).Round(),
//            };

//            var data = new
//            {
//                OrderItem = orderItem,
//                Products_Num = totalentity.Quantity,
//                Products_TotalPrice = totalentity.TotalPrice,
//                Products_DeclareValue = totalentity.DeclareValue,
//                Products_Traiff = totalentity.Traiff,
//                Products_AddTax = totalentity.AddTax,
//                Products_AgencyFee = totalentity.AgencyFee,
//                Products_InspectionFee = totalentity.InspectionFee,
//                Products_TotalTaxFee = totalentity.Traiff + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee,
//                Products_TotalDeclareValue = totalentity.Traiff + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee + totalentity.DeclareValue,
//                ID = order.ID,
//                order.Type,
//                MainStatus = order.MainStatus.GetDescription(),
//                CreateDate = order.CreateDate.ToString("yyyy-MM-dd"),
//                Currency = order.Output?.Currency.GetDescription(),
//                CurrencyCode = order.Output?.Currency.ToString(),
//                IsPayCharge = Convert.ToBoolean(order.Input?.IsPayCharge) ? "是" : "否",
//                HKDeliveryTypeName = order.InWaybill?.Type.GetDescription(),
//                HKDeliveryType = order.InWaybill?.Type,
//                SupplierName = order.SupplierName,
//                TakingDetailAddress = order.InWaybill?.WayLoading?.TakingAddress,
//                TakingDate = order.InWaybill?.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
//                TakingContact = order.InWaybill?.WayLoading?.TakingContact,
//                TakingPhone = order.InWaybill?.WayLoading?.TakingPhone,
//                DeliveryFile = _deliveryFile,
//                PIFiles,
//                //AgentFiles,
//                //IsUploadAgentFile,
//                OrderAgentURL = orderAgentURL,
//                OrderAgentName = orderAgentName,
//                OrderAgentStatus = orderAgentStatus,
//                SZDeliveryTypeName = order.OutWaybill.Type.GetDescription(),
//                SZDeliveryType = order.OutWaybill.Type,
//                OutTakingContact = order.OutWaybill.WayLoading?.TakingContact,
//                OutTakingPhone = order.OutWaybill.WayLoading?.TakingPhone,
//                CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription(),
//                Certificate = order.OutWaybill.Consignee?.IDNumber,
//                ReceivedContact = order.OutWaybill.Consignee.Contact,
//                ReceivedPhone = order.OutWaybill.Consignee.Phone,
//                ReceivedAddress = order.OutWaybill.Consignee.Address,
//                IsFullVehicle = inOrderCondition.IsCharterBus ? "是" : "否",
//                TotalParts = (order.InWaybill?.TotalParts).GetValueOrDefault(),
//                order.Summary,
//                WaybillCode = order.InWaybill?.Code,
//                //Packaging = ((Package)int.Parse(order.InWaybill?.Packaging)).GetDescription(),
//                TotalMoney = string.Format("{0:N}", order.OrderItems.Sum(c => c.TotalPrice)),
//                // Invoice = _invoice,
//                PayExchangeSupplier = order.PayExchangeSupplier.Select(c => c.Name).ToArray(),
//                Packaging = ((Package)int.Parse(order.InWaybill.Packaging)).GetDescription(),
//            };
//            return View(data);
//        }

//        /// <summary>
//        /// 待客户确认取消订单
//        /// </summary>
//        /// <param name="orderID">订单ID</param>
//        /// <param name="reason">取消原因</param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CancelConfirm(string orderID, string reason)
//        {
//            try
//            {
//                Client.Current.MyConfirmOrder.ClientCancel(orderID, reason);
//                Payments.PaymentManager.Erp(Client.Current.ID).Received.Abolish(orderID);
//                OperationLog(orderID, "订单取消成功");
//                return base.JsonResult(VueMsgType.success, "取消成功");
//            }
//            catch (Exception ex)
//            {
//                ErrorOperationLog(orderID, "订单取消失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "取消失败");
//            }
//        }

//        /// <summary>
//        /// POST:订单确认
//        /// </summary>
//        /// <param name="orderID"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult CheckPreConfirm(string orderID, string URL, string name, string fileFormat)
//        {
//            try
//            {
//                var client = Client.Current;
//                client.MyConfirmOrder.ClientConfirm(orderID);
//                if (!string.IsNullOrWhiteSpace(URL))
//                {
//                    //订单附件
//                    List<Services.Models.CenterFileDescription> files = new List<Services.Models.CenterFileDescription>();
//                    //代理报关委托书保存
//                    var file = new Services.Models.CenterFileDescription();
//                    file.WsOrderID = orderID;
//                    file.Type = (int)FileType.AgentTrustInstrument;
//                    file.CustomName = name;
//                    file.Url = URL;
//                    file.AdminID = client.ID;
//                    files.Add(file);
//                    new PvWsOrder.Services.ClientViews.CenterFilesView().XDTUpload(files.ToArray());
//                }
//                OperationLog(orderID, "订单确认成功");
//                return base.JsonResult(VueMsgType.success, "订单确认成功");
//            }
//            catch (Exception ex)
//            {
//                ErrorOperationLog(orderID, "订单确认失败：" + ex.Message);
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }

//        #endregion

//        #region 报关委托书,对账单上传、导出
//        /// <summary>
//        /// 报关委托书,对账单上传

//        /// </summary>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult UploadDeclareFile(HttpPostedFileBase file)
//        {
//            //拿到上传来的文件
//            file = Request.Files["file"];
//            var orderID = Request.Form["orderID"].ToString();
//            var type = Request.Form["type"].ToString();
//            var FileTypes = (FileType)int.Parse(type);


//            //后台也要校验
//            string fileFormat = Path.GetExtension(file.FileName).ToLower();
//            var fileSize = file.ContentLength / 1024;
//            if (fileSize > 1024 * 3 && fileFormat == ".pdf")
//            {
//                return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
//            }
//            string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
//            if (allowFiles.Contains(fileFormat) == false)
//            {
//                return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片或者pdf文件！");
//            }
//            var result = Yahv.Alls.Current.centerFiles.fileSave(file);
//            var fullUrl = PvWsOrder.Services.PvClientConfig.DomainUrl + @"/" + result;//文件全路径,是取网站地址还是绝对路径,杨樱前端决定


//            var UserID = Client.Current.ID;
//            //中心文件保存
//            var centerfile = new Yahv.Services.Models.CenterFileDescription()
//            {
//                WsOrderID = orderID,
//                Type = (int)FileTypes,
//                CustomName = file.FileName,
//                Url = result,
//                AdminID = UserID,
//            };
//            new PvWsOrder.Services.ClientViews.CenterFilesView().XDTUpload(centerfile);

//            return base.JsonResult(VueMsgType.success, "", new { name = file.FileName, URL = result, fullURL = fullUrl, fileFormat = file.ContentType }.Json());
//        }


//        /// <summary>
//        /// 导出对账单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ExportBill(string id)
//        {
//            try
//            {
//                id = id.InputText();
//                var current = Client.Current;

//                //获取报关委托书数据
//                var orderbillProxy = current.MyOrder.GetOrderBillProxy(id);
//                //创建文件目录
//                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
//                file.SetChildFolder(SysConfig.Dowload);
//                file.CreateDateDirectory();
//                orderbillProxy.DueDate = current.MyAgreement.GetDueDate();
//                orderbillProxy.SaveAs(file.FilePath);
//                var url = file.DomainUrl + @"/" + file.RootFileFolder + file.LocalFileUrl;
//                return base.JsonResult(VueMsgType.success, "", url);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }

//        /// <summary>
//        /// 导出代理报关委托书
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ExportAgent(string id)
//        {
//            try
//            {
//                id = id.InputText();
//                var current = Client.Current;

//                //获取报关委托书数据
//                var orderAgentProxy = current.MyOrder.GetOrderAgentProxy(id);
//                //创建文件目录
//                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
//                file.SetChildFolder(SysConfig.Dowload);
//                file.CreateDateDirectory();

//                orderAgentProxy.SaveAs(file.FilePath);
//                var url = file.DomainUrl + @"/" + file.RootFileFolder + file.LocalFileUrl;
//                return base.JsonResult(VueMsgType.success, "", url);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }
//        #endregion

//        #region 发票
//        /// <summary>
//        /// 发票信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DeclareInvoice()
//        {
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
//            var orderDetail = Yahv.Alls.Current.XDTInvoice.GetDetailByOrderID(id).Select(item => new
//            {
//                InvoiceTypeName = item.InvoiceType.GetDescription(),
//                item.InvoiceType,
//                item.TaxName,
//                item.Amount,
//                InvoiceNoticeStatus = item.InvoiceNoticeStatus.GetDescription(),
//            }).ToArray();
//            ////合同发票
//            //var invoiceUrl = "";
//            //var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            //var invoiceFile = order.OrderFiles?.Where(item => item.Type == (int)FileType.Invoice).FirstOrDefault();
//            //if (invoiceFile != null)
//            //{
//            //    invoiceUrl = fileurl + invoiceFile.Url.ToUrl();
//            //}
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
//                //FileUrl = invoiceUrl,
//                orderDetail,
//            };
//            return View(data);
//        }
//        #endregion

//        #region 待收货
//        /// <summary>
//        /// 待收货
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult UnReceivedList()
//        {
//            return View();
//        }

//        /// <summary>
//        /// 待收货订单列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetUnReceivedList()
//        {
//            var order = Client.Current.UnReceivedOrders;
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//          //  var list = order.GetUnReceivedOrder(rows, page);
//            Func<PvWsOrder.Services.ClientModels.OrderExtends, object> convert = item => new
//            {
//                ID = item.ID,
//                WayBillID = item.InWaybill.ID,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                WayBillCreateDate = item.InWaybill.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Type = item.InWaybill.Type.GetDescription(),
//                item.InWaybill.Consignee.Contact,
//                item.InWaybill.Consignee.Company,
//            };
//            #endregion
//            // return this.Paging(list, list.Total, convert);
//            return null;
//        }

//        /// <summary>
//        /// 确认待收货
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ConfirmReceived(string wayBillID)
//        {
//            var Waybill = new Yahv.PvWsOrder.Services.ClientViews.WayBillAlls()[wayBillID];

//            if (Waybill == null)
//            {
//                return base.JsonResult(VueMsgType.error, "该运单不存在！");
//            }

//            Waybill.ConfirmReceipt();

//            return base.JsonResult(VueMsgType.success, "订单确认收货成功");
//        }
//        #endregion
//    }
//}