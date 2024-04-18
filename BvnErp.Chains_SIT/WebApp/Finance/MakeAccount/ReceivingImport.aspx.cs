using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class ReceivingImport : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ClientName = Server.UrlDecode(Request.QueryString["ClientName"]) ?? string.Empty;
            string DataNow = DateTime.Now.ToString("yyyy-MM-dd");
        
            this.Model.EndDate = DataNow;
            this.Model.LastReceiptDateStartDate = DataNow;
            this.Model.LastReceiptDateEndDate = DataNow;
        }

        protected void data()
        {
            string ClientName = Request.QueryString["ClientName"];
            string QuerenStatus = Request.QueryString["QuerenStatus"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string LastReceiptDateStartDate = Request.QueryString["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.QueryString["LastReceiptDateEndDate"];

        
            ReceiptExportExcel download = new ReceiptExportExcel();
            List<ReceiveDetailReportMain> importData = download.ImportData(ClientName, QuerenStatus, StartDate, EndDate, LastReceiptDateStartDate, LastReceiptDateEndDate);

            Func<Needs.Ccs.Services.Models.ReceiveDetailReportMain, object> convert = r => new
            {
                 r.ID,
                 r.SeqNo,
                 r.AccountName,
                 ReceiptDate =  r.ReceiptDate.ToString("yyyy-MM-dd"),
                 r.ClientName,
                 InvoiceType = r.InvoiceType == 0 ? "单抬头" : "双抬头",
                 r.Amount,
                 r.ClearAmount,
                 r.UnClearAmount,
                 r.TotalProduct,
                 r.TotalAddTax,
                 r.TotalTariffTax,
                 r.TotalExciseTax,
                 r.TotalAgency,
                 r.TotalExchangeSpread,
                 r.OrderRecepitID
            };

            this.Paging(importData, convert);
        }
        /// <summary>
        /// 导出做账明细报表
        /// </summary>
        protected void ExportFinanceReport()
        {
            string ClientName = Request.Form["ClientName"];
            string QuerenStatus = Request.Form["QuerenStatus"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string LastReceiptDateStartDate = Request.Form["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.Form["LastReceiptDateEndDate"];


            try
            {
                ReceiptExportExcel download = new ReceiptExportExcel();
                var url = download.ExportForCredential(ClientName, QuerenStatus, StartDate, EndDate, LastReceiptDateStartDate, LastReceiptDateEndDate);

                Response.Write((new { success = true, message = "导出成功", url = url }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        protected void MakeAccount()
        {
            string ClientName = Request.Form["ClientName"];
            string QuerenStatus = Request.Form["QuerenStatus"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string LastReceiptDateStartDate = Request.Form["LastReceiptDateStartDate"];
            string LastReceiptDateEndDate = Request.Form["LastReceiptDateEndDate"];

            if (string.IsNullOrEmpty(LastReceiptDateEndDate))
            {
                Response.Write((new { success = false,msg= "核销日期必选" }).Json());
                return;
            }

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<mk>>();

            List<string> orderRIDs = new List<string>();
            foreach(var item in model)
            {
                orderRIDs.Add(item.ID);
            }

            try 
            {
                var result = new Needs.Ccs.Services.Models.ReceivingImport(ClientName, QuerenStatus, StartDate, EndDate, LastReceiptDateStartDate, LastReceiptDateEndDate, orderRIDs).Make();

                Response.Write((new { success = result, msg = "生成凭证失败!" }).Json());
            }
            catch(Exception ex) 
            {
                Response.Write((new { success = false, msg = ex.ToString()}).Json());
            }
            

        }

        public class mk
        {
            public string ID { get; set; }
        }
    }
}