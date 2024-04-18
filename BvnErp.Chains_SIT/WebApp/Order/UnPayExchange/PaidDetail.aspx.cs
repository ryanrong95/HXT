using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.UnPayExchange
{
    public partial class PaidDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var paidRecords = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.OrderPayExchangeItems.Where(item => item.OrderID == ID).OrderByDescending(item => item.ApplyTime);

            Func<Needs.Ccs.Services.Models.OrderPayExchangeRecord, object> convert = item => new
            {
                SupplierName = item.SupplierName,
                ApplyTime = item.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Applier = item.User == null ? "跟单员" : item.User.Name,
                Amount = Convert.ToString(item.Amount.ToRound(2)),
                Status = item.Status.GetDescription(),
                FatherID = item.FatherID != null ? "Ⅱ" : "Ⅰ",
            };

            this.Paging(paidRecords, convert);
        }
    }
}