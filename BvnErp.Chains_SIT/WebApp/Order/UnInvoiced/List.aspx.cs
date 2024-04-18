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

namespace WebApp.Order.UnInvoiced
{
    /// <summary>
    /// 待申请开票订单查询界面
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

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnInvoicedOrders1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => item.ID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnInvoicedOrder, bool>> lambda = item => item.ClientID == clientID.Trim();
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
                            ClientAgreementID = order.ClientAgreementID,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            Currency = order.Currency,
                            InvoiceStatus = order.InvoiceStatus.GetDescription(),
                            InvoiceType = order.ClientAgreement.InvoiceType.GetDescription(),
                            InvoiceTaxRate = order.ClientAgreement.InvoiceTaxRate,
                            OrderStatusValue = order.OrderStatus,
                            OrderStatus = order.OrderStatus.GetDescription(),
                            CreateDate = order.CreateDate.ToShortDateString()
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion 
        }

        /// <summary>
        /// 验证是否所有订单都已经报关
        /// </summary>
        protected void CheckOrderDeclare()
        {
            var IDs = Request.Form["IDs"].Split(',');
            var ClientCode = Request.Form["ClientCode"];
            var decFlag = true;
            var decmsg = "订单未报关：";
            var peFlag = true;
            var payExchangeMsg = "";

            try
            {
                var decheads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.IsSuccess);

                foreach (var id in IDs)
                {
                    if (!decheads.Any(t => t.OrderID == id))
                    {
                        decFlag = false;
                        decmsg += id + " ";
                    }
                }

                //验证客户设置(超期未付汇被设置为不允许开票)：
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.FirstOrDefault(t=>t.ClientCode == ClientCode);
                if (client != null && !client.IsApplyInvoice.Value)
                {
                    if (!string.IsNullOrEmpty(client.InvoiceExtendDate))
                    {
                        //有宽限日期
                        var extend = DateTime.Parse(client.InvoiceExtendDate);
                        peFlag = DateTime.Compare(extend, DateTime.Now) > 0;
                    }
                    else
                    {
                        //没有宽限日期
                        peFlag = false;
                    }
                }

                payExchangeMsg = "客户超期未付汇金额：" + client.UnPayExchangeAmount + ",近期报关金额：" + client.DeclareAmount + ",近期付汇金额：" + client.PayExchangeAmount; 

                if (!decFlag)
                {
                    Response.Write((new { success = decFlag, message = decmsg }).Json());
                }

                if (!peFlag)
                {
                    Response.Write((new { success = peFlag, message = "此客户因超期未付汇被设置为不允许开票！请联系风控人员" }).Json());
                }

                if (decFlag && peFlag)
                {
                    Response.Write((new { success = true, message = payExchangeMsg }).Json());
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "订单验证错误：" + ex.Message }).Json());
            }
        }
    }
}