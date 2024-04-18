using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Query
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
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        public void LoadData()
        {
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>().Select(item => new { item.Key, item.Value });
            this.Model.OrderStatus = orderStatus.Json();
            this.Model.Type = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "钟玉芳" ? ClientType.Internal.GetHashCode() : ClientType.External.GetHashCode();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string enterCode = Request.QueryString["EnterCode"];
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            string orderStatus = Request.QueryString["OrderStatus"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;

            #region 查询条件
            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID.Contains(orderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode.Contains(clientCode.Trim())).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(enterCode))
            {
                string EnterCode = enterCode.Trim();
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.EnterCode == EnterCode;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ClientID == clientID.Trim();
                lambdas.Add(lambda);
            }
            #endregion

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
                            order.MainOrderID,
                            order.ID,
                            order.Client.ClientCode,
                            ClientName = order.Client.Company.Name,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            order.Currency,
                            CreateDate = order.CreateDate.ToShortDateString(),
                            InvoiceStatus = order.InvoiceStatus.GetDescription(),
                            PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
                            UnpaidAmount = (order.DeclarePrice - order.PaidExchangeAmount).ToString("0.00"),
                            PayExchangeStatus = order.PayExchangeStatus.GetDescription(),
                            OrderStatusValue = order.OrderStatus,
                            OrderStatus = order.OrderStatus.GetDescription(),
                            SpecialType = GenerateOrderSpecialType(order.OrderVoyages),
                            order.EnterCode
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }

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

        protected void UpdateEnterCode()
        {
            string orderID = Request.Form["OrderID"];
            string enterCode = Request.Form["EnterCode"];
            try
            {
                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;
                List<System.Linq.Expressions.LambdaExpression> lambdas = new List<System.Linq.Expressions.LambdaExpression>();
                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID == orderID;
                lambdas.Add(lambda);
                var order = new Orders1ViewBase<Needs.Ccs.Services.Models.Order>().GetAlls(expression, lambdas.ToArray()).FirstOrDefault();
                order.UpdateEnterCode(enterCode);
                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败" }).Json());
            }
        }
    }
}
