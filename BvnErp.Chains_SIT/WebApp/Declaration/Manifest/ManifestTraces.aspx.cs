
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Manifest
{
    public partial class DecTrace : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string BillNo = Request.QueryString["BillNo"];
            string ID = Request.QueryString["ID"];
            var ManifestTrace = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignmentTraces.AsQueryable();
            if (!string.IsNullOrEmpty(BillNo))
            {
                BillNo = BillNo.Trim();
                ManifestTrace = ManifestTrace.Where(t => t.ManifestConsignmentID == BillNo);
            }
            Func<Needs.Ccs.Services.Models.ManifestConsignmentTrace, object> convert = head => new
            {
                BillNo = BillNo,
                StatementCode = head.StatementName,
                Message = head.Message,
                NoticeDate = head.NoticeDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Response.Write(new
            {
                rows = ManifestTrace.Select(convert).ToList()
            }.Json());
        }
    }
}