using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.SzStore
{
    public partial class ReceiptRecordsList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientName = Request.QueryString["ClientName"];
            string SeqNo = Request.QueryString["SeqNo"];

            var receiveds = new Needs.Ccs.Services.Views.PayeeRightsTopView().Where(t => t.FlowFormCode == SeqNo).ToArray();
            int totalCount = receiveds.Count();

            Response.Write(new
            {
                rows = receiveds.Skip(rows * (page - 1)).Take(rows).Select(t => new
                {
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    t.AdminName,
                    t.Price,
                    t.FlowFormCode,
                }).ToArray(),
                total = totalCount,
            }.Json());
        }
    }
}