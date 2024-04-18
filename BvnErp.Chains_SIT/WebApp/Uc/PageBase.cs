using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Uc
{
    abstract public class PageBase : Needs.Web.Sso.Forms.ErpPage
    {
        protected PageBase()
        {
            this.Init += PageBase_Init;
        }

        private void PageBase_Init(object sender, EventArgs e)
        {
            //授权进入芯达通系统的管理员
            //登录系统后，验证是否存在芯达通系统的管理员，否则添加
            //string id = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            //var adminWl = Needs.Wl.Admin.Plat.AdminPlat.Admins[id];
            //if (adminWl == null)
            //{
            //    var admin = new Needs.Ccs.Services.Models.Admin();
            //    admin.ID = id;
            //    admin.Summary = string.Empty;
            //    admin.Enter();
            //}
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