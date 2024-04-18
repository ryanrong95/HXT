using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Catalogues
{
    public partial class ProductEdit : Uc.PageBase
    {
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void LoadData()
        {
            string catalogueID = Request.QueryString["CatalogueID"];
            string id = Request.QueryString["ID"];
            var declareProducts = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID].DeclareProducts.SingleOrDefault(item => item.ID == id); ;
            this.Model.DeclareProducts = declareProducts.Json();
        }


        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var standard = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts;
            this.Model.Standard = standard.Select(item => new { item.ID, item.Name }).Json();
            this.Model.ObjectType = EnumUtils.ToDictionary<ObjectType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Currency = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            //this.Model.Sended = EnumUtils.ToDictionary<Sended>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Status = EnumUtils.ToDictionary<ProductStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
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
            var declareProduct = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID].DeclareProducts.SingleOrDefault(item => item.ID == id) as
                NtErp.Crm.Services.Models.DeclareProduct ?? new NtErp.Crm.Services.Models.DeclareProduct();
            declareProduct.CatelogueID = catalogueID;
            string standardID = Request.Form["StandardID"];
            declareProduct.StandardProduct = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.StandardProduct>.Create(standardID);
            declareProduct.Amount = int.Parse(Request.Form["Amount"]);
            declareProduct.UnitPrice = decimal.Parse(Request.Form["UnitPrice"]);
            declareProduct.Delivery = Request.Form["Delivery"];
            declareProduct.Status = (ProductStatus)int.Parse(Request.Form["Status"]);
            if (!string.IsNullOrWhiteSpace(Request.Form["Count"]))
            {
                declareProduct.Count = int.Parse(Request.Form["Count"]);
            }
            declareProduct.EnterSuccess += DeclareProduct_EnterSuccess;
            declareProduct.Enter();
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeclareProduct_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Uri url = Request.UrlReferrer ?? Request.Url;
            Alert("保存成功！", url, true);
        }

    }
}