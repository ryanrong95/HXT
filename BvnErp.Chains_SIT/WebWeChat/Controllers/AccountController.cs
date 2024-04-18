using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Wl.Logs.Services;
using Needs.Wl.Models;
using Needs.Wl.User.Plat;
using Needs.Wl.User.Plat.Models;
using Needs.Wl.Web.WeChat;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebWeChat.App_Utils;
using WebWeChat.Models;

namespace WebWeChat.Controllers
{
    [UserAuthorize(UserAuthorize = true)]
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class AccountController : UserController
    {
        #region 会员中心
        /// <summary>
        /// 会员中心
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            IPlatUser user = WeChatPlat.Current;

            if (!user.IsValid)
            {
                return View("UnComplete");
            }

            IndexViewModel model = new IndexViewModel();
            var client = user.Client;


            if (client == null)
            {
                return View("Error");
            }
            //客户服务
            model.CompanyName = client.Company.Name;
            model.ClientCode = client.ClientCode;
            model.ServiceManagerName = client.ServiceManager.RealName;
            model.ServiceManagerTel = client.ServiceManager.Tel;
            model.ServiceManagerMail = client.ServiceManager.Email;
            model.MerchandiserName = client.Merchandiser.RealName;
            model.MerchandiserTel = client.Merchandiser.Tel;
            model.MerchandiserMail = client.Merchandiser.Email;

            var Agreement = client.Agreement();
            if (Agreement == null)
            {
                return View("Error");
            }
            //应付款
            var pay = new ClientAccountPayablesView()[client.ID];
            if (pay != null)
            {
                model.ProductPayable = pay.ProductPayable.Twoh() < 0 ? 0 : pay.ProductPayable.Twoh();
                model.TaxPayable = pay.TaxPayable.Twoh() < 0 ? 0 : pay.TaxPayable.Twoh();
                model.AgencyFeePayable = pay.AgencyPayable.Twoh() < 0 ? 0 : pay.AgencyPayable.Twoh();
                model.IncidentalPayable = pay.IncidentalPayable.Twoh() < 0 ? 0 : pay.IncidentalPayable.Twoh();
                decimal total = (pay.ProductPayable + pay.TaxPayable + pay.AgencyPayable + pay.IncidentalPayable).Twoh();
                model.TotalPayableAmount = total < 0 ? 0 : total;
            }
            //订单数量
            var myorders = WeChatPlat.Current.WebSite.MyIndex;
            //订单数量
            model.UnConfirmCount = myorders.GetUnConfirmCount();
            model.UnInvoiceCount = myorders.GetUnInvoiceCount();
            model.UnPayExchangeCount = myorders.GetUnPayExchangeCount();
            model.CompeletedCount = myorders.GetWarehouseExitedCount();
            model.HangUpCount = myorders.GetHangUpCount();
            return View(model);
        }
        #endregion

        #region 收货地址

        public ActionResult MyConsignees()
        {
            var view = WeChatPlat.Current.MyConsignees;
            view.AllowPaging = false;
            var list = view.ToList().Select(item => new
            {
                Name = item.Name,
                Consignee = item.Contact.Name,
                Address = item.Address,
                Mail = item.Contact.Email,
                IsDefault = item.IsDefault,
                Mobile = item.Contact.Mobile,
            });
            return View(list);
        }

        #endregion

        #region 修改密码

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Password()
        {
            return View();
        }

        /// <summary>
        /// 修改密码保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Password(PasswordViewModel model)
        {
            if (model.Password == model.NewPassword)
            {
                return base.JsonResult(VueMsgType.error, "新密码不能与原密码相同");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return base.JsonResult(VueMsgType.error, "两次密码输入不同");
            }

            if (WeChatPlat.Current.ChangePassword(model.Password.StrToMD5(), model.NewPassword))
            {
                return base.JsonResult(VueMsgType.success, "密码修改成功");
            }
            return base.JsonResult(VueMsgType.error, "密码修改失败");
        }

        #endregion

        #region 修改手机

