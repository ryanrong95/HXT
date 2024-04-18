using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Models;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class HomeController : BaseController
    {
        #region 登录

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [LoginCheck(IsNeedCheck = false)]
        public ActionResult Login() { return View(); }

        #endregion

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <returns></returns>
        [LoginCheck(IsNeedCheck = false)]
        [HttpPost]
        public JsonResult Login(LoginPostModel postModel)
        {
            Siteuser siteuser = new Siteuser
            {
                Username = postModel.UserName,
                Password = postModel.Password,
                IsRemeber = postModel.RemberMe,
            };

            try
            {
                siteuser.Login();
                return Json(new { type = "success", msg = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "warning", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoginOut()
        {
            //退出登录
            Yahv.PsWms.SzMvc.SiteCoreInfo.Current.LoginOut();
            return Redirect("Index");
        }
    }
}