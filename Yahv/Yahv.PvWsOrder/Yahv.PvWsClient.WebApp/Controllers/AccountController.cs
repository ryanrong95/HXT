//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Web;
//using System.Web.Mvc;
//using Layers.Data.Sqls;
//using Yahv.PvWsClient.WebApp.App_Utils;
//using Yahv.PvWsClient.WebApp.Controllers;
//using Yahv.PvWsClient.WebApp.Models;
//using Yahv.PvWsOrder.Services.Enums;
//using Yahv.PvWsOrder.Services.Views;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;

//namespace Yahv.PvUser.WebApp.Controllers
//{
//    public class AccountController : UserController
//    {
//        #region 账户信息管理
//        /// <summary>
//        /// 账户信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyInformation()
//        {
//            var CurrentUser = Yahv.Client.Current;
//            var Client = CurrentUser.MyClients;
//            var model = new
//            {
//                UserName = CurrentUser.UserName,
//                Mobile = CurrentUser.Mobile,
//                Email = CurrentUser.Email,
//                CompanyName = Client?.Name,
//                Corporation = Client?.Corporation,
//                RegAddress = Client?.RegAddress,
//                Uscc = Client?.Uscc,
//            };
//            return View(model);
//        }

//        /// <summary>
//        /// 检查登录名是否重复
//        /// </summary>
//        /// <param name="name"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckName(string name)
//        {
//            name = name.InputText();
//            var user = Yahv.Client.Current;

//            if (Yahv.Alls.Current.AllUsers.UserNameIsExist(user.ID, name))
//            {
//                return base.JsonResult(VueMsgType.error, "登录名[" + name + "]已注册，不可使用。");
//            }

//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 修改会员名
//        /// </summary>
//        /// <param name="username"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult ChangeUserName(string username)
//        {
//            username = username.InputText();

//            try
//            {
//                var user = Yahv.Client.Current;
//                if (username == user.UserName)
//                {
//                    return base.JsonResult(VueMsgType.success, "");
//                }
//                Client.Current.ChangeUserName(username);
//                return base.JsonResult(VueMsgType.success, "修改登录名成功");
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//        }
//        #endregion


//        #region 条款协议（补充协议）
//        /// <summary>
//        /// 条款协议
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Agreement()
//        {
//            AgreementViewModel model = new AgreementViewModel();
//            var agree = Client.Current.MyAgreement.FirstOrDefault();
//            if (agree != null)
//            {
//                model.StartDate = agree.StartDate.ToString("yyyy年MM月dd日");
//                model.EndDate = agree.EndDate.ToString("yyyy年MM月dd日");
//                model.AgencyRate = agree.AgencyRate.ToString();
//                model.MinAgencyFee = agree.MinAgencyFee.ToString();
//                if (agree.IsPrePayExchange)
//                {
//                    model.IsPrePayExchange = "预换汇";
//                }
//                if (agree.IsLimitNinetyDays)
//                {
//                    model.IsPrePayExchange = "90天内换汇";
//                }

//                //货款条款
//                var productFeeClause = agree.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 1);

//                model.GoodsPeriodType = productFeeClause.PeriodType.GetDescription();
//                model.GoodsExchangeRateType = productFeeClause.ExchangeRateType.GetDescription();
//                model.GoodsUpperLimit = productFeeClause.UpperLimit.ToString();
//                model.isGoodsPrePaid = productFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid;//是否为预付款
//                model.isGoodsAgreedPeriod = productFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod;//是否为约定期限
//                model.GoodsDaysLimit = productFeeClause.DaysLimit.ToString();
//                model.isGoodsMonthly = productFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly;//是否为月结
//                model.GoodsMonthlyDay = productFeeClause.MonthlyDay.ToString();
//                model.isGoodsAgreed = productFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed;//是否为约定汇率
//                model.GoodsExchangeRateValue = productFeeClause.ExchangeRateValue.ToString();

//                //税费条款
//                var taxFeeClause = agree.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 2);

//                model.TaxPeriodType = taxFeeClause.PeriodType.GetDescription();
//                model.TaxExchangeRateType = taxFeeClause.ExchangeRateType.GetDescription();
//                model.TaxUpperLimit = taxFeeClause.UpperLimit.ToString();
//                model.isTaxPrePaid = taxFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid;//是否为预付款
//                model.isTaxAgreedPeriod = taxFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod;//是否为约定期限
//                model.TaxDaysLimit = taxFeeClause.DaysLimit.ToString();
//                model.isTaxMonthly = taxFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly;//是否为月结
//                model.TaxMonthlyDay = taxFeeClause.MonthlyDay.ToString();
//                model.isTaxAgreed = taxFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed;//是否为约定汇率
//                model.TaxExchangeRateValue = taxFeeClause.ExchangeRateValue.ToString();

//                //代理费条款
//                var agencyFeeClause = agree.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 3);

//                model.AgencyFeePeriodType = agencyFeeClause.PeriodType.GetDescription();
//                model.AgencyFeeExchangeRateType = agencyFeeClause.ExchangeRateType.GetDescription();
//                model.AgencyFeeUpperLimit = agencyFeeClause.UpperLimit.ToString();
//                model.isAgencyPrePaid = agencyFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid;//是否为预付款
//                model.isAgencyAgreedPeriod = agencyFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod;//是否为约定期限
//                model.AgencyDaysLimit = agencyFeeClause.DaysLimit.ToString();
//                model.isAgencyMonthly = agencyFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly;//是否为月结
//                model.AgencyMonthlyDay = agencyFeeClause.MonthlyDay.ToString();
//                model.isAgencyAgreed = agencyFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed;//是否为约定汇率
//                model.AgencyExchangeRateValue = agencyFeeClause.ExchangeRateValue.ToString();

