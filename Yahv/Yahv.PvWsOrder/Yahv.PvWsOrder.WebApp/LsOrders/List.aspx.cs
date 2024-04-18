using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            //租赁订单状态
            this.Model.StatusData = ExtendsEnum.ToArray<LsOrderStatus>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
            //开票状态
            this.Model.InvoiceData = ExtendsEnum.ToArray<OrderInvoiceStatus>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<LsOrder, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            var enterpriseid = Erp.Current.Leagues?.Current?.EnterpriseID;
            var query = Erp.Current.WsOrder.LsOrder(enterpriseid).GetPageList(page, rows, expression);
            return new
            {
                rows = query.Select(t => new
                {
                    ID = t.ID,
                    CompanyName = t.wsClient.Name,
                    EnterCode = t.wsClient.EnterCode,
                    Status = t.Status.GetDescription(),
                    InvoiceStatus = t.InvoiceStatus.GetDescription(),
                    Creator = t.Admin?.RealName,
                    CreateDate = t.CreateDate.ToShortDateString(),
                }).ToArray(),
                total = query.Total,
            }.Json();
        }

        private Expression<Func<LsOrder, bool>> Predicate()
        {
            //查询参数
            var OrderID = Request.QueryString["OrderID"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];
            var Status = Request.QueryString["Status"];
            var InvoiceStatus = Request.QueryString["InvoiceStatus"];

            Expression<Func<LsOrder, bool>> predicate = item => true;
            if (!string.IsNullOrWhiteSpace(OrderID))
            {
                OrderID = OrderID.Trim();
                predicate = predicate.And(item => item.ID.Contains(OrderID));
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
            if (!string.IsNullOrWhiteSpace(Status))
            {
                LsOrderStatus status = (LsOrderStatus)int.Parse(Status);
                predicate = predicate.And(item => item.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(InvoiceStatus))
            {
                OrderInvoiceStatus status = (OrderInvoiceStatus)int.Parse(InvoiceStatus);
                predicate = predicate.And(item => item.InvoiceStatus == status);
            }
            return predicate;
        }
    }
}