using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders
{
    public partial class Edit : ErpParticlePage
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
            var orderid = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.LsOrderAll.FirstOrDefault(item => item.ID == orderid);
            if (query != null)
            {
                this.Model.LsOrder = new
                {
                    OrderID = query.ID,
                    ClientName = query.wsClient.Name,
                    Currency = query.Currency.GetDescription(),
                };
            }
        }

        protected object data()
        {
            var orderid = Request.QueryString["ID"];
            var query = new LsOrderItemRoll(orderid).AsEnumerable();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                Name = t.Product.Name,
                SpecID = t.Product.SpecID,
                StartDate = t.Lease.StartDate.ToShortDateString(),
                EndDate = t.Lease.EndDate.ToShortDateString(),
                Month = t.Lease.Month,
                Quantity = t.Quantity,
                UnitPrice = t.UnitPrice,
                TotalPrice = t.UnitPrice * t.Quantity * t.Lease.Month,
            });
            return new
            {
                rows = linq.ToArray(),
                total = linq.Count(),
            }.Json();
        }

        /// <summary>
        /// 保存单价
        /// </summary>
        protected void Submit()
        {
            try
            {
                var ID = Request.Form["ID"];
                var IDPrices = Request.Form["IDPrices"].Replace("&quot;", "'").Replace("amp;", "");
                var itemList = IDPrices.JsonTo<List<LsOrderItem>>();

                var query = Erp.Current.WsOrder.LsOrderAll.Where(item => item.ID == ID).FirstOrDefault();
                query.OrderItems = itemList;
                query.OperatorID = Erp.Current.ID;

                query.UpdatePrice();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}