using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.UnClassify
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == ClientType.Internal).Select(c => new { c.ID, c.Company.Name }).Json();
            this.Model.OrderStatus = EnumUtils.ToDictionary<OrderStatus>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string orderStatus = Request.QueryString["OrderStatus"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];

            //var orders = new Needs.Ccs.Services.Views.MyUnClassifyOrdersView1();
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnClassifyOrdersView1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => item.ID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnClassifyOrder, bool>> lambda = item => item.ClientID == clientID.Trim();
                lambdas.Add(lambda);
            }

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Response.Write(new
            {
                rows = orderlist.Select(
                        order => new
                        {
                            order.ID,
                            ClientCode = order.Client.ClientCode,
                            ClientName = order.Client.Company.Name,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            Currency = order.Currency,
                            InvoiceStatus = order.InvoiceStatus.GetDescription(),
                            InvoiceType = order.Client.Agreement.InvoiceType.GetDescription(),
                            InvoiceTaxRate = order.Client.Agreement.InvoiceTaxRate,
                            OrderStatusValue = order.OrderStatus,
                            OrderStatus = order.OrderStatus.GetDescription(),
                            CreateDate = order.CreateDate.ToShortDateString()
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion 
        }
    }
}