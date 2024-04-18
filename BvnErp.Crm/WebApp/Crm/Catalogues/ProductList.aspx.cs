using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Catalogues
{
    public partial class ProductList : Uc.PageBase
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

            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var Products = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[ID].DeclareProducts;

            Func<NtErp.Crm.Services.Models.DeclareProduct, object> convert = item => new {
                item.ID,
                item.Amount,
                item.UnitPrice,
                item.Count,
                ProductName = item.StandardProduct.Name,
                StatusName = item.Status.GetDescription(),
            };

            this.Paging(Products, convert);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            string catalogueID = Request.QueryString["ID"];
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues[catalogueID].DeclareProducts.SingleOrDefault(item => item.ID == id);
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}