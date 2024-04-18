//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsClient.WebApp.Controllers;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.PvWsOrder.Services.ClientViews;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsClient.Model;
//using Yahv.Utils.Http;
//using Yahv.Underly;
//using Newtonsoft.Json;
//using Yahv.Payments;
//using System.Linq.Expressions;
//using Yahv.Linq.Extends;
//using Yahv.PvWsOrder.Services.Models;
//using Yahv.Utils.Serializers;
//using Yahv.PvWsOrder.Services.XDTClientView;

//namespace Yahv.PvUser.WebApp.Controllers
//{
//    public class BillController : UserController
//    {
//        /// <summary>
//        /// 对账单确认
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult BillConfirm()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var current = Client.Current;
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
//            var order = current.MyOrder[paraArr[0]];
//            if (order == null || order.PaymentStatus != OrderPaymentStatus.Confirm)
//            {
//                return View("Error");
//            }

//            //获取当前客户的优惠券
//            var coupons = current.MyCoupons.Where(item => item.Balance > 0).Select(item => new
//            {
//                item.ID,
//                item.Name,
//                item.Catalog,
//                item.Subject,
//                item.Price,
//            }).ToArray();
//            ViewBag.navid = paraArr[1];

//            //获取当前客户的账单
//            var myBill = current.MyOrderBills.SearchByOrderID(paraArr[0]);
//            var bill = myBill.ToArray().Select(item => new
//            {
//                item.ReceivableID,
//                item.Catalog,
//                item.Subject,
//                item.LeftPrice,
//                RightPrice=item.RightPrice.GetValueOrDefault(),
//                UnPay=item.LeftPrice- item.RightPrice.GetValueOrDefault(),
//                Coupons = coupons.Where(a => a.Catalog == item.Catalog && a.Subject == item.Subject).ToArray(),
//                CouponID="",
//                IsChecked=false,
//                DiscountAmount=0,//已优惠金额
//            }).ToArray();
//            if (bill.Count() == 0)
//            {
//                return View("Error");
//            }
//            var currency = myBill.First().Currency;

//            #region 获取可用信用额度
//            //查看账单是否逾期
//            bool IsOverDue = PaymentManager.Npc[PvWsOrder.Services.PvClientConfig.CompanyID, current.EnterpriseID][ConductConsts.代仓储]
//                .DebtTerm[DateTime.Now].IsOverdue;

//            //查询当前币种可用额度列表
//            var creditList = current.MyCredits.Where(item => item.Currency == currency).Select(item => new
//            {
//                item.Catalog,
//                Left = item.Total - item.Cost,
//            });
//            var Credit = new CreditAvailable
//            {
//                GoodsCredit = creditList.Where(item=>item.Catalog== "货款").Sum(item=>(decimal?)item.Left).GetValueOrDefault(),
//                TaxCredit = creditList.Where(item => item.Catalog == "税款").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                AgentCredit = creditList.Where(item => item.Catalog == "代理费").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                OtherCredit = creditList.Where(item => item.Catalog == "杂费").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//            };
//            #endregion

//            var data = new
//            {
//                ID = paraArr[0],
//                bill,
//                Credit,
//                creditList,
//                currency = currency.GetCurrency().ChineseName,
//                IsOverDue,
//                currencyCode=currency.GetCurrency().ShortName,
//                TotalLeft=bill.Sum(item=>item.LeftPrice),//应付合计
//                TotalRight= bill.Sum(item => item.RightPrice),
//                TotalUnPay= bill.Sum(item => item.UnPay),
//            };
//            return View(data);
//        }

//        /// <summary>
//        /// 对账单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult BillList()
//        {
//            var para = Request.Form["para"];
//            if (string.IsNullOrWhiteSpace(para))
//            {
//                return View("Error");
//            }
//            var current = Client.Current;
//            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
 
//            ViewBag.navid = paraArr[1];

