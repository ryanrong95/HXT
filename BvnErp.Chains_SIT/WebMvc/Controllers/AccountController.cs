using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Logs.Services;
using Needs.Wl.Models;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 会员信息
    /// 基本信息、公司信息、收货地址、发票信息
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class AccountController : UserController
    {
        #region GET 会员信息 条款协议 公司信息 手机绑定、邮箱绑定、修改密码、 发票信息 收货地址列表 收货地址

        // GET: 会员信息
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Client()
        {
            ClientViewModel model = new ClientViewModel();
            var user = Needs.Wl.User.Plat.UserPlat.Current;
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

        //修改密码
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Password()
        {
            MobileViewModel model = new MobileViewModel();
            model.IsMain = Needs.Wl.User.Plat.UserPlat.Current.IsMain;
            return View(model);
        }

        //修改邮箱
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Email()
        {
            EmailViewModel model = new EmailViewModel();
            var user = Needs.Wl.User.Plat.UserPlat.Current;

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                model.Email = Regex.Replace(user.Email, @"\w{3}(?=@\w+?.com)", "****");
            }
            model.IsMain = user.IsMain;
            return View(model);
        }

        //修改手机
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Mobile()
        {
            MobileViewModel model = new MobileViewModel();
            var user = Needs.Wl.User.Plat.UserPlat.Current;

            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                model.Phone = Regex.Replace(user.Mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            model.IsMain = false;
            return View(model);
        }

        //GET: 客户公司信息
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _particalClientCompany()
        {
            var user = Needs.Wl.User.Plat.UserPlat.Current;
            var company = user.Client.Company; //公司
            var contanct = company.Contact;   //联系人

            ClientCompanyViewModel model = new ClientCompanyViewModel
            {
                Company_Name = company.Name,
                Corporat = company.Corporate,
                Address = company.Address.ToAddress(),
                DetailAddress = company.Address.ToDetailAddress(),
                CustomsCode = company.CustomsCode,
                Code = company.Code,
                Contacts = contanct.Name,
                Contacts_Moblie = contanct.Mobile,
                Phone = contanct.Tel,
                Fax = contanct.Fax,
                QQ = contanct.QQ,
                IsMain = user.IsMain,
                AllAddress = company.Address
            };
            return PartialView(model);
        }

        /// <summary>
        /// 获取公司客户信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetClientCompany()
        {
            var user = Needs.Wl.User.Plat.UserPlat.Current;
            var company = user.Client.Company; //公司
            var contanct = company.Contact;   //联系人

            ClientCompanyViewModel model = new ClientCompanyViewModel
            {
                Company_Name = company.Name,
                Corporat = company.Corporate,
                Address = company.Address.ToAddress(),
                DetailAddress = company.Address.ToDetailAddress(),
                CustomsCode = company.CustomsCode,
                Code = company.Code,
                Contacts = contanct.Name,
                Contacts_Moblie = contanct.Mobile,
                Phone = contanct.Tel,
                Fax = contanct.Fax,
                QQ = contanct.QQ,
                IsMain = user.IsMain,
                AllAddress = company.Address
            };
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 保存营业执照
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SaveBusineseLisence(string name, string url, string ext)
        {
            try
            {
                var filesView = Needs.Wl.User.Plat.UserPlat.Current.Client.Files();
                filesView.Predicate = item => item.FileType == Needs.Wl.Models.Enums.FileType.BusinessLicense;

                var lisence = filesView.FirstOrDefault();
                if (lisence == null)
                {
                    lisence = new Needs.Wl.Models.ClientFile();
                }

                lisence.FileType = Needs.Wl.Models.Enums.FileType.BusinessLicense;
                lisence.FileFormat = ext;
                lisence.Name = name;
                lisence.Url = url;
                lisence.Enter();

                return base.JsonResult(VueMsgType.success, "保存成功！");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// GET: 条款协议（补充协议）
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Agreement()
        {
            AgreementViewModel model = new AgreementViewModel();
            var agree = Needs.Wl.User.Plat.UserPlat.Current.Client.Agreement();
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

        // GET: 发票信息
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Invoice()
        {
            InvoiceAndConsigneeModel model = new InvoiceAndConsigneeModel();
            model.consignee = new InvoiceConsigneeViewModel();
            model.invoice = new InvoiceViewModel();
            var invoice = Needs.Wl.User.Plat.UserPlat.Current.Client.Invoice();
            model.invoice.InvoiceDeliveryType = "1"; //默认邮寄
            if (invoice != null)
            {
                model.invoice.Title = invoice.Title;
                model.invoice.TaxCode = invoice.TaxCode;
                model.invoice.Address = invoice.Address;
                model.invoice.Tel = invoice.Tel;
                model.invoice.BankName = invoice.BankName;
                model.invoice.BankAccount = invoice.BankAccount;
                model.invoice.InvoiceDeliveryType = ((int)invoice.DeliveryType).ToString();
                model.invoice.InvoiceDeliveryTypeOptions = EnumUtils.ToDictionary<InvoiceDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).Json();
                model.invoice.InvoiceDeliveryTypeName = invoice.DeliveryType.GetDescription();
            }
            model.invoice.IsMain = Needs.Wl.User.Plat.UserPlat.Current.IsMain;
            var consignee = Needs.Wl.User.Plat.UserPlat.Current.Client.InvoiceConsignee();
            if (consignee != null)
            {
                model.consignee.ConsigneeName = consignee.Name;
                model.consignee.ConsigneeMobile = consignee.Mobile;
                model.consignee.ConsigneeTel = consignee.Tel;
                model.consignee.ConsigneeAddress = consignee.Address.ToAddress();
                model.consignee.ConsigneeDetailAddress = consignee.Address.ToDetailAddress();
                model.consignee.ConsigneeAllAddress = consignee.Address;
                model.consignee.ConsigneeEmail = consignee.Email;
            }
            return View(model);
        }

        // GET: 收货地址列表
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult MyConsignees()
        {
            MyConsigneesViewModel model = new MyConsigneesViewModel();
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees;
            view.AllowPaging = false;
            var consignees = view.ToList().Select(item => new ConsigneeViewModel
            {
                ID = item.ID,
                Name = item.Name,
                Consignee = item.Contact.Name,
                Address = item.Address.ToAddress(),
                DetailAddress = item.Address.ToDetailAddress(),
                AllAddress = item.Address,
                Mail = item.Contact.Email,
                IsDefault = item.IsDefault,
                Mobile = item.Contact.Mobile,
            }).ToArray();
            model.IsMain = Needs.Wl.User.Plat.UserPlat.Current.IsMain;
            model.consignees = consignees;
            return View(model);
        }

        // GET: 新增收货地址
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _ParticalConsignee(string id)
        {
            ConsigneeViewModel model = new ConsigneeViewModel();
            return PartialView(model);
        }

        #endregion

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>

        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SendCode(string phone)
        {
            Random Ran = new Random();
            string MessageCode = Ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            Needs.Wl.Web.Mvc.Utils.SmsService.Send(phone, string.Format(SmsContents.ChangeMobile, MessageCode));
            Session[phone] = MessageCode;  //存入session
            return base.JsonResult(VueMsgType.success, "提交成功");
        }

        /// <summary>
        /// 检查手机号码是否重复
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckMobile(string phone)
        {
            phone = phone.InputText();
            var user = Needs.Wl.User.Plat.UserPlat.Current;

            if (phone == user.Mobile)
            {
                return base.JsonResult(VueMsgType.error, "手机号码不能与原号码相同。");
            }

            if (Needs.Wl.Client.Services.UserUtils.UserPhoneIsExist(user.ID, phone))
            {
                return base.JsonResult(VueMsgType.error, "手机号码[" + phone + "]已注册，不可使用。");
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckEmail(string email)
        {
            email = email.InputText();
            var user = Needs.Wl.User.Plat.UserPlat.Current;

            if (email == user.Email)
            {
                return base.JsonResult(VueMsgType.error, "邮箱不能与原邮箱账号相同");
            }

            if (Needs.Wl.Client.Services.UserUtils.UserEmailIsExist(user.ID, email))
            {
                return base.JsonResult(VueMsgType.error, "邮箱[" + email + "]已注册，不可使用。");
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 检查登录名是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CheckName(string name)
        {
            name = name.InputText();

            if (Needs.Wl.Client.Services.UserUtils.UserNameIsExist(Needs.Wl.User.Plat.UserPlat.Current.ID, name))
            {
                return base.JsonResult(VueMsgType.error, "登录名[" + name + "]已注册，不可使用。");
            }

            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 修改会员名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult ChangeUserName(string username)
        {
            username = username.InputText();

            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;
                if (username == user.UserName)
                {
                    return base.JsonResult(VueMsgType.success, "");
                }

                if (Needs.Wl.Client.Services.UserUtils.UserNameIsExist(user.ID, username))
                {
                    return base.JsonResult(VueMsgType.error, "登录名[" + username + "]已注册，不可使用。");
                }

                Needs.Wl.User.Plat.UserPlat.ResetUserName(username);
                return base.JsonResult(VueMsgType.success, "修改登录名成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "修改登录名失败,请稍后重试或联系您的业务员");
            }
        }

        /// <summary>
        /// 提交修改手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult Mobile(MobileViewModel model)
        {
            if (Session[model.NewPhone] != null && Session[model.NewPhone].ToString() == model.Code)
            {
                if (Needs.Wl.User.Plat.UserPlat.Current.ChangeMobile(model.NewPhone))
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

        /// <summary>
        /// 修改邮箱账号发送URL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Email(EmailViewModel model)
        {
            if (Session[model.NewEmail] == null || Session[model.NewEmail].ToString() != model.Code)
            {
                return base.JsonResult(VueMsgType.error, "验证码错误");
            }
            if (Needs.Wl.User.Plat.UserPlat.Current.ChangeEmail(model.NewEmail))
            {
                return base.JsonResult(VueMsgType.success, "邮箱绑定成功");
            }
            return base.JsonResult(VueMsgType.error, "邮箱绑定失败");
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public JsonResult SendMail(string email)
        {
            email = email.InputText();
            Random Ran = new Random();
            string MessageCode = Ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            Session[email] = MessageCode;  //存到Session里
            string picUrl = "http://wl.net.cn/images/logo.png";  //logo的地址
            //邮件内容
            StringBuilder sb = new StringBuilder();
            sb.Append("<div  style='WIDTH: 700px; PADDING-BOTTOM: 50px'>");
            sb.Append("	<div  style='HEIGHT: 40px; PADDING-LEFT: 30px'>");
            sb.AppendFormat("	<h2><img  src='{0}'></h2></div>", picUrl);
            sb.Append("	<div  style=' border: 1px solid #a7c5e2;PADDING-BOTTOM: 0px; PADDING-TOP: 10px; PADDING-LEFT: 100px; PADDING-RIGHT: 55px'>");
            sb.Append("<div style='MARGIN-TOP: 25px; FONT: bold 16px/40px arial'>请查收您的验证码，完成邮箱绑定！ <span style='COLOR: #cccccc'>(请在30分钟内完成)：</span> </div>");
            sb.AppendFormat("	<div style='Color:red;HEIGHT: 50px; WIDTH: 170px; TEXT-ALIGN: center; FONT: bold 18px/36px arial; MARGIN: 25px 0px 0px 140px'>验证码：{0}</div>", MessageCode);
            sb.AppendFormat("</div></div>");

            SmtpContext.Current.Send(email, "华芯通绑定邮箱服务", sb.ToString());  //发送邮件
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 修改密码保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
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

            if (Needs.Wl.User.Plat.UserPlat.Current.ChangePassword(model.Password.StrToMD5(), model.NewPassword))
            {
                return base.JsonResult(VueMsgType.success, "密码修改成功");
            }
            return base.JsonResult(VueMsgType.error, "密码修改失败");
        }


        public ActionResult PictureVerificationCode()
        {
            string code = AppUtils.CreateRandomCode(6); //验证码的字符为4个
            TempData["SecurityCode"] = code; //验证码存放在TempData中
            return File(AppUtils.CreateValidateGraphic(code), "image/Jpeg");
        }

        /// <summary>
        /// 新增收货地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _ParticalConsignee(ConsigneeViewModel model)
        {
            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;
                var view = user.MyConsignees;

                var consignCount = view.RecordCount;
                if (consignCount >= 20)
                {
                    return base.JsonResult(VueMsgType.error, "收货地址不能超过20个。");
                }

                var address = string.Join(" ", model.Address) + " " + model.DetailAddress.Replace(" ", "");
                var consignee = user.MyConsignees[model.ID] ?? new Needs.Wl.Models.ClientConsignee();

                ////根据收货单位名称+联系人+收件地址 判断是否重复
                //view.Predicate = item => item.ClientID == Needs.Wl.User.Plat.UserPlat.Current.ClientID
                //                    && item.Name == model.Name
                //                    && item.ID != model.ID;
                //view.AllowPaging = false;

                //if (view.RecordCount > 0 && consignee.Contact.Name == model.Consignee.Trim())
                //{
                //    return base.JsonResult(VueMsgType.error, "不能添加重复的收货地址。");
                //}

                consignee.ClientID = user.ClientID;
                consignee.Name = model.Name;
                consignee.Contact = new Needs.Wl.Models.Contact
                {
                    Name = model.Consignee.Trim(),
                    Mobile = model.Mobile.Trim(),
                    Email = model.Mail
                };
                consignee.Address = address;
                consignee.IsDefault = false;
                consignee.Status = (int)Needs.Wl.Models.Enums.Status.Normal;
                consignee.Enter();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.success, "编辑收货地址失败，请稍后重试或联系您的业务经理");
            }

            return base.JsonResult(VueMsgType.success, "操作成功");
        }

        /// <summary>
        /// 新增发票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Invoice(InvoiceViewModel model)
        {
            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;
                var invoice = user.Client.Invoice() ?? new Needs.Wl.Models.ClientInvoice();
                invoice.ClientID = user.ClientID;
                invoice.Title = model.Title;
                invoice.InvoiceStatus = Needs.Wl.Models.Enums.ClientInvoiceStatus.UnMarked;
                invoice.TaxCode = model.TaxCode;
                invoice.Address = model.Address;
                invoice.Tel = model.Tel;
                invoice.BankAccount = model.BankAccount;
                invoice.BankName = model.BankName;
                invoice.DeliveryType = (Needs.Wl.Models.Enums.InvoiceDeliveryType)int.Parse(model.InvoiceDeliveryType);
                invoice.Enter();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "新增失败");
            }

            return base.JsonResult(VueMsgType.success, "新增成功");
        }

        [UserActionFilter(UserAuthorize = true)]
        public ActionResult InvoiceConsigee(InvoiceConsigneeViewModel model)
        {
            try
            {
                var user = Needs.Wl.User.Plat.UserPlat.Current;
                var consignee = user.Client.InvoiceConsignee() ?? new Needs.Wl.Models.ClientInvoiceConsignee();
                consignee.ClientID = user.ClientID;
                consignee.Name = model.ConsigneeName;
                consignee.Mobile = model.ConsigneeMobile;
                consignee.Tel = model.ConsigneeTel;
                consignee.Email = model.ConsigneeEmail;
                consignee.Address = string.Join(" ", model.ConsigneeAddress) + " " + model.ConsigneeDetailAddress.Replace(" ", "");
                consignee.Enter();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.success, "新增失败");
            }

            return base.JsonResult(VueMsgType.success, "新增成功");
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SetDefaultConsignee(string id)
        {
            var consignee = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees[id];
            if (consignee == null)
            {
                return base.JsonResult(VueMsgType.error, "操作失败");
            }

            consignee.SetDefault();
            return base.JsonResult(VueMsgType.success, "操作成功");
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DeleteConsignee(string id)
        {
            var consignee = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees[id];
            if (consignee == null)
            {
                return base.JsonResult(VueMsgType.error, "删除失败");
            }
            consignee.Abandon();
            return base.JsonResult(VueMsgType.success, "删除成功");
        }

        /// <summary>
        /// 投诉建议
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult Suggestions(SuggestionViewModel model)
        {
            try
            {
                Suggestions suggestion = new Suggestions();
                suggestion.Name = model.name.InputText();
                suggestion.Phone = model.phone.InputText();
                suggestion.Summary = model.summary.InputText();
                suggestion.Enter();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "提交失败");
            }
            return base.JsonResult(VueMsgType.success, "提交成功");
        }

        #region 数据处理

        /// <summary>
        /// 获取发票信息（刷新数据）
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetInvoiceData()
        {
            var user = Needs.Wl.User.Plat.UserPlat.Current;

            InvoiceAndConsigneeModel model = new InvoiceAndConsigneeModel();
            model.consignee = new InvoiceConsigneeViewModel();
            model.invoice = new InvoiceViewModel();
            var invoice = user.Client.Invoice();
            model.invoice.InvoiceDeliveryType = "1"; //默认邮寄
            if (invoice != null)
            {
                model.invoice.Title = invoice.Title;
                model.invoice.TaxCode = invoice.TaxCode;
                model.invoice.Address = invoice.Address;
                model.invoice.Tel = invoice.Tel;
                model.invoice.BankName = invoice.BankName;
                model.invoice.BankAccount = invoice.BankAccount;
                model.invoice.InvoiceDeliveryType = ((int)invoice.DeliveryType).ToString();
                model.invoice.InvoiceDeliveryTypeOptions = EnumUtils.ToDictionary<InvoiceDeliveryType>().Select(item => new { value = item.Key, text = item.Value }).Json();
                model.invoice.InvoiceDeliveryTypeName = invoice.DeliveryType.GetDescription();
            }
            model.invoice.IsMain = true;
            var consignee = user.Client.InvoiceConsignee();
            if (consignee != null)
            {
                model.consignee.ConsigneeName = consignee.Name;
                model.consignee.ConsigneeMobile = consignee.Mobile;
                model.consignee.ConsigneeTel = consignee.Tel;
                model.consignee.ConsigneeAddress = consignee.Address.ToAddress();
                model.consignee.ConsigneeDetailAddress = consignee.Address.ToDetailAddress();
                model.consignee.ConsigneeAllAddress = consignee.Address;
                model.consignee.ConsigneeEmail = consignee.Email;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 获取收货地址列表
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetMyConsigneesList()
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees;
            view.AllowPaging = false;
            var list = view.ToList().Select(item => new
            {
                ID = item.ID,
                Name = item.Name,
                Consignee = item.Contact.Name,
                Address = item.Address.ToAddress(),
                DetailAddress = item.Address.ToDetailAddress(),
                AllAddress = item.Address,
                Mail = item.Contact.Email,
                IsDefault = item.IsDefault,
                Mobile = item.Contact.Mobile,
            });
            return base.JsonResult(VueMsgType.success, "", list.Json());
        }

        /// <summary>
        ///  获取收货地址信息
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult GetConsigneesInfo(string id)
        {
            var model = new ConsigneeViewModel();
            var list = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees[id];
            if (list != null)
            {
                model.ID = list.ID;
                model.Name = list.Name;
                model.Consignee = list.Contact.Name;
                model.Address = list.Address.ToAddress();
                model.DetailAddress = list.Address.ToDetailAddress();
                model.AllAddress = list.Address;
                model.Mail = list.Contact.Email;
                model.IsDefault = list.IsDefault;
                model.Mobile = list.Contact.Mobile;
            }
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        #endregion
    }
}