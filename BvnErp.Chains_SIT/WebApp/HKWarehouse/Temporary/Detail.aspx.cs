using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.HKWarehouse.Temporary
{
    /// <summary>
    /// 暂存新增或编辑操作
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            this.Model.Temporary = "".Json();
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                var temporaryView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Temporary;
                var temporary = temporaryView.Where(item => item.ID == id).FirstOrDefault();
                if (temporary != null)
                {
                    this.Model.Temporary = temporary.Json();
                }
            }

            //包装种类
            this.Model.WrapTypeData = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Select(item => new
            {
                item.ID,
                item.Code,
                item.Name
            }).OrderBy(x => x.Code).Json();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];

            var files = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.TemporaryFile.AsQueryable();
            files = files.Where(t => t.TemporaryID == id);

            Func<TemporaryFile, object> convert = file => new
            {
                ID = file.ID,
                Name = file.Name,
                URL = file.URL,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + file.URL.ToUrl(),//查看路径
                FileFormat = file.FileFormat,
                CreateDate = file.CreateDate.ToString(),
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var logs = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.TemporaryLog.Where(t => t.TemporaryID == ID);
            logs = logs.OrderByDescending(t => t.CreateDate);

            Func<TemporaryLog, object> convert = item => new
            {
                ID = item.ID,
                Summary = item.Summary,
                Type = item.TemporaryStatus.GetDescription(),
                Operator = item.Admin.RealName,
                CreateDate = item.CreateDate.ToString(),
            };
            Response.Write(new
            {
                rows = logs.Select(convert).ToArray()
            }.Json());
        }
    }
}