//                //杂费条款
//                var incidentalFeeClause = agree.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 4);

//                model.IncidentalPeriodType = incidentalFeeClause.PeriodType.GetDescription();
//                model.IncidentalExchangeRateType = incidentalFeeClause.ExchangeRateType.GetDescription();
//                model.IncidentalUpperLimit = incidentalFeeClause.UpperLimit.ToString();
//                model.isIncidentalPrePaid = incidentalFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.PrePaid;//是否为预付款
//                model.isIncidentalAgreedPeriod = incidentalFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.AgreedPeriod;//是否为约定期限
//                model.IncidentalDaysLimit = incidentalFeeClause.DaysLimit.ToString();
//                model.isIncidentalMonthly = incidentalFeeClause.PeriodType == PvWsOrder.Services.XDTClientView.PeriodType.Monthly;//是否为月结
//                model.IncidentalMonthlyDay = incidentalFeeClause.MonthlyDay.ToString();
//                model.isIncidentalAgreed = incidentalFeeClause.ExchangeRateType == PvWsOrder.Services.XDTClientView.ExchangeRateType.Agreed;//是否为约定汇率
//                model.IncidentalExchangeRateValue = incidentalFeeClause.ExchangeRateValue.ToString();

//                model.InvoiceType = agree.InvoiceType.GetDescription();
//                model.InvoiceRate = agree.InvoiceTaxRate.ToString();
//            }

//            return View(model);
//        }

//        #endregion


//        #region 开票信息管理
//        /// <summary>
//        /// 开票信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Invoice()
//        {
//            var model = this.GetData();

//            return View(model);
//        }

//        /// <summary>
//        /// 获取发票信息（刷新数据）
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetInvoiceData()
//        {
//            var model = this.GetData();
//            return base.JsonResult(VueMsgType.success, "", model.Json());
//        }

//        /// <summary>
//        /// 拼凑发票数据
//        /// </summary>
//        /// <returns></returns>
//        private InvoiceViewModel GetData()
//        {
//            var current = Client.Current;
//            var invoice = current.MyInvoice.SingleOrDefault();
//            var model = new InvoiceViewModel()
//            {
//                ID = invoice?.ID,
//                DeliveryType = ((int?)invoice?.DeliveryType).ToString(),
//                DeliveryTypeName = invoice?.DeliveryType.GetDescription(),
//                CompanyName = current.MyClients.Name,
//                CompanyTel = invoice?.CompanyTel,
//                Type = ((int?)invoice?.Type).ToString(),
//                TypeName = invoice?.Type.GetDescription(),
//                Bank = invoice?.Bank,
//                BankAddress = invoice?.BankAddress,
//                Account = invoice?.Account,
//                RegAddress = invoice?.RegAddress,
//                Postzip = invoice?.Postzip,
//                Name = invoice?.Name,
//                Mobile = invoice?.Mobile,
//                Email = invoice?.Email,
//                TaxperNumber = invoice?.TaxperNumber,
//                Tel = invoice?.Tel,
//                Address = invoice?.Address,
//                InvoiceDeliveryTypeOptions = ExtendsEnum.ToDictionary<InvoiceDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).Json(),
//                InvoiceTypeOptions = ExtendsEnum.ToDictionary<InvoiceType>().Select(item => new { value = item.Key, text = item.Value }).Json(),
//                IsMain = current.IsMain,
//            };
//            return model;
//        }

//        /// <summary>
//        /// 新增发票
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Invoice(InvoiceViewModel model)
//        {
//            try
//            {
//                var user = Client.Current;
//                var invoice = new ShencLibrary.SyncInvoice()
//                {
//                    Type = (InvoiceType)int.Parse(model.Type),
//                    Bank = model.Bank,
//                    BankAddress = model.BankAddress,
//                    Account = model.Account,
//                    TaxperNumber = model.TaxperNumber,
//                    Name = model.Name,
//                    Tel = model.Tel,
//                    Mobile = model.Mobile,
//                    Email = model.Email,
//                    District = District.Unknown,
//                    Address = model.Address,
//                    Postzip = model.Postzip,
//                    DeliveryType = (InvoiceDeliveryType)int.Parse(model.DeliveryType),
//                    CompanyTel = model.CompanyTel,
//                };
//                //开票信息持久化
//                new ShencLibrary.DccInvoice().Enter(user.EnterpriseID, invoice);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, "新增失败" + ex.Message);
//            }

//            return base.JsonResult(VueMsgType.success, "新增成功");
//        }
//        #endregion


//        #region 收货地址
//        /// <summary>
//        /// 收货地址
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyConsignees()
//        {
//            var current = Client.Current;
//            //收货地址列表
//            var linq = current.MyConsignees.ToArray().Select(item => new ConsigneeViewModel
//            {
//                ID = item.ID,
//                Title = item.Title,
//                District = item.District.GetDescription(),
//                Address = item.Address,
//                Name = item.Name,
//                Tel = item.Tel,
//                Mobile = item.Mobile,
//                Email = item.Email,
//                IsDefault = item.IsDefault,
//            });
//            var model = new MyConsigneesViewModel
//            {
//                consignees = linq.ToArray(),
//                IsMain = current.IsMain,
//            };
//            return View(model);
//        }

