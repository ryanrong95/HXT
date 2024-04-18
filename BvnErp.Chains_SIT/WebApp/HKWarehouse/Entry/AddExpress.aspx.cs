using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Entry
{
    /// <summary>
    /// 装箱界面
    /// 香港操作
    /// </summary>
    public partial class AddExpress : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            this.Model.CarrierData = "".Json();
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers;
            this.Model.CarrierData = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers.Where(x=>x.Status==Status.Normal).Select(item => new
            {
                value = item.ID,
                text = item.Name,
                Type=item.CarrierType
            }).Where(x=>x.Type==CarrierType.InteExpress).Json();
        }

        /// <summary>
        /// 新增国际快递
        /// </summary>
        /// <returns></returns>
        protected void AddWayBill()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];
                string Carrier = Request.Form["Carrier"];
                string WaybillCode = Request.Form["WaybillCode"];
                string ArrivalTime = Request.Form["ArrivalTime"];
                if (!string.IsNullOrEmpty(OrderID))
                {
                    Needs.Ccs.Services.Models.OrderWaybill waybill = new Needs.Ccs.Services.Models.OrderWaybill();
                    waybill.OrderID = OrderID;
                    waybill.ArrivalDate = Convert.ToDateTime(ArrivalTime);
                    waybill.Carrier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Carriers[Carrier];
                    waybill.WaybillCode = WaybillCode;
                    waybill.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    waybill.Enter();
                }
                //刷新WaybillCode下拉框
                var waybillView = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill;
                var waybills = waybillView.Where(item => item.OrderID == OrderID);
                var data = waybills.Select(item => new { value = item.ID, text = item.WaybillCode }).Json();
                Response.Write((new { success = true, message = "保存成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}