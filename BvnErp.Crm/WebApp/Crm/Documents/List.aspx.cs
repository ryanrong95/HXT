using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Documents
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.TreeData = new NtErp.Crm.Services.Views.DirectoryTree().Tree;
                this.Model.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID).Json();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string directoryid = Request.QueryString["DirectoryID"];
            string path = new NtErp.Crm.Services.Views.DirectoryTree(directoryid).Path;
            var works = new NtErp.Crm.Services.Views.DocumentAlls().Where(item => item.DirectoryID == directoryid);
            //对象转化
            Func<NtErp.Crm.Services.Models.Document, object> linq = item => new
            {
                item.ID,
                item.DirectoryID,
                item.Title,
                item.Name,
                item.Url,
                Size = item.Size + "KB",
                AdminName = item.Admin.RealName,
                item.CreateDate,
                item.Summary,
            };

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            int total = works.Count();
            var query = works.Skip(rows * (page - 1)).Take(rows);

            Response.Write(new
            {
                rows = query.Select(linq).ToArray(),
                total = total,
                path = path,
            }.Json());
        }

        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = new NtErp.Crm.Services.Views.DocumentAlls()[id];
            if (del != null)
            {
                del.Abandon();
            }
        }


        #region 文件转移文件夹
        /// <summary>
        /// 转移文件夹
        /// </summary>
        protected void Move()
        {
            var id = Request.Form["ID"];
            var document = new NtErp.Crm.Services.Views.DocumentAlls()[id];
            if (document != null)
            {
                document.DirectoryID = Request.Form["DirectoryID"];
                document.Enter();
            }
        }
        #endregion


        #region 目录操作
        /// <summary>
        /// 目录保存
        /// </summary>
        protected void SaveDirectory()
        {
            var id = Request.Form["ID"];
            var directory = new NtErp.Crm.Services.Views.DirectoryAlls()[id] ?? new NtErp.Crm.Services.Models.Directory();
            directory.FatherID = Request.Form["FatherID"];
            directory.Name = Request.Form["Name"];
            directory.Enter();
        }

        /// <summary>
        /// 目录删除
        /// </summary>
        protected void DeleteDirectory()
        {
            var ids = Request.Form["ID"].Split(',');
            foreach (string id in ids)
            {
                var directory = new NtErp.Crm.Services.Views.DirectoryAlls()[id];
                if (directory != null)
                {
                    directory.Abandon();
                }
            }
        }
        #endregion

        /// <summary>
        /// 文件下载
        /// </summary>
        protected void download()
        {
            string id = Request.QueryString["id"];
            var doc = new NtErp.Crm.Services.Views.DocumentAlls()[id];
            if (doc != null)
            {
                string strFile = Server.MapPath(doc.Url);

                using (FileStream fs = new FileStream(strFile, FileMode.Open))
                {
                    byte[] bytes = new byte[(int)fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(doc.Name, System.Text.Encoding.UTF8));
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            }

        }
    }
}