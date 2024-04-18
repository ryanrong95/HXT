using Needs.Interpreter.Extends;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Erp.Translate
{
    public partial class En_importlangConfigs : Needs.Web.Forms.BasePage
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void upload_click(object sender, EventArgs e)
        {
            var request = this.Request;
            if (request.Files.Count > 0)
            {
                var path = Server.MapPath("/files");
                var dir = new System.IO.DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                for (int i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    if (file.InputStream.Length > 0)
                    {
                        file.SaveAs(System.IO.Path.Combine(path, file.FileName));
                    }
                }
            }

            Response.ContentType = "text/html";
            Response.Write("操作成功<br/>");
            Response.Write("<a href=" + request.UrlReferrer.PathAndQuery + ">返回<a/>");
            Response.End();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        protected void delete()
        {
            string fileName = Request.QueryString["file"];
            var path = System.IO.Path.Combine(Server.MapPath("/files"), fileName);
            var fi = new System.IO.FileInfo(path);
            if (fi.Exists)
            {
                fi.Delete();
            }
            Response.End();
        }
        protected void import()
        {
            string langType = Request.Form[nameof(langType)];
            string data = Server.HtmlDecode(Request.Form[nameof(data)]);

            //导入之前先备份
            var old = Needs.Interpreter.Models.En_TopObject.Outputs().Json();
            //backups
            var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups");
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            var path = System.IO.Path.Combine(dirPath, $"lang_{DateTime.Now.ToString("yyyyMMddhhmmss")}.js");
            System.IO.File.WriteAllText(path, old);

            Needs.Interpreter.Models.En_TopObject.Import(langType, data.JsonToEn_TopObject());

            Response.Write("{\"status\":200}");
            Response.End();
        }
    }
}