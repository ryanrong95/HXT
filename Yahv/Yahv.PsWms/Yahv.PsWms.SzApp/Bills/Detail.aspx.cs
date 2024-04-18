using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Bills
{
    public partial class Detail : ErpParticlePage
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
            string ID = Request.QueryString["ID"];
            var invoice = new SzMvc.Services.Views.Origins.InvoiceNoticesOrigin().SingleOrDefault(t => t.ID == ID);
            this.Model.InvoiceData = new
            {
                InvoiceType = invoice.Type.GetDescription(),
                DeliveryType = invoice.DeliveryType.GetDescription(),
                Title = invoice.Title ?? "",
                TaxNumber = invoice.TaxNumber ?? "",
                RegAddress = invoice.RegAddress ?? "",
                BankName = invoice.BankName ?? "",
                BankAccount = invoice.BankAccount ?? "",
                Tel = invoice.Tel ?? "",

                PostRecipient = invoice.PostRecipient ?? "",
                PostTel = invoice.PostTel ?? "",
                PostAddress = invoice.PostAddress ?? "",
                Carrier = invoice.CarrierName ?? "",
                WayBillCode = invoice.WayBillCode ?? "",

                Summary = invoice.Summary ?? "",
            };
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var query = new SzMvc.Services.Views.Origins.InvoiceNoticeItemsOrigin().Where(t => t.InvoiceNoticeID == ID);

            return this.Paging(query, t => new
            {
                t.ID,
                ProductName = "*物流辅助服务*服务费",
                ProductModel = "",
                Quantity = 1,
                UnitPrice = (t.UnitPrice / 1.06m).ToString("f2"),
                Amount = (t.Amount / 1.06m).ToString("f2"),
                TaxRate = 0.06,
                TaxUnitPrice = t.UnitPrice.ToString("f2"),
                TaxAmount = t.Amount.ToString("f2"),
                Difference = t.Difference?.ToString("f2"),
                t.InvoiceNo,
                //税务名称
                TaxName = "*物流辅助服务*服务费",
                //税务编码
                TaxCode = "3040407040000000000"

            });
        }
    }
}