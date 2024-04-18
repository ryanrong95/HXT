using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Bill
{
    /// <summary>
    /// 已审核对账单订单列表
    /// </summary>
    [Obsolete]
    public partial class AuditedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 已审核对账单列表
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            var orderBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills.Where(bill => bill.File != null && 
                                            bill.File.FileStatus == OrderFileStatus.Audited).AsQueryable();
            if (!string.IsNullOrEmpty(orderID))
            {
                orderBills = orderBills.Where(item => item.ID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                orderBills = orderBills.Where(item => item.Client.ClientCode.Contains(clientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                orderBills = orderBills.Where(item => item.CreateDate >= DateTime.Parse(startDate));
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                orderBills = orderBills.Where(item => item.CreateDate < DateTime.Parse(endDate).AddDays(1));
            }

            Func<Needs.Ccs.Services.Models.OrderBill, object> convert = bill => new
            {
                bill.ID,
                bill.Client.ClientCode,
                ClientName = bill.Client.Company.Name,
                DeclarePrice = bill.DeclarePrice.ToRound(2).ToString("0.00"),
                bill.Currency,
                bill.CustomsExchangeRate,
                bill.RealExchangeRate,
                CreateDate = bill.CreateDate.ToShortDateString()
            };

            this.Paging(orderBills, convert);
        }
    }
}