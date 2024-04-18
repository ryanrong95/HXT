using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders.BasePrice
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCombobox();
            }
        }

        protected void LoadCombobox()
        {
            //租赁产品
            this.Model.productData = Erp.Current.WsOrder.LsProduct.Select(item => new
            {
                Value = item.ID,
                Text = item.SpecID
            });
        }

        /// <summary>
        /// 提交库位租赁价格配置
        /// </summary>
        protected void Submit()
        {
            try
            {
                string ID = Request.Form["ID"];
                string product = Request.Form["product"];
                string month = Request.Form["month"];
                string price = Request.Form["price"];
                string summary = Request.Form["summary"];

                LsProductPrice lsPrice = new LsProductPrice()
                {
                    ID = ID,
                    ProductID = product,
                    Month = int.Parse(month),
                    Price = decimal.Parse(price),
                    Summary = summary,
                    Creator = Erp.Current.ID,
                };

                lsPrice.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}