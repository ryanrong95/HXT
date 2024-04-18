using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Store
{
    public partial class ReceivedStoreList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientName = Request.QueryString["ClientName"];

            int totalCount = 0;
            var receiveds = new Needs.Ccs.Services.Views.StoreReceiptReceivedClientView().GetResult(out totalCount, page, rows, ClientName);

            Func<Needs.Ccs.Services.Views.StoreReceiptReceivedClientViewModel, object> convert = item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ReceiptAdminName = item.RealName,
                OrderID = item.OrderID,
                TypeName = !string.IsNullOrEmpty(item.Subject) ? item.Subject : (!string.IsNullOrEmpty(item.Catalog) ? item.Catalog : string.Empty),
                Price = item.Price,
                AccountTypeInt = item.AccountType,
            };

            Response.Write(new
            {
                rows = receiveds.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

    }
}