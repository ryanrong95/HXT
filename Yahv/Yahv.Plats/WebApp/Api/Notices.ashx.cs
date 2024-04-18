using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace WebApp.Api
{
    /// <summary>
    /// Notices 的摘要说明
    /// </summary>
    public class Notices : Yahv.Web.Handlers.JsonHandler
    {
        override protected void OnProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];

            #region 读取
            if (action == "read")
            {
                long id = long.Parse(context.Request["id"]);
                Yahv.Erp.Current.Read(id);
                context.Response.Write(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "读取成功"
                }.Json());
            }
            else if (action == "add")
            {
                //new Yahv.Services.Models.Admin
                //{
                //    ID = Yahv.Erp.Current.ID
                //}.Notice(context.Request["title"], context.Request["context"]);
                //context.Response.Write(new JMessage
                //{
                //    success = true,
                //    code = 200,
                //    data = "添加成功"
                //}.Json());
            }

            #endregion
            else
            {
                //条数
                string tops = context.Request.QueryString["tops"];
                int cnt = string.IsNullOrEmpty(tops) ? 500 : int.Parse(tops);
                string json = Yahv.Erp.Current.Tops(cnt).Json();

                context.Response.Write(json);
            }


        }

    }
}