using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Needs.Web
{
    static public class Extends
    {
        static public bool IsAjaxRequest(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return request["X-Requested-With"] == "XMLHttpRequest" || (request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }

        static public object Paging<T>(this Needs.Web.Mvc.ClientController response, IQueryable<T> queryable, Func<T, object> converter = null)
        {
            int page = 1, rows = 20;

            var request = HttpContext.Current.Request;

            int.TryParse(request.QueryString["page"], out page);
            int.TryParse(request.QueryString["rows"], out rows);


            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
               var result =new
                {
                    rows = query.ToArray(),
                    total = total
                };
                return result;
            }
            else
            {
                var result =new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                };
                return result;
            }
        }

        static public void Paging<T>(this HttpResponse response, IQueryable<T> queryable, Func<T, object> converter = null)
        {
            int page = 1, rows = 20;

            var request = HttpContext.Current.Request;

            int.TryParse(request.QueryString["page"], out page);
            int.TryParse(request.QueryString["rows"], out rows);


            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                response.Write(new
                {
                    rows = query.ToArray(),
                    total = total
                }.Json());
            }
            else
            {
                response.Write(new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                }.Json());
            }
        }
        static public void Paging<T>(this HttpResponse response, IEnumerable<T> enumers, Func<T, object> converter = null)
        {
            int page = 1, rows = 20;

            var request = HttpContext.Current.Request;

            int.TryParse(request.QueryString["page"], out page);
            int.TryParse(request.QueryString["rows"], out rows);


            int total = enumers.Count();
            var query = enumers.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                response.Write(new
                {
                    rows = query.ToArray(),
                    total = total
                }.Json());
            }
            else
            {
                response.Write(new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                }.Json());
            }
        }
        static public void Json<T>(this HttpResponse response, IQueryable<T> queryable, Func<T, object> converter = null)
        {
            response.Write(queryable.ToArray().Json());
        }
        static public void Json<T>(this HttpResponse response, IEnumerable<T> enumers, Func<T, object> converter = null)
        {
            response.Write(enumers.ToArray().Json());
        }
    }
}
