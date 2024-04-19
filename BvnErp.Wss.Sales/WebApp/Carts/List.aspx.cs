using Needs.Web;
using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Carts
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        protected string uid
        {
            get
            {
                return Request["uid"];
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            var views = new NtErp.Wss.Sales.Services.Views.CartsView().Where(t=>t.UserID == uid);
            Response.Paging(views.OrderByDescending(t => t.CreateDate), item => new
            {
                ID = item.ID,
                Name = item.Product.Name,
                item.CustomerCode,
                item.ServiceOutputID,
                item.Product,
                item.Quantity,
                item.Supplier,
                District = item.District.ToString(),
                Currency = item.Currency.GetTitle(),
                Price = item.Price,
                Total = item.Total,
                UserID = item.UserID,
                Manufacturer = item.Product.Manufacturer,
                Leadtime = item.Product.Leadtime,
                Summary = item.Product.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate
            });
        }
    }
}