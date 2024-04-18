using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Entry
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
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string OrderID = Request.QueryString["OrderID"];
            string OrderStatus = Request.QueryString["OrderStatus"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(ClientCode))
            {
                ClientCode = ClientCode.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.HKOrderListView.HKOrderModel, bool>>)(t => t.ClientCode.Contains(ClientCode)));
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.HKOrderListView.HKOrderModel, bool>>)(t => t.ClientName.Contains(ClientName)));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.HKOrderListView.HKOrderModel, bool>>)(t => t.OrderID.Contains(OrderID)));
            }
            if (!string.IsNullOrEmpty(OrderStatus))
            {
                OrderStatus = OrderStatus.Trim();

                int IntOrderStatus;
                if (int.TryParse(OrderStatus, out IntOrderStatus))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.HKOrderListView.HKOrderModel, bool>>)(t => t.OrderStatus == (Needs.Ccs.Services.Enums.OrderStatus)IntOrderStatus));
                }
            }

            int total = 0;
            var listOrder = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKOrderListView.GetHKOrderModelResult(out total, page, rows, lamdas.ToArray()).ToList();

            Func<Needs.Ccs.Services.Views.HKOrderListView.HKOrderModel, object> convert = item => new
            {
                OrderID = item.OrderID,
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
                OrderStatus = item.OrderStatus.GetDescription(),
                OrderConsigneeType = item.OrderConsigneeType.GetDescription(),
                ClientSupplierName = item.ClientSupplierName,
            };

            Response.Write(new
            {
                rows = listOrder.Select(convert).ToArray(),
                total = total,
            }.Json());
        }
    }
}