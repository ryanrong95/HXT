using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Models.Adopt;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Stocks.Adopt
{
    public partial class Handle : ErpParticlePage
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
            this.Model.Client = Erp.Current.WsOrder.MyWsClients.Where(item => true).ToArray().Select(item => new
            {
                Value = item.EnterCode,
                Text = item.Name
            });  
        }

        protected void Submit()
        {
            try
            {
                string ID = Request.Form["ID"].Trim();
                string TempType = Request.Form["TempType"].Trim();
                string OrderID = Request.Form["OrderID"].Trim();
                string OrderFee = Request.Form["OrderFee"].Trim();
                string Summary = Request.Form["Summary"].Trim();

                IOrderCheck orderCheck = new ScCustomsCheck();
                switch (TempType)
                {
                    case "2":
                    case "3":
                        orderCheck = new WsOrderCheck();
                        break;
                }

                OrderCheckContext orderCheckContext = new OrderCheckContext(orderCheck);
                if (orderCheckContext.Check(OrderID))
                {
                    AdoptTmepStock adoptTmepStock = new AdoptTmepStock();
                    adoptTmepStock.ID = ID;
                    adoptTmepStock.ForOrderID = OrderID;
                    adoptTmepStock.Summary = Summary;
                    adoptTmepStock.UpdateForOrderID(Erp.Current.ID);

                    decimal orderFee = string.IsNullOrEmpty(OrderFee) ? 0 : Convert.ToDecimal(OrderFee);
                    orderCheckContext.UpdateFee(OrderID, orderFee, Erp.Current.ID);

                    Response.Write((new { success = true, message = "保存成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "订单号不存在" }).Json());
                }               
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}