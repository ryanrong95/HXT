//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.Linq.Extends;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsOrder.Services.XDTClientView;
//using Yahv.Services.Models;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;

//namespace Yahv.PvWsClient.WebApp.Controllers
//{
//    /// <summary>
//    /// 付汇
//    /// </summary>
//    public class PayExchangeController : UserController
//    {
//        #region  待付汇

//        /// <summary>
//        /// 待付汇
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult UnPayExchange()
//        {
//            //币种列表
//            var list = Client.Current.MyUnPayExchangeOrder.Select(item => new { text = item.Currency }).Distinct().ToArray();
//            ViewBag.CurrencyOptions = list;
//            return View();
//        }

//        /// <summary>
//        /// 获取待付汇数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetUnPayExchangeList()
//        {
//            var order = Client.Current.MyUnPayExchangeOrder;

//            var currency = Request.Form["currency"];  //币种
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<UnPayExchangeOrder, bool>> expression = item => true;
//            if (!string.IsNullOrWhiteSpace(currency) && currency != "all")
//            {
//                Expression<Func<UnPayExchangeOrder, bool>> exp = item => item.Currency == currency;
//                expression = expression.And(exp);
//            }
//            lambdas.Add(expression);
//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = order.GetPageListData(lambdas.ToArray(), rows, page);
//            Func<PvWsOrder.Services.XDTClientView.UnPayExchangeOrder, object> convert = item => new
//            {
//                item.ID,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                item.Currency,
//                item.DeclarePrice,
//                PaySuppliers = item.PayExchangeSuppliers.Select(c => c.ClientSupplierID).ToArray(),
//                PaySuppliersName = item.PayExchangeSuppliers.Select(c => c.ChineseName).ToArray(),
//                OrderStatus = item.OrderStatus.GetDescription(),
//                isDelete = item.OrderStatus == OrderStatus.Draft ? true : false,
//                InvoiceStatus = item.InvoiceStatus.GetDescription(),
//                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
//                PayExchangeType = item.IsPrePayExchange ? "预换汇" : "90天内换汇",
//                IsPrePayExchange = item.IsPrePayExchange,
//                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToString("0.00"),
//                Remittanced = item.PaidExchangeAmount.ToString("0.00"),
//                IsCheck = false,
//                isLoading = true,
//                DeclareDate = item.DeclareDate,
//                MainOrderID = item.MainOrderID,
//                MainOrderCreateDate = item.MainOrderCreateDate.ToString("yyyy-MM-dd HH:mm"),
//            };
//            #endregion
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 根据订单ID跳转不同订单详情
//        /// </summary>
//        /// <returns></returns>
//        public JsonResult ReturnDetail()
//        {
//            var id = Request.Form["id"];
//            if (string.IsNullOrWhiteSpace(id))
//            {
//                return base.JsonResult(VueMsgType.error, "跳转失败");
//            }
//            var order = Client.Current.MyOrder[id];
//            if (order == null)
//            {
//                return base.JsonResult(VueMsgType.error, "跳转失败");
//            }
//            return base.JsonResult(VueMsgType.success, "", ((int)order.Type).ToString());
//        }

//        /// <summary>
//        /// 付汇申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Apply()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            //id数组
//            var ids = para.Split(',');

//            var current = Client.Current;
//            var orders = current.MyUnPayExchangeOrder;

//            //获取id数据的订单列表
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<UnPayExchangeOrder, bool>> exp = item => ids.Contains(item.ID);
//            lambdas.Add(exp);
//            var list = orders.GetOrdersByExceptions(lambdas.ToArray());
//            if (list.Count() != ids.Length || ids.Length == 0)
//            {
//                return View("Error");
//            }

//            ApplyViewModel model = new ApplyViewModel();
//            model.PayExchangeApplyFiles = new List<FileModel>();
//            //取供应商交集
//            var first_list = list.FirstOrDefault();
//            IEnumerable<string> supplier = first_list.PayExchangeSuppliers.Select(item => item.ClientSupplierID);
//            foreach (var order in list)
//            {
//                //获取订单中的供应商交集
//                supplier = supplier.Intersect(order.PayExchangeSuppliers.Select(item => item.ClientSupplierID));
//            }