//            var order = current.MyOrder.GetOrderDetail(paraArr[0]);
//            if (order == null)
//            {
//                return View("Error");
//            }

//            //获取当前客户的账单
//            var myBill = current.MyOrderBills.SearchByOrderID(paraArr[0]);
//            var bill = myBill.ToArray().Select(item => new
//            {
//                item.ReceivableID,
//                item.Catalog,
//                item.Subject,
//                item.LeftPrice,
//                CouponPrice= item.CouponPrice.GetValueOrDefault(),
//                RightPrice = item.RightPrice.GetValueOrDefault(),
//                UnPay = item.LeftPrice - item.RightPrice.GetValueOrDefault(),
//                IsChecked = false,
//            }).ToArray();
//            var currency = order.SettlementCurrency;

//            #region 获取可用信用额度
//            //查看账单是否逾期
//            bool IsOverDue = PaymentManager.Npc[PvWsOrder.Services.PvClientConfig.CompanyID, current.EnterpriseID][ConductConsts.代仓储]
//                .DebtTerm[DateTime.Now].IsOverdue;

//            //查询当前币种可用额度列表
//            var creditList = current.MyCredits.Where(item => item.Currency == currency).Select(item => new
//            {
//                item.Catalog,
//                Left = item.Total - item.Cost,
//            });
//            var Credit = new CreditAvailable
//            {
//                GoodsCredit = creditList.Where(item => item.Catalog == "货款").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                TaxCredit = creditList.Where(item => item.Catalog == "税款").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                AgentCredit = creditList.Where(item => item.Catalog == "代理费").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//                OtherCredit = creditList.Where(item => item.Catalog == "杂费").Sum(item => (decimal?)item.Left).GetValueOrDefault(),
//            };
//            #endregion

//            var data = new
//            {
//                ID = paraArr[0],
//                bill,
//                Credit,
//                creditList,
//                currency = currency?.GetCurrency().ChineseName,
//                IsOverDue,
//                currencyCode = currency?.GetCurrency().ShortName,
//                TotalLeft = bill.Sum(item => item.LeftPrice),//应付合计
//                TotalRight = bill.Sum(item => item.RightPrice),
//                TotalUnPay = bill.Sum(item => item.UnPay),
//                TotalCoupon = bill.Sum(item => item.CouponPrice),
//                IsConfirm = order.PaymentStatus == OrderPaymentStatus.ToBePaid|| order.PaymentStatus == OrderPaymentStatus.PartPaid,
//            };
//            return View(data);
//        }

//        /// <summary>
//        /// 信用支付
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult CreditPay(string payData,string confirmData, string id,decimal creditMoney)
//        {
//            try
//            {
//                var user = Client.Current;
//                var order = user.MyOrder.GetOrderDetail(id);
//                var arrPay = JsonConvert.DeserializeObject<string[]>(payData);
//                var maps = JsonConvert.DeserializeObject<UsedMap[]>(confirmData);
//                if (order.PaymentStatus != OrderPaymentStatus.Confirm)
//                {
//                    return base.JsonResult(VueMsgType.error, "该账单状态异常，请刷新页面重试");
//                }
//                #region 账单确认
//                CouponManager.Current[Yahv.PvWsOrder.Services.PvClientConfig.CompanyID, user.EnterpriseID].Pay(user.ID, maps);
//                //发货订单需强制结算仓储费
//                if (order.Type == OrderType.Delivery)
//                {
//                    PaymentManager.Site(user.ID)[PvWsOrder.Services.PvClientConfig.CompanyID, user.EnterpriseID][ConductConsts.代仓储]
//                        .Receivable.Confirm(order.ID, Currency.CNY, true);
//                }
//                order.CreatorID = user.ID;
//                order.ConfirmBill();
//                OperationLog(id, "账单确认成功");
//                #endregion