//        /// <summary>
//        ///  获取收货地址信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult GetConsigneesInfo(string id)
//        {
//            var model = new ConsigneeViewModel();
//            var list = Client.Current.MyConsignees[id];
//            if (list != null)
//            {
//                model.ID = list.ID;
//                model.Title = list.Title;
//                model.District = ((int)list.District).ToString();
//                model.Address = list.Address;
//                model.Name = list.Name;
//                model.Tel = list.Tel;
//                model.Mobile = list.Mobile;
//                model.Email = list.Email;
//            }
//            return base.JsonResult(VueMsgType.success, "", model.Json());
//        }

//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _ParticalConsignee(string id)
//        {
//            ConsigneeViewModel model = new ConsigneeViewModel();
//            var data = ExtendsEnum.ToDictionary<District>().Select(item => new { value = item.Key, text = item.Value }).ToArray();
//            ViewBag.Options = data;
//            return PartialView(model);
//        }

//        /// <summary>
//        /// 获取收货地址列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetMyConsigneesList()
//        {
//            var current = Client.Current;
//            //收货地址列表
//            var linq = current.MyConsignees.ToArray().Select(item => new ConsigneeViewModel
//            {
//                ID = item.ID,
//                Title = item.Title,
//                District = item.District.GetDescription(),
//                Address = item.Address,
//                Name = item.Name,
//                Tel = item.Tel,
//                Mobile = item.Mobile,
//                Email = item.Email,
//                IsDefault = item.IsDefault,
//            });
//            return base.JsonResult(VueMsgType.success, "", linq.Json());
//        }

//        /// <summary>
//        /// 新增收货地址
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _ParticalConsignee(ConsigneeViewModel model)
//        {
//            try
//            {
//                var user = Client.Current;
//                if (user.MyConsignees.Count() > 20)
//                {
//                    return base.JsonResult(VueMsgType.error, "收货地址不能超过20个。");
//                }
//                var consignee = new ShencLibrary.SyncConsignee()
//                {
//                    Title = model.Title,
//                    District = (District)int.Parse(model.District),
//                    Address = model.Address,
//                    Postzip = string.Empty,
//                    Name = model.Name,
//                    Tel = model.Tel,
//                    Mobile = model.Mobile,
//                    Email = model.Email,
//                    IsDefault = model.IsDefault,
//                    DyjCode = string.Empty,
//                };
//                //数据持久化
//                new ShencLibrary.DccConsignee().Enter(user.EnterpriseID, consignee);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//            return base.JsonResult(VueMsgType.success, "操作成功");
//        }


//        /// <summary>
//        /// 设置默认地址
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult SetDefaultConsignee(string id)
//        {
//            //数据持久化
//            new ShencLibrary.DccConsignee().SetDefault(Client.Current.EnterpriseID, id);
//            return base.JsonResult(VueMsgType.success, "操作成功");
//        }

//        /// <summary>
//        /// 删除收货地址
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DeleteConsignee(string id)
//        {
//            new ShencLibrary.DccConsignee().Abandon(Client.Current.EnterpriseID, id);
//            return base.JsonResult(VueMsgType.success, "删除成功");
//        }
//        #endregion


//        #region 供应商
//        #region 列表查询
//        /// <summary>
//        /// 供应商列表数据
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MySuppliers()
//        {
//            //已经使用供应商
//            var supplierids = new PvWsOrder.Services.ClientViews.OrderBaseOrigin().Where(item => item.ClientID == Client.Current.EnterpriseID).
//                Select(item => item.SupplierID).Distinct().ToArray();
//            var supplier = Client.Current.MySupplier.ToList().Select(item => new
//            {
//                item.ID,
//                item.Name,
//                item.EnglishName,
//                item.ChineseName,
//                Grade = item.Grade.GetDescription(),
//                isShowBtn = !supplierids.Contains(item.ID),
//            });
//            return View(supplier);
//        }


//        /// <summary>
//        /// GET: 供应商信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialSupplierInfo()
//        {
//            var model = new SupplierInfoViewModel();
//            return PartialView(model);
//        }

//        /// <summary>
//        /// 供应商银行账户列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult SupplierBank(string id)
//        {
//            string supplerID = id.InputText();
//            if (string.IsNullOrWhiteSpace(supplerID))
//            {
//                return View("Error");
//            }
//            //数据查询
//            var linq = Client.Current.MySupplier[supplerID].MySupplierBank.ToArray().Select(item => new
//            {
//                item.ID,
//                SupplierID = item.EnterpriseID,
//                item.RealName,
//                item.Bank,
//                item.BankAddress,
//                item.Account,
//                item.SwiftCode,
//                Method = item.Methord.GetDescription(),
//                Currency = item.Currency.GetDescription(),
//                InvoiceType = item.InvoiceType.GetDescription(),
//                item.Name,
//                item.Mobile,
//                item.Tel,
//                item.Email,
//            });
//            return View(linq);
//        }

//        /// <summary>
//        /// GET: 供应商银行
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialBeneficiarieInfo()
//        {
//            BeneficiarieInfoViewModel model = new BeneficiarieInfoViewModel();
//            model.Method = ((int)Methord.TT).ToString();
//            model.InvoiceType = ((int)InvoiceType.Unkonwn).ToString();
//            model.Currency = ((int)Currency.Unknown).ToString();
//            var data = new
//            {
//                CurrencyData = ExtendsEnum.ToDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                MethodData = ExtendsEnum.ToDictionary<Methord>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//                InvoiceTypeData = ExtendsEnum.ToDictionary<InvoiceType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
//            };
//            ViewBag.Options = data;
//            return PartialView(model);
//        }

//        /// <summary>
//        /// GET: 供应商提货地址
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialSupplierAddressInfo()
//        {
//            SupplierAddressesViewModel model = new SupplierAddressesViewModel();
//            return PartialView(model);
//        }

