using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Client.Services;
using Needs.Wl.Logs.Services;
using Needs.Wl.Models;
using Needs.Wl.User.Plat;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    public class PayExchangesController : UserController
    {

        public static string CacheApplyIds = "";

        #region 待付汇

        /// <summary>
        /// 待付汇
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult UnPayExchanges()
        {
            UnPayExchangesViewModel model = new UnPayExchangesViewModel();
            //币种列表
            var list = Needs.Wl.User.Plat.UserPlat.Current.MyUnPayExchangeOrders.GetPayCurrencies();
            model.CurrencyOptions = list.Select(item => new
            {
                text = item
            }).Distinct().Json();
            var data = new
            {
                CurrencyOptions = list.Select(item => new { text = item }).Distinct().ToArray(),
            };
            ViewBag.Options = data;
            return View(model);
        }

        /// <summary>
        /// 获取待付汇
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetUnPayExchanges()
        {
            var currency = Request.Form["currency"];  //币种
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyUnPayExchangeOrders;
            view.PageIndex = page;
            view.PageSize = rows;

            var predicate = Needs.Linq.PredicateBuilder.Create<Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder>();

            if ((!string.IsNullOrWhiteSpace(currency)) && currency != "all") //订单状态
            {
                predicate = predicate.And(item => item.Currency == currency);
            }
            view.Predicate = predicate;

            var orderlist = await view.ToListAsync();
            var list = orderlist.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Currency,
                item.DeclarePrice,
                PaySuppliers = item.PayExchangeSuppliers.Select(c => c.ClientSupplierID).ToArray(),
                PaySuppliersName = item.PayExchangeSuppliers.Select(c => c.ChineseName).ToArray(),
                OrderStatus = item.OrderStatus.GetDescription(),
                isDelete = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Draft ? true : false,
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                PayExchangeType = item.IsPrePayExchange ? "预换汇" : "90天内换汇",
                IsPrePayExchange = item.IsPrePayExchange,
                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2).ToString(),
                Remittanced = item.PaidExchangeAmount.ToRound(2).ToString(),
                IsCheck = false,
                isLoading = true,
                DeclareDate = item.DeclareDate,
                MainOrderID = item.MainOrderID,
                MainOrderCreateDate = item.MainOrderCreateDate.ToString("yyyy-MM-dd HH:mm"),
            });
            return JsonResult(VueMsgType.success, "", new { list, total = view.RecordCount }.Json());

        }




        #endregion

        #region 付汇申请

        /// <summary>
        /// 会员申请付汇
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Apply(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                id = CacheApplyIds;
            }
            else
            {
                CacheApplyIds = id;
            }

           // var ss = id.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] orderIds = id.InputText().Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//去掉null 或者空
            //var orderIds = JsonConvert.DeserializeObject<string[]>(id);
            var current = Needs.Wl.User.Plat.UserPlat.Current;

            ApplyViewModel model = new ApplyViewModel();

            var view = current.MyUnPayExchangeOrders;
            view.AllowPaging = false;
            view.Predicate = s => orderIds.Contains(s.ID);

            var firstOrder = view.FirstOrDefault();  //第一个订单
            var orders = view.ToList();

            if (view.RecordCount == 0 || (view.RecordCount != orderIds.Length) || orderIds.Length == 0)
            {
                return View("Error");  //传过来的ID与实际查询的ID不一致
            }

            model.PayExchangeApplyFiles = new List<FileModel>();

            //取第一个元素
            IEnumerable<string> supplier = firstOrder.PayExchangeSuppliers.Select(item => item.ClientSupplierID);

            List<string> mainOrderIDS = new List<string>();
            foreach (var order in orders)
            {
                //获取订单中的供应商交集
                supplier = supplier.Intersect(order.PayExchangeSuppliers.Select(item => item.ClientSupplierID));

                string[] ordernosplit = order.ID.Split('-');
                if (mainOrderIDS.IndexOf(ordernosplit[0]) < 0)
                {
                    mainOrderIDS.Add(ordernosplit[0]);
                }
            }
            //如果选的订单的mainorde 是一样的，则文件只要遍历一遍就可以了
            foreach (var orderid in mainOrderIDS)
            {
                var order = Needs.Wl.User.Plat.UserPlat.Current.MyMianOrders[orderid];
                //获取文件
                var files = order.Files().GetOriginalInvoice();
                foreach (var file in files)
                {
                    var item = new FileModel
                    {
                        name = file.Name,
                        fileFormat = file.FileFormat,
                        URL = file.Url,
                        fullURL = AppConfig.Current.FileServerUrl + @"/" + file.Url
                    };
                    model.PayExchangeApplyFiles.Add(item);
                }
            }

            model.PayExchangeApplyFiles = model.PayExchangeApplyFiles.DistinctBy(t => t.name).ToList();

            var suppliersView = current.Client.Suppliers();
            suppliersView.Predicate = item => supplier.Contains(item.ID);
            suppliersView.AllowPaging = false;

            //供应商数据源
            model.SupplierOptions = suppliersView.ToArray().Select(item => new
            {
                ID = item.ID,
                ChineseName = item.ChineseName,
                EnglishName = item.Name,
            }).Json();

            model.Rate = Needs.Wl.User.Plat.UserPlat.RealTimeExchangeRates.FindByCode(firstOrder.Currency).Rate; //实时汇率
            model.TotalDeclareMoney = orders.Sum(item => item.DeclarePrice).ToRound(2);  //报关总金额
            model.RateType = Needs.Wl.Models.Enums.ExchangeRateType.RealTime.GetDescription(); //汇率类型
            model.RateTypeCode = (Needs.Ccs.Services.Enums.ExchangeRateType)((int)Needs.Wl.Models.Enums.ExchangeRateType.RealTime);//TODO:临时转换，需要改写
            model.SettlementDate = DateTime.Now.ToString("yyyy年MM月dd日");//付汇的结算日期

            var currency = Needs.Wl.User.Plat.UserPlat.Currencies.FindByCode(firstOrder.Currency);
            if (currency != null)
            {
                model.Currency = currency.Name;//币种
                model.CurrencyCode = currency.Code;
            }

            //付汇订单列表
            var payOrders = orders.Select(item => new PayExchangeApplyOrderViewModel
            {
                ID = item.ID,
                Currency = model.CurrencyCode,
                DeclarePrice = item.DeclarePrice,
                PaidExchangeAmount = item.PaidExchangeAmount.ToRound(2),
                CurrentPaidAmount = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
                PaidAmount = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
                IsMatchSupplier = false, //是否匹配到供应商(默认没有匹配到，因为 Apply 页面进入时，供应商名称下拉框默认是不选择的)
            }).ToList();

            model.TotalMoney = (model.Rate * payOrders.Sum(item => item.CurrentPaidAmount)).ToRound(2);
            model.UnPayExchangeOrders = payOrders;
            //付款方式
            model.PayTypeOptions = EnumUtils.ToDictionary<PaymentType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            var purchaser = PurchaserContext.Current;
            model.AgentName = purchaser.CompanyName;
            model.Account = purchaser.BankName;
            model.AccountID = purchaser.AccountId;

            return View(model);
        }


        /// <summary>
        /// 获取供应商银行+地址、订单本次申请金额
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetSupplierBankAndAddress(SupplierBankAndAddressRequestModel data)
        {
            string supplierID = data.SupplierID;

            JsonResult json = new JsonResult();
            supplierID = supplierID.InputText();

            #region 获取供应商银行+地址

            var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplierID];
            if (supplier == null)
            {
                return base.JsonResult(VueMsgType.error, "供应商不存在。");
            }

            var banks = supplier.Banks().ToArray().Select(item => new
            {
                value = item.ID,
                text = item.BankName,
                address = item.BankAddress,
                account = item.BankAccount,
                code = item.SwiftCode
            });

            var address = supplier.Addresses().ToArray().Select(item => new
            {
                value = item.ID,
                text = item.Address
            });

            #endregion

            #region 订单本次申请金额

            string[] orderIds = data.OrderIDs;

            for (int i = 0; i < orderIds.Length; i++)
            {
                orderIds[i] = orderIds[i].InputText();
            }

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyOrders;
            view.AllowPaging = false;
            view.Predicate = s => orderIds.Contains(s.ID);
            var orders = view.ToArray();

            int existCount = (from orderId in orderIds
                              join order in orders on orderId equals order.ID
                              select orderId).Count();
            if (existCount != orderIds.Length)
            {
                return base.JsonResult(VueMsgType.error, "有些订单号不存在。");
            }

            List<OrderCurrentPayAmount> payAmounts = new List<OrderCurrentPayAmount>();

            foreach (var order in orders)
            {
                var thisOrderCurrentPayAmount = order.OrderCurrentPayAmount(supplierID).FirstOrDefault();
                if (thisOrderCurrentPayAmount != null)
                {
                    payAmounts.Add(thisOrderCurrentPayAmount);
                }
            }

            var currentPayAmounts = (from orderID in orderIds
                                     join payAmount in payAmounts on orderID equals payAmount.ID into payAmounts2
                                     from payAmount in payAmounts2.DefaultIfEmpty()
                                     select new OrderCurrentPayAmount
                                     {
                                         ID = orderID,
                                         IsMatchSupplier = payAmount != null,
                                         CurrentPaidAmount = payAmount != null ? payAmount.CurrentPaidAmount : 0,
                                     }).ToList()
                                     .Select(t => new
                                     {
                                         OrderID = t.ID,
                                         IsMatchSupplier = t.IsMatchSupplier,
                                         CurrentPaidAmount = t.CurrentPaidAmount.ToRound(2),
                                     });

            #endregion

            json = new JsonResult
            {
                Data = new
                {
                    banks = banks,
                    address = address,
                    currentPayAmounts = currentPayAmounts,
                }
            };

            return base.JsonResult(VueMsgType.success, "", json.Json());
        }

        /// <summary>
        /// 检查供应商银行敏感性
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckSupplierBankSensitive(SupplierBankSensitive data)
        {
            string supplierBankAddress = data.SupplierBankAddress;

            if (string.IsNullOrEmpty(supplierBankAddress))
            {
                return base.JsonResult(VueMsgType.success, "", new JsonResult
                {
                    Data = new
                    {
                        IsSensitive = false,
                    }
                }.Json());
            }

            supplierBankAddress = supplierBankAddress.Trim();
            var splittedStrs = Split(supplierBankAddress);

            splittedStrs = splittedStrs.Distinct().ToArray();
            splittedStrs = splittedStrs.Where(t => t.Length != 1).ToArray();


            int pageSize = 1000;
            int pageCount = (splittedStrs.Length / pageSize) + (splittedStrs.Length % pageSize > 0 ? 1 : 0);

            bool isSensitive = false;

            for (int i = 1; i <= pageCount; i++)
            {
                var theSplittedStrs = splittedStrs.Skip(pageSize * (i - 1)).Take(pageSize);

                var predicate = PredicateBuilder.Create<Needs.Wl.User.Plat.Views.PayExchangeSensitiveWordCheckModel>();
                predicate = predicate.And(t => t.WordContent == "qwertyuiopasdfg"); //这里是用来使得下面的 Or 生效的

                foreach (var splittedStr in theSplittedStrs)
                {
                    predicate = predicate.Or(t => t.WordContent == splittedStr);
                }

                var payExchangeSensitiveWordCheckView = new Needs.Wl.User.Plat.Views.PayExchangeSensitiveWordCheckView(Needs.Wl.Models.Enums.PayExchangeSensitiveAreaType.Forbid);
                payExchangeSensitiveWordCheckView.AllowPaging = false;
                payExchangeSensitiveWordCheckView.Predicate = predicate;
                var oneSensitiveArea = payExchangeSensitiveWordCheckView.FirstOrDefault();

                if (oneSensitiveArea == null)
                {
                    isSensitive = false;
                }
                else
                {
                    isSensitive = true;
                    break;
                }
            }

            return base.JsonResult(VueMsgType.success, "", new JsonResult
            {
                Data = new
                {
                    IsSensitive = isSensitive,
                }
            }.Json());
        }

        #region 切割字符串

        static string[] Split(string origin)
        {
            List<string> resultStrs = new List<string>();

            for (int i = 1; i <= origin.Length; i++)
            {
                var sdfsf = Split(origin, i);
                resultStrs.AddRange(sdfsf);
            }

            return resultStrs.ToArray();
        }

        static string[] Split(string origin, int len)
        {
            List<string> resultStrs = new List<string>();

            for (int i = 0; i <= origin.Length - len; i++)
            {
                resultStrs.Add(origin.Substring(i, len));
            }

            return resultStrs.ToArray();
        }

        #endregion

        /// <summary>
        /// 付汇申请提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SubmitApply(ApplyViewModel model)
        {
            try
            {
                var current = Needs.Wl.User.Plat.UserPlat.Current;
                var userPayExchangeApply = new Needs.Wl.Client.Services.Models.UserPayExchangeApply()
                {
                    ClientID = current.Client.ID,
                    User = current.ToUser()
                };

                //验证
                foreach (var item in model.UnPayExchangeOrders)
                {
                    var order = current.MyUnPayExchangeOrders[item.ID];
                    if (order == null)
                    {
                        return base.JsonResult(VueMsgType.error, "订单[" + item.ID + "]状态异常，请重新刷新页面.");
                    }
                    else if ((order.DeclarePrice.ToRound(2) - order.PaidExchangeAmount) < item.CurrentPaidAmount)
                    {
                        return base.JsonResult(VueMsgType.error, "订单[" + item.ID + "]申请付汇金额不能大于可申请付汇金额.");
                    }
                }

                userPayExchangeApply.Items = new Needs.Wl.Models.PayExchangeApplyItems(model.UnPayExchangeOrders.Select(item => new Needs.Wl.Models.PayExchangeApplyItem
                {
                    OrderID = item.ID,
                    PaidExchangeAmount = item.PaidExchangeAmount,
                    DeclarePrice = item.DeclarePrice.ToRound(2),
                    Amount = item.CurrentPaidAmount
                }));

                //pi文件明细
                if (model.PayExchangeApplyFiles != null)
                {
                    userPayExchangeApply.Files = new Needs.Wl.Models.PayExchangeApplyFiles(model.PayExchangeApplyFiles.Select(file => new Needs.Wl.Models.PayExchangeApplyFile
                    {
                        UserID = current.ID,
                        FileFormat = file.fileFormat,
                        Name = file.name,
                        FileType = Needs.Wl.Models.Enums.FileType.PIFiles,
                        Url = file.URL
                    }));
                }

                userPayExchangeApply.SupplierName = model.SupplierName;
                userPayExchangeApply.Currency = model.CurrencyCode;
                userPayExchangeApply.SupplierEnglishName = model.SupplierEnglishName;
                userPayExchangeApply.BankName = model.SupplierBankName;
                userPayExchangeApply.BankAddress = model.SupplierBankAddress;
                userPayExchangeApply.BankAccount = model.SupplierBankAccount;
                userPayExchangeApply.SwiftCode = model.SupplierBankCode;
                userPayExchangeApply.OtherInfo = model.Others;
                userPayExchangeApply.Summary = model.Summary;

                //结算日期
                userPayExchangeApply.SettlemenDate = DateTime.Now;
                if (model.ExpectDate != null)
                {
                    userPayExchangeApply.ExpectPayDate = model.ExpectDate.Value;
                }
                userPayExchangeApply.PaymentType = (Needs.Wl.Models.Enums.PaymentType)int.Parse(model.PayType);
                userPayExchangeApply.SupplierAddress = model.SupplierAddressName;
                userPayExchangeApply.ExchangeRateType = (Needs.Wl.Models.Enums.ExchangeRateType)(int)model.RateTypeCode;
                userPayExchangeApply.ExchangeRate = model.Rate;

                UserSubmitPayExchange pay = new UserSubmitPayExchange(userPayExchangeApply);
                pay.Submited += Pay_Submited;
                pay.Submit();

                return base.JsonResult(VueMsgType.success, "", userPayExchangeApply.ID);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        private void Pay_Submited(object sender, Needs.Wl.Models.Hanlders.PayExchangeSubmitedEventArgs e)
        {
            e.PayExchangeApply.Log("用户[" + Needs.Wl.User.Plat.UserPlat.Current.RealName + "]提交了付汇申请");
        }

        /// <summary>
        /// 获取付汇记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetPayRecord(string id)
        {
            id = id.InputText();

            var view = Needs.Wl.User.Plat.UserPlat.Current.OrderContext[id].PayExchangeRecords;
            view.AllowPaging = false;
            var record = view.ToList().Select(item => new
            {
                SupplierName = item.SupplierName,
                ApplyTime = item.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Applier = item.User == null ? "跟单员" : item.User.Name,
                Amount = item.Amount,
                Status = item.Status.GetDescription()
            });
            return base.JsonResult(VueMsgType.success, "", record.Json());
        }

        /// <summary>
        /// 下载付汇委托书
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DownloadPayProxy(string id)
        {
            try
            {
                id = id.InputText();
                var apply = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyPayExchangeApplies[id];
                if (apply == null)
                {
                    return base.JsonResult(VueMsgType.error, "下载付汇委托书失败，付汇申请已删除。");
                }

                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                apply.PayExchangeAgentProxy.SaveAs(file.FilePath);

                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "", ex.Message);
            }
        }

        /// <summary>
        /// 导出付汇委托书
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult ExportPayProxy(string id)
        {
            try
            {
                id = id.InputText();
                var apply = Needs.Wl.User.Plat.UserPlat.Current.MyPayExchangeApplies[id];
                string fileName = DateTime.Now.Ticks + ".pdf";

                //使用流直接导出，不需要先写入硬盘
                var stream = apply.PayExchangeAgentProxy().ToPdf();
                return null;
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "", ex.Message);
            }
        }

        /// <summary>
        /// 上传付汇委托书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SavePayProxy(string id, string filename, string ext, string url)
        {
            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;

                var apply = user.MyPayExchangeApplies[id];
                if (apply.PayExchangeApplyStatus != Needs.Wl.Models.Enums.PayExchangeApplyStatus.Auditing)
                {
                    return base.JsonResult(VueMsgType.error, "上传付汇委托书失败，付汇申请已审核完成。");
                }

                Needs.Wl.Models.PayExchangeApplyFile file = new Needs.Wl.Models.PayExchangeApplyFile();
                file.PayExchangeApplyID = apply.ID;
                file.FileFormat = ext;
                file.Name = filename;
                file.FileType = Needs.Wl.Models.Enums.FileType.PayExchange;
                file.Url = url;
                file.UserID = user.ID;
                file.Enter();

                return base.JsonResult(VueMsgType.success, "上传成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "上传付汇委托书失败，请稍后重试或联系客服。");
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult UploadOrderFile(HttpPostedFileBase file)
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
                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
                }
                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片或者pdf文件！");
                }
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(SysConfig.PayExchange);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);
                return base.JsonResult(VueMsgType.success, "", new { name = fileName, URL = result[1], fileFormat = fileFormat, fullURL = result[2] }.Json());
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        #endregion

        #region 我的付汇

        /// <summary>
        /// 我的付汇
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult MyApplies()
        {
            var model = new MyAppliesViewModel();
            //数据绑定
            model.ApplyStatusOptions = EnumUtils.ToDictionary<Needs.Wl.Models.Enums.PayExchangeApplyStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            return View(model);
        }

        /// <summary>
        /// 获取我的付汇申请
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetApplies()
        {
            var applyStatus = Request.Form["applyStatus"];  //申请状态
            var applyDate = Request.Form["applyDate"];  //开票状态

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyPayExchangeApplies;
            var predicate = PredicateBuilder.Create<Needs.Wl.Client.Services.Models.UserPayExchangeApply>();

            if ((!string.IsNullOrWhiteSpace(applyDate)) && applyDate != "all")
            {
                if (applyDate == "curMonth") //当月订单
                {
                    predicate = predicate.And(item => item.CreateDate.Month == DateTime.Now.Month);
                }
                else if (applyDate == "thrMonth")  //三个月的订单
                {
                    predicate = predicate.And(item => item.CreateDate >= DateTime.Now.AddMonths(-3));
                }
                else if (applyDate == "curYear")  //当年订单
                {
                    predicate = predicate.And(item => item.CreateDate.Year >= DateTime.Now.Year);
                }
            }

            if ((!string.IsNullOrWhiteSpace(applyStatus)) && applyStatus != "all") //订单状态
            {
                predicate = predicate.And(item => item.PayExchangeApplyStatus == (Needs.Wl.Models.Enums.PayExchangeApplyStatus)int.Parse(applyStatus));
            }

            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            view.PageSize = rows;
            view.PageIndex = page;
            view.Predicate = predicate;

            var list = view.ToList().Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Currency,
                TotalAmount = item.Items.Sum(s => s.Amount),
                ApplyStatus = item.PayExchangeApplyStatus.GetDescription(),
                Supplier = item.SupplierName,
                Applier = item.User == null ? "跟单员" : item.User.Name,
                isUploadFile = item.PayExchangeApplyStatus == Needs.Wl.Models.Enums.PayExchangeApplyStatus.Auditing,
                isDelete = item.PayExchangeApplyStatus == Needs.Wl.Models.Enums.PayExchangeApplyStatus.Auditing || item.PayExchangeApplyStatus == Needs.Wl.Models.Enums.PayExchangeApplyStatus.Cancled,
                NoData = false,
                isLoading = true,
            });

            int total = view.RecordCount;
            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 付汇申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult ApplyInfo(string id)
        {
            id = id.InputText();
            var apply = Needs.Wl.User.Plat.UserPlat.Current.MyPayExchangeApplies[id];
            if (apply == null)
            {
                return View("Error");
            }

            ApplyInfoViewModel model = new ApplyInfoViewModel();
            model.ID = id;
            var currency = Needs.Wl.User.Plat.UserPlat.Currencies.FindByCode(apply.Currency);
            if (currency != null)
            {
                model.Currency = currency.Name;
            }

            var purchaser = PurchaserContext.Current;

            //申请明细
            model.ApplyItems = apply.Items.Select(item => new
            {
                OrderID = item.OrderID,
                model.Currency,
                item.Amount,
                DeclarePrice = Needs.Wl.User.Plat.UserPlat.Current.MyOrders[item.OrderID]?.DeclarePrice,
            }).ToArray();

            var applyLog = apply.Logs().GetCompleted();

            //付款信息
            model.SupplierEnglishName = apply.SupplierEnglishName;
            model.Account = purchaser.BankName;
            model.AccountID = purchaser.AccountId;
            model.AgentName = purchaser.CompanyName;
            model.BankName = apply.BankName;
            model.BankAddress = apply.BankAddress;
            model.BankAccount = apply.BankAccount;
            model.BankCode = apply.SwiftCode;
            model.PaymentType = apply.PaymentType.GetDescription();
            model.Others = apply.OtherInfo;
            model.Summary = apply.Summary;
            model.ExchangeRateType = apply.ExchangeRateType.GetDescription();
            model.ExchangeRate = apply.ExchangeRate;
            model.TotalMoney = (apply.Items.Sum(item => item.Amount) * apply.ExchangeRate).ToRound(2);
            model.PayDate = applyLog == null ? "" : applyLog.CreateDate.ToString("yyyy-MM-dd HH:mm");
            model.SettlementDate = apply.SettlemenDate.ToString("yyyy年MM月dd日");   //结算日期

            var listPIFiles = apply.Files().FindPIFiles();
            model.PIFiles = listPIFiles.ToArray().Select(item => new
            {
                FileName = item.Name,
                Status = ((Needs.Wl.Models.Enums.Status)item.Status).GetDescription(),
                URL = AppConfig.Current.FileServerUrl + "/" + item.Url.ToUrl()
            }).ToArray();

            var applyFile = apply.Files().FindPayExchange();
            if (applyFile != null)
            {
                model.AgentTrustInstrumentURL = AppConfig.Current.FileServerUrl + @"/" + applyFile.Url.ToUrl();
                model.AgentTrustInstrumentName = applyFile.Name;
            }

            model.IsUpload = apply.PayExchangeApplyStatus == Needs.Wl.Models.Enums.PayExchangeApplyStatus.Auditing;
            return View(model);
        }

        /// <summary>
        /// 删除我的付汇申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DeleteApply(string id)
        {
            try
            {
                id = id.InputText();
                var current = Needs.Wl.User.Plat.UserPlat.Current;

                var app = current.MyPayExchangeApplies[id];
                app.User = current.ToUser();

                UserSubmitPayExchange pay = new UserSubmitPayExchange(app);
                pay.Deleted += Pay_Deleted;
                pay.Delete();
            }
            catch (Exception ex)
            {
                ex.Log();
                return JsonResult(VueMsgType.error, ex.Message);
            }

            return JsonResult(VueMsgType.success, "删除成功！");
        }

        private void Pay_Deleted(object sender, Needs.Wl.Models.Hanlders.PayExchangeDeletedEventArgs e)
        {
            e.PayExchangeApply.Log("用户[" + Needs.Wl.User.Plat.UserPlat.Current.RealName + "]删除了付汇申请");
        }

        #endregion



        #region 内单待付汇  创建时间2020-02-27
        /// <summary>
        /// 内单待付汇
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult UnPayExchangesInside()
        {
            UnPayExchangesViewModel model = new UnPayExchangesViewModel();
            //币种列表
            var list = Needs.Wl.User.Plat.UserPlat.Current.MyUnPayExchangeOrders.GetPayCurrencies();
            model.CurrencyOptions = list.Select(item => new
            {
                text = item,
                value = item
            }).Distinct().Json();
            return View(model);
        }

        /// <summary>
        /// 获取待付汇
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetUnPayExchangesInside()
        {
            var currency = Request.Form["currency"];  //币种
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var startDate = Request.Form["startDate"];  //日期选择
            var endDate = Request.Form["endDate"];  //日期选择
            var supplier = Request.Form["supplierName"].ToString().Trim();//供应商名称
            var orderIds = Request.Form["mainOrderId"];
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyUnPayExchangeOrders;
            view.PageIndex = page;
            view.PageSize = rows;

            var predicate = Needs.Linq.PredicateBuilder.Create<Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder>();
            // predicate = predicate.And(item => item.Type !=Needs.Wl.Models.Enums.OrderType.Outside);
            //币种
            if ((!string.IsNullOrWhiteSpace(currency)) && currency != "all") //订单状态
            {
                predicate = predicate.And(item => item.Currency == currency);
            }
            //下单日期
            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
            {
                var dStart = DateTime.Parse(startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);

                predicate = predicate.And(item => item.CreateDate >= dStart && item.CreateDate <= dEnd);
            }

            if (!string.IsNullOrWhiteSpace(orderIds))
            {
                var ids = orderIds.Trim().Replace("，", ",").Replace(" ", "").Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);//去掉null 或者空;
                predicate = predicate.And(item => ids.Contains(item.MainOrderID));

            }

            if (!string.IsNullOrWhiteSpace(supplier))
            {
                predicate = predicate.And(item => item.PayExchangeSuppliers.Any(x => x.ChineseName.Contains(supplier)));
            }

            view.Predicate = predicate;

            var orderlist = await view.ToListAsync();

            var list = orderlist.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Currency,
                item.DeclarePrice,
                PaySuppliers = item.PayExchangeSuppliers.Select(c => c.ClientSupplierID).ToArray(),
                PaySuppliersName = item.PayExchangeSuppliers.Select(c => c.ChineseName).ToArray(),
                OrderStatus = item.OrderStatus.GetDescription(),
                isDelete = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Draft ? true : false,
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                PayExchangeType = item.IsPrePayExchange ? "预换汇" : "90天内换汇",
                IsPrePayExchange = item.IsPrePayExchange,
                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
                Remittanced = item.PaidExchangeAmount.ToRound(2).ToString(),
                isCheck = false,
                isLoading = true,
                DeclareDate = item.DeclareDate,
                MainOrderID = item.MainOrderID,
                MainOrderCreateDate = item.MainOrderCreateDate.ToString("yyyy-MM-dd HH:mm"),
            }).OrderByDescending(x=>x.ID);
            return JsonResult(VueMsgType.success, "", new { list, total = view.RecordCount }.Json());
        }

        #endregion
    }
}