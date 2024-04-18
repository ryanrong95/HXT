using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
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
    /// 用于展示需要将订单退回的管控产品详情
    /// 禁运、归类异常、抽检异常等 ...
    /// </summary>
    public partial class ReturnDisplay : Uc.PageBase
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
                ControlTypeValue = control.ControlType,
                ControlType = control.ControlType.GetDescription(),
                AnomalyReason = control.Summary
            }.Json();

            this.Model.IsClassified = control.Order.OrderStatus == OrderStatus.Classified;
            this.Model.IsItemAnomaly = control.ControlType == OrderControlType.ClassifyAnomaly || control.ControlType == OrderControlType.CheckingAbnomaly;
        }

        /// <summary>
        /// 初始化管控产品列表
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
            if (control.ControlType == OrderControlType.SortingAbnomaly)
            {
                var orderItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(item=> item.OrderID == control.Order.ID);

                Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
                {
                    item.ID,
                    Name = item.Category?.Name ?? item.Name,
                    item.Model,
                    item.Manufacturer,
                    item.Category?.HSCode,
                    item.Quantity,
                    UnitPrice = item.UnitPrice.ToString("0.0000"),
                    TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
                    item.Origin,
                    Declarant = item.Category?.ClassifySecondOperator.RealName,
                    AnomalyReason = item.Summary
                };

                Response.Write(new
                {
                    rows = orderItems.Select(convert).ToList(),
                    total = orderItems.Count()
                }.Json());
            }
            else
            {
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
                    Declarant = item.OrderItem.Category?.ClassifySecondOperator.RealName,
                    AnomalyReason = item.Summary
                };

                Response.Write(new
                {
                    rows = control.Items.Select(convert).ToList(),
                    total = control.Items.Count()
                }.Json());
            }
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