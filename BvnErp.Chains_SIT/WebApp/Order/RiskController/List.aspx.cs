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

namespace WebApp.Order.RiskController
{
    /// <summary>
    /// 订单综合查询界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboboxData();
            LoadData();
        }
        protected void LoadComboboxData()
        {
            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == ClientType.Internal).Select(c => new { c.ID, c.Company.Name }).Json();
            this.Model.AllClients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.Status == Status.Normal && c.ClientCode != null && c.ClientStatus == ClientStatus.Confirmed).Select(c => new { Value = c.ID, Text = c.ClientCode }).Json();
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        public void LoadData()
        {
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>().Select(item => new { item.Key, item.Value });
            this.Model.OrderStatus = orderStatus.Json();
            this.Model.Type = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "陈蓉" ? ClientType.Internal.GetHashCode() : ClientType.External.GetHashCode();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        //protected void data()
        //{
        //    string orderID = Request.QueryString["OrderID"];
        //    string clientCode = Request.QueryString["ClientCode"];
        //    string startDate = Request.QueryString["StartDate"];
        //    string endDate = Request.QueryString["EndDate"];
        //    string orderStatus = Request.QueryString["OrderStatus"];
        //    var clientID = Request.QueryString["ClientID"];
        //    var type = Request.QueryString["ClientType"];
        //    var advance = Request.QueryString["Advance"];

        //    var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.RiskOrders;
        //    List<LambdaExpression> lambdas = new List<LambdaExpression>();
        //    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;

        //    #region 查询条件
        //    if (!string.IsNullOrEmpty(orderID))
        //    {
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID.Contains(orderID.Trim());
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(clientCode))
        //    {
        //        var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientCode.Contains(clientCode.Trim())).Select(item => item.ID).ToArray();
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => clientIds.Contains(item.ClientID);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(startDate))
        //    {
        //        var from = DateTime.Parse(startDate);
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate >= from;
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(endDate))
        //    {
        //        var to = DateTime.Parse(endDate);
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate < to.AddDays(1);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(orderStatus))
        //    {
        //        int status = Int32.Parse(orderStatus);
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        int itype = Int32.Parse(type);
        //        var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => clientIds.Contains(item.ClientID);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(clientID))
        //    {
        //        Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ClientID == clientID.Trim();
        //        lambdas.Add(lambda);
        //    }
        //    #endregion

        //    #region 页面需要数据
        //    int page, rows;
        //    int.TryParse(Request.QueryString["page"], out page);
        //    int.TryParse(Request.QueryString["rows"], out rows);
        //    var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

        //    Response.Write(new
        //    {
        //        rows = orderlist.Select(
        //                order => new
        //                {
        //                    order.MainOrderID,
        //                    order.ID,
        //                    order.Client.ClientCode,
        //                    ClientName = order.Client.Company.Name,
        //                    DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
        //                    order.Currency,
        //                    CreateDate = order.CreateDate.ToShortDateString(),
        //                    InvoiceStatus = order.InvoiceStatus.GetDescription(),
        //                    PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
        //                    UnpaidAmount = (order.DeclarePrice - order.PaidExchangeAmount).ToString("0.00"),
        //                    PayExchangeStatus = order.PayExchangeStatus.GetDescription(),
        //                    OrderStatusValue = order.OrderStatus,
        //                    OrderStatus = order.OrderStatus.GetDescription(),
        //                    SpecialType = GenerateOrderSpecialType(order.OrderVoyages)
        //                }
        //             ).ToArray(),
        //        total = orderlist.Total,
        //    }.Json());
        //    #endregion
        //}

        /// <summary>
        /// 订单特殊类型
        /// </summary>
        /// <param name="orderVoyages"></param>
        /// <returns></returns>
        private string GenerateOrderSpecialType(List<Needs.Ccs.Services.Models.OrderVoyage> orderVoyages)
        {
            var result = string.Empty;
            orderVoyages.ForEach(t =>
            {
                result += t.Type.GetDescription() + "|";
            });

            result = result.TrimEnd('|');

            return string.IsNullOrEmpty(result) ? "-" : result;
        }

        protected void sumDeclarePrice()
        {
            string orderID = Request.Form["OrderID"];
            string clientCode = Request.Form["ClientCode"];
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];
            string orderStatus = Request.Form["OrderStatus"];
            var clientID = Request.Form["ClientID"];
            var type = Request.Form["ClientType"];

            var orders = new Needs.Ccs.Services.Views.Orders2View().Where(t => 1 == 1);

            #region 查询条件
            if (!string.IsNullOrEmpty(orderID))
            {
                orders = orders.Where(t => t.ID == orderID);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientCode.Contains(clientCode.Trim())).Select(item => item.ID).ToArray();
                orders = orders.Where(t => clientIds.Contains(t.ClientID));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                orders = orders.Where(t => t.CreateDate > from);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                orders = orders.Where(t => t.CreateDate < to.AddDays(1));
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                orders = orders.Where(t => t.OrderStatus == (OrderStatus)status);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                orders = orders.Where(t => clientIds.Contains(t.ClientID));
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                orders = orders.Where(t => t.ClientID == clientCode);
            }
            #endregion



            string showMsg = "";
            var currencies = orders.Select(t => t.Currency).Distinct().ToList();
            foreach (var currency in currencies)
            {
                decimal totalAmount = orders.Where(t => t.Currency == currency).Sum(t => t.DeclarePrice);
                showMsg += currency + "金额:" + totalAmount + "|  ";
            }
            if (orders.Count() == 0)
            {
                showMsg = "";
            }

            Response.Write((new { success = true, message = showMsg }).Json());
        }



        /// <summary>
        /// 新的
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            string orderStatus = Request.QueryString["OrderStatus"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];
            var advance = Request.QueryString["Advance"];

            using (var query = new Needs.Ccs.Services.Views.RiskOrderViewRJ())
            {
                var view = query;


                if (!string.IsNullOrEmpty(orderID))
                {
                    view = view.SearchByOrderID(orderID);
                }

                if (!string.IsNullOrEmpty(clientCode))
                {
                    view = view.SearchByOrderID(clientCode);
                }

                if (!string.IsNullOrEmpty(startDate))
                {
                    var from = DateTime.Parse(startDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    var to = DateTime.Parse(endDate);
                    view = view.SearchByTo(to);
                }

                if (!string.IsNullOrEmpty(orderStatus))
                {
                    int status = Int32.Parse(orderStatus);
                    view = view.SearchByOrderStatus(status);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    int itype = Int32.Parse(type);
                    view = view.SearchByClientType(itype);
                }

                if (advance == "1")
                {
                    view = view.SearchByAdvance(true);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}