//                #region 信用支付
//                var receiveids = arrPay;
//                PaymentManager.Site(Client.Current.ID).Credit.For(receiveids).Pay(order.SettlementCurrency.GetValueOrDefault(), creditMoney);
//                OperationLog(id, "信用支付成功");
//                return base.JsonResult(VueMsgType.success, "信用支付成功");
//                #endregion
//            }
//            catch (Exception ex)
//            {
//                ErrorOperationLog(id, "信用支付失败" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "信用支付失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 信用支付
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CreditPayOnly(string payData, string id, decimal creditMoney)
//        {
//            try
//            {
//                var user = Client.Current;
//                var order = user.MyOrder.GetOrderDetail(id);
//                var arrPay = JsonConvert.DeserializeObject<string[]>(payData);
//                if (order.PaymentStatus != OrderPaymentStatus.PartPaid && order.PaymentStatus != OrderPaymentStatus.ToBePaid)
//                {
//                    return base.JsonResult(VueMsgType.error, "该账单状态异常，请刷新页面重试");
//                }

//                #region 信用支付
//                var receiveids = arrPay;
//                PaymentManager.Site(Client.Current.ID).Credit.For(receiveids).Pay(order.SettlementCurrency.GetValueOrDefault(), creditMoney);
//                OperationLog(id, "信用支付成功");
//                return base.JsonResult(VueMsgType.success, "信用支付成功");
//                #endregion
//            }
//            catch (Exception ex)
//            {
//                ErrorOperationLog(id, "信用支付失败" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "信用支付失败：" + ex.Message);
//            }
//        }


//        /// <summary>
//        /// 账单确认
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Confirm(string data,string id)
//        {
//            try
//            {
//                var user = Client.Current;
//                var order = user.MyOrder.GetOrderDetail(id);
//                var maps = JsonConvert.DeserializeObject<UsedMap[]>(data);
//                CouponManager.Current[Yahv.PvWsOrder.Services.PvClientConfig.CompanyID, user.EnterpriseID].Pay(user.ID, maps);
//                //发货订单需强制结算仓储费
//                if (order.Type == OrderType.Delivery)
//                {
//                    PaymentManager.Site(user.ID)[PvWsOrder.Services.PvClientConfig.CompanyID, user.EnterpriseID][ConductConsts.代仓储]
//                        .Receivable.Confirm(order.ID, Currency.CNY, true);
//                }
//                order.CreatorID = user.ID;
//                order.ConfirmBill();
//                OperationLog(id, "账单确认成功");
//                return base.JsonResult(VueMsgType.success, "账单确认成功");
//            }
//            catch(Exception ex)
//            {
//                ErrorOperationLog(id, "账单确认失败" + ex.Message);
//                return base.JsonResult(VueMsgType.error, "账单确认失败：" + ex.Message);
//            }
//        }

//        /// <summary>
//        /// 我的账单
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult BillRecord()
//        {
//            //一周之内
//            string[] dateArr = new string[] { DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
//            var total_Remains = Client.Current.MyOrderBills.OrderBill(ExtendsEnum.ToArray<OrderType>()).ToArray().Sum(item => item.Remains);
//            var data = new
//            {
//                QDate = dateArr,
//                Total_Remains = string.Format("{0:N}", total_Remains),
//            };
//            return View(data);
//        }

//        /// <summary>
//        /// 获取账单数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetBillRecord()
//        {
//            string[] orderStatus = JsonConvert.DeserializeObject<string[]>(Request.Form["orderStatus"].ToString());
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var data = Client.Current.MyOrderBills;

//            #region 筛选数据
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<PvWsOrder.Services.ClientModels.Bill, bool>> expression = item => true;
//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                Expression<Func<PvWsOrder.Services.ClientModels.Bill, bool>> exp = item => item.CreateDate >= dStart && item.CreateDate <= dEnd;
//                expression = expression.And(exp);
//            }
//            lambdas.Add(expression);

