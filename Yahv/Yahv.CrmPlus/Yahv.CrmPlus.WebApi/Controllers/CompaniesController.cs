using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.CrmPlus.Service.Views.Plugins;
using Yahv.Web.Mvc;

namespace Yahv.CrmPlus.WebApi.Controllers
{
    public class CompaniesController : ClientController
    {
        /// <summary>
        /// 公司付款人
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns></returns>
        public ActionResult Payers(string id)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("公司ID不能为空");
                }

                using (var view = new CompanyPayersView())
                {
                    var data = view.SearchByCompanyID(id);

                    var result = data.ToMyArray();
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 公司收款人
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns></returns>
        public ActionResult Payees(string id)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("公司ID不能为空");
                }

                using (var view = new CompanyPayeesView())
                {
                    var data = view.SearchByCompanyID(id);

                    var result = data.ToMyArray();
                    return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, code = 500, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}