//        /// <summary>
//        /// 供应商提货地址列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult SupplierAddress(string id)
//        {
//            string supplerID = id.InputText();
//            if (string.IsNullOrWhiteSpace(supplerID))
//            {
//                return View("Error");
//            }
//            var linq = Client.Current.MySupplier[supplerID].MySupplierAddress.ToArray().Select(item => new
//            {
//                item.ID,
//                SupplierID = item.EnterpriseID,
//                item.Address,
//                item.Postzip,
//                item.Name,
//                item.Tel,
//                item.Mobile,
//                item.Email,
//                IsDefault = item.IsDefault ? "是" : "否",
//            });
//            return View(linq);
//        }

//        /// <summary>
//        /// 获取供应商列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult GetSuppliersList()
//        {
//            var current = Client.Current;
//            //已经使用供应商
//            var supplierids = new PvWsOrder.Services.ClientViews.OrderBaseOrigin().Where(item => item.ClientID == current.EnterpriseID).
//                Select(item => item.SupplierID).Distinct().ToArray();

//            var list = current.MySupplier.ToList().Select(item => new
//            {
//                item.ID,
//                item.Name,
//                item.EnglishName,
//                item.ChineseName,
//                isShowBtn = !supplierids.Contains(item.ID),
//            });
//            return base.JsonResult(VueMsgType.success, "", list.Json());
//        }

//        /// <summary>
//        /// 获取供应商银行列表
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetSuppliersBankList(string ID)
//        {
//            string supplerID = ID.InputText();
//            if (string.IsNullOrWhiteSpace(supplerID))
//            {
//                return base.JsonResult(VueMsgType.error, "查询失败");
//            }
//            //数据查询
//            var linq = Client.Current.MySupplier[supplerID].MySupplierBank.ToArray().Select(item => new
//            {
//                item.ID,
//                SupplierID = item.EnterpriseID,
//                item.RealName,
//                item.Bank,
//                item.BankAddress,
//                item.Account,
//                item.SwiftCode,
//                Method = item.Methord.GetDescription(),
//                Currency = item.Currency.GetDescription(),
//                InvoiceType = item.InvoiceType.GetDescription(),
//                item.Name,
//                item.Mobile,
//                item.Tel,
//                item.Email,
//            });
//            return base.JsonResult(VueMsgType.success, "", linq.Json());
//        }

//        /// <summary>
//        /// 获取供应商地址列表
//        /// </summary>
//        /// <param name="ID"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetSuppliersaddressList(string ID)
//        {
//            string supplerID = ID.InputText();
//            if (string.IsNullOrWhiteSpace(supplerID))
//            {
//                return base.JsonResult(VueMsgType.error, "查询失败");
//            }
//            var linq = Client.Current.MySupplier[supplerID].MySupplierAddress.ToArray().Select(item => new
//            {
//                item.ID,
//                SupplierID = item.EnterpriseID,
//                item.Address,
//                item.Postzip,
//                item.Name,
//                item.Tel,
//                item.Mobile,
//                item.Email,
//                IsDefault = item.IsDefault ? "是" : "否",
//            });
//            return base.JsonResult(VueMsgType.success, "", linq.Json());
//        }
//        #endregion


//        #region 新增修改删除
//        /// <summary>
//        /// POST: 新增修改供应商
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialSupplierInfo(SupplierInfoViewModel model)
//        {
//            var current = Client.Current;

//            var supplier = new ShencLibrary.SyncSupplier()
//            {
//                Chinesename = model.ChineseName,
//                EnglishName = model.EnglishName,
//                Grade = SupplierGrade.Ninth,
//                Corporation = model.Corporation,
//                Uscc = model.Uscc,
//                RegAddress = model.RegAddress,
//            };
//            //供应商持久化
//            new ShencLibrary.DccSupplier().Enter(current.EnterpriseID, supplier);

//            return base.JsonResult(VueMsgType.success, "操作成功");
//        }

//        /// <summary>
//        /// POST: 新增修改供应商银行
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialBeneficiarieInfo(BeneficiarieInfoViewModel data)
//        {
//            try
//            {
//                var user = Client.Current;
//                var bank = new ShencLibrary.SyncBeneficiary()
//                {
//                    RealName = data.RealName,
//                    Bank = data.Bank,
//                    BankAddress = data.BankAddress,
//                    Account = data.Account,
//                    SwiftCode = data.SwiftCode,
//                    Methord = (Methord)int.Parse(data.Method),
//                    Currency = (Currency)int.Parse(data.Currency),
//                    InvoiceType = (InvoiceType)int.Parse(data.InvoiceType),
//                    District = District.Unknown,
//                    Name = data.Name,
//                    Tel = data.Tel,
//                    Mobile = data.Mobile,
//                    Email = data.Email,
//                    IsDefault = false,
//                };
//                //供应商银行账户持久化
//                new ShencLibrary.DccBeneficiary().Enter(Client.Current.EnterpriseID, data.SupplierID, bank);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }
//            return base.JsonResult(VueMsgType.success, "操作成功");
//        }

//        /// <summary>
//        /// POST: 新增修改供应商提货地址
//        /// </summary>
//        /// <returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult _PartialSupplierAddressInfo(SupplierAddressesViewModel data)
//        {
//            try
//            {
//                var user = Client.Current;
//                var supplieraddress = new ShencLibrary.SyncConsignor()
//                {
//                    Address = data.Address,
//                    Postzip = data.Postzip ?? string.Empty,
//                    Name = data.Name,
//                    Tel = data.Tel,
//                    Mobile = data.Mobile,
//                    IsDefault = data.IsDefault,
//                    Email = data.Email,
//                    DyjCode = string.Empty,
//                };
//                //供应商提货地址持久化
//                new ShencLibrary.DccConsignor().Enter(user.EnterpriseID, data.SupplierID, supplieraddress);
//            }
//            catch (Exception ex)
//            {
//                return base.JsonResult(VueMsgType.error, ex.Message);
//            }

