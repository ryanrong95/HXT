using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class ClientController : BaseController
    {
        #region 页面

        /// <summary>
        /// 会员基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult MyInfo() { return View(); }

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Password() { return View(); }

        #endregion

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (model.Password == model.NewPassword)
                {
                    return Json(new { type = "error", msg = "新密码不能与原密码相同" }, JsonRequestBehavior.AllowGet);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    return Json(new { type = "error", msg = "两次密码输入不同" }, JsonRequestBehavior.AllowGet);
                }

                Yahv.PsWms.SzMvc.SiteCoreInfo.Current.ChangePassword(model.Password, model.NewPassword);

                return Json(new { type = "success", msg = "密码修改成功！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}