//            Expression<Func<PvWsOrder.Services.ClientModels.Bill, bool>> orexpression = null;
//            foreach (var i in orderStatus)
//            {
//                Expression<Func<PvWsOrder.Services.ClientModels.Bill, bool>> exp = item => item.Type == (OrderType)(int.Parse(i));
//                orexpression = orexpression.Or(exp);
//            }
//            if (orexpression != null)
//            {
//                lambdas.Add(orexpression);
//            }
//            #endregion

//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = data.GetPageListBills(lambdas.ToArray(),ExtendsEnum.ToArray<OrderType>(), rows, page);

//            Func<PvWsOrder.Services.ClientModels.Bill, object> convert = item => new
//            {
//                item.ID,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                TypeName = item.Type.GetDescription(),
//                Type = item.Type,
//                LeftTotalPrice = string.Format("{0:N}", item.LeftPrice.GetValueOrDefault()),
//                RightTotalPrice = string.Format("{0:N}", item.RightPrice.GetValueOrDefault()),
//                Remains = string.Format("{0:N}", item.Remains),
//                CouponPrice = string.Format("{0:N}", item.CouponPrice.GetValueOrDefault()),
//                DetailList = new List<object>(),
//            };
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 获取账单详情
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetBillByID()
//        {
//            var id = Request.Form["orderID"];
//            var bill = Client.Current.MyOrderBills.SearchByOrderID(id).ToArray().Select(item => new
//            {
//                item.Catalog,
//                item.Subject,
//                LeftPrice = string.Format("{0:N}", item.LeftPrice),
//                RightPrice = string.Format("{0:N}", item.RightPrice.GetValueOrDefault()),
//                Remains = string.Format("{0:N}", item.Remains),
//                CouponPrice = string.Format("{0:N}", item.CouponPrice.GetValueOrDefault()),
//            }).ToArray().OrderBy(item=>item.Subject);
//            return JsonResult(VueMsgType.success, "", bill.Json());
//        }

//        #region 我的付汇
//        /// <summary>
//        /// 我的付汇
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyApplies()
//        {
//            //数据绑定
//            ViewBag.ApplyStatusOptions = ExtendsEnum.ToDictionary<PayExchangeApplyStatus>().Select(item => new
//            {
//                value = item.Key,
//                text = item.Value
//            }).ToArray();

//            return View();
//        }

//        /// <summary>
//        /// 获取我的付汇申请
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetApplies()
//        {
//            var applyStatus = Request.Form["applyStatus"];  //申请状态
//            var applyDate = Request.Form["applyDate"];  //开票状态

//            var view = Client.Current.MyPayExchangeApplies;
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            Expression<Func<UserPayExchangeApply, bool>> expression = item => true;

//            if ((!string.IsNullOrWhiteSpace(applyDate)) && applyDate != "all")
//            {
//                if (applyDate == "curMonth") //当月订单
//                {
//                    Expression<Func<UserPayExchangeApply, bool>> exp = item => item.CreateDate.Month == DateTime.Now.Month;
//                    expression = expression.And(exp);
//                }
//                else if (applyDate == "thrMonth")  //三个月的订单
//                {
//                    Expression<Func<UserPayExchangeApply, bool>> exp = item => item.CreateDate >= DateTime.Now.AddMonths(-3);
//                    expression = expression.And(exp);
//                }
//                else if (applyDate == "curYear")  //当年订单
//                {
//                    Expression<Func<UserPayExchangeApply, bool>> exp = item => item.CreateDate.Year >= DateTime.Now.Year;
//                    expression = expression.And(exp);
//                }
//            }

