using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders.Declare
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
            this.Model.OrderPaymentStatus = ExtendsEnum.ToArray<OrderPaymentStatus>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            this.Model.CgOrderStatus = ExtendsEnum.ToArray<CgOrderStatus>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }

        protected object data()
        {
            Expression<Func<Order_Show, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var enterpriseid = Erp.Current.Leagues?.Current?.EnterpriseID;
            var query = Erp.Current.WsOrder.MyDeclareOrders(enterpriseid).GetPageList(page, rows, expression);
            
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
                    LoadingExcuteStatus = t.LoadingExcuteStatus == null ? "--" : t.LoadingExcuteStatus.GetDescription(),
                    MainStatus = t.MainStatus,
                    IsPayForGoods = t.IsPayCharge == true ? "是" : "否",
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    Type = t.EnterType.GetDescription(),
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
            var orderStatus = Request.QueryString["OrderStatus"];
            var orderPayStatus = Request.QueryString["OrderPayStatus"];
            var isPayForGoods = Request.QueryString["IsPayForGoods"];
            var isReceiveForGoods = Request.QueryString["IsReceiveForGoods"];


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
            if (!string.IsNullOrWhiteSpace(isReceiveForGoods))
            {
                if (isReceiveForGoods == "true")
                {
                    predicate = predicate.And(item => item.IsReciveCharge == true);
                }
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.CreateDate < end);
            }
            if (!string.IsNullOrWhiteSpace(orderStatus))
            {
                predicate = predicate.And(item => item.MainStatus == (CgOrderStatus)Enum.Parse(typeof(CgOrderStatus), orderStatus));
            }
            if (!string.IsNullOrWhiteSpace(orderPayStatus))
            {
                predicate = predicate.And(item => item.PaymentStatus == (OrderPaymentStatus)Enum.Parse(typeof(OrderPaymentStatus), orderPayStatus));
            }
            return predicate;
        }

        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"];
                var query = Erp.Current.WsOrder.Orders.Where(item => item.ID == ID).FirstOrDefault();
                if (query == null)
                {
                    throw new Exception("订单" + ID + "不存在");
                }
                query.OperatorID = Erp.Current.ID;
                query.Abandon();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }
    }
}