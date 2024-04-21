using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;

namespace Yahv.CrmPlus.WebApi.Controllers
{
    public class StandardController : Controller
    {
        // GET: Standard
        public ActionResult Index()
        {
            return View();
        }
        #region 标准型号
        /// <summary>
        /// 标准型号
        /// </summary>
        /// <param name="key">型号名称</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult PartNumbers(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var pns = new StandardPartNumbersRoll().Where(item => item.PartNumber.Contains(key) || item.Brand.Contains(key) || item.ID == key).Select(item => new
                {
                    ID = item.ID,
                    item.PartNumber,
                    item.Brand
                });
                return Json(new
                {
                    success = true,
                    code = 200,
                    data = pns.Take(20).ToArray()
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                success = true,
                code = 200,
                data = new object[0]
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 标准品牌
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult StandardBrands(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var brands = Erp.Current.CrmPlus.StandardBrands.Where(item => item.Name.Contains(key) || item.ID == key).Select(item => new
                {
                    item.ID,
                    item.Name,
                });
                return Json(new
                {
                    success = true,
                    code = 200,
                    data = brands.Take(20).ToArray()
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                success = true,
                code = 200,
                data = new object[0]
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 代理品牌
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AgentBrands(string key)
        {
            var brands = Erp.Current.CrmPlus.StandardBrands.Where(item => item.IsAgent && (item.Name.Contains(key) || item.ID == key))
                .Select(item => new
                {
                    item.ID,
                    item.Name,
                });

            return Json(new
            {
                success = true,
                code = 200,
                data = brands.Take(20).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}