using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Views.Rolls.Document;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Document
{
    public partial class List : ErpParticlePage
    {
        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.CatalogData = new vCatalogTree().tree;
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var paramslist = new
            {
                page = int.Parse(Request.QueryString["page"]),
                rows = int.Parse(Request.QueryString["rows"]),
                StartDate = Request.QueryString["StartDate"],
                EndDate = Request.QueryString["EndDate"],
                CatalogID = Request.QueryString["CatalogID"],
            };
            //根据查询条件筛选数据
            var data = new vDocumentsRoll().Where(item => true);
            if (!string.IsNullOrWhiteSpace(paramslist.StartDate))
            {
                data = data.Where(item => item.CreateDate >= DateTime.Parse(paramslist.StartDate));
            }
            if (!string.IsNullOrWhiteSpace(paramslist.EndDate))
            {
                data = data.Where(item => item.CreateDate <= DateTime.Parse(paramslist.EndDate));
            }
            if (!string.IsNullOrWhiteSpace(paramslist.CatalogID))
            {
                var catalogids = paramslist.CatalogID.Split(',');
                data = data.Where(item => catalogids.Contains(item.CatalogID));
            }
            int total = data.Count();
            var query = data.Skip(paramslist.rows * (paramslist.page - 1)).Take(paramslist.rows).ToArray();
            return new
            {
                rows = query.Select(t => new
                {
                    ID = t.ID,
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd hh:mm:ss"),
                    Title = t.Title,
                    CatalogName = t.CatalogName,
                    Abstract = t.Context.Substring(0, t.Context.IndexOf("</p>") + 4),
                    CreatorName = t.CreatorName,
                }).ToArray(),
                total = total,
            }.Json();
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"].Trim();
                var query = new vDocumentsRoll().FirstOrDefault(item => item.ID == ID);
                if (query == null)
                {
                    throw new Exception("该发布内容不存在");
                }
                query.Abandon();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }
    }
}