//            // var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.XDTFileServerUrl + @"/";
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            //如果选的订单的mainorde 是一样的，则文件只要遍历一遍就可以了
//            var mainOrderIDS = list.Select(c => c.MainOrderID).Distinct();
//            foreach (var orderid in mainOrderIDS)
//            {
//                var order = current.MyOrder.GetOrderDetail(orderid);
//                //获取文件
//                var files = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice);
//                foreach (var file in files)
//                {
//                    var item = new FileModel
//                    {
//                        ID = file.ID,
//                        name = file.CustomName,
//                        //fileFormat = file.FileFormat,
//                        URL = file.Url,
//                        fullURL = fileurl + file.Url
//                    };
//                    model.PayExchangeApplyFiles.Add(item);
//                }
//            }
//            model.PayExchangeApplyFiles = model.PayExchangeApplyFiles.DistinctBy(t => t.name).ToList();


//            //供应商数据源
//            ViewBag.SupplierOptions = Yahv.Alls.Current.Supplier.Where(item => supplier.Contains(item.ID)).Select(item => new
//            {
//                ID = item.ID,
//                ChineseName = item.ChineseName,
//                EnglishName = item.Name,
//            }).ToArray();

//            model.Rate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode(first_list.Currency).Rate; //实时汇率
//            model.TotalDeclareMoney = list.Sum(item => item.DeclarePrice).ToRound(2);  //报关总金额
//            model.RateType = ExchangeRateType.RealTime.GetDescription(); //汇率类型
//            model.RateTypeCode = ExchangeRateType.RealTime;
//            model.SettlementDate = DateTime.Now.ToString("yyyy年MM月dd日");//付汇的结算日期

//            var currency = Yahv.Alls.Current.Currency.FindByCode(first_list.Currency);
//            model.Currency = currency?.Name;//币种
//            model.CurrencyCode = currency?.Code;


//            //付汇订单列表
//            var payOrders = list.Select(item => new PayExchangeApplyOrderViewModel
//            {
//                ID = item.ID,
//                Currency = model.CurrencyCode,
//                DeclarePrice = item.DeclarePrice.ToRound(2),
//                PaidExchangeAmount = item.PaidExchangeAmount.ToRound(2),
//                CurrentPaidAmount = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
//                PaidAmount = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
//                IsMatchSupplier = false, //是否匹配到供应商(默认没有匹配到，因为 Apply 页面进入时，供应商名称下拉框默认是不选择的)
//            }).ToList();

//            model.TotalMoney = (model.Rate * payOrders.Sum(item => item.CurrentPaidAmount)).ToRound(2);
//            model.UnPayExchangeOrders = payOrders;
//            //付款方式
//            ViewBag.PayTypeOptions = ExtendsEnum.ToDictionary<PaymentType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();

//            var purchaser = PvWsOrder.Services.PurchaserContext.Current;
//            model.AgentName = purchaser.CompanyName;
//            model.Account = purchaser.BankName;
//            model.AccountID = purchaser.AccountId;

//            return View(model);
//        }

//        /// <summary>
//        /// 获取付汇记录
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetPayRecord(string id)
//        {
//            id = id.InputText();

//            var view = Client.Current.MyPayExchangeApplies.GetAppliesByOrderID(id).ToList().OrderByDescending(item=>item.CreateDate).Select(item => new
//            {
//                SupplierName = item.SupplierName,
//                ApplyTime = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Amount = item.Amount,
//                Status = item.PayExchangeApplyStatus.GetDescription()
//            });
//            return base.JsonResult(VueMsgType.success, "", view.Json());
//        }

//        /// <summary>
//        /// 获取供应商银行+地址、订单本次申请金额
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetSupplierBankAndAddress(SupplierBankAndAddressRequestModel data)
//        {
//            string supplierID = data.SupplierID.InputText();

//            var current = Client.Current;

