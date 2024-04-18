using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class AddFeeNew : ErpParticlePage
    {
        private string json = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\jsons", "Sorage.json");

        private T JsonTo<T>(string json)
        {
            T data;
            using (System.IO.StreamReader file = System.IO.File.OpenText(json))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JArray jArray = (JArray)JToken.ReadFrom(reader);
                    data = jArray.ToObject<T>();
                }
            }
            return data;
        }

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
            var listFee = JsonTo<List<FeeContent>>(json);
            this.Model.feeSubjectData = listFee.Select(item => new
            {
                value = item.value,
                label = item.label
            });

            string OrderID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.Orders.Where(item => item.ID == OrderID).FirstOrDefault();
            this.Model.orderData = new
            {
                payerName = query.OrderClient.Name,
                payeeName = "深圳市芯达通供应链管理有限公司",
            };
        }

        protected void SelectSubject()
        {
            try
            {
                string Subject = Request.Form["Subject"];
                var listFee = JsonTo<List<FeeContent>>(json);
                var data = listFee.Where(item => item.value == Subject).First();

                Response.Write((new { success = true, data = data }).Json());
            }
            catch (Exception e)
            {
                Response.Write((new { success = false, data = e.Message }).Json());
            }
        }

        protected void SelectSubject2()
        {
            try
            {
                string Subject = Request.Form["Subject"];
                string Subject2 = Request.Form["Subject2"];
                var listFee = JsonTo<List<FeeContent>>(json);
                var data = listFee.Where(item => item.value == Subject).First().children
                    .Where(item => item.value == Subject2).First();
                Response.Write((new { success = true, data = data }).Json());
            }
            catch (Exception e)
            {
                Response.Write((new { success = false, data = e.Message }).Json());
            }
        }

        protected void SelectSubject3()
        {
            try
            {
                string Subject = Request.Form["Subject"];
                string Subject2 = Request.Form["Subject2"];
                string Subject3 = Request.Form["Subject3"];
                var listFee = JsonTo<List<FeeContent>>(json);
                var data = listFee.Where(item => item.value == Subject).First().children
                    .Where(item => item.value == Subject2).First().children
                    .Where(item => item.value == Subject3).First();

                Response.Write((new { success = true, data = data }).Json());
            }
            catch (Exception e)
            {
                Response.Write((new { success = false, data = e.Message }).Json());
            }
        }

        protected void Submit()
        {
            try
            {
                //基本信息
                string ID = Request.Form["ID"].Trim();
                string Subject = Request.Form["Subject"].Trim();
                string Currency = Request.Form["Currency"].Trim();
                string Price = Request.Form["Price"].Trim();//建议价格
                string Amount = Request.Form["Amount"].Trim();//实际应收

                var order = Erp.Current.WsOrder.Orders.Where(item => item.ID == ID).FirstOrDefault();
                var currency = (Currency)Enum.Parse(typeof(Currency), Currency);
                var amount = decimal.Parse(Amount);
                var price = string.IsNullOrEmpty(Price) ? decimal.Parse(Amount) : decimal.Parse(Price);

                //添加订单应收
                var receiveId = PaymentManager.Erp(Erp.Current.ID)[order.ClientID, order.PayeeID][ConductConsts.供应链].Receivable[CatalogConsts.仓储服务费, Subject].Record(
                       currency, amount, ID, originPrice: price);

                Response.Write((new { success = true, message = "新增成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "新增失败：" + ex.Message }).Json());
            }
        }
    }

    public class FeeContent
    {
        public string value { get; set; }
        public string label { get; set; }
        public Currency? currency { get; set; }
        public bool Isquantity { get; set; }
        public int? prices { get; set; }

        public List<FeeContent> children { get; set; }
    }
}