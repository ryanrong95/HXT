using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class InvoiceNoticeFile : Uc.PageBase
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
            this.Model.InvoiceNoticeID = Request.QueryString["InvoiceNoticeID"];
        }

        /// <summary>
        /// 发票文件
        /// </summary>
        protected void InvoiceFiles()
        {
            string invoiceNoticeID = Request.QueryString["InvoiceNoticeID"];

            if (!string.IsNullOrEmpty(invoiceNoticeID))
            {
                Needs.Ccs.Services.Views.InvoiceNoticeFileView view = new Needs.Ccs.Services.Views.InvoiceNoticeFileView();
                var files = view.Where(t => t.InvoiceNoticeID == invoiceNoticeID && t.FileType == Needs.Ccs.Services.Enums.InvoiceNoticeFileType.Invoice);
                Func<Needs.Ccs.Services.Models.InvoiceNoticeFile, object> convert = file => new
                {
                    file.ID,
                    file.Name,
                    FileType = file.FileType.GetDescription(),
                    file.FileFormat,
                    Url = FileDirectory.Current.PvDataFileUrl + "/" + file.Url.ToUrl(),
                };

                Response.Write(new
                {
                    rows = files.Select(convert).ToList(),
                    total = files.Count()
                }.Json());
            }
            else
            {
                Response.Write(new
                {
                    rows = new List<string>(),
                    total = 0
                }.Json());
            }
        }
    }
}