//            JsonResult json = new JsonResult();
//            #region 获取供应商银行+地址

//            var supplier = Yahv.Alls.Current.Supplier[supplierID];
//            if (supplier == null)
//            {
//                return base.JsonResult(VueMsgType.error, "供应商不存在。");
//            }

//            var banks = Alls.Current.SupplierBank.SearchBySupplierID(supplier.ID).Select(item => new
//            {
//                value = item.ID,
//                text = item.BankName,
//                address = item.BankAddress,
//                account = item.BankAccount,
//                code = item.SwiftCode
//            });

//            var address = Alls.Current.SupplierAddresses.SearchBySupplierID(supplier.ID).Select(item => new
//            {
//                value = item.Address,
//                text = item.Address,
//            });

//            #endregion

//            #region 订单本次申请金额

//            string[] orderIds = data.OrderIDs;

//            for (int i = 0; i < orderIds.Length; i++)
//            {
//                orderIds[i] = orderIds[i].InputText();
//            }

//            //获取id数据的订单列表
//            var orders = current.MyUnPayExchangeOrder;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<UnPayExchangeOrder, bool>> exp = item => orderIds.Contains(item.ID);
//            lambdas.Add(exp);
//            var list = orders.GetOrdersByExceptions(lambdas.ToArray());


//            List<PvWsOrder.Services.XDTModels.OrderCurrentPayAmount> payAmounts = new List<PvWsOrder.Services.XDTModels.OrderCurrentPayAmount>();

//            foreach (var order in list)
//            {
//                var thisOrderCurrentPayAmount = order.OrderCurrentPayAmount(supplier.ID).FirstOrDefault();
//                if (thisOrderCurrentPayAmount != null)
//                {
//                    payAmounts.Add(thisOrderCurrentPayAmount);
//                }
//            }

//            var currentPayAmounts = (from orderID in orderIds
//                                     join payAmount in payAmounts on orderID equals payAmount.ID into payAmounts2
//                                     from payAmount in payAmounts2.DefaultIfEmpty()
//                                     select new
//                                     {
//                                         OrderID = orderID,
//                                         IsMatchSupplier = payAmount != null,
//                                         CurrentPaidAmount = payAmount != null ? payAmount.CurrentPaidAmount.ToRound(2) : 0
//                                     });
//            #endregion

//            json = new JsonResult
//            {
//                Data = new
//                {
//                    banks = banks,
//                    address = address,
//                    currentPayAmounts = currentPayAmounts,
//                }
//            };

//            return base.JsonResult(VueMsgType.success, "", json.Json());
//        }

//        /// <summary>
//        /// 检查供应商银行敏感性
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckSupplierBankSensitive(string address)
//        {
//            if (string.IsNullOrEmpty(address))
//            {
//                return base.JsonResult(VueMsgType.success, "", new JsonResult
//                {
//                    Data = new
//                    {
//                        IsSensitive = false,
//                    }
//                }.Json());
//            }

//            address = address.Trim();
//            var splittedStrs = Split(address);

//            splittedStrs = splittedStrs.Distinct().ToArray();
//            splittedStrs = splittedStrs.Where(t => t.Length != 1).ToArray();


//            int pageSize = 1000;
//            int pageCount = (splittedStrs.Length / pageSize) + (splittedStrs.Length % pageSize > 0 ? 1 : 0);

//            bool isSensitive = false;

//            for (int i = 1; i <= pageCount; i++)
//            {
//                var theSplittedStrs = splittedStrs.Skip(pageSize * (i - 1)).Take(pageSize);
//                Expression<Func<PayExchangeSensitiveWordCheckModel, bool>> predicate = item => false;
//                foreach (var splittedStr in theSplittedStrs)
//                {
//                    predicate = predicate.Or(t => t.WordContent == splittedStr);
//                }

//                var payExchangeSensitiveWordCheckView = Yahv.Alls.Current.PayExchangeSensitiveWordCheck.GetDataByAreaType(PayExchangeSensitiveAreaType.Forbid).Where(predicate);
//                var oneSensitiveArea = payExchangeSensitiveWordCheckView.FirstOrDefault();

