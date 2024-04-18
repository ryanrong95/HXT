using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.PvWsOrder.Services.Views.Alls;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Approval.Orders
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.OrderType = ExtendsEnum.ToArray<OrderType>(OrderType.Delivery).Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            this.Model.OrderPaymentStatus = ExtendsEnum.ToArray<OrderPaymentStatus>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Order_Show, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var query = new SuspendedOrderView().GetPageList(page, rows, expression);
            return new
            {
                rows = query.Select(t => new
                {
                    ID = t.ID,
                    CompanyName = t.ClientName,
                    EnterCode = t.EnterCode,
                    Supplier = t.SupplierName,
                    OrderType = t.Type.GetDescription(),
                    OrderStatus = t.MainStatus.GetDescription(),
                    OrderPayStatus = t.PaymentStatus.GetDescription(),
                    PayStatus = t.PaymentStatus,
                    MainStatus = t.MainStatus,
                    IsPayForGoods = t.IsPayCharge == true ? "是" : "否",
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    Type = t.EnterType == null ? "--" : t.EnterType.GetDescription(),
                }).ToArray(),
                total = query.Total,
            }.Json();
        }
        Expression<Func<Order_Show, bool>> Predicate()
        {
            Expression<Func<Order_Show, bool>> predicate = item => true;

            //查询参数
            var orderid = Request.QueryString["OrderID"];
            var companyName = Request.QueryString["CompanyName"];
            var clientCode = Request.QueryString["ClientCode"];
            var supplier = Request.QueryString["Supplier"];
            var OrderType = Request.QueryString["OrderType"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            //快速筛选参数
            var orderPayStatus = Request.QueryString["OrderPayStatus"];
            var isPayForGoods = Request.QueryString["IsPayForGoods"];

            if (!string.IsNullOrWhiteSpace(orderid))
            {
                orderid = orderid.Trim();
                predicate = predicate.And(item => item.ID.Contains(orderid));
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                companyName = companyName.Trim();
                predicate = predicate.And(item => item.ClientName.Contains(companyName));
            }
            if (!string.IsNullOrWhiteSpace(clientCode))
            {
                clientCode = clientCode.Trim();
                predicate = predicate.And(item => item.EnterCode.Contains(clientCode));
            }
            if (!string.IsNullOrWhiteSpace(supplier))
            {
                supplier = supplier.Trim();
                predicate = predicate.And(item => item.SupplierName.Contains(supplier));
            }
            if (!string.IsNullOrWhiteSpace(OrderType))
            {
                predicate = predicate.And(item => item.Type == (OrderType)Enum.Parse(typeof(OrderType), OrderType));
            }

            if (!string.IsNullOrWhiteSpace(isPayForGoods))
            {
                if (isPayForGoods == "true")
                {
                    predicate = predicate.And(item => item.IsPayCharge == true);
                }
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(StartDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.CreateDate < end);
            }
            if (!string.IsNullOrWhiteSpace(orderPayStatus))
            {
                predicate = predicate.And(item => item.PaymentStatus == (OrderPaymentStatus)Enum.Parse(typeof(OrderPaymentStatus), orderPayStatus));
            }
            return predicate;
        }
    }
}