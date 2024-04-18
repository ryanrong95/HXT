using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Sorting
{
    public partial class Anomaly : Uc.PageBase
    { 
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void LoadData()
        {
            string orderID = Request.QueryString["OrderID"];

          var orderControl = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderControls.
                    Where(x => x.OrderID == orderID && x.ControlType == Needs.Ccs.Services.Enums.OrderControlType.SortingAbnomaly).FirstOrDefault();
            if (orderControl != null)
            {
                this.Model.Summry = new
                {

                    Summary = orderControl.Summary,

                }.Json();

            }
            else {
                this.Model.Summry = new { }.Json();
            }
        }

        /// <summary>
        /// 分拣异常
        /// </summary>
        protected void AbnormalSorting()
        {
            try
            {
                string ID = Request.Form["ID"];
                string Summry = Request.Form["Summary"];
                var entryNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNotice[ID];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                entryNotice.SetAdmin(admin); ;
                entryNotice.Summary = Summry;
                entryNotice.AbnormalSorting();
                Response.Write((new { success = true, message = "操作成功，订单已挂起！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

    }
}