        /// <summary>
        /// 修改手机页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Mobile()
        {
            MobileViewModel model = new MobileViewModel();
            if (!string.IsNullOrWhiteSpace(WeChatPlat.Current.Mobile))
            {
                model.Phone = Regex.Replace(WeChatPlat.Current.Mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            //model.IsMain = false;
            return View(model);
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>

        public JsonResult SendCode(string phone)
        {
            Random Ran = new Random();
            string MessageCode = Ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            App_Utils.SmsService.Send(phone, string.Format(SmsContents.ChangeMobile, MessageCode));
            Session[phone] = MessageCode;  //存入session
            return base.JsonResult(VueMsgType.success, "提交成功");
        }

        /// <summary>
        /// 提交修改手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Mobile(MobileViewModel model)
        {
            if (Session[model.NewPhone] != null && Session[model.NewPhone].ToString() == model.Code)
            {

                if (WeChatPlat.Current.ChangeMobile(model.NewPhone))
                {
                    return base.JsonResult(VueMsgType.success, "手机号码修改成功");
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, "手机号码修改失败");
                }
            }
            return base.JsonResult(VueMsgType.error, "验证码错误");

        }

        #endregion

        #region 客户协议
        public async Task<ActionResult> Agreement()
        {
            AgreementViewModel model = new AgreementViewModel();
            var agree = await WeChatPlat.Current.Client.AgreementAsync();
            if (agree != null)
            {
                model.StartDate = agree.StartDate.ToString("yyyy年MM月dd日");
                model.EndDate = agree.EndDate.ToString("yyyy年MM月dd日");
                model.AgencyRate = agree.AgencyRate.ToString();
                model.MinAgencyFee = agree.MinAgencyFee.ToString();
                if (agree.IsPrePayExchange)
                {
                    model.IsPrePayExchange = "预换汇";
                }
                if (agree.IsLimitNinetyDays)
                {
                    model.IsPrePayExchange = "90天内换汇";
                }

                //货款条款
                var productFeeClause = agree.ProductFeeClause;

                model.GoodsPeriodType = productFeeClause.PeriodType.GetDescription();
                model.GoodsExchangeRateType = productFeeClause.ExchangeRateType.GetDescription();
                model.GoodsUpperLimit = productFeeClause.UpperLimit.ToString();
                model.isGoodsPrePaid = productFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.PrePaid;//是否为预付款
                model.isGoodsAgreedPeriod = productFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.AgreedPeriod;//是否为约定期限
                model.GoodsDaysLimit = productFeeClause.DaysLimit.ToString();
                model.isGoodsMonthly = productFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.Monthly;//是否为月结
                model.GoodsMonthlyDay = productFeeClause.MonthlyDay.ToString();
                model.isGoodsAgreed = productFeeClause.ExchangeRateType == Needs.Wl.Models.Enums.ExchangeRateType.Agreed;//是否为约定汇率
                model.GoodsExchangeRateValue = productFeeClause.ExchangeRateValue.ToString();

                //税费条款
                var taxFeeClause = agree.TaxFeeClause;

                model.TaxPeriodType = taxFeeClause.PeriodType.GetDescription();
                model.TaxExchangeRateType = taxFeeClause.ExchangeRateType.GetDescription();
                model.TaxUpperLimit = taxFeeClause.UpperLimit.ToString();
                model.isTaxPrePaid = taxFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.PrePaid;//是否为预付款
                model.isTaxAgreedPeriod = taxFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.AgreedPeriod;//是否为约定期限
                model.TaxDaysLimit = taxFeeClause.DaysLimit.ToString();
                model.isTaxMonthly = taxFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.Monthly;//是否为月结
                model.TaxMonthlyDay = taxFeeClause.MonthlyDay.ToString();
                model.isTaxAgreed = taxFeeClause.ExchangeRateType == Needs.Wl.Models.Enums.ExchangeRateType.Agreed;//是否为约定汇率
                model.TaxExchangeRateValue = taxFeeClause.ExchangeRateValue.ToString();

                //代理费条款
                var agencyFeeClause = agree.AgencyFeeClause;

                model.AgencyFeePeriodType = agencyFeeClause.PeriodType.GetDescription();
                model.AgencyFeeExchangeRateType = agencyFeeClause.ExchangeRateType.GetDescription();
                model.AgencyFeeUpperLimit = agencyFeeClause.UpperLimit.ToString();
                model.isAgencyPrePaid = agencyFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.PrePaid;//是否为预付款
                model.isAgencyAgreedPeriod = agencyFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.AgreedPeriod;//是否为约定期限
                model.AgencyDaysLimit = agencyFeeClause.DaysLimit.ToString();
                model.isAgencyMonthly = agencyFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.Monthly;//是否为月结
                model.AgencyMonthlyDay = agencyFeeClause.MonthlyDay.ToString();
                model.isAgencyAgreed = agencyFeeClause.ExchangeRateType == Needs.Wl.Models.Enums.ExchangeRateType.Agreed;//是否为约定汇率
                model.AgencyExchangeRateValue = agencyFeeClause.ExchangeRateValue.ToString();

                //杂费条款
                var incidentalFeeClause = agree.IncidentalFeeClause;

                model.IncidentalPeriodType = incidentalFeeClause.PeriodType.GetDescription();
                model.IncidentalExchangeRateType = incidentalFeeClause.ExchangeRateType.GetDescription();
                model.IncidentalUpperLimit = incidentalFeeClause.UpperLimit.ToString();
                model.isIncidentalPrePaid = incidentalFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.PrePaid;//是否为预付款
                model.isIncidentalAgreedPeriod = incidentalFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.AgreedPeriod;//是否为约定期限
                model.IncidentalDaysLimit = incidentalFeeClause.DaysLimit.ToString();
                model.isIncidentalMonthly = incidentalFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.Monthly;//是否为月结
                model.IncidentalMonthlyDay = incidentalFeeClause.MonthlyDay.ToString();
                model.isIncidentalAgreed = incidentalFeeClause.ExchangeRateType == Needs.Wl.Models.Enums.ExchangeRateType.Agreed;//是否为约定汇率
                model.IncidentalExchangeRateValue = incidentalFeeClause.ExchangeRateValue.ToString();

                model.InvoiceType = agree.InvoiceType.GetDescription();
                model.InvoiceRate = agree.InvoiceTaxRate.ToString();
            }
            return View(model);
        }
        #endregion

        #region 客户服务
        /// <summary>
        /// 客户服务
        /// </summary>
        /// <returns></returns>
        public ActionResult ClientService()
        {
            ClientServiceViewModel model = new ClientServiceViewModel();
            IPlatUser user = WeChatPlat.Current;
            var client = user.Client;
            if (client == null)
            {
                return View("Error");
            }
            //客户服务
            model.ServiceManagerName = client.ServiceManager.RealName;
            model.ServiceManagerTel = client.ServiceManager.Tel;
            model.ServiceManagerMail = client.ServiceManager.Email;
            model.MerchandiserName = client.Merchandiser.RealName;
            model.MerchandiserTel = client.Merchandiser.Tel;
            model.MerchandiserMail = client.Merchandiser.Email;
            return View(model);
        }
        #endregion

        #region 账户信息

        /// <summary>
        /// 账户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Client()
        {
            ClientViewModel model = new ClientViewModel();
            var user = WeChatPlat.Current;
            var client = user.Client;

            model.ID = client.ClientCode;
            model.User_Name = user.UserName;
            model.Password = "******";
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                model.Mobile = Regex.Replace(user.Mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                model.Mail = Regex.Replace(user.Email, @"\w{3}(?=@\w+?.com)", "****");
            }
            model.Company_Name = client.Company.Name;
            model.IsMain = user.IsMain;
            return View(model);
        }

        #endregion

        #region 公司信息

        /// <summary>
        /// 公司信息(部分视图页)
        /// </summary>
        /// <returns></returns>
        public ActionResult _particalClientCompany()
        {
            ClientCompanyViewModel model = new ClientCompanyViewModel();
            var user = WeChatPlat.Current;
            var company = user.Client.Company; //公司
            var contanct = company.Contact;   //联系人
            model.Company_Name = company.Name;
            model.Corporat = company.Corporate;
            model.Address = company.Address.ToAddress();
            model.DetailAddress = company.Address.ToDetailAddress();
            model.CustomsCode = company.CustomsCode;
            model.Code = company.Code;
            model.Contacts = contanct.Name;
            model.Contacts_Moblie = contanct.Mobile;
            model.Phone = contanct.Tel;
            model.Fax = contanct.Fax;
            model.QQ = contanct.QQ;
            model.IsMain = user.IsMain;
            model.AllAddress = company.Address;
            return PartialView(model);
        }

        #endregion

        #region 账户设置

        /// <summary>
        /// 账户设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Settings()
        {
            return View();
        }

        #endregion

        #region 发票

        /// <summary>
        /// 发票信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Invoice()
        {
            InvoiceAndConsigneeModel model = new InvoiceAndConsigneeModel();
            var client = WeChatPlat.Current.Client;
            var invoice = client.Invoice();
            if (invoice != null)
            {
                model.Title = invoice.Title;
                model.TaxCode = invoice.TaxCode;
                model.Address = invoice.Address;
                model.Tel = invoice.Tel;
                model.BankName = invoice.BankName;
                model.BankAccount = invoice.BankAccount;
                model.InvoiceDeliveryTypeName = invoice.DeliveryType.GetDescription();
            }
            var consignee = client.InvoiceConsignee();
            if (consignee != null)
            {
                model.ConsigneeName = consignee.Name;
                model.ConsigneeMobile = consignee.Mobile;
                model.ConsigneeTel = consignee.Tel;
                model.ConsigneeAllAddress = consignee.Address;
                model.ConsigneeEmail = consignee.Email;
            }
            return View(model);
        }

        #endregion

        #region 修改登录名

        /// <summary>
        /// 修改会员名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeUserName(string username)
        {
            username = username.InputText();

            try
            {
                var user = Needs.Wl.User.Plat.WeChatPlat.Current;
                if (username == user.UserName)
                {
                    return base.JsonResult(VueMsgType.success, "");
                }

                if (Needs.Wl.Client.Services.UserUtils.UserNameIsExist(user.ID, username))
                {
                    return base.JsonResult(VueMsgType.error, "登录名[" + username + "]已注册，不可使用。");
                }

                Needs.Wl.User.Plat.WeChatPlat.ResetUserName(username);
                return base.JsonResult(VueMsgType.success, "修改登录名成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "修改登录名失败,请稍后重试或联系您的业务员");
            }
        }

        #endregion

        #region 登出

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public JsonResult Logout()
        {
            WeChatPlat.Current.Logout();
            return base.JsonResult(VueMsgType.success, "登录名修改成功");
        }

        #endregion

        public ActionResult UnComplete()
        {
            return View();
        }
    }
}