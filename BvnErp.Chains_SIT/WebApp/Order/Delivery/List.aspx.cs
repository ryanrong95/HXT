using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Delivery
{
    /// <summary>
    /// 报关完成，待发货订单查询界面
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
            this.Model.ClientData = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Select(item => new { item.ID, item.Company.Name }).Json();
        }

        /// <summary>
        /// 初始化订单数据
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
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];           

            var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Models.OrderPendingDelieveryViewModel>();

            if (!string.IsNullOrEmpty(orderID))
            {
                orderID = orderID.Trim();
                predicate = predicate.And(item => item.ID.Contains(orderID));
            }

            if (!string.IsNullOrEmpty(clientCode))
            {
                clientCode = clientCode.Trim();
                predicate = predicate.And(item => item.ClientCode.Contains(clientCode));
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                startDate = startDate.Trim();
                predicate = predicate.And(item => item.CreateDate>Convert.ToDateTime(startDate));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                endDate = endDate.Trim();
                predicate = predicate.And(item => item.CreateDate < Convert.ToDateTime(endDate));
            }

            if (!string.IsNullOrEmpty(type))
            {
                type = type.Trim();
                predicate = predicate.And(item => item.ClientType == (Needs.Ccs.Services.Enums.ClientType)Convert.ToInt16(type));
            }

            //predicate = predicate.And(item => item.HasExited == false);

            //var testorder = new Needs.Ccs.Services.Views.OrderPendingDeliveryView();
            var testorder = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyMainOrderPendingDeliveryView;
            testorder.AllowPaging = true;
            testorder.PageIndex = page;
            testorder.PageSize = rows;
            //view.IsOnReadShips = true;
            testorder.Predicate = predicate;
            

            var exitNotices = testorder.ToList();
            int recordCount = exitNotices.Count();

            Response.Write(new
            {
                rows = exitNotices.Select(
                        order => new
                        {
                            order.ID,
                            order.MainOrderID,
                            order.ClientCode,
                            order.ClientName,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            order.Currency,
                            //SZDeliveryType = order.OrderConsignor.Type.GetDescription(),
                            CreateDate = order.CreateDate.ToShortDateString(),
                            order.HasNotified
                        }
                     ).ToArray(),
                total = recordCount,
            }.Json());

            //var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyDeclaredOrders1;
            //List<LambdaExpression> lambdas = new List<LambdaExpression>();
            //Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> expression = item => true;

            //if (!string.IsNullOrEmpty(orderID))
            //{
            //    Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> lambda = item => item.ID == orderID.Trim();
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(clientCode))
            //{
            //    var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
            //    Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(startDate))
            //{
            //    var from = DateTime.Parse(startDate);
            //    Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> lambda = item => item.CreateDate >= from;
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(endDate))
            //{
            //    var to = DateTime.Parse(endDate);
            //    Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(type))
            //{
            //    int itype = Int32.Parse(type);
            //    var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
            //    Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(clientID))
            //{
            //    Expression<Func<Needs.Ccs.Services.Models.DeclaredOrder, bool>> lambda = item => item.ClientID == clientID.Trim();
            //    lambdas.Add(lambda);
            //}

            //#region 页面需要数据
            ////int page, rows;
            //int.TryParse(Request.QueryString["page"], out page);
            //int.TryParse(Request.QueryString["rows"], out rows);
            //var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            //Response.Write(new
            //{
            //    rows = orders.Select(
            //            order => new
            //            {
            //                order.ID,
            //                order.Client.ClientCode,
            //                ClientName = order.Client.Company.Name,
            //                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
            //                order.Currency,
            //                SZDeliveryType = order.OrderConsignor.Type.GetDescription(),
            //                CreateDate = order.CreateDate.ToShortDateString(),
            //                order.HasNotified
            //            }
            //         ).ToArray(),
            //    total = orderlist.Total,
            //}.Json());
            //#endregion
        }
    }
}