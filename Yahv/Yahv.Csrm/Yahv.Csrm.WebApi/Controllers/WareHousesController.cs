using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class WareHousesController : ClientController
    {
        // GET: WareHouses
        [HttpGet]
        public ActionResult Index(string callback)
        {
            var all = new WareHousesRoll().Where(item => item.Status == ApprovalStatus.Normal).
                Select(item => new
                {
                    ID = item.ID,
                    Name = item.Enterprise.Name,
                }).OrderBy(item => item.Name).Take(20).ToArray();
            var data = all.OrderBy(item => item.Name.Contains("上海") ? 1 : 0).ThenBy(item => item.Name);

            if (string.IsNullOrWhiteSpace(callback))
            {

                return this.Json(new Result
                {
                    Code = "200",
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }

            return this.Jsonp(new Result { Code = "200", Data = data }, callback);

        }

        [HttpGet]
        public ActionResult All()
        {
            var all = new WareHousesRoll().Where(item => item.Status == ApprovalStatus.Normal).
               Select(item => new
               {
                   ID = item.ID,
                   Name = item.Enterprise.Name,
               }).OrderBy(item => item.Name).Take(20).ToArray();

            return Json(all, JsonRequestBehavior.AllowGet);
        }
    }
}