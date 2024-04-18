using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.New
{
    public partial class SetControl : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string ClientID = Request.QueryString["ClientID"];
            string IsDownloadDecTax = Request.QueryString["IsDownloadDecTax"];
            string DecTaxExtendDate = Request.QueryString["DecTaxExtendDate"];
            string IsApplyInvoice = Request.QueryString["IsApplyInvoice"];
            string InvoiceExtendDate = Request.QueryString["InvoiceExtendDate"];

            this.Model.ClientID = ClientID;
            this.Model.IsDownloadDecTax = IsDownloadDecTax;
            this.Model.DecTaxExtendDate = DecTaxExtendDate;
            this.Model.IsApplyInvoice = IsApplyInvoice;
            this.Model.InvoiceExtendDate = InvoiceExtendDate;
        }

        /// <summary>
        /// 获取操作日志
        /// </summary>
        protected void data()
        {
            string ClientID = Request.QueryString["ClientID"];

            var logs = new Needs.Ccs.Services.Views.LogsView().Where(t => t.MainID == ClientID && t.Name == "超期未付汇管控处理").OrderByDescending(t => t.CreateDate).AsQueryable();


            Func<Needs.Ccs.Services.Models.Logs, object> convert = log => new
            {
                AdminName = log.AdminName,
                CreateDate = log.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = log.Summary
            };

            Response.Write(new
            {
                rows = logs.Select(convert).ToArray(),
                total = logs.Count()
            }.Json());
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void SavePayExControl()
        {
            try
            {
                var ClientID = Request.Form["ClientID"];
                var dectaxRD = Request.Form["dectaxRD"];
                var dectaxDate = Request.Form["dectaxDate"];
                var invoiceRD = Request.Form["invoiceRD"];
                var invoiceDate = Request.Form["invoiceDate"];
                var summary = Request.Form["summary"];

                if (!string.IsNullOrEmpty(ID))
                {
                    var message = "";
                    var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[ClientID];

                    client.IsDownloadDecTax = dectaxRD == "0" ? false : true;
                    client.DecTaxExtendDate = dectaxRD == "0" ? dectaxDate : "";
                    client.IsApplyInvoice = invoiceRD == "0" ? false : true;
                    client.InvoiceExtendDate = invoiceRD == "0" ?  invoiceDate : "";
                    client.Summary = summary;

                    message += "[海关发票：" + (client.IsDownloadDecTax.Value ? "不限制" : "限制") + ";宽限日期：" + client.DecTaxExtendDate + "]";
                    message += "[申请开票：" + (client.IsApplyInvoice.Value ? "不限制" : "限制") + ";宽限日期：" + client.InvoiceExtendDate + "]";
                    message += "备注：" + client.Summary;

                    Needs.Ccs.Services.Models.Logs log = new Needs.Ccs.Services.Models.Logs();
                    log.Name = "超期未付汇管控处理";
                    log.MainID = ClientID;
                    log.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    log.Json = "";
                    log.Summary = message;
                    log.Enter();

                    client.Enter();

                    Response.Write((new { success = true, message = "设置成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "设置失败，会员ID错误" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "设置失败" + ex.Message }).Json());
            }

        }
    }
}