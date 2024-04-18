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

namespace WebApp.Order.UnPayExchange
{
    /// <summary>
    /// 待付汇订单查询界面
    /// </summary>
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
            this.Model.Currencies = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Key = item.Code, Value = item.Code + " " + item.Name }).Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string currency = Request.QueryString["Currency"];
            string orderStatus = Request.QueryString["OrderStatus"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];


            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnPayExchangeOrders1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => item.ID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(currency))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => item.Currency.Contains(currency.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnPayExchangeOrder, bool>> lambda = item => item.ClientID == clientID.Trim();
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
                            SupplierName = order.PayExchangeSuppliers.Select(ps => ps.ClientSupplier.ChineseName),
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            Currency = order.Currency,
                            PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
                            UnpaidAmount = (order.DeclarePrice - order.PaidExchangeAmount).ToString("0.00"),
                            PaymentStatus = order.PayExchangeStatus.GetDescription(),
                            IsPrePayExchange = order.ClientAgreement.IsPrePayExchange,
                            PaymentType = order.ClientAgreement.IsPrePayExchange ? "预换汇" : "90天内换汇",
                            OrderStatusValue = order.OrderStatus,
                            OrderStatus = order.OrderStatus.GetDescription(),
                            CreateDate = order.CreateDate.ToShortDateString(),
                            DeclareDate = order.DeclareDate?.ToShortDateString()
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 多个订单一起付汇时，判断订单的付汇供应商是否有交集
        /// </summary>
        /// <returns></returns>
        protected bool HasIntersection()
        {
            var suppliers = Request.Form["Suppliers"].Replace("&quot;", "'");
            var supplierList = suppliers.JsonTo<List<List<string>>>();
            var supplier = supplierList[0];
            for (int i = 1; i < supplierList.Count; i++)
            {
                supplier = supplier.Intersect(supplierList[i]).ToList();
                if (supplier.Count() == 0)
                    return false;
            }

            return true;
        }
    }
}