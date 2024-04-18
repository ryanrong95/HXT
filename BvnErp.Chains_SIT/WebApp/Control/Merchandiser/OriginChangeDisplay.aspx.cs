using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Merchandiser
{
    /// <summary>
    /// 用于展示原产地变更导致的管控产品详情
    /// </summary>
    public partial class OriginChangeDisplay : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化管控数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];

            this.Model.ControlData = new
            {
                control.ID,
                OrderID = control.Order.ID,
                ClientName = control.Order.Client.Company.Name,
                ClientRank = control.Order.Client.ClientRank,
                DeclarePrice = control.Order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = control.Order.Currency,
                Merchandiser = control.Order.Client.Merchandiser.RealName,
                ControlType = control.ControlType.GetDescription()
            }.Json();

            this.Model.IsClassifyAnomaly = control.ControlType == OrderControlType.ClassifyAnomaly;
        }

        /// <summary>
        /// 初始化管控产品列表
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];

            Func<Needs.Ccs.Services.Models.OrderControlItem, object> convert = item => new
            {
                item.ID,
                Name = item.OrderItem.Category?.Name ?? item.OrderItem.Name,
                item.OrderItem.Model,
                item.OrderItem.Manufacturer,
                item.OrderItem.Category?.HSCode,
                item.OrderItem.Quantity,
                UnitPrice = item.OrderItem.UnitPrice.ToString("0.0000"),
                TotalPrice = item.OrderItem.TotalPrice.ToRound(2).ToString("0.00"),
                item.OrderItem.Origin,
                TariffRate = item.OrderItem.ImportTax?.Rate.ToString("0.0000"),
                AddTaxRate = item.OrderItem.AddedValueTax?.Rate.ToString("0.0000")
            };

            Response.Write(new
            {
                rows = control.Items.Select(convert).ToList(),
                total = control.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 重新生成对账单
        /// </summary>
        protected void GenerateBill()
        {
            try
            {
                string id = Request.Form["ID"];
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[id];
                var bill = order.Files.Where(file => file.FileType == FileType.OrderBill && file.Status == Status.Normal).SingleOrDefault();

                bill?.Abandon();
                order.BillGenerated += Order_BillGenerated;
                order.GenerateBill(order.OrderBillType, order.PointedAgencyFee);
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "对账单生成失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批通过，取消订单挂起，允许客户订单报关
        /// </summary>
        protected void CancelHangUp()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                control.SetAdmin(admin);
                control.HangUpCanceled += Control_CancelHangUpSuccess;
                control.CancelHangUp();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 对账单生成成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_BillGenerated(object sender, GenerateOrderBillEventArgs e)
        {
            Response.Write((new { success = true, message = "对账单生成成功" }).Json());
        }

        /// <summary>
        /// 订单取消挂起成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_CancelHangUpSuccess(object sender, OrderControledEventArgs e)
        {
            Response.Write((new { success = true, message = "订单取消挂起成功！" }).Json());
        }

        /// <summary>
        /// 订单是否退回
        /// </summary>
        /// <returns></returns>
        protected bool IsOrderReturned()
        {
            string orderID = Request.Form["OrderID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];

            return order.OrderStatus == OrderStatus.Returned;
        }
    }
}