//            return base.JsonResult(VueMsgType.success, "操作成功");
//        }


//        /// <summary>
//        /// POST: 删除供应商
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult DelSupplier(string id)
//        {
//            //供应商删除
//            new ShencLibrary.DccSupplier().Abandon(Client.Current.EnterpriseID, id);
//            return base.JsonResult(VueMsgType.success, "删除成功");
//        }

//        /// <summary>
//        /// 删除供应商银行账号
//        /// </summary>
//        /// <param name="bankid">银行账号ID</param>
//        /// <param name="supplierid">供应商ID</param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DelSupplierBank(string bankid, string supplierid)
//        {
//            //供应商银行账户删除
//            new ShencLibrary.DccBeneficiary().Abandon(Client.Current.EnterpriseID, supplierid, bankid);
//            return base.JsonResult(VueMsgType.success, "删除成功");
//        }

//        /// <summary>
//        /// 删除供应商提货地址
//        /// </summary>
//        /// <param name="addressid">提货地址ID</param>
//        /// <param name="supplierid">供应商ID</param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult DelSupplierAddress(string addressid, string supplierid)
//        {
//            //供应商提货地址删除
//            new ShencLibrary.DccConsignor().Abandon(Client.Current.EnterpriseID, supplierid, addressid);
//            return base.JsonResult(VueMsgType.success, "删除成功");
//        }
//        #endregion


//        #region 数据校验
//        /// <summary>
//        /// 验证供应商中文名是否重复
//        /// </summary>
//        /// <param name="CheckSupplierChineseName"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckSupplierChineseName(string ChineseName, string ID)
//        {
//            ChineseName = ChineseName.InputText();
//            ID = ID.InputText();
//            var view = Client.Current.MySupplier;
//            if (view.Count(item => item.ID != ID && item.ChineseName == ChineseName) > 0)
//            {
//                return base.JsonResult(VueMsgType.error, "该供应商名称已存在！");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 验证供应商的银行账号是否重复
//        /// </summary>
//        /// <param name="BankAccount">账号</param>
//        /// <param name="ClientSupplierID">供应商ID</param>
//        /// <param name="ID">账号ID</param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckSupplierBank(string BankAccount, string Code, string ClientSupplierID, string ID)
//        {
//            BankAccount = BankAccount.InputText();
//            ClientSupplierID = ClientSupplierID.InputText();
//            ID = ID.InputText();

//            int count = Client.Current.MySupplier[ClientSupplierID].MySupplierBank.
//                Count(item => item.ID != ID && item.Account == BankAccount && item.SwiftCode == Code);
//            if (count > 0)
//            {
//                return base.JsonResult(VueMsgType.error, "该银行账号已存在！");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        #endregion 


//        #region 详情查询
//        /// <summary>
//        ///  获取供应商信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult GetSupplierInfo(string id)
//        {
//            var model = new SupplierInfoViewModel();
//            var list = Client.Current.MySupplier[id];
//            if (list != null)
//            {
//                model.ID = list.ID;
//                model.ChineseName = list.Name;
//                model.EnglishName = list.EnglishName;
//                model.Uscc = list.Uscc;
//                model.Corporation = list.Corporation;
//                model.RegAddress = list.RegAddress;
//            }
//            return base.JsonResult(VueMsgType.success, "", model.Json());
//        }

//        /// <summary>
//        ///  获取供应商银行信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult GetBeneficiarieInfo(string bankid, string SupplierId)
//        {
//            bankid = bankid.InputText();
//            var model = new BeneficiarieInfoViewModel();
//            var list = Client.Current.MySupplier[SupplierId].MySupplierBank[bankid];
//            if (list == null)
//            {
//                return base.JsonResult(VueMsgType.error, "该银行账户不存在！");
//            }
//            model.ID = list.ID;
//            model.SupplierID = list.EnterpriseID;
//            model.RealName = list.RealName;
//            model.Bank = list.Bank;
//            model.BankAddress = list.BankAddress;
//            model.Account = list.Account;
//            model.SwiftCode = list.SwiftCode;
//            model.Method = ((int)list.Methord).ToString();
//            model.Currency = ((int)list.Currency).ToString();
//            model.InvoiceType = ((int)list.InvoiceType).ToString();
//            model.District = ((int)list.District).ToString();
//            model.Name = list.Name;
//            model.Tel = list.Tel;
//            model.Mobile = list.Mobile;
//            model.Email = list.Email;
//            return base.JsonResult(VueMsgType.success, "", model.Json());
//        }

//        /// <summary>
//        ///  获取供应商提货地址信息
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult GetSupplierAddressInfo(string addressid, string SupplierId)
//        {
//            addressid = addressid.InputText();
//            var model = new SupplierAddressesViewModel();
//            var list = Client.Current.MySupplier[SupplierId].MySupplierAddress[addressid];
//            if (list == null)
//            {
//                return base.JsonResult(VueMsgType.error, "提货地址不存在！");
//            }
//            model.ID = list.ID;
//            model.SupplierID = list.EnterpriseID;
//            model.Name = list.Name;
//            model.Mobile = list.Mobile;
//            model.Address = list.Address;
//            model.Postzip = list.Postzip;
//            model.Tel = list.Tel;
//            model.Email = list.Email;
//            model.IsDefault = list.IsDefault;
//            return base.JsonResult(VueMsgType.success, "", model.Json());
//        }
//        #endregion
//        #endregion


