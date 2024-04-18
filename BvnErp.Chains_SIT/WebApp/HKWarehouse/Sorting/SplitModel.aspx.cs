using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Models;

namespace WebApp.HKWarehouse.Sorting
{
    public partial class SplitModel : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        ///拆分型号信息
        /// </summary>
        protected void LoadData()
        {
            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;

            this.Model.OriginData = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item =>
            new
            {
                OriginValue = item.Code,
                OriginText = item.Code + " " + item.Name
            }).Json();

            this.Model.OrderItemData = "".Json();
            string orderItemID = Request.QueryString["OrderItemID"];
            var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
            if (orderItem != null)
            {
                this.Model.OrderItemData = new
                {
                    Origin = orderItem.Origin,
                    Model = orderItem.Model.Replace("\"", ReplaceQuotes),
                    Manufacturer = orderItem.Manufacturer,
                }.Json();
            }
        }

        /// <summary>
        /// 拆分型号
        /// </summary>
        protected void SplitModelData()
        {
            try
            {
                //string productID = Request.Form["ProductID"];
                string orderItemID = Request.Form["OrderItemID"];
                string origin = Request.Form["Origin"];
                string manufacturer = Request.Form["Manufacturer"].Replace("amp;","");
                decimal quantity = Convert.ToDecimal(Request.Form["Quantity"]);
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems[orderItemID];
                orderItem.SplitModel(origin, quantity,manufacturer, admin);

                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改失败：" + ex.Message }).Json());
            }
        }
    }
}