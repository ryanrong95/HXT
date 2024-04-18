using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Bill
{
    /// <summary>
    /// 待上传对账单订单列表
    /// </summary>
    public partial class UnUploadedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 待上传对账单列表
        /// </summary>
        protected void data1()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            var orderBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills.Where(item => item.OrderStatus > OrderStatus.Quoted && item.OrderStatus < OrderStatus.Completed &&
                                                                                      item.File == null).AsQueryable();
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

            orderBills = orderBills.OrderByDescending(item => item.CreateDate);

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


        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            //var orderBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnHandleMainOrderBill.Where(item => item.MainOrderFile == null).AsQueryable();
            var orderBills = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnHandleMainOrderBill.Where(item=>item.MainOrderFileStatus== OrderFileStatus.NotUpload).AsQueryable();
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

            orderBills = orderBills.OrderByDescending(item => item.CreateDate);

            Func<Needs.Ccs.Services.Models.OrderBill, object> convert = bill => new
            {
                bill.ID,
                bill.Client.ClientCode,
                ClientName = bill.Client.Company.Name,               
                CreateDate = bill.CreateDate.ToShortDateString()
            };

            this.Paging(orderBills, convert);
        }
    }
}