//        #region 账号安全设置：修改登录名、修改手机、邮箱、密码
//        /// <summary>
//        /// 修改手机
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Mobile()
//        {
//            MobileViewModel model = new MobileViewModel();
//            if (!string.IsNullOrWhiteSpace(Client.Current.Mobile))
//            {
//                model.Phone = Regex.Replace(Client.Current.Mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
//            }
//            return View(model);
//        }

//        /// <summary>
//        /// 提交修改手机号
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult Mobile(MobileViewModel model)
//        {
//            if (Session[model.NewPhone] != null && Session[model.NewPhone].ToString() == model.Code)
//            {
//                try
//                {
//                    Client.Current.ChangeMobile(model.NewPhone);
//                }
//                catch (Exception e)
//                {
//                    return base.JsonResult(VueMsgType.error, e.Message);
//                }
//                return base.JsonResult(VueMsgType.success, "手机号码修改成功");
//            }
//            return base.JsonResult(VueMsgType.error, "验证码错误");

//        }

//        /// <summary>
//        /// 发送手机验证码
//        /// </summary>
//        /// <param name="phone"></param>
//        /// <returns></returns>

//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult SendCode(string phone)
//        {
//            Random Ran = new Random();
//            string MessageCode = Ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
//            SmsService.Send(phone, string.Format(SmsContents.ChangeMobile, MessageCode));
//            Session[phone] = MessageCode;  //存入session
//            return base.JsonResult(VueMsgType.success, "提交成功");
//        }

//        /// <summary>
//        /// 检查手机号码是否重复
//        /// </summary>
//        /// <param name="phone"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckMobile(string phone)
//        {
//            phone = phone.InputText();
//            var user = Client.Current;
//            if (phone == user.Mobile)
//            {
//                return base.JsonResult(VueMsgType.error, "手机号码不能与原号码相同");
//            }
//            if (Yahv.Alls.Current.AllUsers.UserPhoneIsExist(user.ID, phone))
//            {
//                return base.JsonResult(VueMsgType.error, "手机号[" + phone + "]已经绑定了芯达通账户");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 修改邮箱
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Email()
//        {
//            EmailViewModel model = new EmailViewModel();
//            if (!string.IsNullOrWhiteSpace(Client.Current.Email))
//            {
//                model.Email = Regex.Replace(Client.Current.Email, @"\w{3}(?=@\w+?.com)", "****");
//            }
//            return View(model);
//        }

//        /// <summary>
//        /// 修改邮箱账号发送URL
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult Email(EmailViewModel model)
//        {
//            if (Session[model.NewEmail] == null || Session[model.NewEmail].ToString() != model.Code)
//            {
//                return base.JsonResult(VueMsgType.error, "验证码错误");
//            }
//            try
//            {
//                Client.Current.ChangeEmail(model.NewEmail);
//            }
//            catch (Exception e)
//            {
//                return base.JsonResult(VueMsgType.error, e.Message);
//            }
//            return base.JsonResult(VueMsgType.success, "邮箱绑定成功");
//        }

//        /// <summary>
//        /// 检查邮箱是否存在
//        /// </summary>
//        /// <param name="phone"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult CheckEmail(string email)
//        {
//            email = email.InputText();
//            var user = Client.Current;
//            if (email == user.Email)
//            {
//                return base.JsonResult(VueMsgType.error, "邮箱账号不能与原邮箱账号相同");
//            }
//            if (Yahv.Alls.Current.AllUsers.UserEmailIsExist(user.ID, email))
//            {
//                return base.JsonResult(VueMsgType.error, "邮箱账号[" + email + "]已经绑定了芯达通账户");
//            }
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 发送邮箱验证码
//        /// </summary>
//        /// <param name="email"></param>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult SendMail(string email)
//        {
//            email = email.InputText();
//            Random Ran = new Random();
//            string MessageCode = Ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
//            Session[email] = MessageCode;  //存到Session里
//            string picUrl = "http://wl.net.cn/images/logo.png";  //logo的地址
//            //邮件内容
//            StringBuilder sb = new StringBuilder();
//            sb.Append("<div  style='WIDTH: 700px; PADDING-BOTTOM: 50px'>");
//            sb.Append("	<div  style='HEIGHT: 40px; PADDING-LEFT: 30px'>");
//            sb.AppendFormat("	<h2><img  src='{0}'></h2></div>", picUrl);
//            sb.Append("	<div  style=' border: 1px solid #a7c5e2;PADDING-BOTTOM: 0px; PADDING-TOP: 10px; PADDING-LEFT: 100px; PADDING-RIGHT: 55px'>");
//            sb.Append("<div style='MARGIN-TOP: 25px; FONT: bold 16px/40px arial'>请查收您的验证码，完成邮箱绑定！ <span style='COLOR: #cccccc'>(请在30分钟内完成)：</span> </div>");
//            sb.AppendFormat("	<div style='Color:red;HEIGHT: 50px; WIDTH: 170px; TEXT-ALIGN: center; FONT: bold 18px/36px arial; MARGIN: 25px 0px 0px 140px'>验证码：{0}</div>", MessageCode);
//            sb.AppendFormat("</div></div>");

//            SmtpContext.Current.Send(email, "芯达通绑定邮箱服务", sb.ToString());  //发送邮件
//            return base.JsonResult(VueMsgType.success, "");
//        }