//                if (oneSensitiveArea == null)
//                {
//                    isSensitive = false;
//                }
//                else
//                {
//                    isSensitive = true;
//                    break;
//                }
//            }

//            return base.JsonResult(VueMsgType.success, "", new JsonResult
//            {
//                Data = new
//                {
//                    IsSensitive = isSensitive,
//                }
//            }.Json());
//        }

//        /// <summary>
//        /// 付汇申请提交
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult SubmitApply(ApplyViewModel model)
//        {
//            try
//            {
//                var current = Client.Current;
//                //验证
//                foreach (var item in model.UnPayExchangeOrders)
//                {
//                    var order = current.MyUnPayExchangeOrder[item.ID];
//                    if (order == null)
//                    {
//                        return base.JsonResult(VueMsgType.error, "订单[" + item.ID + "]状态异常，请重新刷新页面.");
//                    }
//                    else if ((order.DeclarePrice.ToRound(2) - order.PaidExchangeAmount) < item.CurrentPaidAmount)
//                    {
//                        return base.JsonResult(VueMsgType.error, "订单[" + item.ID + "]申请付汇金额不能大于可申请付汇金额.");
//                    }
//                }
//                PvWsOrder.Services.XDTModels.PayExchangeApply pay = new PvWsOrder.Services.XDTModels.PayExchangeApply();
//                pay.ClientID = current.XDTClientID;
//                pay.UserID = current.ID; 
//                pay.SupplierName = model.SupplierName;
//                pay.SupplierEnglishName = model.SupplierEnglishName;
//                pay.SupplierAddress = model.SupplierAddress;
//                pay.BankAccount = model.SupplierBankAccount;
//                pay.BankAddress = model.SupplierBankAddress;
//                pay.BankName = model.SupplierBankName;
//                pay.SwiftCode = model.SupplierBankCode;
//                pay.ExchangeRateType = model.RateTypeCode.ToString();
//                pay.Currency = model.CurrencyCode;
//                pay.ExchangeRate = model.Rate;
//                pay.PaymentType = model.PayType;
//                pay.ExpectPayDate = model.ExpectDate;
//                pay.SettlemenDate = model.SettlementDate;
//                pay.OtherInfo = model.Others;
//                pay.Summary = model.Summary;
//                pay.UnPayExchangeOrders = model.UnPayExchangeOrders.Select(item => new PvWsOrder.Services.XDTModels.UnPayExchangeOrderItem
//                {
//                    OrderID = item.ID,
//                    DeclarePrice = item.DeclarePrice,
//                    PaidExchangeAmount = item.PaidExchangeAmount,
//                    CurrentPaidAmount = item.CurrentPaidAmount
//                }).ToArray();

//                //pi文件明细
//                if (model.PayExchangeApplyFiles != null)
//                {
//                    pay.PayExchangeApplyFiles = model.PayExchangeApplyFiles.Select(file => new Services.Models.CenterFileDescription
//                    {
//                        ID=file.ID,
//                        AdminID = current.ID,
//                        ClientID = current.MyClients.ID,
//                        Url = file.URL,
//                        CustomName = file.name,
//                        Type = (int)FileType.PIFiles,
//                    }).ToArray();
//                }

//                pay.SubmitApply();
//                var response = pay.ResponseData;
//                if (response.success)
//                {
//                    return base.JsonResult(VueMsgType.success, "新增成功", response.data);
//                }
//                else
//                {
//                    return base.JsonResult(VueMsgType.error, "新增失败");
//                }

//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }

//        /// <summary>
//        /// 下载付汇委托书
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DownloadPayProxy(string id)
//        {
//            try
//            {
//                id = id.InputText();
//                var apply = Client.Current.MyPayExchangeApplies.GetDetailDataByID(id);
//                if (apply == null)
//                {
//                    return base.JsonResult(VueMsgType.error, "下载付汇委托书失败，付汇申请不存在。");
//                }

//                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
//                file.SetChildFolder(SysConfig.Dowload);
//                file.CreateDateDirectory();

