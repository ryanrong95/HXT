using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Preminum
{
    public partial class Edit : Uc.PageBase
    {
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string catalogueID = Request.QueryString["CatalogueID"];
            string id = Request.QueryString["ID"];
            //var declareProducts = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID].Preminums[id];
            //this.Model.DeclareProducts = declareProducts.Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string catalogueID = Request.QueryString["CatalogueID"];
            string id = Request.QueryString["ID"];
            //var preminum = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID].Preminums[id] as
            //    NtErp.Crm.Services.Models.Preminum ?? new NtErp.Crm.Services.Models.Preminum();
            //preminum.CatalogueID = catalogueID;
            //preminum.Name = Request.Form["Name"];
            //preminum.Price = decimal.Parse(Request.Form["Price"]);
            //preminum.EnterSuccess += Preminum_EnterSuccess;
            //preminum.Enter();
        }


        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preminum_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Uri url = Request.UrlReferrer ?? Request.Url;
            Alert("保存成功", url, true);
        }

    }
}