//        /// <summary>
//        /// 修改密码
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult Password()
//        {
//            MobileViewModel model = new MobileViewModel();
//            return View(model);
//        }

//        /// <summary>
//        /// 修改密码保存
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        [HttpPost]
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult Password(PasswordViewModel model)
//        {
//            if (model.Password == model.NewPassword)
//            {
//                return base.JsonResult(VueMsgType.error, "新密码不能与原密码相同");
//            }

//            if (model.NewPassword != model.ConfirmPassword)
//            {
//                return base.JsonResult(VueMsgType.error, "两次密码输入不同");
//            }
//            try
//            {
//                Client.Current.ChangePassWord(model.Password, model.NewPassword);
//            }
//            catch (Exception e)
//            {
//                return base.JsonResult(VueMsgType.error, e.Message);
//            }
//            return base.JsonResult(VueMsgType.success, "密码修改成功！");
//        }
//        #endregion


//        #region 资金管理
//        /// <summary>
//        /// 资金管理
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult FundManagement()
//        {
//            var currentClient = Client.Current;
//            //按科目的信用数据
//            var myCreaditData = currentClient.MyCredits.Where(item=>item.Currency == Currency.CNY).Select(item => new
//            {
//                item.Business,
//                item.Catalog,
//                item.Total,
//                item.Cost,
//                Left = item.Total - item.Cost,
//            });
//            var goods = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "货款").FirstOrDefault();
//            var tax = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "税款").FirstOrDefault();
//            var agent = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "代理费").FirstOrDefault();
//            var other = myCreaditData.Where(item => item.Business == "代仓储" && item.Catalog == "杂费").FirstOrDefault();
//            var entrygoods = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "货款").FirstOrDefault();
//            var entrytax = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "税款").FirstOrDefault();
//            var entryagent = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "代理费").FirstOrDefault();
//            var entryother = myCreaditData.Where(item => item.Business == "代报关" && item.Catalog == "杂费").FirstOrDefault();

//            decimal? TotalCredit = myCreaditData.Sum(item => (decimal?)item.Total);
//            decimal? TotalCost = myCreaditData.Sum(item => (decimal?)item.Cost);
//            decimal? TotalLeft = myCreaditData.Sum(item => (decimal?)item.Left);
//            //我的总信用
//            var mytotalCredit = new
//            {
//                TotalCredit = string.Format("{0:N}", TotalCredit.GetValueOrDefault()),
//                TotalCost = string.Format("{0:N}", TotalCost.GetValueOrDefault()),
//                TotalLeft = string.Format("{0:N}", TotalLeft.GetValueOrDefault()),
//            };
//            //本港货款货款
//            var myPayable = currentClient.MyPayable.Where(item => item.Business == "代仓储" && item.Currency == Currency.CNY);
//            ///本港应收
//            var TotalVouchersHK = myPayable.Sum(item => (decimal?)item.Price).GetValueOrDefault();

//            var myEntryPayable = currentClient.MyPayable.Where(item => item.Business == "代报关");
//            var EntryTotalVouchers = myEntryPayable.Sum(item => (decimal?)item.Price).GetValueOrDefault();
//            var TotalPayable = EntryTotalVouchers + TotalVouchersHK;

//            //余额
//            var myBalance_CNY = currentClient.MyBalance.Where(item => item.Currency == Currency.CNY).Sum(item => (decimal?)item.Price).GetValueOrDefault();
//            var myBalance_HK = currentClient.MyBalance.Where(item => item.Currency == Currency.HKD).Sum(item => (decimal?)item.Price).GetValueOrDefault();
//            var myBalance_USD = currentClient.MyBalance.Where(item => item.Currency == Currency.USD).Sum(item => (decimal?)item.Price).GetValueOrDefault();

//            //现金流水
//            var myCashRecords = currentClient.MyCashRecords.FirstOrDefault();

//            //一周之内
//            string[] dateArr = new string[] { DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
//            var data = new
//            {
//                MytotalCredit = mytotalCredit,
//                OriginalDate = myCashRecords?.OriginalDate?.ToString("yyyy年MM月dd日"),
//                myCreaditData = new
//                {
//                    GoodsData = new
//                    {
//                        total = string.Format("{0:N}", (goods?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (goods?.Left).GetValueOrDefault()),
//                    },
//                    TaxData = new
//                    {
//                        total = string.Format("{0:N}", (tax?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (tax?.Left).GetValueOrDefault()),
//                    },
//                    AgentData = new
//                    {
//                        total = string.Format("{0:N}", (agent?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (agent?.Left).GetValueOrDefault()),
//                    },
//                    OtherData = new
//                    {
//                        total = string.Format("{0:N}", (other?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (other?.Left).GetValueOrDefault()),
//                    },
//                    EntryGoods = new
//                    {
//                        total = string.Format("{0:N}", (entrygoods?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entrygoods?.Left).GetValueOrDefault()),
//                    },
//                    EntryTax = new
//                    {
//                        total = string.Format("{0:N}", (entrytax?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entrytax?.Left).GetValueOrDefault()),
//                    },
//                    EntryAgent = new
//                    {
//                        total = string.Format("{0:N}", (entryagent?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entryagent?.Left).GetValueOrDefault()),
//                    },
//                    EntryOther = new
//                    {
//                        total = string.Format("{0:N}", (entryother?.Total).GetValueOrDefault()),
//                        left = string.Format("{0:N}", (entryother?.Left).GetValueOrDefault()),
//                    },
//                },
//                HKReceive = new
//                {
//                    TotalVouchersHK = string.Format("{0:N}", TotalVouchersHK),
//                },
//                EntryReceive = new
//                {
//                    EntryTotalVouchers = string.Format("{0:N}", EntryTotalVouchers),
//                },
//                QDate1 = dateArr,
//                QDate2 = dateArr,
//                MyBalance_CNY = string.Format("{0:N}", myBalance_CNY),
//                MyBalance_HK = string.Format("{0:N}", myBalance_HK),
//                MyBalance_USD = string.Format("{0:N}", myBalance_USD),
//            };
//            return View(data);
//        }

