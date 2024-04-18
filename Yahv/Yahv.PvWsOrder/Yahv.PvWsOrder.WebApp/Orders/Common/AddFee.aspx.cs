using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class AddFee : ErpParticlePage
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
            this.Model.currencyData = ExtendsEnum.ToArray<Currency>().Where(t => t == Currency.USD || t == Currency.CNY || t == Currency.HKD).Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });

            string OrderID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.Orders.Where(item => item.ID == OrderID).FirstOrDefault();
            this.Model.orderData = new
            {
                payerName = query.OrderClient.Name,
                payeeName = "深圳市芯达通供应链管理有限公司",
            };
        }

        protected void Submit()
        {
            try
            {
                ////基本信息
                string ID = Request.Form["ID"].Trim();
                string Subject = Request.Form["Subject"].Trim();
                string Currency = Request.Form["Currency"].Trim();
                string Amount = Request.Form["Amount"].Trim();

                var order = Erp.Current.WsOrder.Orders.Where(item => item.ID == ID).FirstOrDefault();
                var currency = (Currency)Enum.Parse(typeof(Currency), Currency);
                var amount = decimal.Parse(Amount);

                //添加订单应收
                var receiveId = PaymentManager.Erp(Erp.Current.ID)[order.ClientID, order.PayeeID][ConductConsts.供应链].Receivable[CatalogConsts.仓储服务费, Subject].Record(
                       currency, amount, ID);

                Response.Write((new { success = true, message = "新增成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "新增失败：" + ex.Message }).Json());
            }
        }
    }
}