using Needs.Utils;
using Needs.Utils.Serializers;
using Needs.Wl.User.Plat;
using Needs.Wl.User.Plat.Models;
using Needs.Wl.User.Plat.Views;
using Needs.Wl.Web.WeChat;
using System;
using System.Web.Mvc;
using WebWeChat.App_Utils;
using WebWeChat.Models;
using WeChat.Api;
using Needs.Linq;
using System.Linq;

namespace WebWeChat.Controllers
{
    [UserAuthorize(UserAuthorize = false)]
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class HomeController : UserController
    {
        #region 会员登录

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string id = "")
        {
            if (WeChatPlat.Current != null)
            {
                return Redirect("/Account/Index");
            }

            LoginViewModel model = new LoginViewModel();
            model.Url = id;
            return View(model);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            IPlatUser user = new WeChatUser()
            {
                UserName = model.Name,
                Password = model.Password,
            };
            user.Login();
            if (user != null && user.ID != null && user.UserName == model.Name)
            {
                return base.JsonResult(VueMsgType.success, "");
            }
            else
            {
                return base.JsonResult(VueMsgType.warning, "用户名或密码错误");
            }
        }
        #endregion

        #region 关于我们

        /// <summary>
        /// 公司简介
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CompanyProfile()
        {
            return View();
        }

        /// <summary>
        /// 人才招聘
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Recruitment()
        {
            return View();
        }

        #endregion

        [AllowAnonymous]
        public ActionResult ClearCookie()
        {
            WeChat.Api.CookieHelper.CleanCookie(new[] { WeChat.Api.CookieHelper.COOKIE_NAME });
            return Content("清除Cookie完成");
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public JsonResult SendCode(string phone)
        {
            Random ran = new Random();
            string messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            App_Utils.SmsService.Send(phone, string.Format(SmsContents.Register, messageCode));
            Session.Timeout = 3;
            Session[phone] = messageCode;  //存入session
          
            return JsonResult(VueMsgType.success, "提交成功");
        }

        /// <summary>
        /// 注册申请
        /// </summary>
        /// <returns></returns>
        public JsonResult RegisterApply(string Company, string User, string Password, string Contact, string Phone, string Code)
        {
            if (Session[Phone] != null && Session[Phone].ToString() == Code)
            {
                var data = new
                {
                    CustomsCode = "",
                    Enterprise = new
                    {
                        Name = Company,
                        Uscc = "",
                        Corporation = "",
                        RegAddress = "",
                    },
                    SiteUser = new
                    {
                        UserName = User,
                        PassWord = Password,
                        RealName = Contact,
                        Mobile = Phone,
                    },
                };
                CrmNoticeExtends.ClientInfo(data.Json());
                return base.JsonResult(VueMsgType.success, "", "保存成功");
            }
            else
            {
                return base.JsonResult(VueMsgType.error, "验证码错误");
            }

        }

        #region 验证企业名称和用户名

        /// <summary>
        /// 检查企业名称是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult CheckCompanyName(string name)
        {
            name = name.InputText();
            var user = new WsClientsTopView();
            if (user.Any(item => item.Name == name))
            {
                return base.JsonResult(VueMsgType.error, "企业[" + name + "]已注册，不可重复注册!");
            }
            return base.JsonResult(VueMsgType.success, "");
        }


        /// <summary>
        /// 检查登录名是否重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult CheckUserName(string name)
        {
            name = name.InputText();
            var user = new UsersAlls();
            if (user.Any(item => item.UserName == name))
            {
                return base.JsonResult(VueMsgType.error, "用户名[" + name + "]已注册，不可使用!");
            }

            return base.JsonResult(VueMsgType.success, "");
        }
        #endregion
    }
}