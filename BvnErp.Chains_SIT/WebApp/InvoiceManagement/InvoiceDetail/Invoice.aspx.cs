using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InvoiceManagement.InvoiceDetail
{
    public partial class Invoice : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
                      
        }

        private void LoadData()
        {
            string ID = Request.QueryString["ID"];
            this.Model.Invoice = "";
            var c = new Needs.Ccs.Services.Views.InvoiceResultView().Where(t => t.ID == ID).FirstOrDefault();
            if (c != null)
            {
                this.Model.Invoice = new
                {
                    ID = c.ID,
                    invoiceTypeName = c.invoiceTypeName,
                    invoiceTypeCode = c.invoiceTypeCode,
                    checkDate = c.checkDate,
                    checkNum = c.checkNum,
                    invoiceDataCode = c.invoiceDataCode,
                    invoiceNumber = c.invoiceNumber,
                    billingTime = c.billingTime.Value.ToString("yyyy-MM-dd"),
                    purchaserName = c.purchaserName,
                    taxpayerNumber = c.taxpayerNumber,
                    taxDiskCode = c.taxDiskCode,
                    taxpayerAddressOrId = c.taxpayerAddressOrId,
                    taxpayerBankAccount = c.taxpayerBankAccount,
                    salesName = c.salesName,
                    salesTaxpayerNum = c.salesTaxpayerNum,
                    salesTaxpayerAddress = c.salesTaxpayerAddress,
                    salesTaxpayerBankAccount = c.salesTaxpayerBankAccount,
                    totalAmount = c.totalAmount,
                    totalTaxNum = c.totalTaxNum,
                    totalTaxSum = c.totalTaxSum,
                    invoiceRemarks = c.invoiceRemarks,
                    goodsClerk = c.goodsClerk,
                    checkCode = c.checkCode,
                    voidMark = c.voidMark,
                    isBillMark = c.isBillMark,
                    tollSign = c.tollSign,
                    tollSignName = c.tollSignName
                }.Json();

                this.Model.InvoiceItems = c.InvoiceDetailData.Json();
            }
        }
    }
}