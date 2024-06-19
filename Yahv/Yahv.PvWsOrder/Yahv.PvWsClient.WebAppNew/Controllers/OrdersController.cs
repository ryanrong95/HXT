using Layers.Data.Sqls;
using Newtonsoft.Json;
using ShencLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class OrdersController : UserController
    {
        #region 代付/收货款
        /// <summary>
        /// 代付货款页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult PrePayApplyList()
        {
            //数据绑定
            ViewBag.ApplyStatusOptions = ExtendsEnum.ToDictionary<ApplicationStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).ToArray();
            ViewBag.PayStatusOptions = ExtendsEnum.ToDictionary<ApplicationPaymentStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 代付货款申请页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult PrePayApply(string id)
        {
            var para = Request.Form["para"];
            var current = Client.Current;
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var ids = paraArr[0].Split(',');

            var orders = current.MyOrder.GetApplyOrders(ApplicationType.Payment, ids);
            if (orders == null)
            {
                return View("Error");
            }
            Application model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = new Application(id, ApplicationType.Payment);
                ViewBag.http = Yahv.Services.Models.CenterFile.Web;
                //付款方式描述
                ViewBag.PayerMethod = model.PayPayer.Method.GetDescription();
                //付款币种描述
                ViewBag.PayerCurrency = model.PayPayer.Currency.GetDescription();
            }
            else
            {
                var USDRate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode(Currency.USD.ToString()).Rate; //美元实时汇率

                model = new Application
                {
                    PayPayer = new PvWsOrder.Services.Models.ApplicationPayer(),
                    PayPayee = new PvWsOrder.Services.Models.ApplicationPayee(),
                    //勾选的订单项
                    Items = orders.Select(m => new PvWsOrder.Services.Models.ApplicationItem
                    {
                        OrderID = m.OrderID,
                        AppliedPrice = m.AppliedPrice,
                        SettlementCurrency = m.SettlementCurrency,
                        SupplierID = m.SupplierID,
                        Amount = m.AppLeftPrice
                    }),

                    HandlingFeePayerType = HandlingFeePayerType.双方承担.GetHashCode().ToString(),
                    USDRate = USDRate,
                };
            }
            //我方收款人
            var payees = Alls.Current.wsPayeeAll.OrderByDescending(m => m.CreateDate)
                .Where(item => item.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                    && item.Status == GeneralStatus.Normal
                    && item.Methord != Methord.Cash).ToList();
            //畅运账户1
            var payeeInfo1 = payees.ElementAt(0);
            ViewBag.PayeeInfo1 = new
            {
                InCompanyName = payeeInfo1 != null ? (payeeInfo1.EnterpriseName ?? "") : "",
                InBankAccount = payeeInfo1 != null ? (payeeInfo1.Account ?? "") : "",
                InBankName = payeeInfo1 != null ? (payeeInfo1.Bank ?? "") : "",
                InBankAddress = payeeInfo1 != null ? (payeeInfo1.BankAddress ?? "") : "",
                InSwiftCode = payeeInfo1 != null ? (payeeInfo1.SwiftCode ?? "") : ""
            }.Json();
            //畅运账户2
            var payeeInfo2 = payees.ElementAt(1);
            ViewBag.PayeeInfo2 = new
            {
                InCompanyName = payeeInfo2 != null ? (payeeInfo2.EnterpriseName ?? "") : "",
                InBankAccount = payeeInfo2 != null ? (payeeInfo2.Account ?? "") : "",
                InBankName = payeeInfo2 != null ? (payeeInfo2.Bank ?? "") : "",
                InBankAddress = payeeInfo2 != null ? (payeeInfo2.BankAddress ?? "") : "",
                InSwiftCode = payeeInfo2 != null ? (payeeInfo2.SwiftCode ?? "") : ""
            }.Json();
            if (model.InBankAccount == payeeInfo2?.Account)
            {
                model.InBankAddress = payeeInfo2?.BankAddress;
                model.InSwiftCode = payeeInfo2?.SwiftCode;
            }
            else
            {
                //默认为畅运账户1
                model.InCompanyName = payeeInfo1?.EnterpriseName;
                model.InBankAccount = payeeInfo1?.Account;
                model.InBankName = payeeInfo1?.Bank;
                model.InBankAddress = payeeInfo1?.BankAddress;
                model.InSwiftCode = payeeInfo1?.SwiftCode;
            }
            //我方付款人
            var companyPayer = Alls.Current.wsPayerAll.OrderByDescending(m => m.CreateDate)
                .FirstOrDefault(item => item.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                                     && item.Status == GeneralStatus.Normal
                                     && item.Methord != Methord.Cash);
            model.OutCompanyName = companyPayer != null ? (companyPayer.EnterpriseName ?? "") : "";
            model.OutBankName = companyPayer != null ? (companyPayer.Bank ?? "") : "";
            model.OutBankAccount = companyPayer != null ? (companyPayer.Account ?? "") : "";
            //客户付款人
            ViewBag.AppPayerOptions = GetPayerOptions("5").Data;
            //供应商收款人
            string supplierId = "";
            if (!string.IsNullOrEmpty(model.PayPayee.PayeeID))
            {
                supplierId = new wsnSupplierPayeesTopView<PvbCrmReponsitory>().FirstOrDefault(m => m.ID == model.PayPayee.PayeeID)?.nSupplierID;
            }
            else
            {
                supplierId = orders.First().SupplierID;
            }
            //ViewBag.SupplierBankOptions = GetSupplierBankOptions(supplierId).Data;

            var theSupplierBankOptions = new MySupplierPayees(Client.Current.EnterpriseID)
                                                .GetSupplierPayeesByClientID()
                                                .Select(item => new
                                                {
                                                    value = item.ID,
                                                    text = item.Account,
                                                    item.Account,
                                                    item.Bank,
                                                    item.RealEnterpriseID,
                                                    item.RealEnterpriseName,
                                                    item.BankAddress,
                                                    item.Place,
                                                    item.SwiftCode,
                                                }).ToArray();
            ViewBag.SupplierBankOptions = JsonResult(VueMsgType.success, "", theSupplierBankOptions.Json()).Data;

            ViewBag.SupplierBankCurrency = (int)model.Items.First().SettlementCurrency;
            //支付方式
            ViewBag.PayMethodOptions = ExtendsEnum.ToDictionary<Methord>()
                .Where(item => item.Key == "3" || item.Key == "5" || item.Key == "6")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            //支付币种
            ViewBag.PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();

            ////按科目的欠款额度数据(人民币)
            //var myCreaditData = current.MyCredits.Where(item => item.Currency == Currency.CNY).Select(item => new
            //{
            //    item.Business,
            //    item.Catalog,
            //    item.Total,
            //    item.Cost,
            //    Left = item.Total - item.Cost,
            //}).ToArray();
            //var warehouse = myCreaditData.FirstOrDefault(item => item.Catalog == "仓储费");
            ////仓储余额（人民币）
            //ViewBag.WareHouseLeft = (warehouse?.Left).GetValueOrDefault();

            var Credit = current.MyUsdCredits.FirstOrDefault(item => item.Business == "供应链" && item.Catalog == Payments.CatalogConsts.仓储服务费 && item.Currency == Currency.USD);
            ViewBag.WareHouseLeft = (Credit?.Total).GetValueOrDefault() - (Credit?.Cost).GetValueOrDefault();

            // 去除“新增付款人”弹框中“付款方式”和“付款币种”后，在外面下拉选择的选项
            ViewBag.PayerMethordOptions = ExtendsEnum.ToDictionary<Methord>()
                .Where(c => c.Key != ((int)Methord.TT).ToString()
                    && c.Key != ((int)Methord.Alipay).ToString()
                    && c.Key != ((int)Methord.Exchange).ToString())
                .Select(item => new { value = Convert.ToInt32(item.Key), text = item.Value }).ToArray();
            ViewBag.PayerCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0")
                .Select(item => new { value = Convert.ToInt32(item.Key), text = item.Value }).ToArray();

            //手续费承担方选项
            ViewBag.HandlingFeePayerTypeOptions = ExtendsEnum.ToDictionary<HandlingFeePayerType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();

            return View(model);
        }

        /// <summary>
        /// 获取供应商收款下拉数据
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetSupplierBankOptions(string supplierId)
        {
            var rslt = new MySupplierPayees(Client.Current.EnterpriseID, supplierId).Where(m => m.Status == GeneralStatus.Normal)
                 .Select(item => new
                 {
                     value = item.ID,
                     text = item.Account,
                     item.Account,
                     item.Bank,
                     item.RealEnterpriseID,
                     item.RealEnterpriseName,
                     item.BankAddress,
                     item.Place,
                     item.SwiftCode,
                 }).Json();
            return JsonResult(VueMsgType.success, "", rslt);
        }

        /// <summary>
        /// 检查代付货款金额
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult CheckPrePayMoney(string orderID, decimal applyMoney)
        {
            var order = Client.Current.MyOrder.GetOrderDetail(orderID);
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单异常请返回主页面");
            }
            //if (order.AppLeftPrice < applyMoney)
            //{
            //    return base.JsonResult(VueMsgType.error, "本次申请金额最多为" + order.ApplicationLeftPrice.ToRound(2) + "(" + order.OrderItems[0].Currency.GetDescription() + ")");
            //}
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 提交代付货款申请
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult PrePaySubmit(Application model, List<CenterFileDescription> fileItems)
        {
            var current = Client.Current;
            try
            {
                model.FileItems = fileItems;
                model.ClientID = current.EnterpriseID;
                model.Type = ApplicationType.Payment;
                model.UserID = current.ID;
                model.Currency = model.Items.First().SettlementCurrency.GetValueOrDefault();

                //从 Crm( SELECT  Bank , Account , * FROM    PvWsOrder.dbo.wsPayersTopView
                //        WHERE EnterpriseID = '8C7BF4F7F1DE9F69E1D96C96DAF6768E'ORDER BY CreateDate DESC; ) 
                //中查询的 Bank, Account 为空字符串, 这里 OutBankName, OutBankAccount 为null 会导致后面管理端某处有错误
                model.OutBankName = model.OutBankName ?? "";
                model.OutBankAccount = model.OutBankAccount ?? "";

                model.Sumbit();
                OperationLog(model.ID, "代付货款申请保存成功");
                return base.JsonResult(VueMsgType.success, "新增成功", model.ID);
            }
            catch (Exception ex)
            {
                current.Errorlog.Log("代付货款申请保存失败：" + ex.Message);
                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取代付货款申请列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetPrePayApplyList()
        {
            var paralist = new
            {
                startDate = Request.Form["startDate"],  //开始日期
                endDate = Request.Form["endDate"],      //结束日期
                ApplicationStatus = Request.Form["ApplicationStatus"], //审批状态
                PaymentStatus = Request.Form["PaymentStatus"],     //付款状态
            };
            var order = Client.Current.MyApplictions.GetPayApplication();
            if ((!string.IsNullOrWhiteSpace(paralist.startDate)) && (!string.IsNullOrWhiteSpace(paralist.endDate)))
            {
                var dStart = DateTime.Parse(paralist.startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(paralist.endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
                order = order.Where(item => item.CreateDate >= dStart && item.CreateDate <= dEnd);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.ApplicationStatus)))
            {
                order = order.Where(item => item.ApplicationStatus == (ApplicationStatus)int.Parse(paralist.ApplicationStatus));
            }
            if ((!string.IsNullOrWhiteSpace(paralist.PaymentStatus)))
            {
                order = order.Where(item => item.PaymentStatus == (ApplicationPaymentStatus)int.Parse(paralist.PaymentStatus));
            }
            order = order.OrderByDescending(item => item.CreateDate);
            #region 页面数据
            Func<Application, object> convert = item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                ApplicationStatus = item.ApplicationStatus.GetDescription(),
                PaymentStatus = item.PaymentStatus.GetDescription(),
                SupplierID = item.PayPayee.EnterpriseID,
                SupplierName = item.PayPayee.EnterpriseName,
                item.TotalPrice
            };
            #endregion
            return this.Paging(order, convert);
        }

        /// <summary>
        /// 代收货款页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult ReceivableList()
        {
            //数据绑定
            ViewBag.ApplyStatusOptions = ExtendsEnum.ToDictionary<ApplicationStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).ToArray();
            ViewBag.RecStatusOptions = ExtendsEnum.ToDictionary<ApplicationReceiveStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 提交代收货款申请
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult ReceiveSubmit(Application model)
        {
            var current = Client.Current;
            try
            {
                model.ClientID = current.EnterpriseID;
                model.Type = ApplicationType.Receival;
                model.UserID = current.ID;
                model.Currency = model.Items.First().SettlementCurrency.GetValueOrDefault();
                model.Sumbit();
                OperationLog(model.ID, "代收货款申请保存成功");
                return base.JsonResult(VueMsgType.success, "新增成功", model.ID);
            }
            catch (Exception ex)
            {
                current.Errorlog.Log("代收货款申请保存失败：" + ex.Message);
                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 代收货款申请页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult ReceivedApply(string id)
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var ids = paraArr[0].Split(',');

            var orders = Client.Current.MyOrder.GetApplyOrders(ApplicationType.Receival, ids);
            if (orders == null)
            {
                return View("Error");
            }

            Application model = null;
            if (!string.IsNullOrEmpty(id))
            {
                model = new Application(id, ApplicationType.Receival);
                //付款方式描述
                ViewBag.PayerMethod = model.ReceivePayer.Method.GetDescription();
                //付款币种描述
                ViewBag.PayerCurrency = model.ReceivePayer.Currency.GetDescription();
            }
            else
            {
                model = new Application
                {
                    ReceivePayer = new PvWsOrder.Services.Models.ApplicationPayer(),
                    //勾选的订单项
                    Items = orders.Select(m => new PvWsOrder.Services.Models.ApplicationItem
                    {
                        OrderID = m.OrderID,
                        AppliedPrice = m.AppliedPrice,
                        SettlementCurrency = m.SettlementCurrency,
                        SupplierID = m.SupplierID,
                        Amount = m.AppLeftPrice
                    })
                };
            }
            //我方收款人
            var payees = Alls.Current.wsPayeeAll.OrderByDescending(m => m.CreateDate)
                .Where(item => item.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                            && item.Status == GeneralStatus.Normal
                            && item.Methord != Methord.Cash).ToList();
            //畅运账户1
            var payeeInfo1 = payees.ElementAt(0);
            ViewBag.PayeeInfo1 = new
            {
                InCompanyName = payeeInfo1 != null ? (payeeInfo1.EnterpriseName ?? "") : "",
                InBankAccount = payeeInfo1 != null ? (payeeInfo1.Account ?? "") : "",
                InBankName = payeeInfo1 != null ? (payeeInfo1.Bank ?? "") : "",
                InBankAddress = payeeInfo1 != null ? (payeeInfo1.BankAddress ?? "") : "",
                InSwiftCode = payeeInfo1 != null ? (payeeInfo1.SwiftCode ?? "") : ""
            }.Json();
            //畅运账户2
            var payeeInfo2 = payees.ElementAt(1);
            ViewBag.PayeeInfo2 = new
            {
                InCompanyName = payeeInfo2 != null ? (payeeInfo2.EnterpriseName ?? "") : "",
                InBankAccount = payeeInfo2 != null ? (payeeInfo2.Account ?? "") : "",
                InBankName = payeeInfo2 != null ? (payeeInfo2.Bank ?? "") : "",
                InBankAddress = payeeInfo2 != null ? (payeeInfo2.BankAddress ?? "") : "",
                InSwiftCode = payeeInfo2 != null ? (payeeInfo2.SwiftCode ?? "") : ""
            }.Json();
            if (model.InBankAccount == payeeInfo2?.Account)
            {
                model.InBankAddress = payeeInfo2?.BankAddress;
                model.InSwiftCode = payeeInfo2?.SwiftCode;
            }
            else
            {
                //默认为畅运账户1
                model.InCompanyName = payeeInfo1?.EnterpriseName;
                model.InBankAccount = payeeInfo1?.Account;
                model.InBankName = payeeInfo1?.Bank;
                model.InBankAddress = payeeInfo1?.BankAddress;
                model.InSwiftCode = payeeInfo1?.SwiftCode;
            }
            //我方付款人
            var companyPayer = Alls.Current.wsPayerAll.OrderByDescending(m => m.CreateDate)
                .FirstOrDefault(item => item.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                                     && item.Status == GeneralStatus.Normal
                                     && item.Methord != Methord.Cash);
            model.OutCompanyName = companyPayer != null ? (companyPayer.EnterpriseName ?? "") : "";
            model.OutBankName = companyPayer != null ? (companyPayer.Bank ?? "") : "";
            model.OutBankAccount = companyPayer != null ? (companyPayer.Account ?? "") : "";
            //客户付款人
            ViewBag.ClientPayerOptions = GetPayerOptions("").Data;
            //支付方式
            ViewBag.PayMethodOptions = ExtendsEnum.ToDictionary<Methord>()
                .Where(item => item.Key != "1" && item.Key != "2" && item.Key != "4")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            //支付币种
            ViewBag.PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            //发货时机
            ViewBag.DelivaryOppOptions = ExtendsEnum.ToDictionary<DelivaryOpportunity>()
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();
            //支票投送方式
            ViewBag.CheckDelTypeOptions = ExtendsEnum.ToDictionary<CheckDeliveryType>()
                .Select(item => new { value = item.Key, text = item.Value }).ToArray();

            // 去除“新增付款人”弹框中“付款方式”和“付款币种”后，在外面下拉选择的选项
            ViewBag.ReceivePayerMethordOptions = ExtendsEnum.ToDictionary<Methord>()
                .Where(c => c.Key != ((int)Methord.TT).ToString()
                    && c.Key != ((int)Methord.Alipay).ToString()
                    && c.Key != ((int)Methord.Exchange).ToString())
                .Select(item => new { value = Convert.ToInt32(item.Key), text = item.Value }).ToArray();
            ViewBag.ReceivePayerCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0")
                .Select(item => new { value = Convert.ToInt32(item.Key), text = item.Value }).ToArray();

            return View(model);
        }

        /// <summary>
        /// 获取代收货款申请列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetReceivableList()
        {
            var paralist = new
            {
                startDate = Request.Form["startDate"],  //开始日期
                endDate = Request.Form["endDate"],      //结束日期
                ApplicationStatus = Request.Form["ApplicationStatus"], //审批状态
                ReceiveStatus = Request.Form["ReceiveStatus"],     //付款状态
            };
            var order = Client.Current.MyApplictions.GetReceiveApplication();
            if ((!string.IsNullOrWhiteSpace(paralist.startDate)) && (!string.IsNullOrWhiteSpace(paralist.endDate)))
            {
                var dStart = DateTime.Parse(paralist.startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(paralist.endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
                order = order.Where(item => item.CreateDate >= dStart && item.CreateDate <= dEnd);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.ApplicationStatus)))
            {
                order = order.Where(item => item.ApplicationStatus == (ApplicationStatus)int.Parse(paralist.ApplicationStatus));
            }
            if ((!string.IsNullOrWhiteSpace(paralist.ReceiveStatus)))
            {
                order = order.Where(item => item.ReceiveStatus == (ApplicationReceiveStatus)int.Parse(paralist.ReceiveStatus));
            }
            order = order.OrderByDescending(item => item.CreateDate);

            #region 页面数据
            Func<Application, object> convert = item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                ApplicationStatus = item.ApplicationStatus.GetDescription(),
                ReceiveStatus = item.ReceiveStatus.GetDescription(),
                PayerEntID = item.PayPayer.EnterpriseID,
                PayerEntName = item.PayPayer.EnterpriseName,
                item.TotalPrice
            };
            #endregion

            return this.Paging(order, convert);
        }
        #endregion


        #region 订单列表
        /// <summary>
        /// 收货列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult ReceivedList()
        {
            var orderstatuslist = new[] { CgOrderStatus.待交货, CgOrderStatus.已交货, CgOrderStatus.待收货,
                                          CgOrderStatus.客户待收货, CgOrderStatus.客户已收货, };
            ViewBag.OrderStatusOptions = // ExtendsEnum.ToDictionary<CgOrderStatus>()
            orderstatuslist.ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
            .Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 代付款申请列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult ReceivedApplyList()
        {
            ViewBag.OrderStatusOptions = ExtendsEnum.ToDictionary<CgOrderStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 根据查询条件查询收货订单数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetReceivedList()
        {
            var receivedOrders = Yahv.Client.Current.MyReceivedOrders;

            #region 查询条件
            var paramslist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                PartNumber = Request.Form["PartNumber"],
                Supplier = Request.Form["Supplier"],
            };
            if (!string.IsNullOrWhiteSpace(paramslist.Supplier))
            {
                receivedOrders = receivedOrders.SearchBySupplier(paramslist.Supplier.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramslist.PartNumber))
            {
                receivedOrders = receivedOrders.SearchByPart(paramslist.PartNumber.Trim());
            }
            #endregion

            //获取页面查询条件组成的条件表达式
            var lambdas = GetExpressions();
            var data = receivedOrders.GetPageListOrders(lambdas.ToArray(), paramslist.rows, paramslist.page);
            var linq = data.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                Type = item.Type.GetDescription(),
                Currency = item.Input.Currency.GetDescription(),
                item.TotalPrice,
                OrderStatus = item.MainStatus.GetDescription(),
                IsEdit = item.MainStatus == CgOrderStatus.暂存,
                IsConfirm = item.PaymentStatus == OrderPaymentStatus.Confirm,
                IsTransfer = item.Type == OrderType.Transport,
                IsCancel = item.MainStatus == CgOrderStatus.取消,
                IsPay = item.TotalPrice > item.ApplicationApplyPrice,
                SupplierName = item.SupplierName2,
            });

            return this.Paging(linq, data.Total);
        }

        /// <summary>
        /// 根据查询条件查询代付款申请数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetReceivedApplyList()
        {
            var receivedOrders = Yahv.Client.Current.MyReceivedOrders;

            #region 查询条件
            var paramslist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                Supplier = Request.Form["Supplier"],
                MultiField = Request.Form["MultiField"],
            };
            if (!string.IsNullOrWhiteSpace(paramslist.Supplier))
            {
                receivedOrders = receivedOrders.SearchBySupplier(paramslist.Supplier.Trim());
            }
            if (!string.IsNullOrEmpty(paramslist.MultiField))
            {
                receivedOrders = receivedOrders.SearchByMultiField(paramslist.MultiField.Trim());
            }
            #endregion

            //获取页面查询条件组成的条件表达式
            var lambdas = GetExpressions();
            var data = receivedOrders.GetUnPayedOrders(lambdas.ToArray(), paramslist.rows, paramslist.page);
            var linq = data.Select(item => new
            {
                item.ID,
                item.SupplierID,
                item.SupplierName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                Type = item.Type.GetDescription(),
                Currency = item.Input.Currency.GetDescription(),
                TotalPrice = item.TotalPrice.ToString("0.00"),
                OrderStatus = item.MainStatus.GetDescription(),
                IsTransfer = item.Type == OrderType.Transport,
                IsChecked = false
            });

            return this.Paging(linq, data.Total);
        }

        /// <summary>
        /// 发货列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult DeliveryList()
        {
            var orderstatuslist = new[] { CgOrderStatus.待交货, CgOrderStatus.已交货, CgOrderStatus.待收货,
                                          CgOrderStatus.客户待收货, CgOrderStatus.客户已收货, };
            ViewBag.OrderStatusOptions = // ExtendsEnum.ToDictionary<CgOrderStatus>()
            orderstatuslist.ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
            .Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 代收款申请
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult DeliveryApplyList()
        {
            ViewBag.OrderStatusOptions = ExtendsEnum.ToDictionary<CgOrderStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 根据查询条件获取列表数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetDeliveryList()
        {
            var deliveryOrders = Client.Current.MyDeliveryOrders;

            #region 查询条件
            var paramslist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                PartNumber = Request.Form["PartNumber"],
                Supplier = Request.Form["Supplier"],
            };
            if (!string.IsNullOrWhiteSpace(paramslist.Supplier))
            {
                deliveryOrders = deliveryOrders.SearchBySupplier(paramslist.Supplier.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramslist.PartNumber))
            {
                deliveryOrders = deliveryOrders.SearchByPart(paramslist.PartNumber.Trim());
            }
            #endregion

            //获取页面查询条件组成的条件表达式
            var lambdas = GetExpressions();
            var data = deliveryOrders.GetPageListOrders(lambdas.ToArray(), paramslist.rows, paramslist.page);
            var linq = data.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                WaybillType = item.OutWaybill.Type.GetDescription(),
                OrderStatus = item.MainStatus.GetDescription(),
                IsConfirm = item.PaymentStatus == OrderPaymentStatus.Confirm,
                IsCancel = item.MainStatus == CgOrderStatus.取消,
            });

            return this.Paging(linq, data.Total);
        }

        /// <summary>
        /// 根据查询条件获取代收款申请数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetDeliveryApplyList()
        {
            var deliveryOrders = Client.Current.UnReceiveAmountOrdes;

            #region 查询条件
            var paramslist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                Supplier = Request.Form["Supplier"],
            };
            if (!string.IsNullOrWhiteSpace(paramslist.Supplier))
            {
                deliveryOrders = deliveryOrders.SearchBySupplier(paramslist.Supplier.Trim());
            }
            #endregion

            //获取页面查询条件组成的条件表达式
            var lambdas = GetExpressions();
            var data = deliveryOrders.GetToChargeOrders(lambdas.ToArray(), paramslist.rows, paramslist.page);
            var linq = data.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                WaybillType = item.OutWaybill?.Type.GetDescription(),
                Currency = item.Output.Currency.GetDescription(),
                TotalPrice = item.TotalPrice.ToString("0.00"),
                OrderStatus = item.MainStatus.GetDescription(),
                IsTransfer = item.Type == OrderType.Transport,
                IsChecked = false
            });

            return this.Paging(linq, data.Total);
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private List<LambdaExpression> GetExpressions()
        {
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<WsOrder, bool>> lambda = item => true;
            if (!string.IsNullOrWhiteSpace(Request.Form["startDate"]))
            {
                lambda = item => item.CreateDate >= DateTime.Parse(Request.Form["startDate"]);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["endDate"]))
            {
                lambda = item => item.CreateDate <= DateTime.Parse(Request.Form["endDate"]).AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["OrderID"]))
            {
                lambda = item => item.ID.Contains(Request.Form["OrderID"]);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(Request.Form["OrderStatus"]))
            {
                lambda = item => item.MainStatus == (CgOrderStatus)int.Parse(Request.Form["OrderStatus"]);
                lambdas.Add(lambda);
            }

            return lambdas;
        }
        #endregion


        #region 代收订单新增、提交、详情
        /// <summary>
        /// 新增代收订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult StorageAdd(string id)
        {
            StorageOrdersModel model = new StorageOrdersModel();
            var current = Client.Current;

            model.OrderItems = new Models.OrderItem[0];
            model.SpecialGoods = new SpecialGoodsModel[0];
            model.IsSubmit = false;
            model.PIFiles = new FileModel[0];
            model.PackingFiles = new FileModel[0];
            model.TakingFiles = new FileModel[0];
            model.Origin = (Origin.HKG).ToString(); //默认中国香港
            model.HKWaybillType = ((int)WaybillType.PickUp).ToString(); //默认自提
            model.WaybillType = ((int)WaybillType.PickUp).ToString(); //默认自提

            //获取香港库房信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
            var contact1 = HKWarehouseEnterprise?.Contacts.FirstOrDefault();
            model.WareHouseEnglishName = PvWsOrder.Services.PvClientConfig.WareHouseEnglishName;
            model.WareHouseAddress = HKWarehouse.Address;
            model.WareHouseName = contact1?.Name;
            model.WareHouseTel = contact1?.Tel ?? contact1?.Mobile;
            model.WareHouseInfoForCopy = "收货公司: " + model.WareHouseEnglishName + "\r\n"
                                       + "收货地址: " + model.WareHouseAddress + "\r\n"
                                       + "联系人: " + model.WareHouseName + "\r\n"
                                       + "联系电话: " + model.WareHouseTel; //用于复制
            //我方收款信息
            var payees = Alls.Current.wsPayeeAll.OrderByDescending(m => m.CreateDate)
                .Where(item => item.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                            && item.Status == GeneralStatus.Normal
                            && item.Methord != Methord.Cash)
                .Select(item => new
                {
                    item.ID,
                    item.EnterpriseName,
                    item.Account,
                    item.Bank,
                    item.BankAddress,
                    item.SwiftCode,

                    ForCopy = "账户：" + item.EnterpriseName + "\r\n"
                            + "账号：" + item.Account + "\r\n"
                            + "银行：" + item.Bank + "\r\n"
                            + "银行地址：" + item.BankAddress + "\r\n"
                            + "SWIFTCODE：" + item.SwiftCode,  //用于复制
                }).Take(2).ToList();
            model.CompanyBankID = payees.First()?.ID;
            model.PayCompanyBankID = model.CompanyBankID;
            model.IsEntry = true;
            //账户余额
            //var Credit = current.MyCredits.FirstOrDefault(item => item.Currency == Currency.CNY && item.Catalog == "仓储费");
            var Credit = current.MyUsdCredits.FirstOrDefault(item => item.Business == "供应链" && item.Catalog == Payments.CatalogConsts.仓储服务费 && item.Currency == Currency.USD);
            model.WareHouseLeft = (Credit?.Total).GetValueOrDefault() - (Credit?.Cost).GetValueOrDefault();

            //收货地址
            var receiveOptions = current.MyConsignees.Select(item => new { value = item.ID, text = item.Title, address = item.Address, name = item.Name, mobile = item.Mobile }).ToArray();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var order = current.MyReceivedOrders.GetOrderDetail(id);
                if (order == null || order.MainStatus != CgOrderStatus.暂存)
                {
                    return View("Error"); //待提交订单方可修改
                }
                #region 基本信息
                model.ID = order.ID;
                model.isedit = true; //是否编辑
                model.SupplierID = order.SupplierID;
                model.SettlementCurrency = ((int?)order.SettlementCurrency).ToString();
                model.IsTransfer = order.Type == OrderType.Transport; //是否即收即发
                model.PackNo = order.InWaybill.TotalParts; //总件数
                model.Currency = ((int?)order.Input.Currency).ToString();//货物币种
                model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
                {
                    ID = item.ID,
                    InputID = item.InputID,
                    Manufacturer = item.Product.Manufacturer,
                    Name = item.Name,
                    Origin = item.Origin,
                    OutputID = item.OutputID,
                    PartNumber = item.Product.PartNumber,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    Unit = (int)item.Unit,
                    UnitPrice = item.UnitPrice,
                }).ToArray();
                #endregion

                #region 交货和收货信息
                //交货信息
                model.HKWaybillType = ((int)order.InWaybill.Type).ToString(); //香港交货方式
                if (order.InWaybill.Type == WaybillType.PickUp)
                {
                    model.HKTakingDate = order.InWaybill.WayLoading.TakingDate; //提货时间
                    //提货人
                    model.HKSupplierAddress = Alls.Current.SupplierConsignors.Where(item => item.Contact == order.InWaybill.WayLoading.TakingContact &&
                    item.Address == order.InWaybill.WayLoading.TakingAddress && item.Mobile == order.InWaybill.WayLoading.TakingPhone).FirstOrDefault()?.ID;
                }
                model.HKFreight = order.InWaybill.WayCondition.PayForFreight;//是否垫付运费
                model.IsPay = order.Input.IsPayCharge.GetValueOrDefault(); //是否代付货款
                model.HKExpressNumber = order.InWaybill.Code;//运单号
                //model.HKExpressSubNumber = order.InWaybill.Subcodes;//子运单号
                model.HKExpressName = order.InWaybill.CarrierID;//快递公司
                //model.HKAirCode = order.InWaybill.VoyageNumber;//航次号

                //发货信息
                if (model.IsTransfer) //即收即发业务
                {
                    model.WaybillType = ((int)order.OutWaybill.Type).ToString();//发货方式
                    model.IsRecieve = order.Output.IsReciveCharge.GetValueOrDefault(); //是否代收货款
                    if (order.OutWaybill.Type == WaybillType.PickUp) //客户自提
                    {
                        model.PickupTime = order.OutWaybill.WayLoading?.TakingDate;//提货时间
                        model.ClientPicker = order.OutWaybill.Consignee.Contact; //提货联系人
                        model.ClientPickerMobile = order.OutWaybill.Consignee.Phone; //提货人手机
                        model.IDType = ((int?)order.OutWaybill.Consignee.IDType).ToString(); //提货人证件类型
                        model.SealContext = order.OutWaybill.Consignee.IDNumber; //公章内容
                        model.IDNumber = order.OutWaybill.Consignee.IDNumber; //提货人证件号码
                    }
                    var consignee = order.OutWaybill.Consignee;
                    var address = receiveOptions.Where(item => item.name == consignee.Contact && item.address == consignee.Address && item.mobile == consignee.Phone).FirstOrDefault();
                    model.ClientConsignee = address?.value;
                    model.ClientConsigneeName = order.OutWaybill.Consignee.Contact; //收货人
                    model.ClientContactMobile = order.OutWaybill.Consignee.Phone; //收货人联系号码
                    model.ClientConsigneeAddress = order.OutWaybill.Consignee.Address; //收货人地址
                    model.ZipCode = order.OutWaybill.Consignee.Zipcode; //收货人邮编
                    model.ExpressID = order.OutWaybill.CarrierID; //快递公司
                    model.Freight = order.OutWaybill.WayCondition.PayForFreight; //是否垫付运费
                }
                #endregion

                #region 文件
                //提货文件
                model.TakingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).Select(item => new FileModel
                {
                    name = item.CustomName,
                    URL = item.Url,
                    fileFormat = Path.GetExtension(item.CustomName).ToLower()
                }).ToArray();
                //PI文件
                model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new FileModel
                {
                    name = item.CustomName,
                    URL = item.Url,
                    fileFormat = Path.GetExtension(item.CustomName).ToLower()
                }).ToArray();
                //装箱文件
                model.PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new FileModel
                {
                    name = item.CustomName,
                    URL = item.Url,
                    fileFormat = Path.GetExtension(item.CustomName).ToLower()
                }).ToArray();
                #endregion

                #region 特殊货物处理
                if (order.Requirements != null)
                {
                    var fileUrl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
                    model.SpecialGoods = order.Requirements.Select(item => new SpecialGoodsModel
                    {
                        Name = item.Name,
                        Type = ((int)(item.Type)).ToString(),
                        TypeName = item.Type.GetDescription(),
                        FileFullURL = fileUrl + item.RequireFiles?.Url.ToUrl(),
                        FileName = item.RequireFiles?.CustomName,
                        FileURL = item.RequireFiles?.Url,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice,
                        Requirement = item.Requirement,
                    }).ToArray();
                }
                #endregion

                #region 代收代付货款申请
                if (model.IsPay)
                {
                    var pay = current.MyApplictions.GetPayDetailByOrderID(order.ID);
                    model.ApplyPrice = pay.TotalPrice;
                    //供应商收款信息
                    model.BankID = pay.PayPayee.PayeeID;
                    model.SupplierMethod = ((int?)pay.PayPayee.Method).ToString();
                    model.SupplierName = pay.PayPayee.EnterpriseName;
                    model.SupplierBankName = pay.PayPayee.BankName;
                    model.SupplierBankAccount = pay.PayPayee.BankAccount;

                    //客户付款信息
                    model.PayPayerID = pay.PayPayer.PayerID;
                    model.PayPayerMethod = (int)pay.PayPayer.Method;
                    model.PayPayerMethodName = pay.PayPayer.Method.GetDescription();
                    model.PayPayerCurrency = (int)pay.PayPayer.Currency;
                    model.PayPayerCurrencyName = pay.PayPayer.Currency.GetDescription();

                    //获取当前的公司收款人
                    var payee = payees.FirstOrDefault(item => item.Account == pay.InBankAccount && item.Bank == pay.InBankName && item.EnterpriseName == pay.InCompanyName);
                    model.PayCompanyBankID = payee?.ID;
                }

                if (model.IsRecieve)
                {
                    var receive = current.MyApplictions.GetReceiveDetailByID(order.ID);
                    model.RecievePrice = receive.TotalPrice;
                    model.IsEntry = receive.IsEntry;
                    model.DelivaryOpportunity = ((int?)receive.DelivaryOpportunity).ToString();
                    model.CheckDelivery = ((int?)receive.CheckDelivery).ToString();
                    model.CheckCarrier = receive.CheckCarrier;
                    model.CheckConsignee = receive.CheckConsignee;
                    model.CheckPayeeAccount = receive.CheckPayeeAccount;

                    //客户付款信息
                    model.PayerID = receive.ReceivePayer.PayerID;
                    model.PayerMethod = (int)receive.ReceivePayer.Method;
                    model.PayerCurrency = (int)receive.ReceivePayer.Currency;
                    model.PayerMethodName = receive.ReceivePayer.Method.GetDescription();
                    model.PayPayerCurrencyName = receive.ReceivePayer.Currency.GetDescription();

                    //获取当前的公司收款人
                    var payee = payees.FirstOrDefault(item => item.Account == receive.InBankAccount && item.Bank == receive.InBankName && item.EnterpriseName == receive.InCompanyName);
                    model.CompanyBankID = payee?.ID;
                }
                #endregion
            }

            var data = new
            {
                ReceiveOptions = receiveOptions,
                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                HKDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0").Select(item => new
                {
                    value = item.Key,
                    text = item.Key == "1" ? "上门提货" : item.Value,
                }).ToArray(),
                SZDeliveryTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0").Select(item => new
                {
                    value = item.Key,
                    text = item.Key == "1" ? "客户自提" : item.Value,
                }).ToArray(),
                //WaybillTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                UnitOptions = UnitEnumHelper.ToUnitDictionary()
                .Where(t => WidelyUsedUnit.Values.Contains(t.Value))
                .Select(item => new { value = item.Value, text = item.Name }).ToArray(), //  + " (" + item.Code + ")"
                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "ZZZ" && item.Value != "NG" && item.Value != "Unknown")
                .Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).ToArray(),
                CarrierOptions = Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
                PayCurrencyOptions = // ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0")
                new[] { Currency.CNY, Currency.USD, Currency.HKD, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                SetCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayMethrodsOptions = ExtendsEnum.ToDictionary<Methord>().Where(item => item.Key == "3" || item.Key == "5" || item.Key == "6")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                DelivaryOppOptions = ExtendsEnum.ToDictionary<DelivaryOpportunity>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                CheckDelTypeOptions = ExtendsEnum.ToDictionary<CheckDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayeeOptions = payees,

                // 去除“新增付款人”弹框中“付款方式”和“付款币种”后，在外面下拉选择的选项
                PayerMethordOptions = ExtendsEnum.ToDictionary<Methord>()
                    .Where(c => c.Key != ((int)Methord.TT).ToString()
                        && c.Key != ((int)Methord.Alipay).ToString()
                        && c.Key != ((int)Methord.Exchange).ToString())
                    .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayerCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
            };
            ViewBag.Options = data;
            return View(model);
        }

        /// <summary>
        /// 代收订单提交
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult StorageSubmit(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<StorageOrdersModel>(data);
                var current = Client.Current;
                var clientinfo = current.MyClients;

                ReceivedOrder order = string.IsNullOrWhiteSpace(model.ID) ? new ReceivedOrder() : current.MyReceivedOrders.GetOrderDetail(model.ID);
                //订单信息
                order.ID = model.ID;
                order.ClientID = current.EnterpriseID;
                order.CreatorID = current.ID;
                order.Type = model.IsTransfer ? OrderType.Transport : OrderType.Recieved;
                order.PayeeID = PvWsOrder.Services.PvClientConfig.CompanyID;
                //order.BeneficiaryID = Alls.Current.CompanyPayee.ID;  //公司的受益人
                order.MainStatus = model.IsSubmit ? CgOrderStatus.已提交 : CgOrderStatus.暂存;
                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
                order.SupplierID = model.SupplierID;
                order.EnterCode = clientinfo.EnterCode;
                order.SettlementCurrency = (Currency)int.Parse(model.SettlementCurrency);

                PvWsOrder.Services.Models.OrderCondition orderCondition = new PvWsOrder.Services.Models.OrderCondition();
                var orderInputs = new OrderInput();
                var wayCondition = new WayCondition();  //货运条件

                //获取香港库房信息
                var hkWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
                var hkWarehouseEnterprise = Alls.Current.Company[hkWarehouse.Enterprise.ID];
                var contact1 = hkWarehouseEnterprise?.Contacts.FirstOrDefault();
                //香港提货地址
                var address = Alls.Current.SupplierConsignors[model.HKSupplierAddress];

                #region 代收

                wayCondition.PayForFreight = model.HKFreight; //是否垫付运费
                orderInputs.IsPayCharge = model.IsPay; //是否代付货款
                orderInputs.Conditions = orderCondition.Json();
                orderInputs.Currency = (Currency)int.Parse(model.Currency);
                order.Input = orderInputs;
                var supplier = Client.Current.MySupplier[model.SupplierID];
                order.InWaybill = new PvWsOrder.Services.ClientModels.Waybill()
                {
                    Consignee = new WayParter
                    {
                        Company = hkWarehouse.Name,
                        Address = hkWarehouse.Address,
                        Contact = contact1?.Name,
                        Phone = contact1?.Mobile,
                        Email = contact1?.Email,
                        Place = Origin.HKG.ToString(),
                    },
                    Consignor = new WayParter
                    {
                        Company = current.XDTClientName,
                        Address = current.MyClients.RegAddress,
                        Place = current.MyClients.Place,
                    },
                    WayLoading = (WaybillType)int.Parse(model.HKWaybillType) != WaybillType.PickUp ? null : new WayLoading
                    {
                        TakingDate = DateTime.Parse(model.HKTakingDateStr),
                        TakingAddress = address.Address,
                        TakingContact = address.Contact,
                        TakingPhone = address.Mobile,
                        CreatorID = current.ID,
                        ModifierID = current.ID,
                    },
                    Code = model.HKExpressNumber,
                    //Subcodes = model.HKExpressSubNumber,
                    Type = (WaybillType)int.Parse(model.HKWaybillType),
                    CarrierID = model.HKExpressName,
                    FreightPayer = model.HKFreight ? WaybillPayer.Consignee : WaybillPayer.Consignor,
                    EnterCode = clientinfo.EnterCode,
                    TotalWeight = model.GrossWeight,
                    TotalParts = model.PackNo,
                    CreatorID = current.ID,
                    ModifierID = current.ID,
                    //VoyageNumber = model.HKAirCode,
                    Condition = wayCondition.Json(),
                    ExcuteStatus = (int)CgSortingExcuteStatus.Sorting,
                    Supplier = supplier.RealEnterpriseName,
                    NoticeType = Services.Enums.CgNoticeType.Enter,
                    Source = model.IsTransfer ? Services.Enums.CgNoticeSource.Transfer : Services.Enums.CgNoticeSource.AgentEnter,
                };
                #endregion

                #region 代发
                if (model.IsTransfer)
                {
                    //条件
                    orderCondition = new PvWsOrder.Services.Models.OrderCondition();

                    wayCondition = new WayCondition { PayForFreight = model.Freight }; //货物条件
                    //是否垫付运费
                    //出库
                    var orderOutputs = new OrderOutput();
                    orderOutputs.IsReciveCharge = model.IsRecieve;
                    orderOutputs.Currency = (Currency)int.Parse(model.Currency);
                    orderOutputs.Conditions = orderCondition.Json();
                    order.Output = orderOutputs;
                    //运单
                    var delivery = (WaybillType)(int.Parse(model.WaybillType)); //交货方式
                    //收货人对象
                    var consignee = new WayParter()
                    {
                        Company = clientinfo.Name,
                        Place = Origin.HKG.ToString(),
                        Zipcode = model.ZipCode,
                    };
                    if (delivery == WaybillType.PickUp)
                    {
                        consignee.Contact = model.ClientPicker;
                        consignee.Phone = model.ClientPickerMobile;
                        consignee.Address = hkWarehouse.Address;
                        consignee.IDType = model.IDType == null ? IDType.IDCard : (IDType)int.Parse(model.IDType);
                        consignee.IDNumber = (IDType)int.Parse(model.IDType) == IDType.PickSeal ? model.SealContext : model.IDNumber;
                    }
                    else
                    {
                        consignee.Contact = model.ClientConsigneeName;
                        consignee.Phone = model.ClientContactMobile;
                        consignee.Address = model.ClientConsigneeAddress;
                        consignee.Place = Origin.HKG.ToString();

                        //如果是送货上门/快递，使用客户的客户公司名称
                        consignee.Company = model.ClientConsigneeCompany;
                    }
                    order.OutWaybill = new PvWsOrder.Services.ClientModels.Waybill()
                    {
                        Consignee = consignee,
                        Consignor = new WayParter()
                        {
                            Company = hkWarehouse.Name,
                            Address = hkWarehouse.Address,
                            Contact = contact1?.Name,
                            Phone = contact1?.Mobile,
                            Email = contact1?.Email,
                            Place = Origin.HKG.ToString(),
                        },
                        WayLoading = delivery != WaybillType.PickUp ? null : new WayLoading
                        {
                            TakingDate = model.PickupTimeStr == null ? SqlDateTime.MinValue.Value.AddHours(12).AddSeconds(1) : DateTime.Parse(model.PickupTimeStr),
                            TakingAddress = hkWarehouse.Address,
                            TakingContact = model.ClientPicker,
                            TakingPhone = model.ClientPickerMobile,
                            CreatorID = current.ID,
                            ModifierID = current.ID,
                        },
                        Type = delivery,
                        CarrierID = model.ExpressID,
                        FreightPayer = model.Freight ? WaybillPayer.Consignor : WaybillPayer.Consignee,
                        CreatorID = current.ID,
                        ModifierID = current.ID,
                        EnterCode = clientinfo.EnterCode,
                        Condition = wayCondition.Json(),
                        ExcuteStatus = (int)CgPickingExcuteStatus.Picking,
                        Status = GeneralStatus.Closed,
                        NoticeType = Services.Enums.CgNoticeType.Out,
                        Source = Services.Enums.CgNoticeSource.Transfer,
                    };
                }
                #endregion

                #region 订单项
                var orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition();
                //订单项    
                order.OrderItems = model.OrderItems.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
                {
                    Product = new CenterProduct
                    {
                        PartNumber = item.PartNumber.Trim(),
                        Manufacturer = item.Manufacturer.Trim(),
                        //PackageCase = item.PackageCase?.Trim(),
                    },
                    ID = item.ID,
                    Name = item.Name?.Trim(),
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
                    Quantity = item.Quantity,
                    Currency = (Currency)int.Parse(model.Currency),
                    UnitPrice = item.UnitPrice,
                    Unit = (LegalUnit)item.Unit,
                    TotalPrice = item.TotalPrice,
                    Conditions = orderItemCondition.Json(),
                    StorageID = item.StorageID,
                    DateCode = item.DateCode,
                }).ToArray();
                #endregion

                #region 附件
                //PI文件
                var files = model.PIFiles.Select(item => new CenterFileDescription { Type = (int)FileType.Invoice, CustomName = item.name, Url = item.URL, AdminID = current.ID }).ToList();

                //装箱单
                files.AddRange(model.PackingFiles.Select(item => new CenterFileDescription { Type = (int)FileType.FollowGoods, CustomName = item.name, Url = item.URL, AdminID = current.ID }));

                //提货文件
                files.AddRange(model.TakingFiles.Select(item => new Yahv.Services.Models.CenterFileDescription { Type = (int)FileType.Delivery, CustomName = item.name, Url = item.URL, AdminID = current.ID }));
                order.OrderFiles = files.ToArray();
                #endregion

                #region 发货特殊物品处理要求
                if (model.IsTransfer)  //即收即发的特殊要求
                {
                    var requireLists = new List<OrderRequirement>();
                    foreach (var item in model.SpecialGoods)
                    {
                        var require = new OrderRequirement
                        {
                            OrderID = order.ID,
                            Name = item.Name,
                            Quantity = item.Quantity ?? 0,
                            Requirement = item.Requirement,
                            TotalPrice = item.TotalPrice,
                            Type = (SpecialRequire)int.Parse(item.Type)
                        };

                        //保存文件
                        if (!string.IsNullOrWhiteSpace(item.FileName))
                        {
                            var file = new CenterFileDescription();
                            if ((SpecialRequire)int.Parse(item.Type) == SpecialRequire.Label) //标签文件
                            {
                                file.Type = (int)FileType.Label;
                            }
                            if ((SpecialRequire)int.Parse(item.Type) == SpecialRequire.ChangePackingFile) //换箱单
                            {
                                file.Type = (int)FileType.Packing;
                            }
                            file.CustomName = item.FileName;
                            file.Url = item.FileURL;
                            file.AdminID = current.ID;
                            require.RequireFiles = file;
                        }
                        requireLists.Add(require);
                    }
                    order.Requirements = requireLists.ToArray();
                }
                #endregion

                //保存订单
                order.Enter();

                #region 代付、代收
                //代付,代收历史数据删除
                if (!string.IsNullOrWhiteSpace(model.ID))
                {
                    current.MyApplictions.DeleteByOrderID(model.ID);
                }

                //代付
                if (model.IsPay)
                {
                    //我方付款信息
                    var companyPayer = Alls.Current.wsPayerAll.OrderByDescending(m => m.CreateDate)
                        .FirstOrDefault(a => a.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                                          && a.Status == GeneralStatus.Normal
                                          && a.Methord != Methord.Cash);
                    var companyPayee = Alls.Current.wsPayeeAll[model.PayCompanyBankID];
                    //代付
                    var Pay = new Application()
                    {
                        ClientID = current.EnterpriseID,
                        Type = ApplicationType.Payment,
                        TotalPrice = model.ApplyPrice,
                        Currency = (Currency)int.Parse(model.Currency),
                        InCompanyName = companyPayee?.EnterpriseName,
                        InBankAccount = companyPayee?.Account,
                        InBankName = companyPayee?.Bank,
                        OutCompanyName = companyPayer != null ? (companyPayer.EnterpriseName ?? "") : "",
                        OutBankAccount = companyPayer != null ? (companyPayer.Account ?? "") : "",
                        OutBankName = companyPayer != null ? (companyPayer.Bank ?? "") : "",
                        UserID = current.ID,
                    };

                    //根据 model.BankID (nPayeeID, 银行信息) 查询对应的供应商信息 Begin

                    var supplierInfoBynPayeeID = new Yahv.PvWsOrder.Services.ClientViews.SupplierViewBynPayeeID().Where(t => t.ID == model.BankID).FirstOrDefault();

                    //根据 model.BankID (nPayeeID, 银行信息) 查询对应的供应商信息 End

                    //供应商收款信息
                    Pay.PayPayee = new PvWsOrder.Services.Models.ApplicationPayee()
                    {
                        PayeeID = model.BankID,
                        EnterpriseID = supplierInfoBynPayeeID.RealEnterpriseID,
                        EnterpriseName = supplierInfoBynPayeeID.RealEnterpriseName,
                        BankAccount = model.SupplierBankAccount,
                        BankName = model.SupplierBankName,
                        Method = (Methord)int.Parse(model.SupplierMethod),
                        Currency = (Currency)int.Parse(model.Currency),
                        Amount = model.ApplyPrice,
                    };
                    //客户付款信息
                    Pay.PayPayer = null;
                    var payerinfo = Client.Current.MyPayers[model.PayPayerID];
                    if (payerinfo != null)
                    {
                        var paycompanyid = payerinfo?.RealEnterpriseID ?? current.EnterpriseID;
                        var paycompanyname = payerinfo?.RealEnterpriseName ?? current.XDTClientName;

                        Pay.PayPayer = new PvWsOrder.Services.Models.ApplicationPayer()
                        {
                            PayerID = model.PayPayerID,
                            EnterpriseID = paycompanyid, //payerinfo.RealEnterpriseID ?? payerinfo.EnterpriseID,
                            EnterpriseName = paycompanyname, //payerinfo.RealEnterpriseName ?? payerinfo.EnterpriseName,
                            BankAccount = payerinfo?.Account,
                            BankName = payerinfo?.Bank,
                            Method = (Methord)model.PayPayerMethod,
                            Currency = (Currency)model.PayPayerCurrency,
                            Amount = model.ApplyPrice,
                        };
                    }

                    //申请项
                    var item = new PvWsOrder.Services.Models.ApplicationItem()
                    {
                        OrderID = order.ID,
                        Amount = model.ApplyPrice,
                    };
                    Pay.Items = new PvWsOrder.Services.Models.ApplicationItem[] { item };
                    Pay.FileItems = model.PIFiles.Select(a => new CenterFileDescription { Type = (int)FileType.Invoice, CustomName = a.name, Url = a.URL, AdminID = current.ID }).ToList();
                    Pay.Sumbit();
                }

                //代收
                if (model.IsRecieve)
                {
                    //我方付款信息
                    var companyPayer = Alls.Current.wsPayerAll.OrderByDescending(m => m.CreateDate)
                        .FirstOrDefault(a => a.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                                          && a.Status == GeneralStatus.Normal
                                          && a.Methord != Methord.Cash);
                    var companyPayee = Alls.Current.wsPayeeAll[model.CompanyBankID];
                    var receive = new Application()
                    {
                        ClientID = current.EnterpriseID,
                        Type = ApplicationType.Receival,
                        TotalPrice = model.RecievePrice,
                        Currency = (Currency)(int.Parse(model.Currency)),  //(Currency)model.PayerCurrency,
                        InCompanyName = companyPayee?.EnterpriseName,
                        InBankAccount = companyPayee?.Account,
                        InBankName = companyPayee?.Bank,
                        OutCompanyName = companyPayer != null ? (companyPayer.EnterpriseName ?? "") : "",
                        OutBankAccount = companyPayer != null ? (companyPayer.Account ?? "") : "",
                        OutBankName = companyPayer != null ? (companyPayer.Bank ?? "") : "",
                        IsEntry = model.IsEntry,
                        UserID = current.ID,
                        CheckTitle = model.CheckTitle,
                        CheckPayeeAccount = model.CheckPayeeAccount,
                        CheckCarrier = model.CheckCarrier,
                        CheckConsignee = model.CheckConsignee,
                    };

                    if (!string.IsNullOrEmpty(model.DelivaryOpportunity))
                    {
                        receive.DelivaryOpportunity = (DelivaryOpportunity)int.Parse(model.DelivaryOpportunity);
                    }
                    if (!string.IsNullOrEmpty(model.CheckDelivery))
                    {
                        receive.CheckDelivery = (CheckDeliveryType)int.Parse(model.CheckDelivery);
                    }

                    //客户付款信息
                    var payerinfo = Client.Current.MyPayers[model.PayerID];
                    receive.ReceivePayer = new PvWsOrder.Services.Models.ApplicationPayer()
                    {
                        PayerID = model.PayerID,
                        EnterpriseID = payerinfo.RealEnterpriseID ?? payerinfo.EnterpriseID,
                        EnterpriseName = payerinfo.RealEnterpriseName ?? payerinfo.EnterpriseName,
                        BankAccount = payerinfo.Account,
                        BankName = payerinfo.Bank,
                        Method = (Methord)model.PayerMethod,
                        Currency = (Currency)model.PayerCurrency,
                        Amount = model.RecievePrice,
                    };

                    //申请项
                    var item = new PvWsOrder.Services.Models.ApplicationItem()
                    {
                        OrderID = order.ID,
                        Amount = model.RecievePrice,
                    };
                    receive.Items = new PvWsOrder.Services.Models.ApplicationItem[] { item };
                    receive.FileItems = model.PIFiles.Select(a => new CenterFileDescription { Type = (int)FileType.Invoice, CustomName = a.name, Url = a.URL, AdminID = current.ID }).ToList();
                    receive.Sumbit();
                }
                #endregion
                return JsonResult(VueMsgType.success, "新增成功", order.ID);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 代收货订单详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult StorageDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            //查询订单详情数据
            var order = Client.Current.MyReceivedOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            //代收自提文件
            var deliveryFile = order.OrderFiles.FirstOrDefault(item => item.Type == (int)FileType.Delivery);
            var _deliveryFile = "";
            if (deliveryFile != null)
            {
                _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
            }
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            //装箱单
            var PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            #endregion

            var orderbills = Client.Current.MyOrderBills.SearchByOrderID(order.ID).ToArray().Select(item => new
            {
                item.Catalog,
                item.Subject,
                Quantity = 1,
                item.Summay,
                Price = item.LeftPrice,
                TotalPrice = item.LeftPrice,
                item.CurrencyName
            }).ToArray();

            //香港仓库信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
            var contact1 = HKWarehouseEnterprise?.Contacts.FirstOrDefault();

            //代付货款信息
            PayDetailInfo payDetailInfo = null;

            if (order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge)
            {
                var payDetail = Client.Current.MyApplictions.GetPayDetailByOrderID(id);
                string bankID = payDetail.PayPayee.PayeeID;  //账户名称 BankID
                var supplierPayee = Alls.Current.SupplierPayees.SingleOrDefault(item => item.ID == bankID);  //账户名称用

                string payPayerID = payDetail.PayPayer?.PayerID;  //付款人 PayPayerID
                var payer = Client.Current.MyPayers[payPayerID];  //付款人、付款方式、付款人币种用

                //收款账户信息
                var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouse.Enterprise.ID);
                string CompanyBankAccountName = companybank?.RealEnterpriseName;
                string CompanyBankAccount = companybank?.Account;
                string CompanyBankName = companybank?.Bank;
                //string CompanyBankID = companybank?.ID;

                payDetailInfo = new PayDetailInfo
                {
                    TotalPrice = payDetail.TotalPrice, //付款金额
                    AccountName = supplierPayee.Contact ?? supplierPayee.RealEnterpriseName,  //账户名称
                    SupplierMethodDes = payDetail.PayPayee.Method.GetDescription(), //支付方式
                    SupplierName = payDetail.PayPayee.EnterpriseName,  //企业名称
                    SupplierBankName = payDetail.PayPayee.BankName,  //银行名称
                    PayerName = string.IsNullOrEmpty(payer?.Contact) ? payer?.EnterpriseName : payer?.Contact, //付款人
                    SupplierBankAccount = payDetail.PayPayee.BankAccount,  //银行账号
                    MethordInt = payer != null ? (int)payer.Methord : (int)Methord.Cash, //付款方式Int
                    MethordDec = payer != null ? payer.MethordDec : Methord.Cash.GetDescription(),  //付款方式名称
                    CurrencyDec = payer != null ? payer.CurrencyDec : Currency.USD.GetDescription(),  //付款人币种
                    CompanyBankAccountName = CompanyBankAccountName,  //收款账户
                    CompanyBankAccount = CompanyBankAccount,  //收款账号
                    CompanyBankName = CompanyBankName,  //收款银行
                };
            }

            var model = new
            {
                order.ID,
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Input.Currency.GetDescription(),
                order.InWaybill.TotalParts,
                order.InWaybill.TotalWeight,
                order.SupplierName,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsPayName = (order.Input != null && order.Input.IsPayCharge.GetValueOrDefault()) ? "是" : "否",
                IsPay = order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge,
                HKWaybillType = order.InWaybill.Type,
                HKWaybillTypeName = order.InWaybill.Type.GetDescription(),
                TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                order.InWaybill.WayLoading?.TakingContact,
                order.InWaybill.WayLoading?.TakingPhone,
                order.InWaybill.WayLoading?.TakingAddress,
                WareHouseAddress = HKWarehouse.Address,
                WareHouseName = contact1?.Name,
                WareHouseTel = contact1?.Tel ?? contact1?.Mobile,
                HKFreight = order.InWaybill.WayCondition.PayForFreight ? "是" : "否",
                HKExpressNumber = order.InWaybill.Code,
                //HKExpressSubNumber = order.InWaybill.Subcodes,
                order.InWaybill.CarrierName,
                IsTransfer = "否",
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.ID,
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Name,
                    Unit = item.Unit.GetDescription(),
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    Origin = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                    item.Product.PackageCase,
                    item.DateCode,
                }).ToArray(),
                totalNum = order.OrderItems.Sum(item => item.Quantity),
                totalPrice = order.OrderItems.Sum(item => item.TotalPrice),
                Bills = orderbills,
                sumTotalPrice = orderbills.Sum(item => (decimal?)item.TotalPrice).GetValueOrDefault(),
                PIFiles,
                PackingFiles,
                DeliveryFile = _deliveryFile,
                IsShow = order.PaymentStatus > OrderPaymentStatus.Confirm,
                PayDetailInfo = payDetailInfo,
            };

            return View(model);
        }


        /// <summary>
        /// 即收即发订单详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult StorageTransDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            //查询订单详情数据
            var order = Client.Current.MyReceivedOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            //代收自提文件
            var deliveryFile = order.OrderFiles.FirstOrDefault(item => item.Type == (int)FileType.Delivery);
            var _deliveryFile = "";
            if (deliveryFile != null)
            {
                _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
            }
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            //装箱单
            var PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            #endregion

            var orderbills = Client.Current.MyOrderBills.SearchByOrderID(order.ID).ToArray().Select(item => new
            {
                item.Catalog,
                item.Subject,
                Quantity = 1,
                item.Summay,
                Price = item.LeftPrice,
                TotalPrice = item.LeftPrice,
                item.CurrencyName
            }).ToArray();

            //香港仓库信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
            var contact1 = HKWarehouseEnterprise?.Contacts.FirstOrDefault();
            //收款账户信息
            var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouseEnterprise.ID);

            //特殊要求
            var requirements = order.Requirements.Select(item => new
            {
                TypeName = item.Type.GetDescription(),
                item.Name,
                item.Quantity,
                item.TotalPrice,
                item.Requirement,
                FileName = item.RequireFiles?.CustomName,
                FileUrl = fileurl + item.RequireFiles?.Url.ToUrl()
            });

            //代付货款信息
            PayDetailInfo payDetailInfo = null;

            if (order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge)
            {
                var payDetail = Client.Current.MyApplictions.GetPayDetailByOrderID(id);
                string bankID = payDetail.PayPayee.PayeeID;  //账户名称 BankID
                var supplierPayee = Alls.Current.SupplierPayees.SingleOrDefault(item => item.ID == bankID);  //账户名称用

                string payPayerID = payDetail.PayPayer.PayerID;  //付款人 PayPayerID
                var payer = Client.Current.MyPayers[payPayerID];  //付款人、付款方式、付款人币种用

                //收款账户信息
                //var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouse.Enterprise.ID);
                string CompanyBankAccountName = companybank?.RealEnterpriseName;
                string CompanyBankAccount = companybank?.Account;
                string CompanyBankName = companybank?.Bank;
                //string CompanyBankID = companybank?.ID;

                payDetailInfo = new PayDetailInfo
                {
                    TotalPrice = payDetail.TotalPrice, //付款金额
                    AccountName = supplierPayee.Contact ?? supplierPayee.RealEnterpriseName,  //账户名称
                    SupplierMethodDes = payDetail.PayPayee.Method.GetDescription(), //支付方式
                    SupplierName = payDetail.PayPayee.EnterpriseName,  //企业名称
                    SupplierBankName = payDetail.PayPayee.BankName,  //银行名称
                    PayerName = string.IsNullOrEmpty(payer.Contact) ? payer.EnterpriseName : payer.Contact, //付款人
                    SupplierBankAccount = payDetail.PayPayee.BankAccount,  //银行账号
                    MethordInt = (int)payer.Methord, //付款方式Int
                    MethordDec = payer.MethordDec,  //付款方式名称
                    CurrencyDec = payer.CurrencyDec,  //付款人币种
                    CompanyBankAccountName = CompanyBankAccountName,  //收款账户
                    CompanyBankAccount = CompanyBankAccount,  //收款账号
                    CompanyBankName = CompanyBankName,  //收款银行
                };
            }

            //代收货款信息
            ReceiveDetailInfo receiveDetailInfo = null;

            if (order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge)
            {
                var receiveDetail = Client.Current.MyApplictions.GetReceiveDetailByID(id);
                string payerID = receiveDetail.ReceivePayer.PayerID;  //付款人 PayerID
                var receivePayer = Client.Current.MyPayers[payerID];  ////付款人、付款方式、付款人币种用

                receiveDetailInfo = new ReceiveDetailInfo
                {
                    ReceivePrice = receiveDetail.TotalPrice,
                    PayerName = string.IsNullOrEmpty(receivePayer.Contact) ? receivePayer.EnterpriseName : receivePayer.Contact,
                    PayerMethodInt = (int)receivePayer.Methord,
                    PayerMethodName = receivePayer.MethordDec,
                    PayerCurrencyName = receivePayer.CurrencyDec,
                    DelivaryOpportunityDes = receiveDetail.DelivaryOpportunity != null ? receiveDetail.DelivaryOpportunity.GetDescription() : "",
                    IsEntryInt = receiveDetail.IsEntry ? 1 : 0,
                    IsEntryDes = receiveDetail.IsEntry ? "是" : "否",
                    CheckDeliveryDes = receiveDetail.CheckDelivery != null ? receiveDetail.CheckDelivery.GetDescription() : "",
                };
            }

            var model = new
            {
                order.ID,
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Input.Currency.GetDescription(),
                order.InWaybill.TotalParts,
                order.InWaybill.TotalWeight,
                order.SupplierName,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsPayName = order.Input.IsPayCharge.GetValueOrDefault() ? "是" : "否",
                IsPay = order.Input.IsPayCharge,
                IsRecieveName = (order.Output != null && order.Output.IsReciveCharge.GetValueOrDefault()) ? "是" : "否",
                IsRecieve = order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge,
                HKWaybillType = order.InWaybill.Type,
                HKWaybillTypeName = order.InWaybill.Type.GetDescription(),
                TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                order.InWaybill.WayLoading?.TakingContact,
                order.InWaybill.WayLoading?.TakingPhone,
                order.InWaybill.WayLoading?.TakingAddress,
                WareHouseAddress = HKWarehouse.Address,
                WareHouseName = contact1?.Name,
                WareHouseTel = contact1?.Tel ?? contact1?.Mobile,
                HKFreight = order.InWaybill.WayCondition.PayForFreight ? "是" : "否",
                HKExpressNumber = order.InWaybill.Code,
                //HKExpressSubNumber = order.InWaybill.Subcodes,
                HKCarrierName = order.InWaybill.CarrierName,
                IsTransfer = "是",
                WaybillType = order.OutWaybill.Type,
                WaybillTypeName = order.OutWaybill.Type.GetDescription(),
                CompanyBankAccountName = companybank?.RealEnterpriseName,
                CompanyBankAccount = companybank?.Account,
                CompanyBankName = companybank?.Bank,
                PickupTime = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                ClientPicker = order.OutWaybill.Consignee?.Contact,
                ClientPickerMobile = order.OutWaybill.Consignee?.Phone,
                IDType = order.OutWaybill.Consignee.IDType?.GetDescription(),
                IDNumber = order.OutWaybill.Consignee.IDNumber,
                SealContext = order.OutWaybill.Consignee.IDNumber,
                ClientConsigneeName = order.OutWaybill.Consignee.Contact,
                ClientContactMobile = order.OutWaybill.Consignee.Phone,
                ClientConsigneeAddress = order.OutWaybill.Consignee.Address,
                CarrierName = order.OutWaybill.CarrierName,
                ZipCode = order.OutWaybill.Consignee.Zipcode,
                Freight = order.OutWaybill.WayCondition.PayForFreight ? "是" : "否",
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.ID,
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Name,
                    Unit = item.Unit.GetDescription(),
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    Origin = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                    item.Product.PackageCase,
                    item.DateCode,
                }).ToArray(),
                totalNum = order.OrderItems.Sum(item => item.Quantity),
                totalPrice = order.OrderItems.Sum(item => item.TotalPrice),
                Bills = orderbills,
                sumTotalPrice = orderbills.Sum(item => (decimal?)item.TotalPrice).GetValueOrDefault(),
                PIFiles,
                PackingFiles,
                DeliveryFile = _deliveryFile,
                IsShow = order.PaymentStatus > OrderPaymentStatus.Confirm,
                Requirements = requirements,
                PayDetailInfo = payDetailInfo,
                ReceiveDetailInfo = receiveDetailInfo,
            };
            return View(model);
        }
        #endregion


        #region 代发货新增、提交、详情
        /// <summary>
        /// 新增代发订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeliveryAdd()
        {
            var current = Client.Current;
            var model = new DeliveryOrdersModel();
            model.OrderItems = new Models.OrderItem[0];
            model.SpecialGoods = new SpecialGoodsModel[0];
            model.IsSubmit = false;
            model.PIFiles = new FileModel[0];
            model.Origin = (Origin.HKG).ToString(); //默认中国香港
            model.WaybillType = ((int)WaybillType.PickUp).ToString(); //默认自提
            model.IsEntry = true;

            #region 是否由我的库存跳转

            var products = Request.Form["products"];
            if (!string.IsNullOrWhiteSpace(products))
            {
                var productList = JsonConvert.DeserializeObject<StorageListViewModel[]>(products).Select(item => new
                {
                    item.ID,
                    item.SaleQuantity,
                    item.CurrencyShortName,
                    item.CurrencyInt,
                }).ToArray();
                var productListIDs = productList.Select(item => item.ID).ToArray();

                var storageList = Yahv.Client.Current.MyStorages.Where(t => productListIDs.Contains(t.ID)).ToArray();
                model.OrderItems = (from storage in storageList
                                    join product in productList on storage.ID equals product.ID
                                    select new Models.OrderItem
                                    {
                                        StorageID = storage.ID, //StorageID
                                        Name = "", //Name - 品名
                                        Manufacturer = storage.Product.Manufacturer, //Manufacturer - 品牌
                                        PartNumber = storage.Product.PartNumber, //PartNumber - 型号
                                        Origin = storage.Origin, //Origin - 产地
                                        Quantity = product.SaleQuantity, //Quantity - 数量
                                        StockNum = storage.Quantity, //StockNum - 库存数量
                                        Unit = (int)LegalUnit.个, //Unit - 单位
                                        TotalPrice = 0, //TotalPrice - 总价
                                        UnitPrice = 0, //UnitPrice - 单价
                                        InputID = storage.InputID, //InputID
                                        CurrencyShortName = product.CurrencyShortName, //币种 ShortName
                                        CurrencyInt = product.CurrencyInt, //币种 Int
                                    }).ToArray();

                var currency = ExtendsEnum.ToArray<Currency>().FirstOrDefault(item => item.ToString() == productList.First().CurrencyShortName);
                model.Currency = ((int)currency).ToString();
            }

            #endregion

            //香港仓库默认信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
            var contact = HKWarehouseEnterprise.Contacts.FirstOrDefault(); //联系人
            //我方收款账户信息
            var payees = Alls.Current.wsPayeeAll.OrderByDescending(m => m.CreateDate)
                .Where(item => item.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                            && item.Status == GeneralStatus.Normal
                            && item.Methord != Methord.Cash)
                .Select(item => new
                {
                    item.ID,
                    item.EnterpriseName,
                    item.Account,
                    item.Bank,
                    item.BankAddress,
                    item.SwiftCode,

                    ForCopy = "账户：" + item.EnterpriseName + "\r\n"
                            + "账号：" + item.Account + "\r\n"
                            + "银行：" + item.Bank + "\r\n"
                            + "银行地址：" + item.BankAddress + "\r\n"
                            + "SWIFTCODE：" + item.SwiftCode,  //用于复制
                }).Take(2).ToList();
            model.CompanyBankID = payees.First()?.ID;
            var data = new
            {
                ReceiveOptions = current.MyConsignees.Select(item => new { value = item.ID, text = item.Title, address = item.Address, name = item.Name, mobile = item.Mobile }).ToArray(),
                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                WaybillTypeOptions = ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0").Select(item => new
                {
                    value = item.Key,
                    text = item.Key == "1" ? "客户自提" : item.Value,
                }).ToArray(),
                UnitOptions = UnitEnumHelper.ToUnitDictionary()
                .Where(t => WidelyUsedUnit.Values.Contains(t.Value))
                .Select(item => new { value = item.Value, text = item.Name }).ToArray(), // + " (" + item.Code + ")"
                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "ZZZ" && item.Value != "NG" && item.Value != "Unknown")
                .Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).ToArray(),
                CarrierOptions = Yahv.Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
                PayCurrencyOptions = // ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0")
                new[] { Currency.CNY, Currency.USD, Currency.HKD, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                SetCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key == "1" || item.Key == "2" || item.Key == "3")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                DelivaryOppOptions = ExtendsEnum.ToDictionary<DelivaryOpportunity>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                CheckDelTypeOptions = ExtendsEnum.ToDictionary<CheckDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayeeOptions = payees,

                // 去除“新增付款人”弹框中“付款方式”和“付款币种”后，在外面下拉选择的选项
                PayerMethordOptions = ExtendsEnum.ToDictionary<Methord>()
                    .Where(c => c.Key != ((int)Methord.TT).ToString()
                        && c.Key != ((int)Methord.Alipay).ToString()
                        && c.Key != ((int)Methord.Exchange).ToString())
                    .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayerCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
            };
            ViewBag.Options = data;

            return View(model);
        }

        /// <summary>
        /// 代发货订单提交
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult DeliverySubmit(string data)
        {
            try
            {
                var model = data.JsonTo<DeliveryOrdersModel>();
                var current = Client.Current;
                var clientInfo = current.MyClients;

                //代发货订单初始化
                var order = new DeliveryOrder();
                order.ClientID = current.EnterpriseID;
                order.Type = OrderType.Delivery;
                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
                order.PayeeID = PvWsOrder.Services.PvClientConfig.CompanyID;
                //order.BeneficiaryID = Yahv.Alls.Current.CompanyPayee.ID;  //公司的受益人
                order.CreatorID = current.ID;
                order.EnterCode = clientInfo.EnterCode;
                order.MainStatus = CgOrderStatus.已提交;
                order.SettlementCurrency = (Currency)int.Parse(model.SettlementCurrency);

                #region 香港交货信息
                //获取香港库房信息
                var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];

                //出库条件
                var orderCondition = new PvWsOrder.Services.Models.OrderCondition();
                var wayCondition = new Services.Models.WayCondition();  //货物条件
                wayCondition.PayForFreight = model.Freight; //是否垫付运费

                var orderOutputs = new OrderOutput();
                orderOutputs.IsReciveCharge = model.IsRecieve;
                orderOutputs.Currency = (Currency)int.Parse(model.Currency);
                orderOutputs.Conditions = orderCondition.Json();
                order.Output = orderOutputs;

                //运单
                var delivery = (WaybillType)(int.Parse(model.WaybillType)); //交货方式
                //收货人对象
                var consignee = new WayParter()
                {
                    Company = clientInfo.Name,
                    Place = Origin.HKG.ToString(),
                    Zipcode = model.ZipCode,
                };
                if (delivery == WaybillType.PickUp)//自提
                {
                    consignee.Contact = model.ClientPicker;
                    consignee.Phone = model.ClientPickerMobile;
                    consignee.Address = HKWarehouse.Address;
                    consignee.IDNumber = (IDType)int.Parse(model.IDType) == IDType.PickSeal ? model.SealContext : model.IDNumber;
                    consignee.IDType = (IDType)int.Parse(model.IDType);
                }
                else//快递，送货上门
                {
                    consignee.Contact = model.ClientConsigneeName;
                    consignee.Phone = model.ClientContactMobile;
                    consignee.Address = model.ClientConsigneeAddress;

                    //如果是送货上门/快递，使用客户的客户公司名称
                    consignee.Company = model.ClientConsigneeCompany;
                }
                order.OutWaybill = new PvWsOrder.Services.ClientModels.Waybill()
                {
                    Consignee = consignee,
                    Consignor = new WayParter()
                    {
                        Company = HKWarehouse.Name,
                        Address = HKWarehouse.Address,
                        Place = Origin.HKG.ToString(),
                    },
                    WayLoading = delivery != WaybillType.PickUp ? null : new Services.Models.WayLoading
                    {
                        TakingDate = DateTime.Parse(model.PickupTimeStr),
                        TakingAddress = HKWarehouse.Address,
                        TakingContact = model.ClientPicker,
                        TakingPhone = model.ClientPickerMobile,
                        CreatorID = current.ID,
                        ModifierID = current.ID,
                    },
                    Type = delivery,
                    CarrierID = model.ExpressID,
                    FreightPayer = model.Freight ? WaybillPayer.Consignor : WaybillPayer.Consignee,
                    CreatorID = current.ID,
                    ModifierID = current.ID,
                    EnterCode = current.MyClients.EnterCode,
                    Condition = wayCondition.Json(),
                    ExcuteStatus = (int)CgPickingExcuteStatus.Picking,
                    //Status = GeneralStatus.Closed,
                    NoticeType = Services.Enums.CgNoticeType.Out,
                    Source = Services.Enums.CgNoticeSource.AgentSend,
                    TotalParts = model.PackNo,
                    TotalWeight = model.GrossWeight
                };
                #endregion

                #region 订单项
                var orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition();
                //订单项    
                order.OrderItems = model.OrderItems.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
                {
                    Product = new Yahv.Services.Models.CenterProduct
                    {
                        PartNumber = item.PartNumber.Trim(),
                        Manufacturer = item.Manufacturer.Trim(),
                    },
                    Name = item.Name?.Trim(),
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
                    Quantity = item.Quantity,
                    Currency = (Currency)int.Parse(model.Currency),
                    UnitPrice = item.UnitPrice,
                    Unit = (LegalUnit)item.Unit,
                    TotalPrice = item.TotalPrice,
                    StorageID = item.StorageID,
                    Conditions = orderItemCondition.Json(),
                }).ToArray();
                #endregion

                #region 附件
                List<Yahv.Services.Models.CenterFileDescription> files = new List<Yahv.Services.Models.CenterFileDescription>();
                //PI文件
                foreach (var item in model.PIFiles)
                {
                    var file = new Yahv.Services.Models.CenterFileDescription();
                    file.Type = (int)FileType.Invoice;
                    file.CustomName = item.name;
                    file.Url = item.URL;
                    file.AdminID = current.ID;
                    files.Add(file);
                }
                order.OrderFiles = files.ToArray();
                #endregion

                #region 发货特殊物品处理要求
                var requireLists = new List<OrderRequirement>();
                foreach (var item in model.SpecialGoods)
                {
                    var require = new OrderRequirement
                    {
                        OrderID = order.ID,
                        Name = item.Name,
                        Quantity = item.Quantity ?? 0,
                        Requirement = item.Requirement,
                        TotalPrice = item.TotalPrice,
                        Type = (SpecialRequire)int.Parse(item.Type)
                    };

                    //保存文件
                    if (!string.IsNullOrWhiteSpace(item.FileName))
                    {
                        var file = new CenterFileDescription();
                        if ((SpecialRequire)int.Parse(item.Type) == SpecialRequire.Label) //标签文件
                        {
                            file.Type = (int)FileType.Label;
                        }
                        if ((SpecialRequire)int.Parse(item.Type) == SpecialRequire.ChangePackingFile) //换箱单
                        {
                            file.Type = (int)FileType.Packing;
                        }
                        file.CustomName = item.FileName;
                        file.Url = item.FileURL;
                        file.AdminID = current.ID;
                        require.RequireFiles = file;
                    }
                    requireLists.Add(require);
                }
                order.Requirements = requireLists.ToArray();
                #endregion

                #region 代收货款
                //代收
                if (model.IsRecieve)
                {
                    //我方付款信息
                    var companyPayer = Alls.Current.wsPayerAll.OrderByDescending(m => m.CreateDate)
                        .FirstOrDefault(a => a.EnterpriseID == Yahv.PvWsOrder.Services.PvClientConfig.CYCompanyID 
                                          && a.Status == GeneralStatus.Normal
                                          && a.Methord != Methord.Cash);
                    var companyPayee = Alls.Current.wsPayeeAll[model.CompanyBankID];
                    order.Receive = new Application()
                    {
                        ClientID = current.EnterpriseID,
                        Type = ApplicationType.Receival,
                        TotalPrice = model.RecievePrice,
                        Currency = (Currency)(int.Parse(model.Currency)), //(Currency)(model.PayerCurrency.Value),
                        InCompanyName = companyPayee?.EnterpriseName,
                        InBankAccount = companyPayee?.Account,
                        InBankName = companyPayee?.Bank,
                        OutCompanyName = companyPayer != null ? (companyPayer.EnterpriseName ?? "") : "",
                        OutBankAccount = companyPayer != null ? (companyPayer.Account ?? "") : "",
                        OutBankName = companyPayer != null ? (companyPayer.Bank ?? "") : "",
                        IsEntry = model.IsEntry,
                        UserID = current.ID,
                        CheckTitle = model.CheckTitle,
                        CheckPayeeAccount = model.CheckPayeeAccount,
                        CheckCarrier = model.CheckCarrier,
                        CheckConsignee = model.CheckConsignee,
                    };

                    if (!string.IsNullOrEmpty(model.DelivaryOpportunity))
                    {
                        order.Receive.DelivaryOpportunity = (DelivaryOpportunity)int.Parse(model.DelivaryOpportunity);
                    }
                    if (!string.IsNullOrEmpty(model.CheckDelivery))
                    {
                        order.Receive.CheckDelivery = (CheckDeliveryType)int.Parse(model.CheckDelivery);
                    }

                    //客户付款信息
                    var payerinfo = Client.Current.MyPayers[model.PayerID];
                    order.Receive.ReceivePayer = new PvWsOrder.Services.Models.ApplicationPayer()
                    {
                        PayerID = model.PayerID,
                        EnterpriseID = payerinfo.RealEnterpriseID ?? payerinfo.EnterpriseID,
                        EnterpriseName = payerinfo.RealEnterpriseName ?? payerinfo.EnterpriseName,
                        BankAccount = payerinfo.Account,
                        BankName = payerinfo.Bank,
                        Method = (Methord)model.PayerMethod,
                        Currency = (Currency)model.PayerCurrency,
                        Amount = model.RecievePrice,
                    };

                    //附件
                    order.Receive.FileItems = model.PIFiles.Select(a => new CenterFileDescription { Type = (int)FileType.Invoice, CustomName = a.name, Url = a.URL, AdminID = current.ID }).ToList();
                }

                //保存订单
                order.Enter();
                #endregion

                return JsonResult(VueMsgType.success, "新增成功", order.ID);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 代发货订单详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeliveryDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            //查询订单详情数据
            var order = Client.Current.MyDeliveryOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            #endregion

            var orderbills = Client.Current.MyOrderBills.SearchByOrderID(order.ID).ToArray().Select(item => new
            {
                item.Catalog,
                item.Subject,
                Quantity = 1,
                item.Summay,
                Price = item.LeftPrice,
                TotalPrice = item.LeftPrice,
                item.CurrencyName
            }).ToArray();

            //代收货款
            //var receiveapplication = Client.Current.MyApplictions.GetReceiveApplication().FirstOrDefault(item => item.OrderID == order.ID);
            //var payer = Client.Current.MyPayers[receiveapplication?.PayerID];

            //香港仓库收款账户默认信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouse.Enterprise.ID);

            //特殊要求
            var requirements = order.Requirements.Select(item => new
            {
                TypeName = item.Type.GetDescription(),
                item.Name,
                item.Quantity,
                item.TotalPrice,
                item.Requirement,
                FileName = item.RequireFiles?.CustomName,
                FileUrl = fileurl + item.RequireFiles?.Url.ToUrl()
            });

            //代收货款信息
            ReceiveDetailInfo receiveDetailInfo = null;

            if (order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge)
            {
                var receiveDetail = Client.Current.MyApplictions.GetReceiveDetailByID(id);
                string payerID = receiveDetail.ReceivePayer.PayerID;  //付款人 PayerID
                var receivePayer = Client.Current.MyPayers[payerID];  ////付款人、付款方式、付款人币种用

                receiveDetailInfo = new ReceiveDetailInfo
                {
                    ReceivePrice = receiveDetail.TotalPrice,
                    PayerName = string.IsNullOrEmpty(receivePayer.Contact) ? receivePayer.EnterpriseName : receivePayer.Contact,
                    PayerMethodInt = (int)receivePayer.Methord,
                    PayerMethodName = receivePayer.MethordDec,
                    PayerCurrencyName = receivePayer.CurrencyDec,
                    DelivaryOpportunityDes = receiveDetail.DelivaryOpportunity != null ? receiveDetail.DelivaryOpportunity.GetDescription() : "",
                    IsEntryInt = receiveDetail.IsEntry ? 1 : 0,
                    IsEntryDes = receiveDetail.IsEntry ? "是" : "否",
                    CheckDeliveryDes = receiveDetail.CheckDelivery != null ? receiveDetail.CheckDelivery.GetDescription() : "",
                };
            }

            var model = new
            {
                order.ID,
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Output.Currency.GetDescription(),
                order.OutWaybill.TotalParts,
                order.OutWaybill.TotalWeight,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsRecieveName = (order.Output != null && order.Output.IsReciveCharge.GetValueOrDefault()) ? "是" : "否",
                IsRecieve = order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge,
                //IsRecieve = receiveapplication != null,
                //IsRecieveName = receiveapplication != null ? "是" : "否",
                //RecievePrice = receiveapplication?.Price,
                //PayerName = payer?.Contact,
                //PayerBank = payer?.Bank,
                //PayerAccount = payer?.Account,
                CompanyBankName = companybank?.RealEnterpriseName,
                CompanyBank = companybank?.Bank,
                CompanyBankAccount = companybank?.Account,
                IsTransfer = "是",
                WaybillType = order.OutWaybill.Type,
                WaybillTypeName = order.OutWaybill.Type.GetDescription(),
                PickupTime = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                ClientPicker = order.OutWaybill.Consignee?.Contact,
                ClientPickerMobile = order.OutWaybill.Consignee?.Phone,
                IDType = order.OutWaybill.Consignee.IDType?.GetDescription(),
                IDNumber = order.OutWaybill.Consignee.IDNumber,
                SealContext = order.OutWaybill.Consignee.IDNumber,
                ClientConsigneeName = order.OutWaybill.Consignee.Contact,
                ClientContactMobile = order.OutWaybill.Consignee.Phone,
                ClientConsigneeAddress = order.OutWaybill.Consignee.Address,
                CarrierName = order.OutWaybill.CarrierName,
                ZipCode = order.OutWaybill.Consignee.Zipcode,
                Freight = order.OutWaybill.WayCondition.PayForFreight ? "是" : "否",
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.ID,
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Name,
                    Unit = item.Unit.GetDescription(),
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    Origin = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                }).ToArray(),
                totalNum = order.OrderItems.Sum(item => item.Quantity),
                totalPrice = order.OrderItems.Sum(item => item.TotalPrice),
                Bills = orderbills,
                sumTotalPrice = orderbills.Sum(item => (decimal?)item.TotalPrice).GetValueOrDefault(),
                PIFiles,
                IsShow = order.PaymentStatus > OrderPaymentStatus.Confirm,
                Requirements = requirements,
                ReceiveDetailInfo = receiveDetailInfo,
            };
            return View(model);
        }
        #endregion


        #region 待确认订单详情
        /// <summary>
        /// 待确认收货订单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult StorageConfirm()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            //查询订单详情数据
            var order = Client.Current.MyReceivedOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            //代收自提文件
            var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).FirstOrDefault();
            var _deliveryFile = "";
            if (deliveryFile != null)
            {
                _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
            }
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            //装箱单
            var PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            #endregion

            var orderbills = Client.Current.MyOrderBills.SearchByOrderID(order.ID).ToArray().Select(item => new
            {
                item.Catalog,
                item.Subject,
                Quantity = 1,
                item.Summay,
                Price = item.LeftPrice,
                TotalPrice = item.LeftPrice,
                item.CurrencyName
            }).ToArray();

            //代付货款
            //var application = Client.Current.MyApplictions.GetPayApplication().Where(item => item.OrderID == order.ID).FirstOrDefault();
            //var beneficiary = Alls.Current.SupplierPayees[application?.BeneficiaryID];
            //香港仓库信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
            var contact1 = HKWarehouseEnterprise?.Contacts.FirstOrDefault();

            //代付货款信息
            PayDetailInfo payDetailInfo = null;

            if (order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge)
            {
                var payDetail = Client.Current.MyApplictions.GetPayDetailByOrderID(id);
                string bankID = payDetail.PayPayee.PayeeID;  //账户名称 BankID
                var supplierPayee = Alls.Current.SupplierPayees.SingleOrDefault(item => item.ID == bankID);  //账户名称用

                string payPayerID = payDetail.PayPayer?.PayerID;  //付款人 PayPayerID
                var payer = Client.Current.MyPayers[payPayerID];  //付款人、付款方式、付款人币种用

                //收款账户信息
                var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouse.Enterprise.ID);
                string CompanyBankAccountName = companybank?.RealEnterpriseName;
                string CompanyBankAccount = companybank?.Account;
                string CompanyBankName = companybank?.Bank;
                //string CompanyBankID = companybank?.ID;

                payDetailInfo = new PayDetailInfo
                {
                    TotalPrice = payDetail.TotalPrice, //付款金额
                    AccountName = supplierPayee.Contact ?? supplierPayee.RealEnterpriseName,  //账户名称
                    SupplierMethodDes = payDetail.PayPayee.Method.GetDescription(), //支付方式
                    SupplierName = payDetail.PayPayee.EnterpriseName,  //企业名称
                    SupplierBankName = payDetail.PayPayee.BankName,  //银行名称
                    PayerName = string.IsNullOrEmpty(payer?.Contact) ? payer?.EnterpriseName : payer?.Contact, //付款人
                    SupplierBankAccount = payDetail.PayPayee.BankAccount,  //银行账号
                    MethordInt = payer != null ? (int)payer.Methord : (int)Methord.Cash, //付款方式Int
                    MethordDec = payer != null ? payer.MethordDec : Methord.Cash.GetDescription(),  //付款方式名称
                    CurrencyDec = payer != null ? payer.CurrencyDec : Currency.USD.GetDescription(),  //付款人币种
                    CompanyBankAccountName = CompanyBankAccountName,  //收款账户
                    CompanyBankAccount = CompanyBankAccount,  //收款账号
                    CompanyBankName = CompanyBankName,  //收款银行
                };
            }

            var model = new
            {
                order.ID,
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Input.Currency.GetDescription(),
                order.InWaybill.TotalParts,
                order.InWaybill.TotalWeight,
                order.SupplierName,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsPayName = (order.Input != null && order.Input.IsPayCharge.GetValueOrDefault()) ? "是" : "否",
                IsPay = order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge,
                //BeneficiaryName = beneficiary?.RealEnterpriseName,
                //ApplyPrice = application?.Price,
                //PayMethord = application?.Methord?.GetDescription(),
                //BankAccount = beneficiary?.Account,
                //BankPlace = beneficiary?.Place,
                //Bank = beneficiary?.Bank,
                //BankCode = beneficiary?.SwiftCode,
                //BankAddress = beneficiary?.BankAddress,
                HKWaybillType = order.InWaybill.Type,
                HKWaybillTypeName = order.InWaybill.Type.GetDescription(),
                TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                order.InWaybill.WayLoading?.TakingContact,
                order.InWaybill.WayLoading?.TakingPhone,
                order.InWaybill.WayLoading?.TakingAddress,
                WareHouseAddress = HKWarehouse.Address,
                WareHouseName = contact1?.Name,
                WareHouseTel = contact1?.Tel ?? contact1?.Mobile,
                HKFreight = order.InWaybill.WayCondition.PayForFreight ? "是" : "否",
                HKExpressNumber = order.InWaybill.Code,
                //HKExpressSubNumber = order.InWaybill.Subcodes, //子运单号不需要
                order.InWaybill.CarrierName,
                IsTransfer = "否",
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.ID,
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Name,
                    Unit = item.Unit.GetDescription(),
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    Origin = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                    item.Product.PackageCase,
                    item.DateCode,
                }).ToArray(),
                totalNum = order.OrderItems.Sum(item => item.Quantity),
                totalPrice = order.OrderItems.Sum(item => item.TotalPrice),
                Bills = orderbills,
                sumTotalPrice = orderbills.Sum(item => (decimal?)item.TotalPrice).GetValueOrDefault(),
                PIFiles,
                PackingFiles,
                DeliveryFile = _deliveryFile,
                PayDetailInfo = payDetailInfo,
            };

            return View(model);
        }

        /// <summary>
        /// 待确认即收即发订单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult StorageTransConfirm()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            //查询订单详情数据
            var order = Client.Current.MyReceivedOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            //代收自提文件
            var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).FirstOrDefault();
            var _deliveryFile = "";
            if (deliveryFile != null)
            {
                _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
            }
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            //装箱单
            var PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            #endregion

            var orderbills = Client.Current.MyOrderBills.SearchByOrderID(order.ID).ToArray().Select(item => new
            {
                item.Catalog,
                item.Subject,
                Quantity = 1,
                item.Summay,
                Price = item.LeftPrice,
                TotalPrice = item.LeftPrice,
                item.CurrencyName
            }).ToArray();

            ////代付货款
            //var payapplication = Client.Current.MyApplictions.GetPayApplication().FirstOrDefault(item => item.OrderID == order.ID);
            //var beneficiary = Alls.Current.SupplierPayees[payapplication?.BeneficiaryID];
            ////代收货款
            //var receiveapplication = Client.Current.MyApplictions.GetReceiveApplication().FirstOrDefault(item => item.OrderID == order.ID);
            //var payer = Client.Current.MyPayers[receiveapplication?.PayerID];

            //香港仓库信息
            var HKWarehouse = Services.WhSettings.HK[PvWsOrder.Services.PvClientConfig.WareHouseID];
            var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
            var contact1 = HKWarehouseEnterprise?.Contacts.FirstOrDefault();
            //收款账户信息
            var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouseEnterprise.ID);

            //代付货款信息
            PayDetailInfo payDetailInfo = null;

            if (order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge)
            {
                var payDetail = Client.Current.MyApplictions.GetPayDetailByOrderID(id);
                string bankID = payDetail.PayPayee.PayeeID;  //账户名称 BankID
                var supplierPayee = Alls.Current.SupplierPayees.SingleOrDefault(item => item.ID == bankID);  //账户名称用

                string payPayerID = payDetail.PayPayer.PayerID;  //付款人 PayPayerID
                var payer = Client.Current.MyPayers[payPayerID];  //付款人、付款方式、付款人币种用

                //收款账户信息
                //var companybank = Alls.Current.wsPayeeAll.FirstOrDefault(item => item.EnterpriseID == HKWarehouse.Enterprise.ID);
                string CompanyBankAccountName = companybank?.RealEnterpriseName;
                string CompanyBankAccount = companybank?.Account;
                string CompanyBankName = companybank?.Bank;
                //string CompanyBankID = companybank?.ID;

                payDetailInfo = new PayDetailInfo
                {
                    TotalPrice = payDetail.TotalPrice, //付款金额
                    AccountName = supplierPayee.Contact ?? supplierPayee.RealEnterpriseName,  //账户名称
                    SupplierMethodDes = payDetail.PayPayee.Method.GetDescription(), //支付方式
                    SupplierName = payDetail.PayPayee.EnterpriseName,  //企业名称
                    SupplierBankName = payDetail.PayPayee.BankName,  //银行名称
                    PayerName = string.IsNullOrEmpty(payer.Contact) ? payer.EnterpriseName : payer.Contact, //付款人
                    SupplierBankAccount = payDetail.PayPayee.BankAccount,  //银行账号
                    MethordInt = (int)payer.Methord, //付款方式Int
                    MethordDec = payer.MethordDec,  //付款方式名称
                    CurrencyDec = payer.CurrencyDec,  //付款人币种
                    CompanyBankAccountName = CompanyBankAccountName,  //收款账户
                    CompanyBankAccount = CompanyBankAccount,  //收款账号
                    CompanyBankName = CompanyBankName,  //收款银行
                };
            }

            //代收货款信息
            ReceiveDetailInfo receiveDetailInfo = null;

            if (order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge)
            {
                var receiveDetail = Client.Current.MyApplictions.GetReceiveDetailByID(id);
                string payerID = receiveDetail.ReceivePayer.PayerID;  //付款人 PayerID
                var receivePayer = Client.Current.MyPayers[payerID];  ////付款人、付款方式、付款人币种用

                receiveDetailInfo = new ReceiveDetailInfo
                {
                    ReceivePrice = receiveDetail.TotalPrice,
                    PayerName = string.IsNullOrEmpty(receivePayer.Contact) ? receivePayer.EnterpriseName : receivePayer.Contact,
                    PayerMethodInt = (int)receivePayer.Methord,
                    PayerMethodName = receivePayer.MethordDec,
                    PayerCurrencyName = receivePayer.CurrencyDec,
                    DelivaryOpportunityDes = receiveDetail.DelivaryOpportunity != null ? receiveDetail.DelivaryOpportunity.GetDescription() : "",
                    IsEntryInt = receiveDetail.IsEntry ? 1 : 0,
                    IsEntryDes = receiveDetail.IsEntry ? "是" : "否",
                    CheckDeliveryDes = receiveDetail.CheckDelivery != null ? receiveDetail.CheckDelivery.GetDescription() : "",
                };
            }

            var model = new
            {
                order.ID,
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Input.Currency.GetDescription(),
                order.InWaybill.TotalParts,
                order.InWaybill.TotalWeight,
                order.SupplierName,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsPayName = (order.Input != null && order.Input.IsPayCharge.GetValueOrDefault()) ? "是" : "否",
                IsPay = order.Input != null && order.Input.IsPayCharge != null && (bool)order.Input.IsPayCharge,
                IsRecieveName = (order.Output != null && order.Output.IsReciveCharge.GetValueOrDefault()) ? "是" : "否",
                IsRecieve = order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge,
                //IsPay = payapplication != null,
                //IsPayName = payapplication != null ? "是" : "否",
                //BeneficiaryName = beneficiary?.RealEnterpriseName,
                //ApplyPrice = payapplication?.Price,
                //PayMethord = payapplication?.Methord?.GetDescription(),
                //BankAccount = beneficiary?.Account,
                //BankPlace = beneficiary?.Place,
                //Bank = beneficiary?.Bank,
                //BankCode = beneficiary?.SwiftCode,
                //BankAddress = beneficiary?.BankAddress,
                HKWaybillType = order.InWaybill.Type,
                HKWaybillTypeName = order.InWaybill.Type.GetDescription(),
                TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                order.InWaybill.WayLoading?.TakingContact,
                order.InWaybill.WayLoading?.TakingPhone,
                order.InWaybill.WayLoading?.TakingAddress,
                WareHouseAddress = HKWarehouse.Address,
                WareHouseName = contact1?.Name,
                WareHouseTel = contact1?.Tel ?? contact1?.Mobile,
                HKFreight = order.InWaybill.WayCondition.PayForFreight ? "是" : "否",
                HKExpressNumber = order.InWaybill.Code,
                //HKExpressSubNumber = order.InWaybill.Subcodes,
                HKCarrierName = order.InWaybill.CarrierName,
                IsTransfer = "是",
                WaybillType = order.OutWaybill.Type,
                WaybillTypeName = order.OutWaybill.Type.GetDescription(),
                //IsRecieve = receiveapplication != null,
                //IsRecieveName = receiveapplication != null ? "是" : "否",
                //RecievePrice = receiveapplication?.Price,
                //PayerName = payer?.Contact,
                //PayerBank = payer?.Bank,
                //PayerAccount = payer?.Account,
                CompanyBankAccountName = companybank?.RealEnterpriseName,
                CompanyBankAccount = companybank?.Account,
                CompanyBankName = companybank?.Bank,
                PickupTime = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                ClientPicker = order.OutWaybill.Consignee?.Contact,
                ClientPickerMobile = order.OutWaybill.Consignee?.Phone,
                IDType = order.OutWaybill.Consignee.IDType?.GetDescription(),
                IDNumber = order.OutWaybill.Consignee.IDNumber,
                SealContext = order.OutWaybill.Consignee.IDNumber,
                ClientConsigneeName = order.OutWaybill.Consignee.Contact,
                ClientContactMobile = order.OutWaybill.Consignee.Phone,
                ClientConsigneeAddress = order.OutWaybill.Consignee.Address,
                CarrierName = order.OutWaybill.CarrierName,
                ZipCode = order.OutWaybill.Consignee.Zipcode,
                Freight = order.OutWaybill.WayCondition.PayForFreight ? "是" : "否",
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.ID,
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Name,
                    Unit = item.Unit.GetDescription(),
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    Origin = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                    item.Product.PackageCase,
                    item.DateCode,
                }).ToArray(),
                totalNum = order.OrderItems.Sum(item => item.Quantity),
                totalPrice = order.OrderItems.Sum(item => item.TotalPrice),
                Bills = orderbills,
                sumTotalPrice = orderbills.Sum(item => (decimal?)item.TotalPrice).GetValueOrDefault(),
                PIFiles,
                PackingFiles,
                DeliveryFile = _deliveryFile,
                PayDetailInfo = payDetailInfo,
                ReceiveDetailInfo = receiveDetailInfo,
            };
            return View(model);
        }

        /// <summary>
        /// 待确认发货订单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeliveryConfirm()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            //查询订单详情数据
            var order = Client.Current.MyDeliveryOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            #endregion

            var orderbills = Client.Current.MyOrderBills.SearchByOrderID(order.ID).ToArray().Select(item => new
            {
                item.Catalog,
                item.Subject,
                Quantity = 1,
                item.Summay,
                Price = item.LeftPrice,
                TotalPrice = item.LeftPrice,
                item.CurrencyName
            }).ToArray();

            //代收货款
            //var receiveapplication = Client.Current.MyApplictions.GetReceiveApplication().FirstOrDefault(item => item.OrderID == order.ID);
            //var payer = Client.Current.MyPayers[receiveapplication?.PayerID];
            //var benificary = Alls.Current.wsPayeeAll[receiveapplication?.BeneficiaryID];

            //代收货款信息
            ReceiveDetailInfo receiveDetailInfo = null;

            if (order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge)
            {
                var receiveDetail = Client.Current.MyApplictions.GetReceiveDetailByID(id);
                string payerID = receiveDetail.ReceivePayer.PayerID;  //付款人 PayerID
                var receivePayer = Client.Current.MyPayers[payerID];  ////付款人、付款方式、付款人币种用

                receiveDetailInfo = new ReceiveDetailInfo
                {
                    ReceivePrice = receiveDetail.TotalPrice,
                    PayerName = string.IsNullOrEmpty(receivePayer.Contact) ? receivePayer.EnterpriseName : receivePayer.Contact,
                    PayerMethodInt = (int)receivePayer.Methord,
                    PayerMethodName = receivePayer.MethordDec,
                    PayerCurrencyName = receivePayer.CurrencyDec,
                    DelivaryOpportunityDes = receiveDetail.DelivaryOpportunity != null ? receiveDetail.DelivaryOpportunity.GetDescription() : "",
                    IsEntryInt = receiveDetail.IsEntry ? 1 : 0,
                    IsEntryDes = receiveDetail.IsEntry ? "是" : "否",
                    CheckDeliveryDes = receiveDetail.CheckDelivery != null ? receiveDetail.CheckDelivery.GetDescription() : "",
                };
            }

            var model = new
            {
                order.ID,
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Output.Currency.GetDescription(),
                order.OutWaybill.TotalParts,
                order.OutWaybill.TotalWeight,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                IsRecieveName = (order.Output != null && order.Output.IsReciveCharge.GetValueOrDefault()) ? "是" : "否",
                IsRecieve = order.Output != null && order.Output.IsReciveCharge != null && (bool)order.Output.IsReciveCharge,
                //IsRecieve = receiveapplication != null,
                //IsRecieveName = receiveapplication != null ? "是" : "否",
                //RecievePrice = receiveapplication?.Price,
                //PayerName = payer?.Contact,
                //PayerBank = payer?.Bank,
                //PayerAccount = payer?.Account,
                //CompanyBankName = benificary?.RealEnterpriseName,
                //CompanyBank = benificary?.Bank,
                //CompanyBankAccount = benificary?.Account,
                IsTransfer = "是",
                WaybillType = order.OutWaybill.Type,
                WaybillTypeName = order.OutWaybill.Type.GetDescription(),
                PickupTime = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                ClientPicker = order.OutWaybill.Consignee?.Contact,
                ClientPickerMobile = order.OutWaybill.Consignee?.Phone,
                IDType = order.OutWaybill.Consignee.IDType?.GetDescription(),
                IDNumber = order.OutWaybill.Consignee.IDNumber,
                SealContext = order.OutWaybill.Consignee.IDNumber,
                ClientConsigneeName = order.OutWaybill.Consignee.Contact,
                ClientContactMobile = order.OutWaybill.Consignee.Phone,
                ClientConsigneeAddress = order.OutWaybill.Consignee.Address,
                CarrierName = order.OutWaybill.CarrierName,
                ZipCode = order.OutWaybill.Consignee.Zipcode,
                Freight = order.OutWaybill.WayCondition.PayForFreight ? "是" : "否",
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.ID,
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Name,
                    Unit = item.Unit.GetDescription(),
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    Origin = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                }).ToArray(),
                totalNum = order.OrderItems.Sum(item => item.Quantity),
                totalPrice = order.OrderItems.Sum(item => item.TotalPrice),
                Bills = orderbills,
                sumTotalPrice = orderbills.Sum(item => (decimal?)item.TotalPrice).GetValueOrDefault(),
                PIFiles,
                ReceiveDetailInfo = receiveDetailInfo,
            };
            return View(model);
        }
        #endregion


        #region 账单确认,取消
        /// <summary>
        /// 仓储订单客户确认账单
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult Confirm(string orderID)
        {
            Client.Current.MyOrder.ConfirmBill(orderID);

            return JsonResult(VueMsgType.success, "账单确认成功");
        }

        /// <summary>
        /// 仓储订单客户取消
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult CancelConfirm(string OrderID)
        {
            Client.Current.MyOrder.CancelBill(OrderID);

            return JsonResult(VueMsgType.success, "账单取消成功");
        }
        #endregion


        #region 特殊货物处理要求
        /// <summary>
        /// 特殊货物处理要求
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult _PartialSpecialRequirements()
        {
            var model = new SpecialGoodsModel();
            //基础数据
            var data = new
            {
                SpecialTypeOptions = ExtendsEnum.ToDictionary<SpecialRequire>()
                .Select(item => new 
                {
                    value = item.Key, 
                    text = item.Value == "换箱单" ? "换箱单文件" : item.Value,
                }).ToArray(),
                LabelTypeOptions = ExtendsEnum.ToDictionary<LabelType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
            };
            ViewBag.Options = data;
            return PartialView(model);
        }
        #endregion


        #region 基础数据获取
        /// <summary>
        /// 获取供应商受益人
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetSupplierBeneficiaries() //string supplier
        {
            //supplier = supplier.InputText();
            var beneficiaries = "".Json();
            //if (!string.IsNullOrWhiteSpace(supplier))
            //{
            //var entity = new MySupplierPayees(Client.Current.EnterpriseID, supplier).ToArray();
            var entity = Alls.Current.SupplierPayees.Where(t => t.OwnID == Client.Current.EnterpriseID
                                                             && t.Status != GeneralStatus.Deleted).ToArray();

            beneficiaries = entity.Select(item => new
            {
                value = item.ID,
                text = item.Contact,
                item.RealEnterpriseName,
                item.Bank,
                item.BankAddress,
                item.Place,
                item.SwiftCode,
                item.Account,
            }).Json();
            //}
            return JsonResult(VueMsgType.success, "", beneficiaries);
        }

        /// <summary>
        /// 获取支付人
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetPayerOptions(string type)
        {
            var payers = Client.Current.MyPayers.Where(m => m.Status == GeneralStatus.Normal && (m.Methord == Methord.Cash || m.Methord == Methord.Check
            || m.Methord == Methord.Transfer));
            //是否需要过滤支票数据
            if (!string.IsNullOrEmpty(type))
            {
                payers = payers.Where(item => item.Methord != Methord.Check);
            }

            var data = payers.OrderByDescending(m => m.CreateDate).ToArray().
                Select(c => new
                {
                    value = c.ID,//即PayerID
                    c.EnterpriseID,
                    c.EnterpriseName,
                    c.Bank,
                    c.Account,
                    Method = c.Methord,
                    MethodDec = c.MethordDec,
                    c.Currency,
                    c.CurrencyDec,
                    text = string.IsNullOrEmpty(c.Contact) ? c.EnterpriseName : c.Contact,
                }).Json();
            return JsonResult(VueMsgType.success, "", data);
        }

        /// <summary>
        /// 获取公司受益人
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetCompanyBankOptions(string currency)
        {
            if (!string.IsNullOrWhiteSpace(currency))
            {
                //银行账号
                var bank = Yahv.Alls.Current.wsPayeeAll.Where(c => c.EnterpriseID == PvWsOrder.Services.PvClientConfig.ThirdCompanyID && c.Currency == (Currency)(int.Parse(currency))).Select(item => new
                {
                    ID = item.ID,
                    Name = item.RealEnterpriseName,
                    item.Bank,
                    item.Account,
                }).ToArray();
                return JsonResult(VueMsgType.success, "", bank.Json());
            }
            else
            {
                return JsonResult(VueMsgType.success, "", "".Json());
            }
        }

        /// <summary>
        /// 付款人新增分部视图
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult _PartialPayer()
        {
            PayerModel model = new PayerModel();
            var data = new
            {
                MethordOptions = ExtendsEnum.ToDictionary<Methord>().Where(c => c.Key != ((int)Methord.TT).ToString()
                    && c.Key != ((int)Methord.Alipay).ToString()).Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                CurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PlaceOptions = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27")
                .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
            };
            ViewBag.Options = data;
            return PartialView(model);
        }

        /// <summary>
        /// 付款人新增分部视图-mini
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult _SimplePayer(
            bool? useDefaultMethord,
            bool? useDefaultCurrency
        )
        {
            PayerModel model = new PayerModel();

            //国家/地区
            Dictionary<string, string> dicPlace = new Dictionary<string, string>();

            var allNeed = ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27");

            string[] alwaysUsePlaceName = { "中国香港", "中国台湾", "美国" };

            foreach (var item in alwaysUsePlaceName)
            {
                var thePair = allNeed.Where(t => t.Value == item).FirstOrDefault();
                if (thePair.Key != null)
                {
                    dicPlace.Add(thePair.Key, thePair.Value);
                }
            }

            foreach (var item in allNeed)
            {
                if (!alwaysUsePlaceName.Contains(item.Value))
                {
                    dicPlace.Add(item.Key, item.Value);
                }
            }

            var data = new
            {
                MethordOptions = ExtendsEnum.ToDictionary<Methord>().Where(c => c.Key != ((int)Methord.TT).ToString()
                    && c.Key != ((int)Methord.Alipay).ToString()
                    && c.Key != ((int)Methord.Exchange).ToString()
                    && c.Key != ((int)Methord.Check).ToString()).Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                MethordOptions22 = ExtendsEnum.ToDictionary<Methord>().Where(c => c.Key != ((int)Methord.TT).ToString()
                    && c.Key != ((int)Methord.Alipay).ToString()
                    && c.Key != ((int)Methord.Exchange).ToString()).Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                CurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PlaceOptions = dicPlace.Select(item => new { value = item.Key, text = item.Value }),//ExtendsEnum.ToDictionary<Origin>().Where(item => item.Key != "8888888" && item.Key != "9999999" && item.Key != "27")
                //.Select(item => new { value = item.Key, text = item.Value }).ToArray(),
            };

            // 使用默认付款方式
            if (useDefaultMethord != null && useDefaultMethord.Value)
            {
                model.Methord = ((int)Methord.Transfer).ToString();
            }
            // 使用默认付款币种
            if (useDefaultCurrency != null && useDefaultCurrency.Value)
            {
                model.Currency = ((int)Currency.CNY).ToString();
            }

            ViewBag.Options = data;
            ViewBag.UseDefaultMethord = useDefaultMethord != null && useDefaultMethord.Value;
            ViewBag.useDefaultCurrency = useDefaultCurrency != null && useDefaultCurrency.Value;

            return PartialView(model);
        }

        /// <summary>
        /// 付款人新增分部视图
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult SubmitPayer(PayerModel model)
        {
            var current = Client.Current;
            try
            {
                int methord = 1;
                int.TryParse(model.Methord, out methord);
                int currency = 0;
                int.TryParse(model.Currency, out currency);
                var payerId = new DccPayer().Enter(new DccPayer.MyClientPayer()
                {
                    ClientID = current.EnterpriseID,
                    Methord = (Methord)methord,
                    Currency = (Currency)currency,
                    Place = ((Origin)int.Parse(model.Place)).GetOrigin().Code,
                    Contact = model.Name,
                });

                return JsonResult(VueMsgType.success, "新增成功", payerId);
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }
        #endregion
    }
}