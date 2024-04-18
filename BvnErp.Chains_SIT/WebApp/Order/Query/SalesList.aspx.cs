using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Query
{
    /// <summary>
    /// 订单综合查询界面
    /// </summary>
    public partial class SalesList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        public void LoadData()
        {
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>().Select(item => new { item.Key, item.Value });
            this.Model.OrderStatus = orderStatus.Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            string orderStatus = Request.QueryString["OrderStatus"];

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders.OrderByDescending(item => item.CreateDate).AsQueryable();
            if (!string.IsNullOrEmpty(orderID))
            {
                orders = orders.Where(t => t.ID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                orders = orders.Where(t => t.Client.ClientCode.Contains(clientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                orders = orders.Where(t => t.CreateDate >= from);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                orders = orders.Where(t => t.CreateDate < to.AddDays(1));
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                orders = orders.Where(t => t.OrderStatus == (Needs.Ccs.Services.Enums.OrderStatus)status);
            }

            Func<Needs.Ccs.Services.Models.Order, object> convert = order => new
            {
                order.MainOrderID,
                order.ID,
                order.Client.ClientCode,
                ClientName = order.Client.Company.Name,
                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                order.Currency,
                CreateDate = order.CreateDate.ToShortDateString(),
                InvoiceStatus = order.InvoiceStatus.GetDescription(),
                PayExchangeStatus = order.PayExchangeStatus.GetDescription(),
                OrderStatusValue = order.OrderStatus,
                OrderStatus = order.OrderStatus.GetDescription(),
            };
            this.Paging(orders, convert);
        }
    }
}