//            if ((!string.IsNullOrWhiteSpace(applyStatus)) && applyStatus != "all") //订单状态
//            {
//                Expression<Func<UserPayExchangeApply, bool>> exp = item => item.PayExchangeApplyStatus == (PayExchangeApplyStatus)int.Parse(applyStatus);
//                expression = expression.And(exp);
//            }
//            lambdas.Add(expression);
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = view.GetPageListOrders(lambdas.ToArray(), rows, page);
//            Func<UserPayExchangeApplyExtends, object> convert = item => new
//            {
//                item.Apply.ID,
//                CreateDate = item.Apply.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                item.Apply.Currency,
//                TotalAmount = item.Items.Sum(s => s.Amount),
//                ApplyStatus = item.Apply.PayExchangeApplyStatus.GetDescription(),
//                Supplier = item.Apply.SupplierName,
//                Applier = item.User == null ? "跟单员" : item.User.Name,
//                isUploadFile = item.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing,
//                isDelete = item.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing || item.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Cancled,
//                NoData = false,
//                isLoading = true,
//                CompletedDate = item.CompetedLog?.CreateDate.ToString("yyyy-MM-dd"),
//            };
//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 付汇申请详情
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult ApplyInfo(string id)
//        {
//            id = id.InputText();
//            var apply = Client.Current.MyPayExchangeApplies.GetDetailDataByID(id);
//            if (apply == null)
//            {
//                return View("Error");
//            }

//            ApplyInfoViewModel model = new ApplyInfoViewModel();
//            model.ID = id;
//            var currency = Yahv.Alls.Current.Currency.FindByCode(apply.Apply.Currency);
//            if (currency != null)
//            {
//                model.Currency = currency.Name;
//            }

//            var purchaser = PvWsOrder.Services.PurchaserContext.Current;

//            //申请明细
//            model.ApplyItems = apply.Items.Select(item => new
//            {
//                OrderID = item.OrderID,
//                model.Currency,
//                item.Amount,
//                DeclarePrice = Client.Current.MyXDTOrder[item.OrderID]?.DeclarePrice,
//            }).ToArray();

//            var applyLog = apply.CompetedLog;

//            //付款信息
//            model.SupplierEnglishName = apply.Apply.SupplierEnglishName;
//            model.Account = purchaser.BankName;
//            model.AccountID = purchaser.AccountId;
//            model.AgentName = purchaser.CompanyName;
//            model.BankName = apply.Apply.BankName;
//            model.BankAddress = apply.Apply.BankAddress;
//            model.BankAccount = apply.Apply.BankAccount;
//            model.BankCode = apply.Apply.SwiftCode;
//            model.PaymentType = apply.Apply.PaymentType.GetDescription();
//            model.Others = apply.Apply.OtherInfo;
//            model.Summary = apply.Apply.Summary;
//            model.ExchangeRateType = apply.Apply.ExchangeRateType.GetDescription();
//            model.ExchangeRate = apply.Apply.ExchangeRate;
//            model.TotalMoney = (apply.Items.Sum(item => item.Amount) * apply.Apply.ExchangeRate).ToRound(2);
//            model.PayDate = applyLog == null ? "" : applyLog.CreateDate.ToString("yyyy-MM-dd HH:mm");
//            model.SettlementDate = apply.Apply.SettlemenDate.ToString("yyyy年MM月dd日");   //结算日期

//            var listPIFiles = apply.PIFiles;
//            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";
//            model.PIFiles = listPIFiles.ToArray().Select(item => new
//            {
//                FileName = item.CustomName,
//                Status = ((PvWsOrder.Services.XDTModels.Status)item.Status).GetDescription(),
//                URL = fileurl + item.Url.ToUrl()
//            }).ToArray();

//            var applyFile = apply.PayExchangeFile;
//            if (applyFile != null)
//            {
//                model.AgentTrustInstrumentURL = fileurl + applyFile.Url.ToUrl();
//                model.AgentTrustInstrumentName = applyFile.CustomName;
//            }
//            model.PaymentDate = apply.CompetedLog?.CreateDate.ToString("yyyy-MM-dd");
//            model.IsUpload = apply.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing;
//            return View(model);
//        }
//        #endregion
//    }
//}