using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApp.Controllers
{
    public class UserController : Controller
    {
        protected object Paging<T>(IQueryable<T> queryable, Func<T, object> converter = null)
        {
            int page = 1, rows = 10;

            var request = System.Web.HttpContext.Current.Request;

            int.TryParse(request.QueryString["page"], out page);
            int.TryParse(request.QueryString["rows"], out rows);
            if (page <= 0)
            {
                page = 1;
            }
            if (rows <= 0)
            {
                rows = 10;
            }

            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                return new
                {
                    rows = query.ToArray(),
                    total = total
                };
            }
            else
            {
                return new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                };
            }
        }

        protected object Paging<T>(IEnumerable<T> queryable, Func<T, object> converter = null)
        {
            int page = 1, rows = 10;

            var request = System.Web.HttpContext.Current.Request;

            int.TryParse(request.QueryString["page"], out page);
            int.TryParse(request.QueryString["rows"], out rows);
            if (page <= 0)
            {
                page = 1;
            }
            if (rows <= 0)
            {
                rows = 10;
            }

            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                return new
                {
                    rows = query.ToArray(),
                    total = total
                };
            }
            else
            {
                return new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                };
            }
        }

        protected object Paging<T>(IQueryable<T> queryable, int pageindex, int pagesize = 10, Func<T, object> converter = null)
        {
            var request = System.Web.HttpContext.Current.Request;

            int total = queryable.Count();
            var query = queryable.Skip(pagesize * (pageindex - 1)).Take(pagesize);

            if (converter == null)
            {
                return new
                {
                    rows = query.ToArray(),
                    total = total
                };
            }
            else
            {
                return new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                };
            }
        }

        protected ActionResult CurrentResult;

        protected void eJson(object data)
        {
            this.CurrentResult = Json(data);
        }

        protected Buyer.Services.Models.ClientTop Client { get; private set; }

        /// <summary>
        /// 请求执行前
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var user = this.Client = MvcApp.Buyer.Services.OldSso.Current;

            if (user == null)
            {
                throw new Exception("logon information lost!!");
            }
        }
    }
}