//        /// <summary>
//        /// 获取现金流水明细
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetCashRecordList()
//        {
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var payStatus = Request.Form["payStatus"];
//            var currency = Request.Form["currency"];
//            var cash = Client.Current.MyCashRecords;

//            #region 筛选数据
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            if (!string.IsNullOrWhiteSpace(payStatus))
//            {
//                //支出
//                if (payStatus == "1")
//                {
//                    Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.Price < 0;
//                    lambdas.Add(expression);
//                }
//                else //还款
//                {
//                    Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.Price > 0;
//                    lambdas.Add(expression);
//                }
//            }
//            if (!string.IsNullOrWhiteSpace(currency))
//            {
//                var cur = currency == "1" ? Currency.CNY : Currency.USD;
//                Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.Currency == cur;
//                lambdas.Add(expression);
//            }
//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.CreateDate >= dStart && item.CreateDate <= dEnd;
//                lambdas.Add(expression);
//            }
//            #endregion

//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = cash.GetPageListOrders(lambdas.ToArray(), rows, page);
//            Func<Services.Models.FlowAccount, object> convert = item => new
//            {
//                ID = item.ID,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Currency.GetDescription(),
//                Pay = item.Price < 0 ? item.Price.ToString("0.00") : "",
//                Repayment = item.Price > 0 ? item.Price.ToString("0.00") : "",
//                item.OrderID,
//                item.Subject,
//                //Origin = ((Origin)(GetEnumValue(typeof(Origin), item.InWaybill.Consignor.Place))).GetDescription(),
//            };
//            #endregion

//            return this.Paging(list, list.Total, convert);
//        }

//        /// <summary>
//        /// 获取信用流水明细
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public JsonResult GetCreditRecordList()
//        {
//            var startDate = Request.Form["startDate"];  //日期选择
//            var endDate = Request.Form["endDate"];  //日期选择
//            var payStatus = Request.Form["payStatus"];
//            var currency = Request.Form["currency"];

//            #region 筛选数据
//            List<LambdaExpression> lambdas = new List<LambdaExpression>();
//            if (!string.IsNullOrWhiteSpace(payStatus))
//            {
//                //支出
//                if (payStatus == "1")
//                {
//                    Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.Price < 0;
//                    lambdas.Add(expression);
//                }
//                else //还款
//                {
//                    Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.Price > 0;
//                    lambdas.Add(expression);
//                }
//            }
//            if (!string.IsNullOrWhiteSpace(currency))
//            {
//                var cur = currency == "1" ? Currency.CNY : Currency.USD;
//                Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.Currency == cur;
//                lambdas.Add(expression);
//            }
//            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
//            {
//                var dStart = DateTime.Parse(startDate);
//                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
//                var dEnd = DateTime.Parse(endDate);
//                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
//                Expression<Func<Services.Models.FlowAccount, bool>> expression = item => item.CreateDate >= dStart && item.CreateDate <= dEnd;
//                lambdas.Add(expression);
//            }
//            #endregion

//            #region 页面数据
//            int page = int.Parse(Request.Form["page"]);
//            int rows = int.Parse(Request.Form["rows"]);
//            var list = Client.Current.MyCreditRecords.GetPageListOrders(lambdas.ToArray(), rows, page);
//            Func<Services.Models.FlowAccount, object> convert = item => new
//            {
//                ID = item.ID,
//                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
//                Currency = item.Currency.GetDescription(),
//                Pay = item.Price < 0 ? item.Price.ToString("0.00") : "",
//                Repayment = item.Price > 0 ? item.Price.ToString("0.00") : "",
//                item.OrderID,
//                item.Subject,
//                //Origin = ((Origin)(GetEnumValue(typeof(Origin), item.InWaybill.Consignor.Place))).GetDescription(),
//            };
//            #endregion

//            return this.Paging(list, list.Total, convert);
//        }
//        #endregion


//        #region 我的优惠券
//        /// <summary>
//        /// 我的优惠券
//        /// </summary>
//        /// <returns></returns>
//        [UserAuthorize(UserAuthorize = true)]
//        public ActionResult MyCoupon()
//        {
//            var coupon = Client.Current.MyCoupons.ToArray();
//            //未使用优惠券
//            List<object> unUseList = new List<object>();
//            //已使用
//            List<object> usedList = new List<object>();
//            foreach (var item in coupon)
//            {
//                var obj = new
//                {
//                    item.Name,
//                    item.Price,
//                    Type = item.Type.GetDescription(),
//                    item.Conduct,
//                };
//                //剩余优惠券
//                for (var i = 0; i < item.Balance; i++)
//                {
//                    unUseList.Add(obj);
//                }
//                //已使用优惠券
//                for (var i = 0; i < item.Output; i++)
//                {
//                    usedList.Add(obj);
//                }
//            }
//            var data = new
//            {
//                UnUseCount = unUseList.Count,
//                UsedCount = usedList.Count,
//                InvalidCount = 0,
//                UnUseList = unUseList.ToArray(),
//                UsedList = usedList.ToArray(),
//                InvalidList = new List<object>(), //暂无失效优惠券
//            };
//            return View(data);
//        }
//        #endregion

//    }
//}