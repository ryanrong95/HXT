using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebWeChat.Models
{
    public class InvoiceScanReturnModel
    {
        public string InvoiceNoticeID { get; set; }

        public string InvoiceTypeName { get; set; }

        public string ClientID { get; set; }

        public string CompanyName { get; set; }

        public string BankName { get; set; }

        public string BankAccount { get; set; }

        public string TaxCode { get; set; }

        public string Summary { get; set; }

        public string Amount { get; set; }

        public string Difference { get; set; }
    }

    public class GetInvoiceListModel
    {
        public string InvoiceNoticeID { get; set; }
    }

    public class InsertInvoiceModel
    {
        public string InvoiceNoticeID { get; set; }

        //public string InvoiceTypeInt { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public string InvoiceAmount { get; set; }

        public string InvoiceDate { get; set; }

        private DateTime? _invoiceDateDt;

        public DateTime? InvoiceDateDt
        {
            get
            {
                //日期格式在 controller 中有校验
                string year = this.InvoiceDate.Substring(0, 4);
                string month = this.InvoiceDate.Substring(4, 2);
                string day = this.InvoiceDate.Substring(6, 2);

                string formatDateStr = $"{year}-{month}-{day}";

                DateTime dt;
                if (DateTime.TryParse(formatDateStr, out dt))
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            set { _invoiceDateDt = value; }
        }
    }

    public class DeleteInvoiceModel
    {
        public string TaxManageID { get; set; }

        public string InvoiceNoticeID { get; set; }
    }
}