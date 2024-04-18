using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Needs.Cbs.WebApp.Uc
{
    abstract public class PageBase : Needs.Web.Sso.Forms.ErpPage
    {
        protected PageBase()
        {
            this.Init += PageBase_Init;
        }

        private void PageBase_Init(object sender, EventArgs e)
        {
            
        }

        protected void Paging<T>(IEnumerable<T> queryable, Func<T, object> converter = null)
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                Response.Write(new
                {
                    rows = query.ToArray(),
                    total = total
                }.Json());
            }
            else
            {
                Response.Write(new
                {
                    rows = query.Select(converter).ToArray(),
                    total = total
                }.Json());
            }
        }
    }
}