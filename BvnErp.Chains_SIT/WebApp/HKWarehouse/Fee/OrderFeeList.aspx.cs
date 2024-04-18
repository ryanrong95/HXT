using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Fee
{
    public partial class OrderFeeList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
        }

        /// <summary>
        /// 费用明细
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];

            var Fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.AsQueryable();
            Fees = Fees.Where(t => t.OrderID == OrderID).OrderByDescending(x => x.CreateDate);

            Func<OrderWhesPremium, object> convert = fee => new
            {
                ID = fee.ID,
                Type = fee.WarehousePremiumType.GetDescription(),
                Currency = fee.Currency,
                Count = fee.Count,
                UnitPrice = fee.UnitPrice,
                ApprovalPrice = fee.ApprovalPrice,
                Creater = fee.Creater.RealName,
                Approver = fee.Approver?.RealName,
                PremiumsStatus = fee.WarehousePremiumsStatus.GetDescription(),
                Summary = fee.Summary,
                CreateDate = fee.CreateDate.ToString(),
                WhsePaymentType=fee.WhsePaymentType
            };

            Response.Write(new
            {
                rows = Fees.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// 删除费用
        /// </summary>
        protected void DeleteFee()
        {
            try
            {
                string ID = Request.Form["ID"];
                var Fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.AsQueryable();
                var fee = Fees.Where(t => t.ID == ID).FirstOrDefault();
                //if(fee.WarehousePremiumsStatus==Needs.Ccs.Services.Enums.WarehousePremiumsStatus.Audited)
                //{
                //    Response.Write((new { success = false, message = "删除失败：费用已审批或已付款" }).Json());
                //    return;
                //}
                fee.SetOperator(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
                fee.Abandon();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }
    }
}