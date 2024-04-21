using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class AgentController : ClientController
    {
        // GET: Agent
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 代理品牌信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Brands(string id)
        {

            var items = new CompanyBrandsView().Select(item => new
            {
                ID = item.BrandID,
                Name = item.BrandName
            });
            if (!string.IsNullOrWhiteSpace(id))
            {
                return this.Json(new Result
                {
                    Code = "200",
                    Data = items.Where(item => item.ID == id).ToArray()
                }, JsonRequestBehavior.AllowGet);
               
            }
            return this.Json(new Result
            {
                Code = "200",
                Data = items.ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 客户或供应商
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Enterprises(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return this.Json(new Result { Code = "200", Data = new object[0] }, JsonRequestBehavior.AllowGet);
            }
            var all = new AgentEnterprisesView().Where(item => item.Name.Contains(key)).Take(20).ToArray();
            return this.Json(new Result { Code = "200", Data = all }, JsonRequestBehavior.AllowGet);
        }
    }
}