//                var proxy = new Yahv.PvWsOrder.Services.XDTModels.PayExchangeAgentProxy(apply);
//                proxy.SaveAs(file.FilePath);

//                return base.JsonResult(VueMsgType.success, "", "/Files" + file.LocalFileUrl);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "", ex.Message);
//            }
//        }


//        /// <summary>
//        /// 导出付汇委托书
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult ExportPayProxy(string id)
//        {
//            try
//            {
//                id = id.InputText();
//                var applyextends = Client.Current.MyPayExchangeApplies.GetDetailDataByID(id);
//                if (applyextends == null)
//                {
//                    return base.JsonResult(VueMsgType.error, "下载付汇委托书失败，付汇申请已删除。");
//                }
//                string fileName = DateTime.Now.Ticks + ".pdf";

//                //使用流直接导出，不需要先写入硬盘
//                var proxy = new Yahv.PvWsOrder.Services.XDTModels.PayExchangeAgentProxy(applyextends);

//                var stream = proxy.ToPDF();


//                return null;
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "", ex.Message);
//            }
//        }

//        /// <summary>
//        /// 删除付汇申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DeleteApply(string id)
//        {
//            id = id.InputText();
//            var apply = Client.Current.MyPayExchangeApplies.GetDetailDataByID(id);
//            if (apply == null)
//            {
//                return base.JsonResult(VueMsgType.error, "删除失败");
//            }
//            apply.DeleteApply();
//            var response = apply.Apply.ResponseData;
//            if (response.success)
//            {
//                return base.JsonResult(VueMsgType.success, "删除成功");
//            }
//            else
//            {
//                return base.JsonResult(VueMsgType.error, "删除失败");
//            }
//        }

//        #region 切割字符串

//        static string[] Split(string origin)
//        {
//            List<string> resultStrs = new List<string>();

//            for (int i = 1; i <= origin.Length; i++)
//            {
//                var sdfsf = Split(origin, i);
//                resultStrs.AddRange(sdfsf);
//            }

//            return resultStrs.ToArray();
//        }

//        static string[] Split(string origin, int len)
//        {
//            List<string> resultStrs = new List<string>();

//            for (int i = 0; i <= origin.Length - len; i++)
//            {
//                resultStrs.Add(origin.Substring(i, len));
//            }

//            return resultStrs.ToArray();
//        }

//        #endregion


//        /// <summary>
//        /// 上传付汇委托书
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="file"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult SavePayProxy(string id, string filename, string ext, string url)
//        {
//            try
//            {
//                var user = Client.Current;

//                var apply = user.MyPayExchangeApplies[id];
//                if (apply.PayExchangeApplyStatus != PayExchangeApplyStatus.Auditing)
//                {
//                    return base.JsonResult(VueMsgType.error, "上传付汇委托书失败，付汇申请已审核完成。");
//                }

//                var file = new PayExchangeApplyFile();
//                file.PayExchangeApplyID = apply.ID;
//                file.FileFormat = ext;
//                file.Name = filename;
//                file.FileType = FileType.PayExchange;
//                file.Url = url;
//                file.UserID = user.ID;
//                file.ClientID = user.MyClients.ID;
//                file.Status = (int)FileDescriptionStatus.Normal;
//                file.Enter();

//                var n_apply = Client.Current.MyPayExchangeApplies.GetDetailDataByID(id);
//                var fileURL = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + n_apply.PayExchangeFile.Url.ToUrl();
//                return base.JsonResult(VueMsgType.success, "上传成功", fileURL);
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
//                string fileName = file.FileName.ReName();
//                HttpFile httpFile = new HttpFile(fileName);
//                httpFile.SetChildFolder(SysConfig.PayExchange);
//                httpFile.CreateDataDirectory();
//                string[] result = httpFile.SaveAs(file);
//                return base.JsonResult(VueMsgType.success, "", new { name = fileName, URL = result[1], fileFormat = fileFormat, fullURL = result[2] }.Json());
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }
//        #endregion

//    }
//}