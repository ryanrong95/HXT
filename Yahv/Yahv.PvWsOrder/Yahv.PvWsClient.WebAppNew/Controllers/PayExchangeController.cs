using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Yahv.Linq.Extends;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class PayExchangeController : UserController
    {
        #region 我的付汇
        /// <summary>
        /// 我的付汇
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyApplies()
        {
            //数据绑定
            ViewBag.ApplyStatusOptions = ExtendsEnum.ToDictionary<PayExchangeApplyStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).ToArray();

            return View();
        }

        /// <summary>
        /// 获取我的付汇申请
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetApplies()
        {
            var paralist = new
            {
                payStatus = Request.Form["payStatus"], //申请状态
                startDate = Request.Form["startDate"],  //开始日期
                endDate = Request.Form["endDate"],      //结束日期
                orderID = Request.Form["orderID"],     //订单ID
                supplierName = Request.Form["supplierName"], //供应商
                accountName = Request.Form["accountName"], //收款方户名
                MultiField = Request.Form["MultiField"],
            };
            var view = Client.Current.MyPayExchangeApplies;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<UserPayExchangeApply, bool>> expression = item => item.FatherID == null;

            if ((!string.IsNullOrWhiteSpace(paralist.startDate)) && (!string.IsNullOrWhiteSpace(paralist.endDate)))
            {
                var dStart = DateTime.Parse(paralist.startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(paralist.endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
                Expression<Func<UserPayExchangeApply, bool>> exp = item => item.CreateDate >= dStart && item.CreateDate <= dEnd;
                expression = expression.And(exp);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.payStatus))) //状态
            {
                Expression<Func<UserPayExchangeApply, bool>> exp = item => item.PayExchangeApplyStatus == (PayExchangeApplyStatus)int.Parse(paralist.payStatus);
                expression = expression.And(exp);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.orderID))) //订单ID
            {
                view = view.SearchByOrderID(paralist.orderID.Trim());
            }
            if ((!string.IsNullOrWhiteSpace(paralist.supplierName))) //供应商
            {
                Expression<Func<UserPayExchangeApply, bool>> exp = item => item.SupplierName.Contains(paralist.supplierName.Trim()) || item.SupplierEnglishName.Contains(paralist.supplierName.Trim());
                expression = expression.And(exp);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.accountName))) //收款方户名
            {
                Expression<Func<UserPayExchangeApply, bool>> exp = item => item.BankAccount.Contains(paralist.accountName.Trim());
                expression = expression.And(exp);
            }
            //订单ID、供应商、收款方户名
            if (!string.IsNullOrWhiteSpace(paralist.MultiField))
            {
                view = view.SearchByMultiField(paralist.MultiField.Trim());
            }
            lambdas.Add(expression);
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var list = view.GetPageListOrders(lambdas.ToArray(), rows, page);
            Func<UserPayExchangeApplyExtends, object> convert = item => new
            {
                item.Apply.ID,
                CreateDate = item.Apply.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.Apply.CreateDate.ToString("yyyy-MM-dd"),
                item.Apply.Currency,
                TotalAmount = item.Items.Sum(s => s.Amount),
                ApplyStatus = item.Apply.PayExchangeApplyStatus.GetDescription(),
                Supplier = item.Apply.SupplierEnglishName,
                isUploadFile = item.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing,
                isDelete = item.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing || item.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Cancled,
                CompletedDate = item.CompetedLog?.CreateDate.ToString("yyyy-MM-dd"),
            };
            return this.Paging(list, list.Total, convert);
        }

        /// <summary>
        /// 删除付汇申请
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult DeleteApply(string id)
        {
            id = id.InputText();
            var current = Client.Current;
            var apply = current.MyPayExchangeApplies.GetDetailDataByID(id);
            if (apply == null)
            {
                return base.JsonResult(VueMsgType.error, "删除失败");
            }

            apply.DeleteApply(current.ID);
            var response = apply.Apply.ResponseData;
            if (response.success)
            {
                return base.JsonResult(VueMsgType.success, "删除成功");
            }
            else
            {
                return base.JsonResult(VueMsgType.error, "删除失败");
            }
        }

        /// <summary>
        /// 付汇申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult ApplyInfo()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var current = Client.Current;
            var apply = current.MyPayExchangeApplies.GetDetailDataByID(id);
            if (apply == null)
            {
                return View("Error");
            }
            var currency = Yahv.Alls.Current.Currency.FindByCode(apply.Apply.Currency);

            //申请明细
            var applyItems = apply.Items.Select(item => new
            {
                OrderID = item.OrderID,
                Currency = currency?.Name,
                item.Amount,
                DeclarePrice = current.MyXDTOrder[item.OrderID]?.DeclarePrice,
            }).ToArray();

            var applyLog = apply.CompetedLog; //付汇日志

            var purchaser = PvWsOrder.Services.PurchaserContext.Current; //公司信息

            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/"; //文件路径前缀

            //PI文件
            var PIFiles = apply.PIFiles.ToArray().Select(item => new
            {
                FileName = item.CustomName,
                Status = ((PvWsOrder.Services.XDTModels.Status)item.Status).GetDescription(),
                URL = fileurl + item.Url.ToUrl()
            }).ToArray();
            //付汇申请书
            var applyFile = apply.PayExchangeFile;
            var agentTrustInstrumentURL = "";
            var agentTrustInstrumentName = "";
            if (applyFile != null)
            {
                agentTrustInstrumentURL = fileurl + applyFile.Url.ToUrl();
                agentTrustInstrumentName = applyFile.CustomName;
            }
            var data = new
            {
                apply.Apply.ID,
                applyItems,
                totalAmount = applyItems.Sum(item => item.Amount),
                totalDeclarePrice = applyItems.Sum(item => item.DeclarePrice),
                apply.Apply.SupplierEnglishName,
                Currency = currency?.Name,
                apply.Apply.BankName,
                apply.Apply.ExchangeRate,
                ExchangeRateType = apply.Apply.ExchangeRateType.GetDescription(),
                apply.Apply.BankAddress,
                TotalMoney = (apply.Items.Sum(item => item.Amount) * apply.Apply.ExchangeRate).ToRound(2),
                apply.Apply.BankAccount,
                apply.Apply.SwiftCode,
                PaymentType = apply.Apply.PaymentType.GetDescription(),
                PayDate = applyLog == null ? "" : applyLog.CreateDate.ToString("yyyy-MM-dd HH:mm"), //付汇日期
                apply.Apply.Summary,
                SettlementDate = apply.Apply.SettlemenDate.ToString("yyyy年MM月dd日"),
                Account = purchaser.BankName,
                AccountID = purchaser.AccountId,
                AgentName = purchaser.CompanyName,
                PIFiles,
                AgentTrustInstrumentURL = agentTrustInstrumentURL,
                AgentTrustInstrumentName = agentTrustInstrumentName,
                IsUpload = apply.Apply.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing, //是否可以上传

                HandlingFeePayerType = apply.Apply.HandlingFeePayerType != null ? apply.Apply.HandlingFeePayerType.ToString() : null,
                HandlingFeePayerTypeName = apply.Apply.HandlingFeePayerType != null
                                            ? ((HandlingFeePayerType)apply.Apply.HandlingFeePayerType).GetDescription()
                                            : null,
                apply.Apply.HandlingFee,
                apply.Apply.USDRate,
            };
            return View(data);
        }
        #endregion

        #region 待付汇
        /// <summary>
        /// 待付汇
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult UnPayExchange()
        {
            //数据绑定
            // ExtendsEnum.ToDictionary<PayExchangeStatus>()
            var payStatusesOptions = new[] { PayExchangeStatus.UnPay, PayExchangeStatus.Partial, }
            .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
            .Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).ToArray();

            ViewBag.ApplyStatusOptions = payStatusesOptions;
            ViewBag.DefaultPayStatuses = payStatusesOptions.Select(t => t.value).ToArray();

            return View();
        }

        /// <summary>
        /// 获取待付汇数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetUnPayExchangeList()
        {
            var order = Client.Current.MyUnPayExchangeOrder;
            var allPayExchangeStatus = ExtendsEnum.ToDictionary<PayExchangeStatus>().Select(t => t.Key).ToArray();
            var paralist = new
            {
                payStatus = Request.Form["payStatus"].ToString().Trim(), //申请状态
                startDate = Request.Form["startDate"].ToString().Trim(),  //开始日期
                endDate = Request.Form["endDate"].ToString().Trim(),      //结束日期
                orderID = Request.Form["orderID"].ToString().Trim(),     //订单ID
                supplierName = Request.Form["supplierName"].ToString().Trim(), //供应商
                payStatuses = Request.Form["payStatuses"].ToArray().Select(t => Convert.ToString(t)).ToArray()
                                                                   .Where(t => allPayExchangeStatus.Contains(t)).ToArray(), //多选的申请状态
                MultiField = Request.Form["MultiField"]?.ToString()?.Trim(),
            };

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<UnPayExchangeOrder, bool>> expression = item => true;
            if ((!string.IsNullOrWhiteSpace(paralist.startDate)) && (!string.IsNullOrWhiteSpace(paralist.endDate)))
            {
                var dStart = DateTime.Parse(paralist.startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(paralist.endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
                Expression<Func<UnPayExchangeOrder, bool>> exp = item => item.CreateDate >= dStart && item.CreateDate <= dEnd.AddDays(1);
                expression = expression.And(exp);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.orderID))) //订单ID
            {
                var ids1 = paralist.orderID.Split(';'); //以分号隔开
                var ids2 = paralist.orderID.Split(','); //以逗号隔开
                var ids3 = paralist.orderID.Split('，');//以逗号隔开
                if (ids1 != null && ids1.Any())
                {
                    ids1 = ids1.Select(t => t.Trim()).ToArray();
                }
                if (ids2 != null && ids2.Any())
                {
                    ids2 = ids2.Select(t => t.Trim()).ToArray();
                }
                if (ids3 != null && ids3.Any())
                {
                    ids3 = ids3.Select(t => t.Trim()).ToArray();
                }
                Expression<Func<UnPayExchangeOrder, bool>> exp = item => ids1.Contains(item.ID) || ids1.Contains(item.MainOrderID) || ids2.Contains(item.ID) || ids2.Contains(item.MainOrderID) || ids3.Contains(item.ID) || ids3.Contains(item.MainOrderID);
                expression = expression.And(exp);
            }
            if ((!string.IsNullOrWhiteSpace(paralist.supplierName))) //供应商
            {
                Expression<Func<UnPayExchangeOrder, bool>> exp = item => item.PayExchangeSuppliers.Any(c => c.Name.Contains(paralist.supplierName) || c.ChineseName.Contains(paralist.supplierName));
                expression = expression.And(exp);
            }
            if (!string.IsNullOrWhiteSpace(paralist.MultiField))
            {
                Expression<Func<UnPayExchangeOrder, bool>> exp = item => item.ID.Contains(paralist.MultiField) || item.MainOrderID.Contains(paralist.MultiField)
                                                                      || item.PayExchangeSuppliers.Any(c => c.Name.Contains(paralist.MultiField) || c.ChineseName.Contains(paralist.MultiField));
                expression = expression.And(exp);
            }
            lambdas.Add(expression);

            #region 页面数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var list = order.GetPageListData(lambdas.ToArray(), rows, page, paralist.payStatus, paralist.payStatuses);
            Func<UnPayExchangeOrder, object> convert = item => new
            {
                item.MainOrderID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                Currency = item.TinyOrders.FirstOrDefault().Currency,
                Tiny = item.TinyOrders.Select(c => new
                {
                    c.ID,
                    c.DeclarePrice,
                    c.PaidExchangeAmount,
                    UnPaidExchangeAmount = c.DeclarePrice - c.PaidExchangeAmount,
                    SuppliersName = c.suppliers.Select(a => a.Name),
                    SuppliersID = c.suppliers.Select(a => a.ClientSupplierID),
                    IsChecked = false,
                    DDate = c.DDate.HasValue ? c.DDate.Value.ToString("yyyy-MM-dd") : "-"
                }).ToArray(),
                Type = item.OrderType,
            };
            #endregion

            return this.Paging(list, list.Total, convert);
        }

        /// <summary>
        /// 获取付汇记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetPayRecord(string id)
        {
            id = id.InputText();

            var view = Client.Current.MyPayExchangeApplies.GetAppliesByOrderID(id).ToList().OrderByDescending(item => item.CreateDate).Select(item => new
            {
                SupplierName = item.SupplierName,
                ApplyTime = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Amount = item.Amount,
                Status = item.PayExchangeApplyStatus.GetDescription()
            });
            return base.JsonResult(VueMsgType.success, "", view.Json());
        }
        #endregion

        #region 付汇申请
        /// <summary>
        /// 付汇申请
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult Apply()
        {
            var para = Request.Form["para"];
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            //id数组
            var ids = paraArr[0].Split(',');
            ViewBag.lasturl = paraArr[1];
            var current = Client.Current;
            var orders = current.MyUnPayExchangeOrder;

            //获取id数据的订单列表
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<UnPayExchangeOrder, bool>> exp = item => ids.Contains(item.ID);
            lambdas.Add(exp);
            var list = orders.GetOrdersByExceptions(lambdas.ToArray());
            if (list.Count() != ids.Length || ids.Length == 0)
            {
                return View("Error");
            }

            ApplyViewModel model = new ApplyViewModel();
            model.PayExchangeApplyFiles = new List<FileModel>();
            //取供应商交集
            var first_list = list.FirstOrDefault();
            IEnumerable<string> supplier = new string[0];
            list.ForEach(item =>
            {
                if (!supplier.Any())
                {
                    supplier = item.PayExchangeSuppliers.Select(a => a.ClientSupplierID);
                }
                else
                {
                    supplier = supplier.Intersect(item.PayExchangeSuppliers.Select(a => a.ClientSupplierID));
                }
            });

            var supplierDeclares = current.MyUnPayExchangeOrder.GetSupplierDeclarePrice(supplier.ToArray(), ids).Select(item => new
            {
                DeclarePrice = item.DeclarePrice.ToRound(2),
                item.OrderID,
                item.SupplierID,
            });
            ViewBag.ApplyPriceOptions = supplierDeclares.ToArray();

            //IEnumerable<string> supplier = first_list.PayExchangeSuppliers.Select(item => item.ClientSupplierID);
            //foreach (var order in list)
            //{
            //    //获取订单中的供应商交集
            //    supplier = supplier.Intersect(order.PayExchangeSuppliers.Select(item => item.ClientSupplierID));
            //}

            var fileurl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/";

            //如果选的订单的mainorder 是一样的，则文件只要遍历一遍就可以了
            var mainOrderIDS = list.Select(c => c.MainOrderID).Distinct();
            foreach (var orderid in mainOrderIDS)
            {
                var order = current.MyOrder.GetOrderDetail(orderid);
                //获取文件
                var files = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice);
                foreach (var file in files)
                {
                    var item = new FileModel
                    {
                        ID = file.ID,
                        name = file.CustomName,
                        URL = file.Url,
                        fullURL = fileurl + file.Url
                    };
                    model.PayExchangeApplyFiles.Add(item);
                }
            }
            model.PayExchangeApplyFiles = model.PayExchangeApplyFiles.DistinctBy(t => t.name).ToList();

            //供应商数据源
            ViewBag.SupplierOptions = Yahv.Alls.Current.XDTSupplier.Where(item => supplier.Contains(item.ID)).ToArray().Select(item => new
            {
                item.ID,
                item.ChineseName,
                EnglishName = item.Name,
                item.Place,
                Type = item.Type | 0
            }).ToArray();

            model.Rate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode(first_list.Currency).Rate; //实时汇率
            model.USDRate = Yahv.Alls.Current.RealTimeExchangeRates.FindByCode(Currency.USD.ToString()).Rate; //美元实时汇率

            var currency = Yahv.Alls.Current.Currency.FindByCode(first_list.Currency);
            model.Currency = currency?.Name;//币种
            model.CurrencyCode = currency?.Code;

            //付汇订单列表
            var payOrders = list.Select(item => new PayExchangeApplyOrderViewModel
            {
                ID = item.ID,
                Currency = model.Currency,
                DeclarePrice = item.DeclarePrice.ToRound(2),
                PaidExchangeAmount = item.PaidExchangeAmount.ToRound(2),
                CurrentPaidAmount = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
                PaidAmount = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2),
                IsReadonly = false,
            }).ToList();
            model.TotalPayMoney = payOrders.Sum(item => item.CurrentPaidAmount).ToRound(2);
            decimal totalMoney = (model.Rate * payOrders.Sum(item => item.CurrentPaidAmount)).ToRound(2); //结算金额(人民币)
            model.TotalMoney = totalMoney;
            model.UnPayExchangeOrders = payOrders;

            //垫款额度、默认垫款/不垫款选项
            decimal currentUnUsedAdvanceMoney = new AdvanceMoneyAppliesView().GetProductAdvanceMoneyApply(current.XDTClientID);
            model.UnUsedAdvanceMoney = currentUnUsedAdvanceMoney;
            if(currentUnUsedAdvanceMoney > 0 && currentUnUsedAdvanceMoney >= totalMoney)
            {
                model.IsAdvance = "0"; //页面打开先选择垫款
            }
            else
            {
                model.IsAdvance = "1"; //页面打开先选择不垫款
            }

            //手续费承担方,默认“双方承担”
            model.HandlingFeePayerType = HandlingFeePayerType.双方承担.GetHashCode().ToString();

            //付款方式
            ViewBag.PayTypeOptions = ExtendsEnum.ToDictionary<PaymentType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            //手续费承担方选项
            ViewBag.HandlingFeePayerTypeOptions = ExtendsEnum.ToDictionary<HandlingFeePayerType>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
            return View(model);
        }

        /// <summary>
        /// 获取供应商银行
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetSupplierBankAndAddress(string supplierID)
        {
            var current = Client.Current;
            var supplier = Yahv.Alls.Current.XDTSupplier[supplierID];
            if (supplier == null)
            {
                return base.JsonResult(VueMsgType.error, "供应商不存在。");
            }

            var banks = Alls.Current.XDTSupplierBank.SearchBySupplierID(supplier.ID).Select(item => new
            {
                value = item.ID,
                text = item.BankName,
                address = item.BankAddress,
                account = item.BankAccount,
                code = item.SwiftCode,
                region = item.Palce,
                type = item.Type | 0
            });
            return base.JsonResult(VueMsgType.success, "", banks.Json());
        }

        /// <summary>
        /// 付汇申请提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult SubmitApply(ApplyViewModel model)
        {
            try
            {
                var current = Client.Current;
                //验证
                foreach (var item in model.UnPayExchangeOrders)
                {
                    var order = current.MyUnPayExchangeOrder[item.ID];
                    if (order == null)
                    {
                        return base.JsonResult(VueMsgType.error, "订单[" + item.ID + "]状态异常，请重新刷新页面.");
                    }
                    else if ((order.DeclarePrice.ToRound(2) - order.PaidExchangeAmount) < item.CurrentPaidAmount)
                    {
                        return base.JsonResult(VueMsgType.error, "订单[" + item.ID + "]申请付汇金额不能大于可申请付汇金额.");
                    }
                }
                var supplier = Yahv.Alls.Current.XDTSupplier[model.Supplier];
                PvWsOrder.Services.XDTModels.PayExchangeApply pay = new PvWsOrder.Services.XDTModels.PayExchangeApply();
                pay.ClientID = current.XDTClientID;
                pay.UserID = current.ID;
                pay.SupplierName = supplier?.Name;
                pay.SupplierEnglishName = supplier?.Name;
                pay.BankAccount = model.SupplierBankAccount;
                pay.BankAddress = model.SupplierBankAddress ?? "";
                pay.BankName = model.SupplierBankName;
                pay.SwiftCode = model.SupplierBankCode ?? "";
                pay.ABA = model.ABA;
                pay.IBAN = model.IBAN;
                pay.ExchangeRateType = ExchangeRateType.RealTime.ToString();
                pay.Currency = model.CurrencyCode;
                pay.ExchangeRate = model.Rate;
                pay.PaymentType = model.PayType;
                pay.ExpectPayDate = model.ExpectDate;
                pay.SettlemenDate = DateTime.Now.ToString("yyyy年MM月dd日");
                pay.Summary = model.Summary;
                pay.IsAdvanceMoney = int.Parse(model.IsAdvance);

                pay.UnPayExchangeOrders = model.UnPayExchangeOrders.Select(item => new PvWsOrder.Services.XDTModels.UnPayExchangeOrderItem
                {
                    OrderID = item.ID,
                    DeclarePrice = item.DeclarePrice,
                    PaidExchangeAmount = item.PaidExchangeAmount,
                    CurrentPaidAmount = item.CurrentPaidAmount
                }).ToArray();

                //pi文件明细
                if (model.PayExchangeApplyFiles != null)
                {
                    pay.PayExchangeApplyFiles = model.PayExchangeApplyFiles.Select(file => new Services.Models.CenterFileDescription
                    {
                        ID = file.ID,
                        AdminID = current.ID,
                        ClientID = current.MyClients.ID,
                        Url = file.URL,
                        CustomName = file.name,
                        Type = (int)FileType.PIFiles,
                    }).ToArray();
                }

                //手续费承担方、手续费
                pay.HandlingFeePayerType = model.HandlingFeePayerType;
                pay.HandlingFee = model.HandlingFee;
                pay.USDRate = model.USDRate;

                pay.SubmitApply();
                var response = pay.ResponseData;
                if (response.success)
                {
                    return base.JsonResult(VueMsgType.success, "新增成功", response.data);
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, "新增失败");
                }
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion
    }
}