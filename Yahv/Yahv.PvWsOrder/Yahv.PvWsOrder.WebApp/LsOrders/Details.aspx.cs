using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            var orderid = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.LsOrderAll.FirstOrDefault(item => item.ID == orderid);
            if (query != null)
            {
                this.Model.LsOrder = new
                {
                    ID = query.ID,
                    Currency = query.Currency.GetDescription(),
                    ClientName = query.wsClient.Name,
                };
            }
            //发票合同
            this.Model.ContractUrl = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + query.ContractFile?.Url;
            this.Model.InvoiceUrl = PvWsOrder.Services.Common.FileDirectory.ServiceRoot  +query.InvoiceFile?.Url;
        }

        protected object data()
        {
            var orderid = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.LsOrderItem.Where(item => item.OrderID == orderid).AsEnumerable();
            return new
            {
                rows = query.Select(item => new
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    ProductID = item.ProductID,
                    SpecID = item.Product?.SpecID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Quantity * item.UnitPrice * item.Lease.Month,
                    CreateDate = item.CreateDate.ToShortDateString(),
                    StartDate = item.Lease.StartDate.ToShortDateString(),
                    EndDate = item.Lease.EndDate.ToShortDateString(),
                    Month = item.Lease.Month,
                }).ToArray(),
                total = query.Count(),
            }.Json();
        }
    }
}