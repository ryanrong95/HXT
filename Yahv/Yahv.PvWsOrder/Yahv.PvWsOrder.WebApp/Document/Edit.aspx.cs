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
    /// <summary>
    /// 新增编辑页面
    /// </summary>
    public partial class Edit : ErpParticlePage
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
                var documentid = Request.QueryString["ID"];
                var query = new vDocumentsRoll().SingleOrDefault(item => item.ID == documentid);
                if (query != null)
                {
                    this.Model.Document = new
                    {
                        query.CatalogID,
                        query.Title,
                        query.Context,
                    };
                }
            }
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        protected void Submit()
        {
            try
            {
                var paramdata = new
                {
                    ID = Request.Form["ID"],
                    Title = Request.Form["Title"],
                    CatalogID = Request.Form["CatalogID"],
                    Context = Request.Form["Context"],
                };
                //发布内容对象初始化
                var document = new vDocumentsRoll()[paramdata.ID] ?? new Services.Models.vDocument();
                document.Title = paramdata.Title;
                document.CatalogID = paramdata.CatalogID;
                document.Context = HttpUtility.UrlDecode(paramdata.Context);
                document.CreatorID = Erp.Current.ID;
                document.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}