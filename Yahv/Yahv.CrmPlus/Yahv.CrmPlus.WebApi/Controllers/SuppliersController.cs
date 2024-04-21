using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.CrmPlus.Service.Views.Plugins;
using Yahv.Web.Mvc;

namespace Yahv.CrmPlus.WebApi.Controllers
{
    public class SuppliersController : ClientController
    {
        /// <summary>
        /// 获取新注册的客户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewRegistered()
        {
            //TODO: 暂时使用该视图
            //var supplier = Erp.Current.CrmPlus.Suppliers.OrderByDescending(item => item.ModifyDate)
            //    .Select(item => new { item.ID, item.Name, item.Grade }).FirstOrDefault();

            var supplier = new { ID = "Ep20210302001", Name = "测试", Grade = 1 };

            return Json(new { code = 200, success = true, data = supplier }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 供应商收款人
        /// </summary>
        /// <param name="name">供应商名称</param>
        /// <returns></returns>
        public ActionResult Payees(string name)
        {
            try
            {
                if (Erp.Current == null)
                {
                    throw new Exception("请先登录");
                }

                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception("供应商名称不能为空");
                }

                using (var view = new SupplierPayeesView())
                {
                    var data = view.SearchBySupplierName(name);

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