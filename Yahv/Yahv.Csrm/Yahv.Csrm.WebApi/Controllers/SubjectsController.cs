using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class SubjectsController : ClientController
    {
        /// <summary>
        /// 科目
        /// </summary>
        /// <param name="type">类型 0应收 1应付</param>
        /// <param name="catalog">分类</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Subjects(int type = 0, string catalog = "")
        {
            var json = new JSingle<dynamic>()
            {
                code = 200,
                success = true
            };

            try
            {
                using (var subjectsView = new SubjectRoll())
                {
                    Expression<Func<Subject, bool>> expression = item => item.Type == (SubjectType)type;

                    if (!string.IsNullOrWhiteSpace(catalog))
                    {
                        expression = expression.And(item => item.Catalog == catalog);
                    }

                    json.data = subjectsView.Where(expression).ToArray().Select(item => new
                    {
                        item.Type,
                        item.Catalog,
                        item.Conduct,
                        Currency = item.Currency?.GetDescription(),
                        item.Name,
                        item.Price,
                        Steps = string.IsNullOrWhiteSpace(item.Steps) ? null : item.Steps.Replace("\r", "").Replace("\n", "").Replace("\t", ""),
                    });
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
                json.data = ex.Message;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}