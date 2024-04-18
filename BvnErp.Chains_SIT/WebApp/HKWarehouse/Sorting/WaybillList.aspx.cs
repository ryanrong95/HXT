using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Sorting
{
    public partial class WaybillList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 国际快递信息
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            var waybill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill;
            var data = waybill.Where(item => item.OrderID == OrderID);
            Func<Needs.Ccs.Services.Models.OrderWaybill, object> convert = item => new
            {
                ID = item.ID,
                CompanyName = item.Carrier.Name,
                WaybillCode = item.WaybillCode,
                ArrivalDate = item.ArrivalDate.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray()
            }.Json());
        }


        /// <summary>
        /// 删除国际快递
        /// </summary>
        /// <returns></returns>
        protected void DeleteWayBill()
        {
            string id = Request.Form["ID"];
            var waybill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill[id];
            if (waybill != null)
            {
                waybill.AbandonSuccess += WayBill_AbandonSuccess;
                waybill.AbandonError += WayBill_AbandonError;
                waybill.Abandon();
            }
        }

        private void WayBill_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
        private void WayBill_AbandonError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Alert("删除失败!");
        }

    }
}