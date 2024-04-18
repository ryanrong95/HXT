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

namespace WebApp.HKWarehouse.Report
{
    public partial class OrderList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> dicOrderStatus = new Dictionary<string, string>();
            var OrderStatusEnum = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>();
            dicOrderStatus.Add(string.Empty, "全部");
            foreach (var item in OrderStatusEnum)
            {
                dicOrderStatus.Add(item.Key, item.Value);
            }
            this.Model.OrderStatus = dicOrderStatus.Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
        }

        protected void OrderData()
        {
            string clientCode = Request.QueryString["ClientCode"];
            string clientName = Request.QueryString["ClientName"];
            string orderID = Request.QueryString["OrderID"];
            string orderStatus = Request.QueryString["OrderStatus"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKOrdersAll;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> expression = item => true;

            #region 查询条件
            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> lambda = item => item.ID.Contains(orderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> lambda = item => item.ClientCode.Contains(clientCode.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientName))
            {
                Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> lambda = item => item.ClientName.Contains(clientName);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int intOrderStatus;
                if (int.TryParse(orderStatus, out intOrderStatus))
                {
                    Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> lambda = item => (int)item.OrderStatus == intOrderStatus;
                    lambdas.Add(lambda);
                }
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Views.Alls.HKOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
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
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.ID,
                            ClientCode = item.ClientCode,
                            ClientName = item.ClientName,
                            DeclarePrice = item.DeclarePrice.ToRound(2).ToString("0.00"),
                            Currency = item.Currency,
                            OrderConsigneeType = item.HKDeliveryType.GetDescription(),
                            OrderStatus = item.OrderStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString()
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }
    }
}