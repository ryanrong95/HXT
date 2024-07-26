using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Http;
using Needs.Utils.Serializers;
using Needs.Wl.Models;
using Needs.Wl.User.Plat.Models;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebMvc.Models;
using Cookie = Needs.Wl.User.Plat.Utils.Cookie;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 会员的账号信息
    /// 登陆、注册、找回密码页面
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class HomeController : UserController
    {
        #region 登陆、注册、注销、找回密码

        [HttpPost]
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public ActionResult Login(LoginViewModel model)
        {
            //验证登录方式
            IPlatUser user = new PlatUser()
            {
                UserName = model.UserName,
                Password = model.Password,
                RemberInWebSite = model.RemberMe
            };

            if (model.Password.StartsWith("token:"))
            {
                //2、通过记住我自动匹配的token登录              
                var cookie = Cookie.Current[AppConfig.Current.CookieName];
                string token = model.Password.Replace("token:", "");

                if (cookie != null)
                {
                    var userName = HttpUtility.UrlDecode(cookie["name"]);
                    //使用Token登录，需要验证IP是否与当时的IP匹配                        
                    user = Needs.Wl.User.Plat.Identity.TokenLogin(userName, token);

                    //身份验证通过，重新写入cookie
                    if (user != null && user.ID != null && user.UserName == model.UserName)
                    {
                        Session["User"] = user;
                        Needs.Wl.User.Plat.Identity.ResponseCookie(userName, token, model.RemberMe);
                    }
                }
            }
            else
            {
                //1、使用输入的用户名密码登录
                user.Login();
            }

            if (user != null && user.ID != null && user.UserName == model.UserName)
            {
                if (user.Client == null || user.Client.ClientStatus == Needs.Wl.Models.Enums.ClientStatus.Auditing)  //防止登进来的会员信息未完善
                {
                    //清除Cookie
                    HttpCookie cookie = HttpContext.Request.Cookies[AppConfig.Current.CookieName];
                    if (cookie != null)
                    {
                        cookie.Expires = DateTime.Now.AddDays(-1);
                    }

                    HttpContext.Response.AppendCookie(cookie);

                    return base.JsonResult(VueMsgType.warning, "您的企业信息还未完善,请联系您的业务员");
                }
                return base.JsonResult(VueMsgType.success, "");
            }
            else
            {
                return base.JsonResult(VueMsgType.warning, "用户名或密码错误");
            }
        }

        /// <summary>
        /// GET: 登陆
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();

            if (Cookies.Supported)
            {
                var cookie = Cookie.Current[AppConfig.Current.CookieName];
                if (cookie != null && cookie["isRem"] == "True")
                {
                    var name = HttpUtility.UrlDecode(cookie["name"]);
                    var token = HttpUtility.UrlDecode(cookie["token"]);
                    model.UserName = name;
                    model.RemberMe = true;

                    if (string.IsNullOrEmpty(token) == false)
                    {
                        model.Password = "token:" + token;
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// GET: 注册
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        /// <summary>
        /// GET: 找回密码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public ActionResult FindPassword()
        {
            FindPasswordViewModel model = new FindPasswordViewModel();
            return View(model);
        }

        /// <summary>
        /// 会员注销
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Logout()
        {
            HttpContext.Session["User"] = null;

            //设置Cookie无效，不能用于自动登录，如果未设置记住用户名密码，清空Cookie
            Needs.Wl.User.Plat.Identity.ClearCookie();
            //到首页
            return Redirect("/Home/Login");
        }

        /// <summary>
        /// 注册提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public JsonResult Register(RegisterViewModel model)
        {
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 前台用户报名申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public JsonResult Apply(ServiceApplyViewModel model)
        {
            return base.JsonResult(VueMsgType.success, "");
        }

        /// <summary>
        /// 验证邮箱账号是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public JsonResult ValidateEmail(string email)
        {
            //过滤危险字符
            email = email.InputText();
            if (Needs.Wl.Client.Services.UserUtils.UserEmailIsExist(email) == false)
            {
                return base.JsonResult(VueMsgType.warning, "邮箱账号未注册");
            }
            else
            {
                return base.JsonResult(VueMsgType.success, "");
            }
        }

        /// <summary>
        /// 发送邮箱验证地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public JsonResult FindPassword(FindPasswordViewModel model)
        {
            //防止提交时篡改，发送邮件前，需要再次验证邮箱的正确性及用户信息
            var user = Needs.Wl.User.Plat.Identity.FindUserByEmail(model.Email.InputText());

            if (user != null && (string.IsNullOrEmpty(user.ID) == false))
            {
                string token = string.Concat("$", user.ID, user.Email, DateTime.Today, "*", "&", "#").MD5("x");

                var email = new Needs.Wl.Models.EmailToken();
                email.ID = "TOKEN" + token;
                email.Email = model.Email;
                email.Token = token;
                email.Enter();

                string action = Needs.Ccs.Services.Enums.MailVerificationAction.FPA.ToString();   //验证类型（找回密码）

                string url = "Https://" + string.Concat(Request.Url.Authority, "/Home/Verification/?v=", action, "&t=", token);  //给用户发的url验证
                string picUrl = "http://wl.net.cn/images/logo.png";  //logo的地址 TODO:

                //邮件内容
                StringBuilder sb = new StringBuilder();
                sb.Append("<div  style='WIDTH: 700px; PADDING-BOTTOM: 50px'>");
                sb.Append("	<div  style='HEIGHT: 40px; PADDING-LEFT: 30px'>");
                sb.AppendFormat("	<h2><img  src='{0}'></h2></div>", picUrl);
                sb.Append("	<div  style=' border: 1px solid #a7c5e2;PADDING-BOTTOM: 0px; PADDING-TOP: 10px; PADDING-LEFT: 100px; PADDING-RIGHT: 55px'>");
                sb.Append("<div style='MARGIN-TOP: 25px; FONT: bold 16px/40px arial'>请确认您的邮箱，只差一步，您的密码就找回成功了！ <span style='COLOR: #cccccc'>(请在24小时内完成)：</span> </div>");
                sb.Append("<div style='HEIGHT: 36px; WIDTH: 170px; TEXT-ALIGN: center; FONT: bold 18px/36px arial; MARGIN: 25px 0px 0px 140px; BACKGROUND-COLOR: #ff3300'><a style='TEXT-DECORATION: ");
                sb.AppendFormat("none; COLOR: #fff' href='{0}' target='_blank'>完成验证</a> </div>", url);
                sb.Append("<div style='MARGIN-TOP: 40px; COLOR: #ccc; FONT: bold 16px/26px arial'>如果您看不到上方的按钮<br>可点击下面的链接以完成验证或复制下面的链接到浏览器地址栏中完成验证：<br><a ");
                sb.AppendFormat("style='WORD-BREAK: break-all; FONT-WEIGHT: normal; COLOR: #3399ff' href={0}' target='_blank'>{0}</a> </div></div></div>", url);

                SmtpContext.Current.Send(model.Email, "华芯通找回密码服务", sb.ToString());  //发送邮件
                return base.JsonResult(VueMsgType.success, "");
            }
            else
            {
                return base.JsonResult(VueMsgType.error, "邮箱未注册或输入的邮箱错误，请重试");
            }
        }

        /// <summary>
        /// 提交更改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult ResetPassword(ChangePasswordViewModel model)
        {
            try
            {
                if (model.Password == model.ConfirmPassword)
                {
                    Needs.Wl.User.Plat.UserPlat.ResetPassword(model.Token, model.Password);
                    return base.JsonResult(VueMsgType.success, "");
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, "修改密码失败，请填写正确的密码");
                }
            }
            catch
            {
                return base.JsonResult(VueMsgType.error, "修改密码失败，请填写正确的邮件");
            }
        }

        /// <summary>
        /// 验证邮箱中打开的Url
        /// </summary>
        /// <param name="action"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [UserActionFilter(UserAuthorize = false)]
        public ActionResult Verification(string v, string t)
        {
            //是否是邮箱验证
            if (MailVerificationAction.FPA.ToString() == v)
            {
                var token = Needs.Wl.User.Plat.Identity.EmailsToken(t);
                //验证token的有效性
                if (token != null)
                {
                    if (token.CreateDate.AddHours(24) >= DateTime.Now)
                    {
                        var model = new ChangePasswordViewModel();
                        model.Token = token.Token;
                        model.Email = token.Email;
                        return View("ResetPassword", model);
                    }
                }

                //返回已经过时页面
                return View("Error");
            }
            else
            {
                //404 页面不存在
                return View("Error");
            }
        }

        #endregion

        #region 会员中心首页

        // GET: 会员中心首页
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Index()
        {
            IPlatUser user = Needs.Wl.User.Plat.UserPlat.Current;
            IndexViewModel model = new IndexViewModel();
            var client = user.Client;
            if (client == null)
            {
                return View("Error");
            }

            model.ClientCode = client.ClientCode;
            model.ServiceManagerName = client.ServiceManager.RealName;
            model.ServiceManagerTel = client.ServiceManager.Tel;
            model.ServiceManagerMail = client.ServiceManager.Email;
            model.MerchandiserName = client.Merchandiser.RealName;
            model.MerchandiserTel = client.Merchandiser.Tel;
            model.MerchandiserMail = client.Merchandiser.Email;

            var agreement = client.Agreement();
            if (agreement == null)
            {
                return View("Error");
            }

            var payables = client.ClientPayables();
            if (payables != null)
            {
                model.ProductPayable = payables.ProductPayable.GetValueOrDefault().Twoh() < 0 ? 0 : payables.ProductPayable.GetValueOrDefault().Twoh();//应付货款
                model.TaxPayable = payables.TaxPayable.GetValueOrDefault().Twoh() < 0 ? 0 : payables.TaxPayable.GetValueOrDefault().Twoh();//应付税款
                model.AgencyFeePayable = payables.AgencyFeePayable.GetValueOrDefault().Twoh() < 0 ? 0 : payables.AgencyFeePayable.GetValueOrDefault().Twoh();//应付代理费
                model.IncidentalPayable = payables.IncidentalPayable.GetValueOrDefault().Twoh() < 0 ? 0 : payables.IncidentalPayable.GetValueOrDefault().Twoh();//应付杂费
                decimal total = (payables.ProductPayable.GetValueOrDefault() + payables.TaxPayable.GetValueOrDefault() + payables.AgencyFeePayable.GetValueOrDefault() + payables.IncidentalPayable.GetValueOrDefault()).Twoh();
                model.TotalPayableAmount = total < 0 ? 0 : total;//应付总金额

                //货款可用垫款
                model.AvailableProductFee = (agreement.ProductFeeClause.UpperLimit.GetValueOrDefault() - payables.ProductPayable.GetValueOrDefault()).Twoh();
                //货款垫款上限
                model.ProductUpperLimit = agreement.ProductFeeClause.UpperLimit.GetValueOrDefault().Twoh();

                //税款可用垫款
                model.AvailableTaxFee = (agreement.TaxFeeClause.UpperLimit.GetValueOrDefault() - payables.TaxPayable.GetValueOrDefault()).Twoh();
                model.TaxUpperLimit = agreement.TaxFeeClause.UpperLimit.GetValueOrDefault().Twoh();

                //代理费可用垫款
                model.AvailableAgencyFee = (agreement.AgencyFeeClause.UpperLimit.GetValueOrDefault() - payables.AgencyFeePayable.GetValueOrDefault()).Twoh();
                //货款垫款上限
                model.AgencyUpperLimit = agreement.AgencyFeeClause.UpperLimit.GetValueOrDefault().Twoh();

                //杂款可用垫款
                model.AvailableIncidentalFee = (agreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault() - payables.IncidentalPayable.GetValueOrDefault()).Twoh();
                model.IncidentalUpperLimit = agreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault().Twoh();
            }

            //用户登录
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                model.Mobile = Regex.Replace(user.Mobile, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            model.IsOriginPassWord = user.Password == "HXT123".StrToMD5();
            return View(model);
        }

        /// <summary>
        /// 获取首页的订单信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetOrderData(IndexViewModel model)
        {
            var myorders = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyIndex;
            //订单数量
            model.UnConfirmCount = myorders.GetUnConfirmCount();
            model.UnInvoiceCount = myorders.GetUnInvoiceCount();
            model.UnPayExchangeCount = myorders.GetUnPayExchangeCount();
            model.CompeletedCount = myorders.GetWarehouseExitedCount();
            model.HangUpCount = myorders.GetHangUpCount();
            //订单明细
            var list = myorders.OrderByDescending(item => item.CreateDate).Take(11).ToList().Select(item => new
            {
                item.ID,
                Status = item.OrderStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Summary
            }).ToArray();
            model.Orderlist = list;
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SendCode()
        {
            var phone = Needs.Wl.User.Plat.UserPlat.Current.Mobile;
            Random Ran = new Random();
            string MessageCode = Ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            Needs.Wl.Web.Mvc.Utils.SmsService.Send(phone, string.Format(SmsContents.ChangePassword, MessageCode));
            Session[phone] = MessageCode;  //存入session
            return base.JsonResult(VueMsgType.success, "提交成功");
        }

        /// <summary>
        /// 修改密码保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult Password(string Code, string NewPassword)
        {
            var password = Needs.Wl.User.Plat.UserPlat.Current.Password;
            var phone = Needs.Wl.User.Plat.UserPlat.Current.Mobile;
            if (Session[phone] != null && Session[phone].ToString() == Code)
            {
                if (password == NewPassword.Password())
                {
                    return base.JsonResult(VueMsgType.error, "新密码不能与原密码相同");
                }
                if (Needs.Wl.User.Plat.UserPlat.Current.ChangePassword(password, NewPassword))
                {
                    return base.JsonResult(VueMsgType.success, "密码修改成功");
                }
                else
                {
                    return base.JsonResult(VueMsgType.success, "原密码错误");
                }
            }
            return base.JsonResult(VueMsgType.error, "验证码错误");
        }

        /// <summary>
        /// 验证上次页面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult CheckSuperiorPage()
        {
            if (Cookies.Supported)
            {
                var cookie = Cookie.Current[AppConfig.Current.CookieName];
                if (cookie != null)
                {
                    var name = HttpUtility.UrlDecode(cookie["name"]);
                    var token = cookie["token"].ToString();
                    var user = Needs.Wl.User.Plat.Identity.TokenLogin(name, token);
                    if (user != null)
                    {
                        return base.JsonResult(VueMsgType.success, "");
                    }
                }
            }
            return base.JsonResult(VueMsgType.error, "");
        }

        #endregion
    }
}