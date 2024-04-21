using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class EnterprisesController : ClientController
    {
        // GET: Home
        public ActionResult Index(string callback)
        {
            var ets = new YaHv.Csrm.Services.Views.Rolls.EnterprisesRoll().Select(item => new
            {
                item.ID,
                item.Name
            }).Take(20);
            return this.Jsonp(new Result { Code = "200", Data = ets }, callback);
        }
        [HttpGet]
        public ActionResult Search(string name, string callback)
        {
            try
            {
                Expression<Func<Enterprise, bool>> predicate = item => item.Name.Contains(name);
                var all = new EnterprisesRoll().Where(predicate).ToArray().Select(item => new
                {
                    ID = item.ID,
                    Name = item.Name,
                }).OrderBy(item => item.Name).Take(20);
                return this.Jsonp(new Result { Code = "200", Data = all }, callback);

            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }

        [HttpGet]
        public ActionResult Getbyid(string id, string callback)
        {
            try
            {
                var enterprise = new EnterprisesRoll()[id];//.Where(item => item.ID == id).Select(item => new { ID = item.ID, Name = item.Name });
                return this.Jsonp(new Result { Code = "200", Data = new { ID = enterprise.ID, Name = enterprise.Name } }, callback);
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }

        }
    }
}